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
    if (!line.trim()) continue;
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
  w('| Folder | ' + STEPS.map((s) => s[0].toUpperCase() + s.slice(1)).join(' | ') + ' | Needs attention |');
  w('|---|' + STEPS.map(() => '---').join('|') + '|---|');
  for (const [slug, cells] of rows) {
    const cols = STEPS.map((s) => (cells[s] ? ICON[cells[s].status] ?? '·' : '·'));
    const problems = STEPS
      .filter((s) => cells[s] && (cells[s].status === 'refused' || cells[s].status === 'failed'))
      .flatMap((s) => cells[s].details.map((d) => `**${s}**: ${d}`))
      .join('<br>');
    w(`| \`${slug}\` | ${cols.join(' | ')} | ${problems || '—'} |`);
  }
  w();
  w('✅ done · ⏭️ looked at, nothing needed · ⚠️ refused, needs a decision · ❌ failed · · not run');
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
