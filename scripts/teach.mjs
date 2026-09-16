#!/usr/bin/env node
/**
 * teach.mjs - MILESTONE 4a. Writes the study preamble on solution files.
 *
 * One model call per FILE (the headers differ per solution), against the
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
import { SECTIONS_SCHEMA, TEACH_INSTRUCTIONS, buildTeachingBlock, statusFor, sourceFor, splitTrailingTeach, toSections } from './lib/teach.mjs';
import { isSelfMarked } from './lib/complexity.mjs';
import { report, reportCost, group, endGroup } from './lib/report.mjs';
import { isOutOfBudget, stop as budgetStop, announce as announceBudget, haltIfStopped } from './lib/budget.mjs';
import { effortArgs, usageOf, usageLine, leanArgs } from './lib/usage.mjs';
import { mkdirSync } from 'node:fs';

const argv = process.argv.slice(2);
const arg = (n, d = null) => (argv.includes(n) ? argv[argv.indexOf(n) + 1] : d);
const has = (n) => argv.includes(n);

// A comma-separated list, so the workflow can hand over everything classify
// just processed in one argument.
const onlyRaw = arg('--slug');
const only = onlyRaw ? onlyRaw.split(',').map((x) => x.trim()).filter(Boolean) : null;
const limit = Number(arg('--limit', '0')) || 0;
const doApply = has('--apply');
const backfill = has('--backfill');
const force = has('--force');
// Opus. Sonnet was tried in 2026-09 and was cheaper per call, but on the fixed section
// schema it kept repeating one point across sections and claimed "O(1) extra space" for a
// recursive flood fill whose call stack is O(m*n) - a wrong answer to give an interviewer.
// Once it also started taking a third turn, it cost about what Opus does. --model sonnet
// still works for a comparison.
const model = arg('--model', 'opus');
// No --effort means the model's own default. Only ever set by hand, for comparisons.
const effort = arg('--effort');
// One file inside the folder, so a model comparison does not pay for every file.
const fileOnly = arg('--file');
// Also pick up any curated folder with a file that has NO teaching block. Nothing is in that
// state normally; it is how a folder gets finished after a failed run stopped before teach.
const unfinished = has('--unfinished');

/**
 * Read a stream-json run: every event on its own line, the last `result` event being the
 * same envelope --output-format json would have returned.
 *
 * Why stream-json: a call that needs an extra turn only says "3 turns" in the envelope, and
 * the saved response lived in .agent/tmp on the runner, which is deleted with it. The stream
 * carries the conversation itself - including the tool result that told the model its first
 * answer was rejected - so the reason can be printed straight into the CI log.
 */
function parseStream(text) {
  const events = [];
  for (const line of String(text ?? '').split(/\r?\n/)) {
    if (!line.startsWith('{')) continue;
    try { events.push(JSON.parse(line)); } catch { /* partial line */ }
  }
  const env = [...events].reverse().find((e) => e.type === 'result') ?? null;
  if (env && env.structured_output === undefined) {
    // Fall back to the last structured-output tool call, should the result event not carry it.
    for (const e of [...events].reverse()) {
      const call = (e.type === 'assistant' ? e.message?.content ?? [] : []).find((b) => b.type === 'tool_use');
      if (call?.input && typeof call.input === 'object') { env.structured_output = call.input; break; }
    }
  }
  return { events, env };
}

/** What happened between the first answer and the last: rejected tool results and any text. */
function extraTurnReasons(events) {
  const out = [];
  for (const e of events) {
    for (const b of e.message?.content ?? []) {
      if (b.type === 'tool_result' && b.is_error) {
        const t = Array.isArray(b.content) ? b.content.map((c) => c.text ?? '').join(' ') : String(b.content ?? '');
        out.push(`tool rejected: ${t.replace(/\s+/g, ' ').slice(0, 300)}`);
      }
      if (e.type === 'assistant' && b.type === 'text' && b.text?.trim()) {
        out.push(`model said: ${b.text.replace(/\s+/g, ' ').slice(0, 200)}`);
      }
    }
  }
  return out;
}

function ask(dir, file, ctx) {
  const args = [
    '-p', TEACH_INSTRUCTIONS(ctx),
    '--output-format', 'stream-json', '--verbose',
    '--json-schema', JSON.stringify(SECTIONS_SCHEMA),
    '--permission-mode', 'dontAsk',
    '--max-turns', '8',
    '--model', model,
    ...effortArgs(effort),
    ...leanArgs(),
  ];
  let raw;
  try {
    raw = execFileSync('claude', args, {
      input: stripHeader(splitTrailingTeach(readFileSync(join(dir, file), 'utf8')).code).body,
      encoding: 'utf8',
      maxBuffer: 64 * 1024 * 1024,
      stdio: ['pipe', 'pipe', 'pipe'],
    });
  } catch (e) {
    const { env } = parseStream(e.stdout);
    const why = env
      ? `${env.terminal_reason ?? env.subtype ?? 'error'} - ${String(env.result ?? '').slice(0, 300)}`
      : String(e.stderr || e.message).slice(0, 300);
    throw new Error(`claude failed (exit ${e.status}): ${why}`);
  }
  const { events, env } = parseStream(raw);
  if (!env) throw new Error(`no result event in the stream (${events.length} events)`);
  // A clean structured answer takes 2 turns. More means something was retried, and the extra
  // request re-sends the conversation so far. Print why, in the log, where it survives the runner.
  if ((env.num_turns ?? 0) > 2) {
    const reasons = extraTurnReasons(events);
    console.log(`  note: ${env.num_turns} turns - ${reasons.length ? 'why:' : 'no rejection found in the stream'}`);
    for (const r of reasons.slice(0, 4)) console.log(`        ${r}`);
    try {
      mkdirSync(join(REPO, '.agent', 'tmp'), { recursive: true });
      writeFileSync(join(REPO, '.agent', 'tmp', 'teach-extra-turns.jsonl'), raw, 'utf8');
    } catch { /* diagnostics only */ }
  }
  const out = env.structured_output;
  const missing = SECTIONS_SCHEMA.required.filter((k) => out?.[k] === undefined || out?.[k] === '');
  if (!out || missing.length) {
    throw new Error(`no usable structured_output${missing.length ? ` - missing ${missing.join(', ')}` : ''} (result: ${String(env.result).slice(0, 200)})`);
  }
  out.sections = toSections(out, ctx);
  return { out, cost: env.total_cost_usd, turns: env.num_turns, usage: usageOf(env) };
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
  targets = everything.filter((p) => only.includes(p.slug));
} else if (backfill) {
  const all = everything.filter((p) => p.curatedFiles.length);
  targets = force ? all : all.filter((p) => !atCurrentStandard(p));
  console.log(`\nbackfill: ${all.length} folder(s), ${all.length - targets.length} already done, ${targets.length} remaining`);
} else if (unfinished) {
  // Only the stranded folders below - not every folder with a raw submission waiting.
  targets = [];
} else {
  targets = pendingOnly(everything);
}
if (unfinished) {
  const stranded = everything.filter((p) => p.curatedFiles.length && !targets.includes(p) &&
    p.curatedFiles.some((f) => !splitTrailingTeach(readFileSync(join(p.dir, f), 'utf8')).had));
  if (stranded.length) console.log(`\nunfinished from an earlier run: ${stranded.map((p) => p.slug).join(', ')}`);
  targets = [...targets, ...stranded];
}
if (limit) targets = targets.slice(0, limit);

if (!targets.length) {
  console.log(only ? `\nNo folder with slug "${only}".\n` : '\nNothing pending. Use --slug or --backfill --slug.\n');
  process.exit(0);
}

console.log(`\n${doApply ? 'APPLY' : 'DRY RUN'} - teaching blocks, model ${model}, effort ${effort ?? 'default'}, ${targets.length} folder(s)\n`);

if (haltIfStopped('teach', targets.map((p) => p.slug))) process.exit(1);

let failures = 0, wrote = 0, skipped = 0;
// Set when the ACCOUNT runs out rather than a file going wrong. Every file
// after it would get the same refusal, so the loops below stop instead of
// buying the same answer once per file - and summarise.mjs fails the run.
let outOfBudget = null;

for (const p of targets) {
  group(p.path);
  // Read-only views. Creating rec.teachSignatures here would stamp an empty
  // object onto every folder merely looked at, which is a state.json diff and
  // therefore a commit for work that did not happen.
  const rec = state.problems[p.slug] ?? {};
  const sigs = rec.headerSignatures ?? {};
  const teach = rec.teachSignatures ?? {};

  for (const file of p.curatedFiles) {
    if (fileOnly && file !== fileOnly) continue;
    // Staleness is judged against the classification signature, so a file whose
    // ranking and complexity are unchanged keeps the prose it already has.
    const sig = sigs[file] ?? null;
    const src = readFileSync(join(p.dir, file), 'utf8');
    const hasBlock = splitTrailingTeach(src).had;

    if (!force && hasBlock && teach[file] && teach[file] === sig) {
      console.log(`  ${file.padEnd(22)} up to date`);
      report('teach', p.slug, 'skipped', `${file}: up to date`);
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
    catch (e) {
      console.log(`  ${file.padEnd(22)} FAILED: ${e.message}`);
      if (isOutOfBudget(e)) {
        outOfBudget = e.message;
        budgetStop('teach', e.message);
        report('teach', p.slug, 'skipped', `${file}: stopped - out of budget, nothing wrong with this file`);
        break;
      }
      report('teach', p.slug, 'failed', `${file}: ${e.message.slice(0,70)}`);
      failures++;
      continue;
    }

    reportCost('teach', p.slug, r.cost, r.usage);
    const block = buildTeachingBlock(r.out, ctx);
    console.log(`  ${file.padEnd(22)} ${r.out.sections.length} section(s), ${block.split('\n').length} lines  ·  $${r.cost} · ${r.turns} turns`);
    console.log(`  ${''.padEnd(22)} ${usageLine(r.usage)}`);
    if (!doApply) {
      console.log(block.split('\n').map((l) => '      ' + l).join('\n'));
      // Kept on disk (gitignored) so two models' blocks can be read side by side.
      const tryDir = join(REPO, '.agent', 'tmp', 'try');
      mkdirSync(tryDir, { recursive: true });
      const saved = join(tryDir, `teach-${p.slug}-${file.replace(/\.cs$/, '')}-${model}-${effort ?? 'default'}.txt`);
      writeFileSync(saved, `// model ${model} · effort ${effort ?? 'default'} · $${r.cost} · ${r.turns} turns\n// ${usageLine(r.usage)}\n\n${block}\n`, 'utf8');
      console.log(`  saved for comparison: ${saved}`);
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
    report('teach', p.slug, 'ok', `${file}: ${r.out.sections.length} sections`);
    wrote++;
  }
  endGroup();
  if (outOfBudget) break;
}

if (doApply && wrote) saveState(state);
if (outOfBudget) announceBudget(outOfBudget);
console.log(`${wrote} written, ${skipped} left alone, ${failures} failed${outOfBudget ? ', rest not attempted' : ''}.\n`);
if (process.env.GITHUB_OUTPUT) appendFileSync(process.env.GITHUB_OUTPUT, `wrote=${wrote}\n`);
process.exit(failures || outOfBudget ? 1 : 0);
