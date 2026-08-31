#!/usr/bin/env node
/**
 * classify.mjs - the one step that calls a model. DRY RUN ONLY for now:
 * it prints what it would rename. It does not touch a single file.
 *
 * Division of labour, deliberately:
 *   the model  reads code and names the complexity, from a fixed enum
 *   this script does every decision that follows from those complexities
 *
 * That is why the schema forbids free text for complexity. A model that can
 * only answer "O(n log n)" or "other" is a model whose answer you can rank,
 * diff, and disagree with. One that writes prose is one you have to trust.
 *
 *   node scripts/classify.mjs                 # every folder with pending work
 *   node scripts/classify.mjs --slug two-integer-sum
 *   node scripts/classify.mjs --limit 2 --verbose
 *   node scripts/classify.mjs --slug two-integer-sum --model opus
 *   node scripts/classify.mjs --slug two-integer-sum --apply     # renames + rewrites headers
 *   node scripts/classify.mjs --apply --exclude-changed-since <sha> --limit 5
 */

import { readFileSync, writeFileSync, mkdirSync } from 'node:fs';
import { join, dirname } from 'node:path';
import { execFileSync } from 'node:child_process';
import { loadState, scanRepo, pendingOnly, foldersChangedSince, REPO } from './lib/scan.mjs';

// Gitignored (.agent/tmp/). The whole envelope, for when the summary isn't enough.
const DUMP = join(REPO, '.agent', 'tmp', 'last-claude-response.json');
import { COMPLEXITY, assignNames, isSelfMarked } from './lib/complexity.mjs';
import { stripHeader, buildHeader, applyHeader, headerSignature } from './lib/header.mjs';
import { loadState as _ls, saveState } from './lib/scan.mjs';

const argv = process.argv.slice(2);
const arg = (n, d = null) => (argv.includes(n) ? argv[argv.indexOf(n) + 1] : d);
const has = (n) => argv.includes(n);

const only = arg('--slug');
const limit = Number(arg('--limit', '0')) || 0;
const verbose = has('--verbose');
// Per-step model choice, not one global setting.
//
// Classification is a constrained task: six fields, two of them enums, judged
// against a fixed ladder. It is not where a bigger model earns its keep -
// Sonnet reproduced a hand-made optimal/optimal-variant split on the hardest
// folder in the repo. The visualizer and the header prose are the steps with
// real design latitude, and those are worth spending Opus on.
//
// On Pro the account default is already Sonnet 5, so this changes nothing
// today; it keeps the frequent path cheap if the account ever defaults to Opus.
const DEFAULT_MODEL = 'sonnet';
const model = arg('--model', DEFAULT_MODEL);
const doApply = has('--apply');
const deleteDupes = has('--delete-duplicates');

// The //My solution marker must be read from the BODY, not the whole file.
// Generated headers quote the marker text ("marked '//My solution'"), so
// scanning the whole file would let a header assert a marker into existence.
const markedInBody = (src) => isSelfMarked(stripHeader(src).body);

const git = (cwd, ...a) => execFileSync('git', a, { cwd, encoding: 'utf8', stdio: ['pipe','pipe','pipe'] });

const SCHEMA = {
  type: 'object',
  additionalProperties: false,
  properties: {
    solutions: {
      type: 'array',
      items: {
        type: 'object',
        additionalProperties: false,
        properties: {
          file: { type: 'string', description: 'exact filename as given' },
          algorithm: { type: 'string', description: 'the technique, 2-6 words, e.g. "sliding window, shrink while valid"' },
          time: { type: 'string', enum: COMPLEXITY },
          space: { type: 'string', enum: COMPLEXITY },
          approachKey: { type: 'string', description: 'short slug for the technique, e.g. "hashmap-complement". Two files sharing a key are the same idea.' },
          correct: { type: 'boolean', description: 'false only if the code is clearly wrong, not merely slow' },
          note: { type: 'string', description: 'one sentence on the mechanism that makes the complexity what it is' },
        },
        required: ['file', 'algorithm', 'time', 'space', 'approachKey', 'correct', 'note'],
      },
    },
  },
  required: ['solutions'],
};

const INSTRUCTIONS = [
  'You are given several C# solutions to one coding problem, on stdin, each delimited by a ===== FILE: <name> ===== banner.',
  '',
  'For every file, report its worst-case time and space complexity, the technique it uses, and a short approachKey.',
  'Choose time and space ONLY from the allowed enum values. If a solution genuinely does not fit any of them, answer "other" - do not round to the nearest.',
  'Space complexity means auxiliary space, excluding the input and excluding the output where the problem requires building one.',
  'Two files that implement the same idea must share an approachKey. Two files with the same complexity but genuinely different mechanisms must not.',
  'Set correct=false only when the code is actually wrong. Slow is not wrong.',
  'Ignore all comments when judging - they may be stale or misleading. Judge the code.',
  'Report on every file you are given, exactly once, using the filename exactly as it appears in its banner.',
  '',
  'You have no tools available and no access to the filesystem. Everything you need is on stdin. Do not attempt to read, list, or search files - answer directly from the text you were given.',
].join('\n');

function classify(dir, files) {
  const payload = files
    .map((f) => `===== FILE: ${f} =====\n${readFileSync(join(dir, f), 'utf8')}`)
    .join('\n\n');

  const args = [
    '-p', INSTRUCTIONS,
    '--output-format', 'json',
    '--json-schema', JSON.stringify(SCHEMA),
    '--permission-mode', 'dontAsk',
    // 1 was enough for a trivial probe. A real classification thinks first, and
    // a denied tool attempt burns a turn on its own - so give it headroom.
    '--max-turns', '8',
  ];
  if (model) args.push('--model', model);   // --model default overrides back to the account default

  // execFile, not a shell: no quoting, no injection surface from file contents.
  // stdio 'pipe' on stderr so a failure tells us WHY, not just that it failed.
  let raw;
  try {
    raw = execFileSync('claude', args, {
      input: payload,
      encoding: 'utf8',
      maxBuffer: 32 * 1024 * 1024,
      env: { ...process.env },
      stdio: ['pipe', 'pipe', 'pipe'],
    });
  } catch (e) {
    // The envelope's usage block is enormous and says nothing. Slicing the raw
    // JSON just shows you token counts. Parse it and print the fields that
    // actually explain the failure.
    const stdout = String(e.stdout ?? '');
    const lines = [
      e.status != null ? `exit status  : ${e.status}` : null,
      e.code ? `error code   : ${e.code}` : null,
    ].filter(Boolean);

    let env = null;
    try { env = JSON.parse(stdout); } catch { /* not JSON */ }

    if (env) {
      for (const k of ['is_error', 'subtype', 'terminal_reason', 'stop_reason', 'num_turns']) {
        if (env[k] !== undefined) lines.push(`${k.padEnd(13)}: ${JSON.stringify(env[k])}`);
      }
      if (env.result !== undefined) lines.push(`result       : ${String(env.result).slice(0, 500)}`);
      if (Array.isArray(env.permission_denials) && env.permission_denials.length) {
        lines.push(`denials      : ${JSON.stringify(env.permission_denials).slice(0, 400)}`);
      }
      mkdirSync(dirname(DUMP), { recursive: true });
      writeFileSync(DUMP, stdout, 'utf8');
      lines.push(`full response: ${DUMP}`);
    } else {
      if (stdout.trim()) lines.push(`stdout       : ${stdout.trim().slice(0, 500)}`);
      const err = String(e.stderr ?? '').trim();
      if (err) lines.push(`stderr       : ${err.slice(0, 500)}`);
      if (!stdout.trim() && !err) lines.push(`message      : ${e.message.slice(0, 300)}`);
    }
    throw new Error(`claude invocation failed\n        ${lines.join('\n        ')}`);
  }

  const envelope = JSON.parse(raw);
  const out = envelope.structured_output;
  if (!out || !Array.isArray(out.solutions)) {
    throw new Error(`no structured_output in response (result was: ${String(envelope.result).slice(0, 300)})`);
  }

  // Trust nothing: the schema constrains shape, not coverage.
  const got = new Set(out.solutions.map((s) => s.file));
  const missing = files.filter((f) => !got.has(f));
  const extra = [...got].filter((f) => !files.includes(f));
  if (missing.length) throw new Error(`model omitted: ${missing.join(', ')}`);
  if (extra.length) throw new Error(`model invented: ${extra.join(', ')}`);

  return { solutions: out.solutions, cost: envelope.total_cost_usd, turns: envelope.num_turns };
}

// ---------------------------------------------------------------- run

// --slug targets a folder whether or not it has pending work. That makes the
// best available test cheap: run it over folders you already curated by hand
// and see whether the model's proposed names match the ones you chose. Agreement
// is evidence; disagreement is worth reading before this thing renames anything.
const everything = scanRepo(loadState());
let targets = only ? everything.filter((p) => p.slug === only) : pendingOnly(everything);

// Same exclusion contract as detect.mjs, and it has to be here too: on a push
// run this must skip the folder that was just pushed to, or the pipeline
// classifies a problem you may still be submitting against.
const exclude = new Set();
for (const v of argv.flatMap((a, i) => (a === '--exclude' ? [argv[i + 1]] : [])))
  String(v ?? '').split(',').map((x) => x.trim()).filter(Boolean).forEach((x) => exclude.add(x));
for (const slug of foldersChangedSince(arg('--exclude-changed-since'))) exclude.add(slug);
if (exclude.size) {
  const before = targets.length;
  targets = targets.filter((p) => !exclude.has(p.slug));
  console.log(`excluding ${[...exclude].join(', ')} (${before - targets.length} folder(s) held back)`);
}

if (limit) targets = targets.slice(0, limit);

if (targets.length === 0) {
  console.log(only
    ? `\nNo folder with slug "${only}".\n`
    : '\nNothing pending. Pass --slug <name> to classify a folder anyway.\n');
  process.exit(0);
}

console.log(doApply
  ? `\nAPPLY - ${targets.length} folder(s). Files WILL be renamed and re-headered.\n`
  : `\nDRY RUN - ${targets.length} folder(s). No file will be modified.\n`);

const state = _ls();
let failures = 0;
let touched = 0;

for (const p of targets) {
  console.log(`${p.path}`);

  // A raw submission identical to code already curated here has no destination
  // name - naming it optimal-variant.cs would enshrine a copy. Take it out of
  // the classification set and record it as dealt with.
  const dupes = p.pending.filter((s) => s.duplicateOfCurated).map((s) => s.file);
  const fresh = p.pending.filter((s) => !s.duplicateOfCurated).map((s) => s.file);
  if (dupes.length) console.log(`  duplicates of curated code (not classified): ${dupes.join(', ')}`);

  const files = [...p.curatedFiles, ...fresh];
  if (files.length === 0) { console.log('  nothing to classify\n'); continue; }
  console.log(`  classifying ${files.length} file(s): ${files.join(', ')}`);

  let res;
  try {
    res = classify(p.dir, files);
  } catch (e) {
    console.log(`  FAILED: ${e.message}\n`);
    failures++;
    continue;
  }

  const marks = new Map(files.map((f) => [f, markedInBody(readFileSync(join(p.dir, f), 'utf8'))]));
  for (const s of res.solutions) {
    console.log(`    ${s.file}`);
    console.log(`        ${s.time} time / ${s.space} space   ${s.algorithm}  [${s.approachKey}]${s.correct ? '' : '   *** MODEL SAYS INCORRECT ***'}`);
    console.log(`        marked yours: ${marks.get(s.file) ? 'yes' : 'no marker found'}`);
    if (verbose) console.log(`        ${s.note}`);
  }

  const plan = assignNames(res.solutions);
  console.log('  proposed names:');
  if (!plan.ok) {
    console.log(`    REFUSED - ${plan.reason}. Left untouched for you to decide.`);
    console.log(`  est. cost $${res.cost}  ·  turns used: ${res.turns ?? '?'}\n`);
    continue;
  }
  for (const [from, to] of plan.names) {
    console.log(`    ${from.padEnd(22)} ${from === to ? '=  unchanged' : '-> ' + to}`);
  }

  if (doApply) {
    const byFile = new Map(res.solutions.map((s) => [s.file, s]));

    // Rename in two phases through temporary names. A single pass would clobber
    // a file whenever two names swap - which is exactly what a demotion does.
    const moves = [...plan.names].filter(([f, t]) => f !== t);
    try {
      moves.forEach(([f], i) => git(p.dir, 'mv', '--', f, `__pipeline_tmp_${i}`));
      moves.forEach(([, t], i) => git(p.dir, 'mv', '--', `__pipeline_tmp_${i}`, t));
    } catch (e) {
      console.log(`    RENAME FAILED: ${String(e.stderr || e.message).trim().slice(0, 300)}`);
      failures++;
      console.log('');
      continue;
    }

    // Whole-folder header regeneration, best-first so 'ranks above/below' is
    // computed against the same ordering the names came from.
    const ranked = [...res.solutions]
      .map((s) => ({ ...s, name: plan.names.get(s.file) }))
      .sort((a, b) => COMPLEXITY.indexOf(a.time) - COMPLEXITY.indexOf(b.time)
                   || COMPLEXITY.indexOf(a.space) - COMPLEXITY.indexOf(b.space));

    const rec = state.problems[p.slug] ?? (state.problems[p.slug] = {});
    const sigs = rec.headerSignatures ?? (rec.headerSignatures = {});

    let wrote = 0, kept = 0;
    for (const [origin, finalName] of plan.names) {
      const full = join(p.dir, finalName);
      const src = readFileSync(full, 'utf8');
      const sol = byFile.get(origin);
      const sig = headerSignature(finalName, sol, marks.get(origin), ranked);

      // Rewrite only when the file has no header, or when something other than
      // the prose actually changed. Otherwise the nightly run rewords your
      // headers in perpetuity and every diff is noise.
      if (stripHeader(src).had && sigs[finalName] === sig) { kept++; continue; }

      writeFileSync(full, applyHeader(src, buildHeader(finalName, origin, sol, marks.get(origin), ranked)), 'utf8');
      sigs[finalName] = sig;
      wrote++;
    }
    for (const k of Object.keys(sigs)) if (![...plan.names.values()].includes(k)) delete sigs[k];
    console.log(`    applied: ${moves.length} rename(s), ${wrote} header(s) written, ${kept} left as-is`);
    touched++;

    // Duplicates: recorded as handled so they stop showing up as pending.
    if (dupes.length) {
      const rec = state.problems[p.slug] ?? (state.problems[p.slug] = {});
      rec.processedSubmissions = [...new Set([...(rec.processedSubmissions ?? []), ...dupes])].sort();
      if (deleteDupes) {
        for (const d of dupes) git(p.dir, 'rm', '-q', '--', d);
        console.log(`    deleted ${dupes.length} duplicate(s)`);
      } else {
        console.log(`    left ${dupes.length} duplicate(s) in place, marked handled (--delete-duplicates to remove)`);
      }
    }
  }

  console.log(`  est. cost $${res.cost}  ·  turns used: ${res.turns ?? '?'}\n`);
}

if (doApply) saveState(state);

console.log(failures ? `${failures} folder(s) failed.\n` : 'All folders classified.\n');
if (doApply) console.log('Review with: git status && git diff --cached\n');
process.exit(failures ? 1 : 0);
