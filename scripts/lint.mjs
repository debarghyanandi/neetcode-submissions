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
import { LINT_FORMAT, LINT_MAX_ROUNDS, lintDecision, lintWanted } from './lib/lint-rules.mjs';
import { report, reportCost, group, endGroup } from './lib/report.mjs';
import { isOutOfBudget, stop as budgetStop, announce as announceBudget, haltIfStopped } from './lib/budget.mjs';
import { usageOf, usageLine, leanArgs } from './lib/usage.mjs';
import { formatMany, dotnetAvailable, toEol } from './lib/format.mjs';


const argv = process.argv.slice(2);
const arg = (n, d = null) => (argv.includes(n) ? argv[argv.indexOf(n) + 1] : d);
const has = (n) => argv.includes(n);

const onlyRaw = arg('--slug');
const only = onlyRaw ? onlyRaw.split(',').map((s) => s.trim()).filter(Boolean) : null;
const limit = Number(arg('--limit', '0')) || 0;
const doApply = has('--apply');
const backfill = has('--backfill');
const force = has('--force');
// Haiku: lint changes only names and spacing, and sameShape() throws away any rewrite that
// changes more. A wrong answer is refused, not saved, so the cheapest model is safe here.
const model = arg('--model', 'haiku');

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
  '  - the names of LOCAL VARIABLES, and of PRIVATE methods and their parameters',
  '',
  'THE PUBLIC SIGNATURE IS NOT YOURS. It is the stub NeetCode generated - the',
  'method name and its parameters are the grader\'s interface, and renaming one',
  'breaks the submission. In `public bool IsValidBST(TreeNode root)`, all three',
  'of IsValidBST, TreeNode and root stay exactly as they are, however you feel',
  'about them. That holds for the name ANYWHERE in the file: if a private helper',
  'also takes a parameter called `root`, that one stays `root` too. Renaming it',
  'in the helper and not in the public method is one name becoming two, which is',
  'rejected - and it is the single most common way these rewrites are refused.',
  '',
  'You may NOT change anything else. Not a comparison, not a literal, not the order of',
  'arguments, not a member name after a dot, not the class or method names, not the',
  'structure. The rewrite is checked mechanically and will be rejected if the token',
  'sequence differs by anything other than names and spacing, so a "small improvement"',
  'to the logic fails the whole file rather than shipping.',
  '',
  'FORMATTING IS NOT YOUR JOB. The code on stdin has already been formatted by a',
  'deterministic formatter (dotnet format), and it will be run over your answer again',
  'afterwards. Reproduce the whitespace exactly as you find it and spend nothing on it.',
  '',
  'In particular, LEAVE THE BRACES ALONE. The rewrite must contain exactly as many { and',
  '} as the original - count them. A body written without braces stays without braces:',
  '',
  '    if (n == 1)',
  '        return nums[0];',
  '',
  'is left exactly like that. Do NOT wrap it in { }. Adding that pair is two tokens, the',
  'mechanical check reads two extra tokens as an edit to the logic, and the whole file is',
  'refused. It is the single most common way these rewrites fail. Removing a pair that is',
  'already there fails the same way.',
  '',
  'Naming: descriptive enough to read without scrolling back, short enough to scan.',
  '  - Keep i, j, k when they are ordinary loop counters. They are idiomatic, not lazy.',
  '  - Rename anything whose meaning you had to work out: s, t, c, n, l, x, q, op, lc.',
  '  - Say what the value IS, not its type: charCount not intDict, windowSum not tempInt.',
  '  - Two or three words at most. leftBoundaryIndexOfWindow is worse than left.',
  '  - Match the vocabulary of the problem: prices, window, seen, remaining.',
  '  - Never use a C# contextual keyword as a name: value, var, get, set, when, where,',
  '    yield, async, await, nameof, record, dynamic, partial, from, select, with.',
  '  - Overloads are a trap. Two parameters with different names in different methods',
  '    are two different variables; giving them the same name is a rejection, even',
  '    when they clearly mean the same thing. Keep them distinct.',
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
                '--permission-mode', 'dontAsk', '--max-turns', '12', '--model', model, ...leanArgs()];
  let raw;
  try {
    raw = execFileSync('claude', args, {
      input: code, encoding: 'utf8', maxBuffer: 32 * 1024 * 1024, stdio: ['pipe','pipe','pipe'],
      // Renaming needs little thought. On CI Haiku spent ~90% of lint's output on thinking
      // (3.8k-6.4k tokens per small file) and lint became the slowest step. Haiku 4.5 has no
      // --effort, so thinking is capped by budget instead. LINT_THINKING_TOKENS overrides it.
      //
      // 1024 IS THE FLOOR, NOT A CHOICE. The documented minimum thinking budget is 1,024
      // tokens and the API rejects anything smaller, so a lower number is not a tighter
      // cap - it is no cap at all, and the model falls back to its own default.
      //
      // That is not theory. This was briefly set to 600 on the reasoning that renaming
      // needs little thought, and the very next run proved it backwards: the two calls
      // on house-robber thought 1,375 and 4,401 tokens, against 1,582 / 1,281 / 1,988 /
      // 1,481 under the old 2000. One call went from $0.0138 to $0.0289 and lint's
      // per-call wall time roughly doubled. Setting it below the floor made thinking go
      // UP, because nothing was capping it any more.
      //
      // So the value must stay >= 1024 to mean anything. Raise it through
      // LINT_THINKING_TOKENS if rejections climb; the number to watch is the
      // "attempt 1 REJECTED" count in the step log.
      env: { ...process.env, MAX_THINKING_TOKENS: String(Math.max(1024, Number(process.env.LINT_THINKING_TOKENS) || 1024)) },
    });
  } catch (e) {
    let env = null; try { env = JSON.parse(String(e.stdout ?? '')); } catch { /* not JSON */ }
    throw new Error(`claude failed (exit ${e.status}): ${env ? [env.terminal_reason, env.subtype, env.num_turns != null ? env.num_turns + ' turns' : null].filter(Boolean).join(' · ') : String(e.stderr || e.message).slice(0, 200)}`);
  }
  const env = JSON.parse(raw);
  const out = env.structured_output?.code;
  if (!out) throw new Error(`no code returned (result: ${String(env.result).slice(0, 200)})`);
  return { code: out.replace(/^\s*```(?:csharp|cs)?\s*/i, '').replace(/```\s*$/, ''), cost: env.total_cost_usd, turns: env.num_turns, usage: usageOf(env) };
}

// ---------------------------------------------------------------- run

const state = loadState();
const everything = scanRepo(state);

const needsLint = (p) => p.curatedFiles.concat(p.pending.map((s) => s.file)).some((f) => {
  const rec = state.problems[p.slug]?.lint?.[f];
  // Deliberately NOT reading the file to print it: this runs over every folder in
  // the repo to build a queue. A record with a print is judged on its rounds, and
  // a file edited since is caught by the per-file decision in the loop below.
  return lintWanted(rec, null);
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

console.log(`\n${doApply ? 'APPLY' : 'DRY RUN'} - lint, model ${model}, ${targets.length} folder(s)`);
console.log(dotnetAvailable()
  ? '  spacing: dotnet format whitespace (deterministic) · names: the model\n'
  : '  spacing: NOT APPLIED - no dotnet on PATH · names: the model\n');

if (haltIfStopped('lint', targets.map((p) => p.slug))) process.exit(1);

let failures = 0, changed = 0, clean = 0, givenUp = 0, reindented = 0;
// The account, not the file. Recording a lint failure here would be a lie that
// costs money later: a file marked `failed: true` is never retried without
// --force, so one session limit would permanently retire a perfectly good file.
let outOfBudget = null;
const touchedSlugs = new Set();

for (const p of targets) {
  group(p.path);
  // Superseded submissions are deleted by classify, which runs next. Linting
  // one costs a model call on a file that will not exist in a minute, and
  // leaves a lint record keyed to a filename nothing will ever re-key.
  const files = [...p.curatedFiles, ...p.pending.filter((s) => !s.supersededBy).map((s) => s.file)];
  for (const s of p.pending.filter((x) => x.supersededBy)) {
    console.log(`  ${s.file.padEnd(22)} skipped - superseded by ${s.supersededBy}`);
  }

  // Read the folder, then format the whole of it in ONE dotnet call - it pays a few
  // seconds of SDK start-up per invocation, which is worth batching away.
  //
  // This happens BEFORE the model sees anything, for two reasons. The spacing is then
  // already right whether or not a model call follows, so a file lint is not renaming
  // still gets tidied - free, and impossible to get wrong. And the model is handed
  // code that needs no formatting, so the prompt can tell it to leave whitespace alone
  // instead of asking it for an opinion about braces.
  const prepared = files.map((file) => {
    const full = join(p.dir, file);
    const raw = readFileSync(full, 'utf8');
    const { code: withHeader, eol } = splitTrailingTeach(raw);
    const teachBlock = raw.slice(withHeader.length);
    const { body, had } = stripHeader(withHeader);
    const header = had ? withHeader.slice(0, withHeader.length - body.length) : '';
    return { file, full, eol, teachBlock, header, body };
  });

  const formattedIn = formatMany(prepared.map((x) => x.body));
  if (formattedIn) {
    for (let i = 0; i < prepared.length; i++) {
      const x = prepared[i];
      const tidy = toEol(formattedIn[i], x.eol).replace(/\s+$/, '');
      if (tidy.trim() === x.body.trim()) continue;
      // The formatter is not trusted either. Whitespace is all it is allowed to
      // change, and sameShape() is exactly the thing that knows the difference.
      const check = sameShape(x.body, tidy);
      if (!check.ok) {
        console.log(`  ${x.file.padEnd(22)} formatter CHANGED CODE, not just spacing - ignored: ${check.errors[0]}`);
        console.log(`::warning::dotnet format altered tokens in ${p.slug}/${x.file}; the original was kept`);
        report('lint', p.slug, 'refused', `${x.file}: dotnet format changed more than whitespace - ${check.errors[0]}`);
        continue;
      }
      x.body = tidy;
      x.reindented = true;
    }
  } else if (!dotnetAvailable()) {
    console.log('  (no dotnet on PATH - spacing left as submitted)');
  } else {
    console.log('  (dotnet format failed - spacing left as submitted)');
  }

  for (const { file, full, eol, teachBlock, header, body, reindented: wasReindented } of prepared) {
    const rec = state.problems[p.slug]?.lint?.[file];
    const print = shortPrint(body);
    const writeBody = (code) =>
      writeFileSync(full, header + code.replace(/^(\r?\n)+/, '').replace(/\s+$/, '') + eol + teachBlock, 'utf8');
    // A file lint will not rename still gets its spacing, because the formatter
    // already did it and it costs nothing. Safe to do to a curated file: every
    // print in state.json (codePrint, headerSignature, teachSignature, the
    // visualizer's) is computed with whitespace stripped, so reindenting moves
    // none of them and nothing downstream is invalidated.
    const saveReindent = (label) => {
      if (!wasReindented) return;
      if (doApply) { writeBody(body); touchedSlugs.add(p.slug); }
      reindented++;
      console.log(`  ${file.padEnd(22)} ${label} - spacing reformatted`);
    };
    // A recorded FAILURE is not a recorded success, and the two used to be told
    // apart by nothing at all: this branch checked `version` and printed
    // "already linted" either way. house-robber's submission-0 was refused twice
    // in one run, and the very next run announced "2 already linted, 0 failed"
    // and went green. The file went on through classify, teach and visualize
    // carrying the spacing lint was meant to fix, and no report anywhere ever
    // mentioned it again. One red run, then invisible forever.
    //
    // Now a refusal is retried on the next run (LINT_MAX_ROUNDS of them, and any
    // time the code itself changes), which is what every other step in this
    // pipeline already does. When the rounds run out the file is reported as
    // `refused` - the status summarise.mjs renders as "needs a decision" - and
    // keeps being reported, run after run, until somebody forces a retry.
    const decision = lintDecision(rec, print, force);
    if (decision.action === 'skip-done') {
      if (wasReindented) saveReindent('already linted');
      else console.log(`  ${file.padEnd(22)} already linted`);
      report('lint', p.slug, 'skipped', `${file}: already linted${wasReindented ? ', spacing reformatted' : ''}`);
      clean++; continue;
    }
    if (decision.action === 'skip-refused') {
      console.log(`  ${file.padEnd(22)} NOT LINTED - refused on ${decision.rounds} run(s), not retried again: ${rec.reason ?? 'no reason recorded'}`);
      saveReindent('names left as submitted');
      report('lint', p.slug, 'refused', `${file}: still unlinted after ${decision.rounds} run(s) - ${rec.reason ?? 'no reason recorded'}. Retry: run the workflow with Process-Specific-folder=${p.slug} and Back-Fill on`);
      givenUp++; continue;
    }
    if (decision.rounds) console.log(`  ${file.padEnd(22)} retry ${decision.rounds + 1} of ${LINT_MAX_ROUNDS} - refused on an earlier run`);

    let result = null, feedback = null, spend = 0, rejected = false;
    for (let attempt = 1; attempt <= 2 && !result; attempt++) {
      let r;
      try { r = ask(body, feedback); }
      catch (e) {
        console.log(`  ${file.padEnd(22)} attempt ${attempt} FAILED: ${e.message}`);
        if (isOutOfBudget(e)) { outOfBudget = e.message; budgetStop('lint', e.message); }
        break;
      }
      spend += r.cost ?? 0;
      reportCost('lint', p.slug, r.cost, r.usage);
      console.log(`  ${file.padEnd(22)} attempt ${attempt}: $${r.cost} · ${r.turns} turns · ${usageLine(r.usage)}`);
    const check = sameShape(body, r.code);
      if (check.ok) result = { ...r, renames: check.renames };
      else {
        rejected = true;
        console.log(`  ${file.padEnd(22)} attempt ${attempt} REJECTED - the rewrite changed more than names:`);
        check.errors.slice(0, 3).forEach((e) => console.log(`      ${e}`));
        feedback = [
        ...check.errors,
        'Every distinct variable must keep a distinct name. If two variables would end up with the same name, pick different names for both rather than merging them.',
        'Inside one method, one variable keeps one new name everywhere it appears. The same name in two different methods is two different variables - give each the name that fits it there. Fields, properties and method names are shared, so they keep one name across the whole file.',
      ];
      }
    }

    // Before the failure bookkeeping: an out-of-budget stop must not be written
    // into state.json as a file that lint has given up on.
    // The CLI, not the file. Same reasoning as the budget stop below: if no attempt
    // ever came back with a rewrite to judge, nothing has been learned about this
    // file, and burning one of its retries for a crashed invocation would retire a
    // perfectly good file over a bad runner.
    if (!result && !rejected && !outOfBudget) {
      console.log(`  ${file.padEnd(22)} left untouched - the model call failed, nothing to judge`);
      saveReindent('names left as submitted');
      report('lint', p.slug, 'failed', `${file}: the model call failed before returning a rewrite - nothing recorded against the file`);
      failures++;
      continue;
    }

    if (!result && outOfBudget) {
      console.log(`  ${file.padEnd(22)} left untouched - out of budget, not the file`);
      saveReindent('names left as submitted');
      report('lint', p.slug, 'skipped', `${file}: stopped - out of budget, nothing wrong with this file`);
      break;
    }

    if (!result) {
      const round = decision.rounds + 1;
      const last = round >= LINT_MAX_ROUNDS;
      const why = (feedback && feedback[0]) || 'rewrite rejected twice';
      console.log(`  ${file.padEnd(22)} left untouched - round ${round} of ${LINT_MAX_ROUNDS}${last ? ', no more retries' : ', will be retried next run'}`);
      saveReindent('names left as submitted');
      // Record the round, and the print of the code it was refused on. The next
      // run reads both: another try while rounds remain, and a clean slate if
      // the file has been edited since. --force retries regardless.
      if (doApply) {
        const prec = state.problems[p.slug] ?? (state.problems[p.slug] = {});
        (prec.lint ?? (prec.lint = {}))[file] = {
          version: LINT_FORMAT, failed: true, attempts: round, print, reason: why,
        };
      }
      // While a retry is still coming, this is a plain failure: the run goes red,
      // classify and the rest are skipped, and the folder stays pending so the
      // next run picks the whole thing up. Once the rounds are spent it becomes a
      // refusal instead - a decision, not a surprise - so one file that will never
      // pass cannot hold every other problem in the repo behind it forever.
      if (last) {
        report('lint', p.slug, 'refused', `${file}: refused on ${LINT_MAX_ROUNDS} runs, giving up - ${why}. Retry: run the workflow with Process-Specific-folder=${p.slug} and Back-Fill on`);
        givenUp++;
      } else {
        report('lint', p.slug, 'failed', `${file}: rewrite rejected twice - ${why}. Will be retried on the next run (${round} of ${LINT_MAX_ROUNDS}).`);
        failures++;
      }
      continue;
    }

    // Format the model's answer too. It was told to reproduce the whitespace it was
    // given and mostly does, but sameShape() cannot check that - whitespace is the
    // one thing it ignores - so a sloppy reply would otherwise undo the formatting
    // this run just did. If the pass fails or (impossibly) moves a token, the
    // model's own text stands: it already passed sameShape above.
    let finalCode = result.code;
    const formattedOut = formatMany([result.code]);
    if (formattedOut) {
      const tidy = toEol(formattedOut[0], eol).replace(/\s+$/, '');
      if (sameShape(body, tidy).ok) finalCode = tidy;
      else console.log(`  ${file.padEnd(22)} formatter altered the rewrite - keeping the model's text`);
    }

    // Say whether the formatter did anything, even when a rename happened too. The first
    // run of this reported "0 reformatted only" while having reindented both files from
    // K&R to Allman - true, because that counter means "reformatted AND NOTHING ELSE",
    // and completely misleading if you are trying to find out whether dotnet ran.
    const same = finalCode.trim() === body.trim();
    const what = same ? 'nothing to change'
      : result.renames.length ? 'renames: ' + result.renames.map(([a, b]) => `${a}->${b}`).join(', ')
      : 'spacing only';
    console.log(`  ${file.padEnd(22)} ${what}${wasReindented ? '  (+ reformatted)' : ''}  ·  $${(spend || 0).toFixed(4)}`);
    if (!same && !doApply) {
      console.log(finalCode.split('\n').slice(0, 12).map((l) => '      | ' + l).join('\n'));
    }

    if (doApply) {
      writeBody(finalCode);
      const prec = state.problems[p.slug] ?? (state.problems[p.slug] = {});
      (prec.lint ?? (prec.lint = {}))[file] = {
        version: LINT_FORMAT,
        codePrint: shortPrint(finalCode),
        renames: result.renames.map(([a, b]) => `${a}->${b}`),
      };
      report('lint', p.slug, 'ok', `${file}: ${result.renames.length ? result.renames.map(([a, b]) => `${a}->${b}`).join(', ') : same ? 'nothing to change' : 'spacing tidied'}`);
      if (!same) { changed++; touchedSlugs.add(p.slug); }
      else if (wasReindented) { reindented++; touchedSlugs.add(p.slug); }
    }
  }
  endGroup();
  if (outOfBudget) break;
}

if (doApply) saveState(state);
if (outOfBudget) announceBudget(outOfBudget);
console.log(`${changed} file(s) changed, ${reindented} reformatted only, ${clean} already linted, ${failures} failed (retry next run)${givenUp ? `, ${givenUp} given up on` : ''}${outOfBudget ? ', rest not attempted' : ''}.\n`);
if (givenUp) console.log(`::warning::${givenUp} file(s) have now been refused on ${LINT_MAX_ROUNDS} runs and are not retried again. They are listed in the run summary.`);
if (process.env.GITHUB_OUTPUT) {
  appendFileSync(process.env.GITHUB_OUTPUT, `changed=${changed}\n`);
  appendFileSync(process.env.GITHUB_OUTPUT, `slugs=${[...touchedSlugs].join(',')}\n`);
}
process.exit(failures || outOfBudget ? 1 : 0);
