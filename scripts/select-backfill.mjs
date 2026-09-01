#!/usr/bin/env node
/**
 * select-backfill.mjs - choose ONE batch of folders for a backfill run.
 *
 * Each step (lint, classify, teach, visualize) can work out for itself which
 * folders it still owes work to. If each also applies --limit independently
 * they drift apart: once lint has finished folder A but classify has not, a
 * limit of 1 makes lint edit folder B while classify processes folder A - and
 * folder B's code changes without its header, block or visualizer following.
 *
 * So the batch is chosen once, here, and every step is handed the same list.
 *
 *   node scripts/select-backfill.mjs --limit 1
 */

import { appendFileSync, existsSync, readFileSync } from 'node:fs';
import { join } from 'node:path';
import { loadState, scanRepo } from './lib/scan.mjs';
import { HEADER_FORMAT } from './lib/header.mjs';
import { splitTrailingTeach } from './lib/teach.mjs';
import { LINT_FORMAT } from './lib/lint-rules.mjs';

const argv = process.argv.slice(2);
const limit = Number(argv.includes('--limit') ? argv[argv.indexOf('--limit') + 1] : '5') || 5;

const state = loadState();
const problems = scanRepo(state).filter((p) => p.curatedFiles.length || p.pending.length);

const reasons = (p) => {
  const rec = state.problems[p.slug] ?? {};
  const lint = rec.lint ?? {}, sigs = rec.headerSignatures ?? {}, teach = rec.teachSignatures ?? {}, cls = rec.classification ?? {};
  const all = [...p.curatedFiles, ...p.pending.map((s) => s.file)];
  const why = [];

  if (all.some((f) => (lint[f]?.version ?? -1) !== LINT_FORMAT)) why.push('lint');

  const classified = p.curatedFiles.length && !p.pending.length && p.curatedFiles.every((f) => {
    if (!cls[f] || !sigs[f]) return false;
    try { return JSON.parse(sigs[f]).v === HEADER_FORMAT; } catch { return false; }
  });
  if (!classified) why.push('classify');

  const taught = p.curatedFiles.length && p.curatedFiles.every((f) =>
    teach[f] && sigs[f] && teach[f] === sigs[f] && splitTrailingTeach(readFileSync(join(p.dir, f), 'utf8')).had);
  if (!taught) why.push('teach');

  if (!existsSync(join(p.dir, `${p.slug}-visualizer.html`))) why.push('visualizer');

  return why;
};

const pending = problems.map((p) => ({ p, why: reasons(p) })).filter((x) => x.why.length);
const batch = pending.slice(0, limit);

console.log(`\n${problems.length} folder(s) · ${problems.length - pending.length} already current · ${pending.length} remaining`);
console.log(`taking ${batch.length} this run:\n`);
for (const { p, why } of batch) console.log(`  ${p.slug.padEnd(46)} needs: ${why.join(', ')}`);
if (pending.length > batch.length) console.log(`\n  ...and ${pending.length - batch.length} more after this.`);
console.log('');

const slugs = batch.map((x) => x.p.slug).join(',');
if (process.env.GITHUB_OUTPUT) {
  appendFileSync(process.env.GITHUB_OUTPUT, `slugs=${slugs}\n`);
  appendFileSync(process.env.GITHUB_OUTPUT, `count=${batch.length}\n`);
  appendFileSync(process.env.GITHUB_OUTPUT, `remaining=${pending.length}\n`);
}
