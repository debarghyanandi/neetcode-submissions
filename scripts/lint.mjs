#!/usr/bin/env node
/**
 * lint.mjs - tidy spacing and variable names in the solution code.
 *
 * Runs FIRST in the pipeline, before classify. That ordering is the whole point:
 * the header, the teaching block and the visualizer all describe the code by
 * name, so if lint ran after them they would describe variables that no longer
 * exist. Lint first, and everything downstream sees the final code.
 *
 * Every rewrite is checked by sameShape() before it is written: the new file
 * must differ from the old only in whitespace, comments, and a consistent
 * one-to-one renaming of local variables. Anything else - a flipped comparison,
 * a changed literal, a dropped statement, a renamed member - is refused. The
 * model is not trusted to leave the logic alone; it is prevented from changing it.
 *
 *   node scripts/lint.mjs --slug two-integer-sum          # dry run, shows a diff
 *   node scripts/lint.mjs --slug two-integer-sum --apply
 *   node scripts/lint.mjs --apply --limit 3               # folders with raw submissions
 *   node scripts/lint.mjs --apply --backfill --limit 3    # every folder not yet linted
 */

import { readFileSync, writeFileSync, appendFileSync } from 'node:fs';
import { join } from 'node:path';
import { execFileSync } from 'node:child_process';
import { loadState, saveState, scanRepo, pendingOnly, foldersChangedSince } from './lib/scan.mjs';
import { stripHeader } from './lib/header.mjs';
import { splitTrailingTeach } from './lib/teach.mjs';
import { sameShape } from './lib/csharp.mjs';
import { shortPrint } from './lib/normalise.mjs';
import { LINT_FORMAT } from './lib/lint-rules.mjs';
import { report, group, endGroup } from './lib/report.mjs';


const argv = process.argv.slice(2);
const arg = (n, d = null) => (argv.includes(n) ? argv[argv.indexOf(n) + 1] : d);
const has = (n) => argv.includes(n);

const onlyRaw = arg('--slug');
const only = onlyRaw ? onlyRaw.split(',').map((s) => s.trim()).filter(Boolean) : null;
const limit = Number(arg('--limit', '0')) || 0;
const doApply = has('--apply');
const backfill = has('--backfill');
const force = has('--force');
const model = arg('--model', 'sonnet');

const SCHEMA = {
  type: 'object',
  additionalProperties: false,
  properties: {
    code: { type: 'string', description: 'The complete rewritten C# source. No markdown fence, no commentary.' },
  },
  required: ['code'],
};

const INSTRUCTIONS = [
  'Tidy the formatting and the variable names in this C# solution, on stdin.',
  '',
  'You may change ONLY these things:',
  '  - whitespace and indentation',
  '  - comments',
  '  - the names of LOCAL VARIABLES and parameters',
  '',
  'You may NOT change anything else. Not a comparison, not a literal, not the order of',
  'arguments, not a member name after a dot, not the class or method names, not the',
  'structure. The rewrite is checked mechanically and will be rejected if the token',
  'sequence differs by anything other than names and spacing, so a "small improvement"',
  'to the logic fails the whole file rather than shipping.',
  '',
  'Formatting: four spaces per level, Allman braces (opening brace on its own line), one',
  'space around binary operators, no trailing whitespace, no line over roughly 100 columns.',
  '',
  'Naming: descriptive enough to read without scrolling back, short enough to scan.',
  '  - Keep i, j, k when they are ordinary loop counters. They are idiomatic, not lazy.',
  '  - Rename anything whose meaning you had to work out: s, t, c, n, l, x, q, op, lc.',
  '  - Say what the value IS, not its type: charCount not intDict, windowSum not tempInt.',
  '  - Two or three words at most. leftBoundaryIndexOfWindow is worse than left.',
  '  - Match the vocabulary of the problem: prices, window, seen, remaining.',
  '',
  'COMMENTS. Every comment in this file was written by the author for their own use later.',
  '  - Never delete one. Deleting a comment fails the check and the whole file is rejected.',
  '  - Keep the wording. Do not tidy, shorten, formalise or merge them.',
  '  - A note to self, a TODO, or an observation that another approach would be better',
  '    ("this is good but mLogn - we need log(m*n)") is the most valuable thing in the file.',
  '    Leave it exactly as written, even where it points out a flaw.',
  '  - Change a comment ONLY when a rename made it name a variable that no longer exists,',
  '    or when it states something the code plainly contradicts. Then make the smallest',
  '    possible edit and keep the author\'s voice.',
  '',
  'Every file is processed on its merits. Nothing a comment says - not a TODO, not a note',
  'that the solution is imperfect - is a reason to skip the file or leave it alone.',
  'Return the complete file.',
].join('\n');

function ask(code, feedback) {
  const args = ['-p', feedback ? INSTRUCTIONS + '\n\nYour previous attempt was REJECTED:\n' + feedback.map((e) => '  - ' + e).join('\n') + '\nReturn a rewrite that changes only names and spacing.' : INSTRUCTIONS,
                '--output-format', 'json', '--json-schema', JSON.stringify(SCHEMA),
                '--permission-mode', 'dontAsk', '--max-turns', '12', '--model', model];
  let raw;
  try {
    raw = execFileSync('claude', args, { input: code, encoding: 'utf8', maxBuffer: 32 * 1024 * 1024, stdio: ['pipe','pipe','pipe'] });
  } catch (e) {
    let env = null; try { env = JSON.parse(String(e.stdout ?? '')); } catch { /* not JSON */ }
    throw new Error(`claude failed (exit ${e.status}): ${env ? [env.terminal_reason, env.subtype, env.num_turns != null ? env.num_turns + ' turns' : null].filter(Boolean).join(' · ') : String(e.stderr || e.message).slice(0, 200)}`);
  }
  const env = JSON.parse(raw);
  const out = env.structured_output?.code;
  if (!out) throw new Error(`no code returned (result: ${String(env.result).slice(0, 200)})`);
  return { code: out.replace(/^\s*```(?:csharp|cs)?\s*/i, '').replace(/```\s*$/, ''), cost: env.total_cost_usd, turns: env.num_turns };
}

// ---------------------------------------------------------------- run

const state = loadState();
const everything = scanRepo(state);

const needsLint = (p) => p.curatedFiles.concat(p.pending.map((s) => s.file)).some((f) => {
  const rec = state.problems[p.slug]?.lint?.[f];
  return !rec || rec.version !== LINT_FORMAT;
});

let targets;
if (only) {
  targets = everything.filter((p) => only.includes(p.slug));
} else if (backfill) {
  const all = everything.filter((p) => p.curatedFiles.length || p.pending.length);
  targets = force ? all : all.filter(needsLint);
  console.log(`\nbackfill: ${all.length} folder(s), ${all.length - targets.length} already linted, ${targets.length} remaining`);
} else {
  targets = pendingOnly(everything);
}
// Same exclusion contract as detect and classify: on a push run, leave alone
// the folder that was just pushed to.
const exclude = new Set();
for (const v of argv.flatMap((a, i) => (a === '--exclude' ? [argv[i + 1]] : [])))
  String(v ?? '').split(',').map((x) => x.trim()).filter(Boolean).forEach((x) => exclude.add(x));
for (const slug of foldersChangedSince(arg('--exclude-changed-since'))) exclude.add(slug);
if (exclude.size) {
  const before = targets.length;
  targets = targets.filter((p) => !exclude.has(p.slug));
  console.log(`excluding ${[...exclude].join(', ')} (${before - targets.length} held back)`);
}

if (limit) targets = targets.slice(0, limit);

if (!targets.length) { console.log('\nNothing to lint.\n'); process.exit(0); }

console.log(`\n${doApply ? 'APPLY' : 'DRY RUN'} - lint, model ${model}, ${targets.length} folder(s)\n`);

let failures = 0, changed = 0, clean = 0;
const touchedSlugs = new Set();

for (const p of targets) {
  group(p.path);
  const files = [...p.curatedFiles, ...p.pending.map((s) => s.file)];

  for (const file of files) {
    const full = join(p.dir, file);
    const raw = readFileSync(full, 'utf8');
    const { code: withHeader, eol } = splitTrailingTeach(raw);
    const teachBlock = raw.slice(withHeader.length);
    const { body, had } = stripHeader(withHeader);
    const header = had ? withHeader.slice(0, withHeader.length - body.length) : '';

    const rec = state.problems[p.slug]?.lint?.[file];
    if (!force && rec && rec.version === LINT_FORMAT) { console.log(`  ${file.padEnd(22)} already linted`); report('lint', p.slug, 'skipped', `${file}: already linted`); clean++; continue; }

    let result = null, feedback = null, spend = 0;
    for (let attempt = 1; attempt <= 2 && !result; attempt++) {
      let r;
      try { r = ask(body, feedback); }
      catch (e) { console.log(`  ${file.padEnd(22)} attempt ${attempt} FAILED: ${e.message}`); break; }
      spend += r.cost ?? 0;
      const check = sameShape(body, r.code);
      if (check.ok) result = { ...r, renames: check.renames };
      else {
        console.log(`  ${file.padEnd(22)} attempt ${attempt} REJECTED - the rewrite changed more than names:`);
        check.errors.slice(0, 3).forEach((e) => console.log(`      ${e}`));
        feedback = [
        ...check.errors,
        'Every distinct variable must keep a distinct name. If two variables would end up with the same name, pick different names for both rather than merging them.',
      ];
      }
    }

    if (!result) {
      console.log(`  ${file.padEnd(22)} left untouched`);
      report('lint', p.slug, 'failed', `${file}: rewrite rejected twice - ${(feedback && feedback[0]) || 'unknown'}`);
      // Record the attempt so the next backfill does not retry it. Two model
      // calls that end in the same rejection will end in it again, and this
      // file would otherwise be paid for on every run forever. --force retries.
      if (doApply) {
        const prec = state.problems[p.slug] ?? (state.problems[p.slug] = {});
        (prec.lint ?? (prec.lint = {}))[file] = {
          version: LINT_FORMAT,
          failed: true,
          reason: (feedback && feedback[0]) || 'rewrite rejected twice',
        };
      }
      failures++;
      continue;
    }

    const same = result.code.trim() === body.trim();
    console.log(`  ${file.padEnd(22)} ${same ? 'nothing to change' : result.renames.length ? 'renames: ' + result.renames.map(([a, b]) => `${a}->${b}`).join(', ') : 'spacing only'}  ·  $${(spend || 0).toFixed(4)}`);
    if (!same && !doApply) {
      console.log(result.code.split('\n').slice(0, 12).map((l) => '      | ' + l).join('\n'));
    }

    if (doApply) {
      writeFileSync(full, header + result.code.replace(/^(\r?\n)+/, '').replace(/\s+$/, '') + eol + teachBlock, 'utf8');
      const prec = state.problems[p.slug] ?? (state.problems[p.slug] = {});
      (prec.lint ?? (prec.lint = {}))[file] = {
        version: LINT_FORMAT,
        codePrint: shortPrint(result.code),
        renames: result.renames.map(([a, b]) => `${a}->${b}`),
      };
      report('lint', p.slug, 'ok', `${file}: ${result.renames.length ? result.renames.map(([a, b]) => `${a}->${b}`).join(', ') : same ? 'nothing to change' : 'spacing tidied'}`);
      if (!same) { changed++; touchedSlugs.add(p.slug); }
    }
  }
  endGroup();
}

if (doApply) saveState(state);
console.log(`${changed} file(s) changed, ${clean} already linted, ${failures} failed.\n`);
if (process.env.GITHUB_OUTPUT) {
  appendFileSync(process.env.GITHUB_OUTPUT, `changed=${changed}\n`);
  appendFileSync(process.env.GITHUB_OUTPUT, `slugs=${[...touchedSlugs].join(',')}\n`);
}
process.exit(failures ? 1 : 0);
