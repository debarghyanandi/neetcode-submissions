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
 */

import { readFileSync } from 'node:fs';
import { join } from 'node:path';
import { execFileSync } from 'node:child_process';
import { loadState, scanRepo, pendingOnly } from './lib/scan.mjs';
import { COMPLEXITY, assignNames, isSelfMarked } from './lib/complexity.mjs';

const argv = process.argv.slice(2);
const arg = (n, d = null) => (argv.includes(n) ? argv[argv.indexOf(n) + 1] : d);
const has = (n) => argv.includes(n);

const only = arg('--slug');
const limit = Number(arg('--limit', '0')) || 0;
const verbose = has('--verbose');
const model = arg('--model');

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
    '--max-turns', '1',
  ];
  if (model) args.push('--model', model);

  // execFile, not a shell: no quoting, no injection surface from file contents.
  const raw = execFileSync('claude', args, {
    input: payload,
    encoding: 'utf8',
    maxBuffer: 32 * 1024 * 1024,
    env: { ...process.env },
  });

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

  return { solutions: out.solutions, cost: envelope.total_cost_usd };
}

// ---------------------------------------------------------------- run

// --slug targets a folder whether or not it has pending work. That makes the
// best available test cheap: run it over folders you already curated by hand
// and see whether the model's proposed names match the ones you chose. Agreement
// is evidence; disagreement is worth reading before this thing renames anything.
const everything = scanRepo(loadState());
let targets = only ? everything.filter((p) => p.slug === only) : pendingOnly(everything);
if (limit) targets = targets.slice(0, limit);

if (targets.length === 0) {
  console.log(only
    ? `\nNo folder with slug "${only}".\n`
    : '\nNothing pending. Pass --slug <name> to classify a folder anyway.\n');
  process.exit(0);
}

console.log(`\nDRY RUN - ${targets.length} folder(s). No file will be modified.\n`);

let failures = 0;
for (const p of targets) {
  const files = [...p.curatedFiles, ...p.pending.map((s) => s.file)];
  console.log(`${p.path}`);
  console.log(`  classifying ${files.length} file(s): ${files.join(', ')}`);

  let res;
  try {
    res = classify(p.dir, files);
  } catch (e) {
    console.log(`  FAILED: ${e.message.split('\n')[0]}\n`);
    failures++;
    continue;
  }

  for (const s of res.solutions) {
    const self = isSelfMarked(readFileSync(join(p.dir, s.file), 'utf8'));
    console.log(`    ${s.file}`);
    console.log(`        ${s.time} time / ${s.space} space   ${s.algorithm}  [${s.approachKey}]${s.correct ? '' : '   *** MODEL SAYS INCORRECT ***'}`);
    console.log(`        marked yours: ${self ? 'yes' : 'no marker found'}`);
    if (verbose) console.log(`        ${s.note}`);
  }

  const plan = assignNames(res.solutions);
  console.log('  proposed names:');
  if (!plan.ok) {
    console.log(`    REFUSED - ${plan.reason}. Left for you to decide.`);
  } else {
    for (const [from, to] of plan.names) {
      console.log(`    ${from.padEnd(22)} ${from === to ? '=  unchanged' : '-> ' + to}`);
    }
  }
  if (res.cost != null) console.log(`  est. cost this folder: $${res.cost}`);
  console.log('');
}

console.log(failures ? `${failures} folder(s) failed.\n` : 'All folders classified.\n');
process.exit(failures ? 1 : 0);
