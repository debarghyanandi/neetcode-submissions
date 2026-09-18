#!/usr/bin/env node
/**
 * summarise.mjs - render the run summary a person can actually read.
 *
 * Answers, at a glance: what set this run off, which folder was held back and
 * why, which folders were touched, and what each step did to each of them.
 * The old summary said "classify: success" and left you to open four step logs
 * in order to find out that nothing had happened to anything.
 */

import { readFileSync, existsSync, appendFileSync } from 'node:fs';
import { stopped } from './lib/budget.mjs';
import { loadState } from './lib/scan.mjs';
import { lintDecision, LINT_MAX_ROUNDS } from './lib/lint-rules.mjs';

const out = [];
const w = (l = '') => out.push(l);

const env = (k) => process.env[k] || '';
const reportFile = env('PIPELINE_REPORT');
const detectFile = env('DETECT_JSON');

const trigger = env('GITHUB_EVENT_NAME');
const actor = env('GITHUB_ACTOR');
const batch = env('BACKFILL_BATCH');
const named = env('NAMED_BATCH');
const redo = env('REDO');
const remaining = env('BACKFILL_REMAINING');
const headMsg = env('HEAD_COMMIT_MESSAGE').split('\n')[0];

// ---------------------------------------------------------------- what set it off

w('## NeetCode pipeline');
w();

const why = trigger === 'push' ? `**a push** by \`${actor}\``
  : trigger === 'schedule' ? '**the daily schedule**'
  : `**a manual run** by \`${actor}\``;
w(`Started by ${why}.`);
if (trigger === 'push' && headMsg) w(`Triggering commit: \`${headMsg}\``);
w();

// ---------------------------------------------------------------- did it run out
//
// Model steps used to carry continue-on-error, so a run that did almost nothing
// because the subscription had hit its limit came out GREEN. They now fail fast,
// but this banner still explains WHY a run stopped when the cause is budget. It
// happened on a 2am schedule: lint ran, committed its renames, classify was
// refused, teach and visualize found nothing to do, and the badge said success.
//
// This step runs with `if: always()` and WITHOUT continue-on-error, so it is
// the one place in the workflow that can say "no, that was not a success"
// without touching a protected workflow file.
const budget = stopped();

// First thing under the heading, before the detail. The one fact that changes
// what you do next: nothing below is a verdict on the code.
if (budget) {
  w('> [!CAUTION]');
  w(`> ### 🛑 Stopped early — the account ran out of budget during \`${budget.step}\``);
  w('>');
  w(`> \`${budget.message.replace(/`/g, "'")}\``);
  w('>');
  w('> Everything finished before that point was kept and committed. Folders marked ⏭️ below');
  w('> were never asked, and nothing is wrong with them — name them again after the reset.');
  w('>');
  w('> **The run is marked failed so it does not read as a success.**');
  w();
}

// ---------------------------------------------------------------- did a step fail
//
// The model steps fail fast: the first one to fail stops the ones after it. Say which, in
// one line at the top, so the red badge comes with its reason. Empty outcomes mean a local
// run or a step that was never meant to run - nothing to report.
const STEP_ORDER = ['lint', 'classify', 'teach', 'visualize'];
const outcome = Object.fromEntries(STEP_ORDER.map((s) => [s, env(`OUTCOME_${s.toUpperCase()}`)]));
const failedStep = STEP_ORDER.find((s) => outcome[s] === 'failure');
if (failedStep && !budget) {
  const never = STEP_ORDER.slice(STEP_ORDER.indexOf(failedStep) + 1).filter((s) => outcome[s] === 'skipped');
  w('> [!CAUTION]');
  w(`> ### ❌ Stopped: \`${failedStep}\` failed`);
  w('>');
  w(never.length
    ? `> Not run because of it: ${never.map((x) => `\`${x}\``).join(', ')}. The reason is in the table below, and in full in the \`${failedStep}\` step log.`
    : `> The reason is in the table below, and in full in the \`${failedStep}\` step log.`);
  w('>');
  w(env('OUTCOME_COMMIT') === 'success'
    ? '> Work that finished before the failure was committed, so nothing paid for is lost.'
    : '> Nothing was committed.');
  w();
}

// ---------------------------------------------------------------- what is still broken
//
// A standing list, read from state.json rather than from this run's report, and shown on
// EVERY run whether or not the folder was touched.
//
// The folder table below only has rows for folders this run processed. A file lint gave up
// on is, by construction, one lint will not look at again - so after the run that broke it,
// its folder never appears in another table and the failure is invisible. That is exactly
// what happened to house-robber: refused twice at 06:32, and the 06:40 run reported the
// same file as "already linted" and went green.
//
// Nothing else in the pipeline is allowed to go quiet like that, so this section is the
// place a known-bad file has to keep showing up until somebody retries it.
const stuck = [];
try {
  const st = loadState();
  for (const [slug, rec] of Object.entries(st.problems ?? {}))
    for (const [file, l] of Object.entries(rec.lint ?? {}))
      if (l && l.failed) stuck.push({
        slug, file,
        reason: l.reason ?? 'no reason recorded',
        rounds: l.attempts ?? 1,
        retrying: lintDecision(l, null).action === 'lint',
      });
} catch { /* a summary must never be the thing that fails a run */ }

if (stuck.length) {
  const over = stuck.filter((x) => !x.retrying);
  w('> [!WARNING]');
  w(`> ### ⚠️ ${stuck.length} file(s) lint could not rewrite`);
  w('>');
  w('> They carry whatever spacing and variable names you submitted, and everything');
  w('> downstream - header, teaching block, visualizer - describes them as they are.');
  w('>');
  for (const x of stuck) {
    const tag = x.retrying
      ? `refused on ${x.rounds} of ${LINT_MAX_ROUNDS} run(s) — **another retry is due**`
      : `refused on ${x.rounds} run(s) — **given up on, not retried again**`;
    w(`> - \`${x.slug}/${x.file}\` — ${tag}`);
    w(`>   <br><sub>${x.reason.replace(/`/g, "'")}</sub>`);
  }
  if (over.length) {
    w('>');
    w('> **To force one of the given-up files:** run the workflow with `Process-Specific-folder`');
    w('> set to the slug and `Back-Fill` on. That is the only path that passes `--force`.');
  }
  w();
}

// ---------------------------------------------------------------- what it cost
//
// One "$cost" line per model call, written by reportCost(). Parsed here, before the folder
// rows, so the total sits near the top where it is seen.
const costs = [];
if (reportFile && existsSync(reportFile)) {
  for (const line of readFileSync(reportFile, 'utf8').split('\n')) {
    if (!line.startsWith('$cost\t')) continue;
    const [, step, slug, usd, tin, cw, cr, tout] = line.split('\t');
    costs.push({ step, slug, usd: Number(usd) || 0, tokens: [tin, cw, cr, tout].map((x) => Number(x) || 0) });
  }
}
const money = (x) => `$${x.toFixed(2)}`;
const sum = (xs) => xs.reduce((a, c) => a + c.usd, 0);
const costBySlug = new Map();
for (const c of costs) costBySlug.set(c.slug, (costBySlug.get(c.slug) ?? 0) + c.usd);
if (costs.length) {
  const perStep = STEP_ORDER
    .map((st) => [st, costs.filter((c) => c.step === st)])
    .filter(([, xs]) => xs.length)
    .map(([st, xs]) => `${st} ${money(sum(xs))}`);
  const t = costs.reduce((a, c) => a.map((v, i) => v + c.tokens[i]), [0, 0, 0, 0]);
  w(`**Estimated cost: ${money(sum(costs))}** — ${perStep.join(' · ')} · ${costs.length} model call(s)`);
  w();
  w(`<sub>Tokens: in ${t[0].toLocaleString('en-US')} · cache write ${t[1].toLocaleString('en-US')} · cache read ${t[2].toLocaleString('en-US')} · out ${t[3].toLocaleString('en-US')}. ` +
    'API-price estimate from the CLI; on the subscription this is usage, not a bill. Calls that crashed before answering are not counted.</sub>');
  w();
}

let detect = null;
if (detectFile && existsSync(detectFile)) {
  try { detect = JSON.parse(readFileSync(detectFile, 'utf8')); } catch { /* leave null */ }
}

if (detect) {
  const held = detect.excluded ?? [];
  if (held.length) {
    w(`**Held back this run:** ${held.map((h) => `\`${h.path.split('/').pop()}\``).join(', ')}`);
    w();
    w('> A push leaves the folder it touched alone, in case more submissions are coming.');
    w('> The daily run picks it up.');
    w();
  }
  // Captured by detect, before anything ran - so this is what the run set out
  // to do, not what is still outstanding. Saying "waiting" made finished work
  // look stuck.
  const sel = detect.selected ?? [];
  w(sel.length
    ? `**Raw submissions picked up:** ${sel.map((s) => `\`${s.slug}\``).join(', ')}`
    : '**Raw submissions picked up:** none');
  w();
}

if (named) {
  w(`**You asked for these folders only:** ${named.split(',').map((s) => `\`${s.trim()}\``).join(', ')}`);
  w();
  w(redo
    ? '> Redone from scratch. Nothing else was looked at - not the backlog, not new submissions.'
    : '> Brought up to the current standard. Nothing else was looked at - not the backlog, not new submissions.');
  w();
} else if (batch) {
  w(`**Backfill batch:** ${batch.split(',').map((s) => `\`${s}\``).join(', ')}`);
  if (remaining) w(`${remaining} folder(s) needed work when this run began.`);
  w();
}

if (named && redo) {
  w('Work already at the current standard was redone, files lint had given up on were retried, and the visualizer was rebuilt from scratch.');
  w();
}

// ---------------------------------------------------------------- what happened

const ICON = { ok: '✅', skipped: '⏭️', refused: '⚠️', failed: '❌' };
const STEPS = ['lint', 'classify', 'teach', 'visualize'];

const rows = new Map();
if (reportFile && existsSync(reportFile)) {
  for (const line of readFileSync(reportFile, 'utf8').split('\n')) {
    if (!line.trim() || line.startsWith('$cost\t')) continue;
    const [step, slug, status, detail = ''] = line.split('\t');
    if (!rows.has(slug)) rows.set(slug, {});
    // A folder can produce several lines per step (one per file). Keep the
    // worst outcome, and collect the details.
    const cell = rows.get(slug)[step] ?? { status: 'ok', details: [] };
    const rank = { ok: 0, skipped: 1, refused: 2, failed: 3 };
    if (rank[status] > rank[cell.status]) cell.status = status;
    if (detail) cell.details.push(detail);
    rows.get(slug)[step] = cell;
  }
}

if (rows.size) {
  w('### What happened to each folder');
  w();
  // The Notes column carries ONLY what needs a decision. Cramming every step's
  // detail into one cell truncated it mid-word and buried the one line that
  // mattered among five that did not; the rest goes in a foldout below.
  w('| Folder | ' + STEPS.map((s) => s[0].toUpperCase() + s.slice(1)).join(' | ') + ' | Cost | Needs attention |');
  w('|---|' + STEPS.map(() => '---').join('|') + '|---:|---|');
  for (const [slug, cells] of rows) {
    const cols = STEPS.map((s) => (cells[s] ? ICON[cells[s].status] ?? '·' : '·'));
    const problems = STEPS
      .filter((s) => cells[s] && (cells[s].status === 'refused' || cells[s].status === 'failed'))
      .flatMap((s) => cells[s].details.map((d) => `**${s}**: ${d}`))
      .join('<br>');
    w(`| \`${slug}\` | ${cols.join(' | ')} | ${costBySlug.has(slug) ? money(costBySlug.get(slug)) : '—'} | ${problems || '—'} |`);
  }
  w();
  w('✅ done · ⏭️ skipped — nothing needed, or not attempted · ⚠️ refused, needs a decision · ❌ failed · · not run');
  w();

  w('<details><summary>What each step did, folder by folder</summary>');
  w();
  for (const [slug, cells] of rows) {
    w(`**${slug}**`);
    w();
    for (const step of STEPS) {
      const c = cells[step];
      if (!c) continue;
      for (const d of (c.details.length ? c.details : ['(no detail recorded)'])) {
        w(`- ${ICON[c.status] ?? '·'} \`${step}\` — ${d}`);
      }
    }
    w();
  }
  w('</details>');
  w();
} else {
  w('_No folder was processed this run._');
  w();
}

const target = env('GITHUB_STEP_SUMMARY');
if (target) appendFileSync(target, out.join('\n') + '\n');
else console.log(out.join('\n'));

// The only non-zero exit here. A summary that cannot be written is not worth
// failing a run over; a run that quietly did nothing is.
if (budget) {
  console.log(`::error::the pipeline stopped during ${budget.step} - out of budget: ${budget.message}`);
  process.exit(1);
}
// The job is already red from the failed step; this puts the reason on the run page too.
if (failedStep) {
  console.log(`::error::${failedStep} failed - later model steps were not run. See the run summary.`);
  process.exit(1);
}
