#!/usr/bin/env node
/**
 * detect.mjs - READ ONLY. Writes nothing, commits nothing, calls no model.
 *
 * Answers one question: which problem folders have raw NeetCode submissions
 * that have not been processed yet?
 *
 * There is exactly one code path. The push run and the nightly cron run differ
 * only by which folders are excluded:
 *
 *   push  : --exclude-changed-since <before-sha>   (skip what this push touched)
 *   cron  : no exclusions                          (drain everything, incl. the last one)
 *
 * Usage:
 *   node scripts/detect.mjs
 *   node scripts/detect.mjs --exclude two-integer-sum --exclude minimum-stack
 *   node scripts/detect.mjs --exclude-changed-since <sha>
 *   node scripts/detect.mjs --json
 */

import { readdirSync, readFileSync, existsSync, statSync } from 'node:fs';
import { join, dirname, resolve, basename } from 'node:path';
import { fileURLToPath } from 'node:url';
import { execFileSync } from 'node:child_process';
import { fingerprint, shortPrint } from './lib/normalise.mjs';

const REPO = resolve(dirname(fileURLToPath(import.meta.url)), '..');
const STATE_PATH = join(REPO, '.agent', 'state.json');

// Top-level entries that are never topic folders.
const NOT_TOPICS = new Set(['.git', '.github', '.agent', '.vs', 'scripts', 'node_modules', 'bin', 'obj']);

// NeetCode writes submission-<n>.<ext>. Anything else in a problem folder is ours.
const SUBMISSION_RE = /^submission-(\d+)\.([A-Za-z0-9]+)$/;

// ---------------------------------------------------------------- args

function parseArgs(argv) {
  const opts = { exclude: new Set(), changedSince: null, json: false };
  for (let i = 0; i < argv.length; i++) {
    const a = argv[i];
    if (a === '--exclude') {
      // accepts a slug, a repo-relative folder path, or a comma-separated list
      String(argv[++i] ?? '')
        .split(',')
        .map((s) => s.trim())
        .filter(Boolean)
        .forEach((s) => opts.exclude.add(basename(s)));
    } else if (a === '--exclude-changed-since') {
      opts.changedSince = argv[++i] ?? null;
    } else if (a === '--json') {
      opts.json = true;
    } else if (a === '--help' || a === '-h') {
      console.log(readFileSync(fileURLToPath(import.meta.url), 'utf8').split('*/')[0]);
      process.exit(0);
    } else {
      console.error(`detect: unknown argument ${a}`);
      process.exit(2);
    }
  }
  return opts;
}

/**
 * Problem folders touched between <sha> and HEAD.
 * A push with a zero/absent before-sha (new branch, force push) excludes nothing
 * rather than guessing - the cron run will pick up whatever this misses.
 */
function foldersChangedSince(sha) {
  if (!sha || /^0+$/.test(sha)) return [];
  let out;
  try {
    out = execFileSync('git', ['diff', '--name-only', sha, 'HEAD'], {
      cwd: REPO,
      encoding: 'utf8',
    });
  } catch {
    console.error(`detect: cannot diff from ${sha} (shallow clone?) - excluding nothing`);
    return [];
  }
  const slugs = new Set();
  for (const line of out.split('\n')) {
    const p = line.trim();
    if (!p) continue;
    if (SUBMISSION_RE.test(basename(p))) slugs.add(basename(dirname(p)));
  }
  return [...slugs];
}

// ---------------------------------------------------------------- state

function loadState() {
  if (!existsSync(STATE_PATH)) return { version: 1, problems: {} };
  try {
    const s = JSON.parse(readFileSync(STATE_PATH, 'utf8'));
    return { version: s.version ?? 1, problems: s.problems ?? {} };
  } catch (e) {
    console.error(`detect: ${STATE_PATH} is unreadable (${e.message}) - treating as empty`);
    return { version: 1, problems: {} };
  }
}

// ---------------------------------------------------------------- scan

const dirsIn = (p) =>
  readdirSync(p, { withFileTypes: true })
    .filter((d) => d.isDirectory())
    .map((d) => d.name);

function scan(state) {
  const found = [];

  for (const topic of dirsIn(REPO)) {
    if (NOT_TOPICS.has(topic) || topic.startsWith('.')) continue;

    for (const slug of dirsIn(join(REPO, topic))) {
      const dir = join(REPO, topic, slug);
      const entries = readdirSync(dir, { withFileTypes: true }).filter((d) => d.isFile());

      const submissions = entries
        .map((d) => d.name)
        .filter((n) => SUBMISSION_RE.test(n))
        .sort((a, b) => Number(a.match(SUBMISSION_RE)[1]) - Number(b.match(SUBMISSION_RE)[1]));

      if (submissions.length === 0) continue;

      const rec = state.problems[slug] ?? {};
      const done = new Set(rec.processedSubmissions ?? []);
      const knownPrints = new Set(Object.keys(rec.fingerprints ?? {}));

      // Fingerprint whatever we already curated in this folder, so a resubmission
      // that is byte-for-byte the same idea gets flagged rather than reprocessed.
      const curated = entries.map((d) => d.name).filter((n) => !SUBMISSION_RE.test(n) && n.endsWith('.cs'));
      for (const f of curated) {
        knownPrints.add(fingerprint(readFileSync(join(dir, f), 'utf8')));
      }

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
          duplicateOfCurated: knownPrints.has(print),
        });
      }

      if (pending.length) {
        found.push({
          topic,
          slug,
          path: `${topic}/${slug}`,
          curatedFiles: curated,
          hasVisualizer: entries.some((d) => d.name === 'visualizer.html'),
          pending,
        });
      }
    }
  }

  return found.sort((a, b) => a.path.localeCompare(b.path));
}

// ---------------------------------------------------------------- main

const opts = parseArgs(process.argv.slice(2));
for (const slug of foldersChangedSince(opts.changedSince)) opts.exclude.add(slug);

const state = loadState();
const all = scan(state);
const excluded = all.filter((p) => opts.exclude.has(p.slug));
const selected = all.filter((p) => !opts.exclude.has(p.slug));

const report = {
  scannedAt: new Date().toISOString(),
  repo: REPO,
  excludedSlugs: [...opts.exclude],
  counts: { pendingTotal: all.length, excluded: excluded.length, selected: selected.length },
  selected,
  excluded: excluded.map((p) => ({ path: p.path, pending: p.pending.map((s) => s.file) })),
};

if (opts.json) {
  console.log(JSON.stringify(report, null, 2));
} else {
  console.log(`\nscanned  ${REPO}`);
  console.log(`excluded ${report.excludedSlugs.length ? report.excludedSlugs.join(', ') : '(nothing)'}`);
  console.log(`pending  ${all.length} folder(s) -> ${selected.length} selected, ${excluded.length} held back\n`);

  if (selected.length === 0) {
    console.log('  nothing to do.\n');
  }
  for (const p of selected) {
    const state = p.curatedFiles.length ? `already curated: ${p.curatedFiles.join(', ')}` : 'never curated';
    console.log(`  ${p.path}`);
    console.log(`      ${state}${p.hasVisualizer ? ' + visualizer.html' : ''}`);
    for (const s of p.pending) {
      console.log(`      ${s.file.padEnd(18)} ${String(s.bytes).padStart(6)}B  ${s.fingerprint}${s.duplicateOfCurated ? '   [DUPLICATE of curated code]' : ''}`);
    }
    console.log('');
  }
  for (const p of report.excluded) {
    console.log(`  (held back) ${p.path}: ${p.pending.join(', ')}`);
  }
  console.log('');
}

// Emit for later workflow steps. Milestone 1 has no later steps; this is the seam.
if (process.env.GITHUB_OUTPUT) {
  const { appendFileSync } = await import('node:fs');
  appendFileSync(process.env.GITHUB_OUTPUT, `count=${selected.length}\n`);
  appendFileSync(process.env.GITHUB_OUTPUT, `paths=${JSON.stringify(selected.map((p) => p.path))}\n`);
}
