#!/usr/bin/env node
/**
 * teach.mjs - MILESTONE 4a. Writes the study preamble on solution files.
 *
 * One Opus call per FILE (the headers differ per solution), against the
 * classification this repo already stores. Renames nothing and never runs the
 * ranking - classify.mjs owns that.
 *
 *   node scripts/teach.mjs --slug buy-and-sell-crypto            # dry run
 *   node scripts/teach.mjs --slug buy-and-sell-crypto --apply
 *   node scripts/teach.mjs --apply --limit 3                     # pending folders
 *   node scripts/teach.mjs --backfill --slug two-integer-sum --apply
 *
 * Without --backfill, only files whose teaching block is missing or stale get
 * rewritten. Model prose varies run to run, so regenerating unconditionally
 * would reword every file forever; staleness is judged on the classification,
 * not on the words.
 */

import { readFileSync, writeFileSync, appendFileSync } from 'node:fs';
import { join } from 'node:path';
import { execFileSync } from 'node:child_process';
import { loadState, saveState, scanRepo, pendingOnly, REPO } from './lib/scan.mjs';
import { stripHeader } from './lib/header.mjs';
import { SECTIONS_SCHEMA, TEACH_INSTRUCTIONS, buildTeachingBlock, statusFor, sourceFor, splitTrailingTeach } from './lib/teach.mjs';
import { isSelfMarked } from './lib/complexity.mjs';

const argv = process.argv.slice(2);
const arg = (n, d = null) => (argv.includes(n) ? argv[argv.indexOf(n) + 1] : d);
const has = (n) => argv.includes(n);

const only = arg('--slug');
const limit = Number(arg('--limit', '0')) || 0;
const doApply = has('--apply');
const backfill = has('--backfill');
const force = has('--force');
// The step with real design latitude - this is where the larger model earns it.
const model = arg('--model', 'opus');

function ask(dir, file, ctx) {
  const args = [
    '-p', TEACH_INSTRUCTIONS(ctx),
    '--output-format', 'json',
    '--json-schema', JSON.stringify(SECTIONS_SCHEMA),
    '--permission-mode', 'dontAsk',
    '--max-turns', '8',
    '--model', model,
  ];
  let raw;
  try {
    raw = execFileSync('claude', args, {
      input: stripHeader(splitTrailingTeach(readFileSync(join(dir, file), 'utf8')).code).body,
      encoding: 'utf8',
      maxBuffer: 32 * 1024 * 1024,
      stdio: ['pipe', 'pipe', 'pipe'],
    });
  } catch (e) {
    let env = null;
    try { env = JSON.parse(String(e.stdout ?? '')); } catch { /* not JSON */ }
    const why = env
      ? `${env.terminal_reason ?? env.subtype ?? 'error'} - ${String(env.result ?? '').slice(0, 300)}`
      : String(e.stderr || e.message).slice(0, 300);
    throw new Error(`claude failed (exit ${e.status}): ${why}`);
  }
  const env = JSON.parse(raw);
  const out = env.structured_output;
  if (!out || !Array.isArray(out.sections) || !out.pattern) {
    throw new Error(`no usable structured_output (result: ${String(env.result).slice(0, 200)})`);
  }
  return { out, cost: env.total_cost_usd, turns: env.num_turns };
}

// ---------------------------------------------------------------- run

const state = loadState();
const everything = scanRepo(state);

/** Every curated file has a block, generated from the classification on record. */
const atCurrentStandard = (p) => {
  const rec = state.problems[p.slug] ?? {};
  const t = rec.teachSignatures ?? {};
  const sigs = rec.headerSignatures ?? {};
  if (!p.curatedFiles.length) return false;
  return p.curatedFiles.every((f) => {
    if (!t[f] || !sigs[f] || t[f] !== sigs[f]) return false;
    return splitTrailingTeach(readFileSync(join(p.dir, f), 'utf8')).had;
  });
};

// --backfill widens the scope to every curated folder and drops the ones already
// done, so successive runs work THROUGH the repo instead of redoing the first N.
// Previously --backfill widened nothing, so in the workflow it selected
// pendingOnly - which during a backfill is empty, and the step did nothing.
let targets;
if (only) {
  targets = everything.filter((p) => p.slug === only);
} else if (backfill) {
  const all = everything.filter((p) => p.curatedFiles.length);
  targets = force ? all : all.filter((p) => !atCurrentStandard(p));
  console.log(`\nbackfill: ${all.length} folder(s), ${all.length - targets.length} already done, ${targets.length} remaining`);
} else {
  targets = pendingOnly(everything);
}
if (limit) targets = targets.slice(0, limit);

if (!targets.length) {
  console.log(only ? `\nNo folder with slug "${only}".\n` : '\nNothing pending. Use --slug or --backfill --slug.\n');
  process.exit(0);
}

console.log(`\n${doApply ? 'APPLY' : 'DRY RUN'} - teaching blocks, model ${model}, ${targets.length} folder(s)\n`);

let failures = 0, wrote = 0, skipped = 0;

for (const p of targets) {
  console.log(p.path);
  // Read-only views. Creating rec.teachSignatures here would stamp an empty
  // object onto every folder merely looked at, which is a state.json diff and
  // therefore a commit for work that did not happen.
  const rec = state.problems[p.slug] ?? {};
  const sigs = rec.headerSignatures ?? {};
  const teach = rec.teachSignatures ?? {};

  for (const file of p.curatedFiles) {
    // Staleness is judged against the classification signature, so a file whose
    // ranking and complexity are unchanged keeps the prose it already has.
    const sig = sigs[file] ?? null;
    const src = readFileSync(join(p.dir, file), 'utf8');
    const hasBlock = splitTrailingTeach(src).had;

    if (!force && hasBlock && teach[file] && teach[file] === sig) {
      console.log(`  ${file.padEnd(22)} up to date`);
      skipped++;
      continue;
    }

    const prov = rec.provenance?.[file] ?? {
      selfMarked: isSelfMarked(stripHeader(splitTrailingTeach(src).code).body) || null,
      evidence: 'detected from the curated file',
    };
    const ctx = {
      source: sourceFor(null, file, prov),
      status: statusFor(file),
      time: 'unknown', space: 'unknown',
    };
    // Complexity comes from the stored classification signature, never re-asked.
    if (sig) { try { const s = JSON.parse(sig); ctx.time = s.time; ctx.space = s.space; } catch {} }
    if (ctx.time === 'unknown') {
      console.log(`  ${file.padEnd(22)} SKIPPED - no classification on record; run classify.mjs --apply first`);
      skipped++;
      continue;
    }

    let r;
    try { r = ask(p.dir, file, ctx); }
    catch (e) { console.log(`  ${file.padEnd(22)} FAILED: ${e.message}`); failures++; continue; }

    const block = buildTeachingBlock(r.out, ctx);
    console.log(`  ${file.padEnd(22)} ${r.out.sections.length} section(s), ${block.split('\n').length} lines  ·  $${r.cost} · ${r.turns} turns`);
    if (!doApply) {
      console.log(block.split('\n').map((l) => '      ' + l).join('\n'));
      continue;
    }
    // banner header -> code -> teaching block. The block goes last, so the file
    // still opens on the code rather than on fifty lines of prose.
    const { code, eol } = splitTrailingTeach(src);
    const out = code.replace(/(\r?\n)+$/, '') + eol + eol + block.split('\n').join(eol) + eol;
    writeFileSync(join(p.dir, file), out, 'utf8');

    // Materialise the record only now that there is something to record.
    const prec = state.problems[p.slug] ?? (state.problems[p.slug] = {});
    (prec.teachSignatures ?? (prec.teachSignatures = {}))[file] = sig;
    wrote++;
  }
  console.log('');
}

if (doApply && wrote) saveState(state);
console.log(`${wrote} written, ${skipped} left alone, ${failures} failed.\n`);
if (process.env.GITHUB_OUTPUT) appendFileSync(process.env.GITHUB_OUTPUT, `wrote=${wrote}\n`);
process.exit(failures ? 1 : 0);
