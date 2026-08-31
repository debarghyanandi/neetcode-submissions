#!/usr/bin/env node
/**
 * visualize.mjs - MILESTONE 4b. Builds <slug>-visualizer.html for a problem
 * that does not have one.
 *
 * It NEVER overwrites an existing visualizer. The 23 already in this repo were
 * built and checked by hand; regenerating them would trade work that is known
 * good for work that merely validates.
 *
 * Only the PROBLEM object is generated. The surrounding 32KB - palette, fonts,
 * panels, transport, keyboard handling - is spliced verbatim from the chassis,
 * so a new visualizer matches the existing ones exactly rather than
 * approximately.
 *
 *   node scripts/visualize.mjs --slug two-integer-sum          # dry run
 *   node scripts/visualize.mjs --slug two-integer-sum --apply
 *   node scripts/visualize.mjs --apply --limit 3
 */

import { readFileSync, writeFileSync, existsSync, appendFileSync } from 'node:fs';
import { join } from 'node:path';
import { execFileSync } from 'node:child_process';
import { loadState, saveState, scanRepo, REPO } from './lib/scan.mjs';
import { stripHeader } from './lib/header.mjs';
import { splitTrailingTeach } from './lib/teach.mjs';
import { splice, validate, selectForVisualizer, loadChassis } from './lib/visualizer.mjs';

const argv = process.argv.slice(2);
const arg = (n, d = null) => (argv.includes(n) ? argv[argv.indexOf(n) + 1] : d);
const has = (n) => argv.includes(n);

const only = arg('--slug');
const limit = Number(arg('--limit', '0')) || 0;
const doApply = has('--apply');
const model = arg('--model', 'opus');

const SCHEMA = {
  type: 'object',
  additionalProperties: false,
  properties: {
    problemSource: {
      type: 'string',
      description: 'JavaScript source for exactly one statement: const PROBLEM = { ... }; Nothing else - no imports, no markdown fence, no commentary.',
    },
  },
  required: ['problemSource'],
};

/** The contract, taken from the chassis itself rather than described from memory. */
function contract() {
  const c = loadChassis();
  const helpers = c.slice(c.indexOf('<script>') + 8, c.indexOf('/*__PROBLEM__*/'));
  return helpers.trim();
}

const EXAMPLE = () => {
  // A real, working definition from this repo beats any amount of description.
  const f = join(REPO, 'Data Structures & Algorithms', 'buy-and-sell-crypto', 'buy-and-sell-crypto-visualizer.html');
  const src = readFileSync(f, 'utf8').split(/\r?\n/);
  const s = src.findIndex((l) => /^const PROBLEM = \{/.test(l));
  let depth = 0, end = -1;
  for (let i = s; i < src.length && end < 0; i++) {
    for (const ch of src[i]) { if (ch === '{') depth++; else if (ch === '}' && --depth === 0) { end = i; break; } }
  }
  return src.slice(s, end + 1).join('\n');
};

function instructions(slug, sols, feedback) {
  return [
    `Write the PROBLEM definition for the NeetCode problem "${slug}", to be spliced into an existing visualizer.`,
    '',
    'These helper functions and panel constructors already exist. Use them; do not redefine them:',
    '```', contract(), '```',
    '',
    'Here is a complete, working PROBLEM definition for a different problem. Match its shape, its',
    'voice and its level of detail exactly:',
    '```', EXAMPLE(), '```',
    '',
    `Build one entry in "solutions" for each of these ${sols.length} solution file(s), in this order,`,
    'faithfully animating what that code actually does - not a tidier algorithm you would prefer:',
    ...sols.map((s) => `  - ${s.file}: ${s.time} time / ${s.space} space, ${s.algorithm}. badge should end with "${s.file}".`),
    '',
    'PANEL SHAPES - get these exactly right, the renderer does not tolerate a wrong one:',
    "  pTiles(title, values, decorate)   values is a flat array; decorate(i, v) returns {cls}",
    "  pBars(title, values, decorate)    same shape as pTiles, drawn as bars",
    "  pPills(title, items)              items: [{k, v, cls}]",
    "  pSlots(title, items)              items: [{text, cls}]",
    "  pNote(title, html)                html is a string",
    "  pRanges(title, items, min, max)   min and max are numbers",
    "  pChips(title, rows)               rows is an array of ROWS, NOT of chips.",
    "                                    Each row is {left, right, items: [{text, sub, cls}], empty}.",
    "                                    One row of chips is still [{items: [...]}] - a one-element array.",
    '',
    'Requirements:',
    '- Every step\'s "lines" must be 1-based indices into THAT solution\'s own "code" array. A line',
    '  number outside it highlights nothing and the visualizer silently reads as broken.',
    '- parse() must accept its own default input value.',
    '- Keep the default input small enough that the whole run is watchable - well under 60 steps.',
    '- "msg" is HTML; <b>, <code> and <em> are available. Explain WHY the step happens.',
    '- "blurb" must fit five rendered lines: keep it under 450 characters of visible text, and',
    '  under 300 if you can. The existing visualizers in this repo average about 210. It is the',
    '  one-paragraph reason the approach works, not a summary of the teaching block.',
    '- Output only the statement: const PROBLEM = { ... };',
    '',
    'You have no tools and no filesystem access. The solution code is on stdin and everything else',
    'you need is above. Do not attempt to read, list or search files - answer directly.',
    ...(feedback ? ['', 'Your previous attempt failed validation. Fix exactly these:', ...feedback.map((e) => `  - ${e}`)] : []),
  ].join('\n');
}

function ask(prompt, code) {
  const args = ['-p', prompt, '--output-format', 'json', '--json-schema', JSON.stringify(SCHEMA),
                '--permission-mode', 'dontAsk', '--max-turns', '20', '--model', model];
  let raw;
  try {
    raw = execFileSync('claude', args, { input: code, encoding: 'utf8', maxBuffer: 64 * 1024 * 1024, stdio: ['pipe','pipe','pipe'] });
  } catch (e) {
    let env = null; try { env = JSON.parse(String(e.stdout ?? '')); } catch { /* not JSON */ }
    const detail = env
      ? [env.terminal_reason, env.subtype, env.num_turns != null ? `${env.num_turns} turns used` : null,
         env.result ? String(env.result).slice(0, 300) : null].filter(Boolean).join(' · ')
      : String(e.stderr || e.message).slice(0, 300);
    throw new Error(`claude failed (exit ${e.status}): ${detail}`);
  }
  const env = JSON.parse(raw);
  if (!env.structured_output?.problemSource) throw new Error(`no problemSource (result: ${String(env.result).slice(0,200)})`);
  // Models like fences even when told not to.
  const src = env.structured_output.problemSource.replace(/^\s*```(?:javascript|js)?\s*/i, '').replace(/```\s*$/, '').trim();
  return { src, cost: env.total_cost_usd, turns: env.num_turns };
}

// ---------------------------------------------------------------- run

const state = loadState();
let targets = scanRepo(state);
if (only) targets = targets.filter((p) => p.slug === only);
targets = targets.filter((p) => {
  if (existsSync(join(p.dir, `${p.slug}-visualizer.html`))) {
    if (only) console.log(`\n${p.path} already has a visualizer - refusing to overwrite hand-checked work.\n`);
    return false;
  }
  return true;
});
if (limit) targets = targets.slice(0, limit);

if (!targets.length) { console.log('\nNothing to build - every problem already has a visualizer.\n'); process.exit(0); }

console.log(`\n${doApply ? 'APPLY' : 'DRY RUN'} - visualizers, model ${model}, ${targets.length} folder(s)\n`);
let failures = 0, wrote = 0;

for (const p of targets) {
  console.log(p.path);
  const cls = state.problems[p.slug]?.classification ?? {};
  const { chosen, dropped, unclassified } = selectForVisualizer(p.curatedFiles, cls);
  if (dropped.length) console.log(`  dropped (brute force, real solutions exist): ${dropped.join(', ')}`);
  if (unclassified.length) console.log(`  unclassified, run classify --apply first: ${unclassified.join(', ')}`);
  if (!chosen.length) { console.log('  nothing to visualise\n'); failures++; continue; }
  console.log(`  visualising: ${chosen.join(', ')}`);

  const sols = chosen.map((f) => ({ file: f, ...cls[f] }));
  const code = chosen
    .map((f) => `===== FILE: ${f} =====\n${stripHeader(splitTrailingTeach(readFileSync(join(p.dir, f), 'utf8')).code).body}`)
    .join('\n\n');

  let result = null, feedback = null, spend = 0;
  for (let attempt = 1; attempt <= 2 && !result; attempt++) {
    let r;
    try { r = ask(instructions(p.slug, sols, feedback), code); }
    catch (e) {
      console.log(`  attempt ${attempt} FAILED: ${e.message}`);
      // Retry a run that simply ran out of room; do not retry an auth or
      // configuration failure, which will fail identically the second time.
      if (/max_turns|overloaded|rate_limit|timeout/i.test(e.message)) {
        feedback = ['Your previous attempt ran out of room before finishing. Answer immediately, in one reply, with no preamble.'];
        continue;
      }
      break;
    }
    spend += r.cost ?? 0;
    const v = validate(r.src);
    if (!v.errors.length) {
      result = r;
      for (const [k, n] of Object.entries(v.stats)) console.log(`      ${k}: ${n}`);
    } else {
      console.log(`  attempt ${attempt} rejected by validation:`);
      v.errors.slice(0, 6).forEach((e) => console.log(`      ${e}`));
      feedback = v.errors.slice(0, 8);
    }
  }

  if (!result) { console.log(`  giving up on ${p.slug}\n`); failures++; continue; }
  console.log(`  validated  ·  $${spend.toFixed(4)} · ${result.turns} turns`);

  if (doApply) {
    const out = join(p.dir, `${p.slug}-visualizer.html`);
    writeFileSync(out, splice(result.src), 'utf8');
    console.log(`  wrote ${p.slug}-visualizer.html`);
    wrote++;
  }
  console.log('');
}

if (doApply && wrote) saveState(state);
console.log(`${wrote} written, ${failures} failed.\n`);
if (process.env.GITHUB_OUTPUT) appendFileSync(process.env.GITHUB_OUTPUT, `wrote=${wrote}\n`);
process.exit(failures ? 1 : 0);
