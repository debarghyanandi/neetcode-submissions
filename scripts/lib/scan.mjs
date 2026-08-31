/**
 * The single source of truth for "what is in this repo and what is pending".
 *
 * detect.mjs and apply.mjs both import this. Two implementations of the same
 * question is how a pipeline starts lying to you: the dry run says one thing,
 * the real run does another.
 */

import { readdirSync, readFileSync, writeFileSync, existsSync, statSync, mkdirSync } from 'node:fs';
import { join, dirname, resolve, basename } from 'node:path';
import { fileURLToPath } from 'node:url';
import { execFileSync } from 'node:child_process';
import { fingerprint, shortPrint } from './normalise.mjs';

export const REPO = resolve(dirname(fileURLToPath(import.meta.url)), '..', '..');
export const STATE_PATH = join(REPO, '.agent', 'state.json');
export const README_PATH = join(REPO, 'README.md');

/** NeetCode writes submission-<n>.<ext>. Anything else in a problem folder is ours. */
export const SUBMISSION_RE = /^submission-(\d+)\.([A-Za-z0-9]+)$/;

const NOT_TOPICS = new Set(['.git', '.github', '.agent', '.vs', 'scripts', 'node_modules', 'bin', 'obj']);

/** optimal reads first, variants next, suboptimal last - the order you revise in. */
const CURATED_RANK = (n) =>
  n.startsWith('optimal-variant') ? 1 : n.startsWith('optimal') ? 0 : n.startsWith('suboptimal') ? 2 : 3;

export const dirsIn = (p) =>
  readdirSync(p, { withFileTypes: true })
    .filter((d) => d.isDirectory())
    .map((d) => d.name);

// ---------------------------------------------------------------- state

export function loadState() {
  if (!existsSync(STATE_PATH)) return { version: 1, problems: {} };
  try {
    const s = JSON.parse(readFileSync(STATE_PATH, 'utf8'));
    return { version: s.version ?? 1, problems: s.problems ?? {} };
  } catch (e) {
    console.error(`scan: ${STATE_PATH} is unreadable (${e.message}) - treating as empty`);
    return { version: 1, problems: {} };
  }
}

/**
 * Write state only if the substance changed.
 *
 * The timestamp must NOT be part of that decision. Bumping updatedAt on every
 * run makes the file differ every run, which makes the workflow commit every
 * night forever - a repo full of commits that say nothing happened.
 */
export function saveState(state) {
  mkdirSync(dirname(STATE_PATH), { recursive: true });

  const problems = Object.fromEntries(
    Object.keys(state.problems).sort().map((k) => [k, state.problems[k]])
  );
  const body = JSON.stringify(problems, null, 2);

  if (existsSync(STATE_PATH)) {
    try {
      const prev = JSON.parse(readFileSync(STATE_PATH, "utf8"));
      if (JSON.stringify(prev.problems ?? {}, null, 2) === body) return false;
    } catch { /* unreadable - fall through and rewrite */ }
  }

  state.updatedAt = new Date().toISOString();
  writeFileSync(
    STATE_PATH,
    JSON.stringify({ version: state.version ?? 1, updatedAt: state.updatedAt, problems }, null, 2) + "\n",
    "utf8"
  );
  return true;
}

// ---------------------------------------------------------------- git

/**
 * Problem folders touched between <sha> and HEAD.
 * A zero or absent sha (new branch, force push, shallow clone) excludes nothing
 * rather than guessing - the nightly run picks up whatever this misses.
 */
export function foldersChangedSince(sha) {
  if (!sha || /^0+$/.test(sha)) return [];
  let out;
  try {
    out = execFileSync('git', ['diff', '--name-only', sha, 'HEAD'], { cwd: REPO, encoding: 'utf8' });
  } catch {
    console.error(`scan: cannot diff from ${sha} (shallow clone?) - excluding nothing`);
    return [];
  }
  const slugs = new Set();
  for (const line of out.split('\n')) {
    const p = line.trim();
    if (p && SUBMISSION_RE.test(basename(p))) slugs.add(basename(dirname(p)));
  }
  return [...slugs];
}

// ---------------------------------------------------------------- scan

/**
 * Every problem folder in the repo, curated or not.
 * `pending` is non-empty only where raw submissions remain unprocessed.
 */
export function scanRepo(state = loadState()) {
  const problems = [];

  for (const topic of dirsIn(REPO)) {
    if (NOT_TOPICS.has(topic) || topic.startsWith('.')) continue;

    for (const slug of dirsIn(join(REPO, topic))) {
      const dir = join(REPO, topic, slug);
      const files = readdirSync(dir, { withFileTypes: true }).filter((d) => d.isFile()).map((d) => d.name);

      const submissions = files
        .filter((n) => SUBMISSION_RE.test(n))
        .sort((a, b) => Number(a.match(SUBMISSION_RE)[1]) - Number(b.match(SUBMISSION_RE)[1]));

      const curated = files
        .filter((n) => !SUBMISSION_RE.test(n) && n.endsWith('.cs'))
        .sort((a, b) => CURATED_RANK(a) - CURATED_RANK(b) || a.localeCompare(b));

      if (submissions.length === 0 && curated.length === 0) continue;

      const rec = state.problems[slug] ?? {};
      const done = new Set(rec.processedSubmissions ?? []);

      // Fingerprint what we already curated here, so a resubmitted identical
      // idea is recognised rather than reprocessed.
      const curatedPrints = {};
      for (const f of curated) curatedPrints[fingerprint(readFileSync(join(dir, f), 'utf8'))] = f;
      const known = new Set([...Object.keys(curatedPrints), ...Object.keys(rec.fingerprints ?? {})]);

      const pending = [];
      for (const name of submissions) {
        if (done.has(name)) continue;
        const src = readFileSync(join(dir, name), 'utf8');
        const print = fingerprint(src);
        pending.push({
          file: name,
          index: Number(name.match(SUBMISSION_RE)[1]),
          ext: name.match(SUBMISSION_RE)[2],
          bytes: statSync(join(dir, name)).size,
          fingerprint: shortPrint(src),
          duplicateOfCurated: known.has(print),
        });
      }

      problems.push({
        topic,
        slug,
        path: `${topic}/${slug}`,
        dir,
        curatedFiles: curated,
        curatedPrints,
        hasVisualizer: files.includes(`${slug}-visualizer.html`),
        visualizerFile: `${slug}-visualizer.html`,
        allSubmissions: submissions,
        processedSubmissions: [...done],
        pending,
      });
    }
  }

  return problems.sort((a, b) => a.path.localeCompare(b.path));
}

export const pendingOnly = (problems) => problems.filter((p) => p.pending.length > 0);
