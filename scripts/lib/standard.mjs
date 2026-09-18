/**
 * "Is this folder finished?" - asked once, here.
 *
 * There used to be three answers to this question and they disagreed. The index
 * called a folder current when its VISUALIZER was at the shape version, saying
 * nothing about whether the code had been formatted, classified or taught.
 * select-backfill had the real test, in a private function. The audit grew a third.
 * So a folder could read "current" on the index while sitting in the backfill queue
 * for three separate reasons - which is exactly the confusion this replaces.
 *
 * One function now. The index renders its verdict, the backfill queues on it, and
 * they cannot drift because there is nothing to drift from.
 *
 * A folder is AT STANDARD only when every stage is:
 *
 *   lint        every file formatted under the current .editorconfig rules
 *   classify    every file has a classification and a header at HEADER_FORMAT
 *   teach       every file has a teaching block written for its current header
 *   visualizer  the file exists and was built under the current shape contract
 *
 * Anything less is "needs rebuilt", and a backfill pass fixes it. Deliberately all
 * or nothing: a half-finished folder is not a shade of green, it is work to do.
 */

import { existsSync, readFileSync } from 'node:fs';
import { join } from 'node:path';
import { HEADER_FORMAT } from './header.mjs';
import { splitTrailingTeach } from './teach.mjs';
import { lintWanted } from './lint-rules.mjs';
import { VISUALIZER_FORMAT } from './shapes.mjs';

/**
 * @param   p      a folder from scanRepo()
 * @param   state  the whole state object
 * @returns {string[]} the stages still owed work - empty means at standard
 */
export function needs(p, state) {
  const rec = state.problems?.[p.slug] ?? {};
  const lint = rec.lint ?? {}, sigs = rec.headerSignatures ?? {}, teach = rec.teachSignatures ?? {}, cls = rec.classification ?? {};
  const all = [...p.curatedFiles, ...p.pending.map((s) => s.file)];
  const why = [];

  if (all.some((f) => lintWanted(lint[f]))) why.push('lint');

  // Raw submissions still waiting, or a file with no classification or a header
  // written under older rules: classify owns all three.
  const classified = p.curatedFiles.length && !p.pending.length && p.curatedFiles.every((f) => {
    if (!cls[f] || !sigs[f]) return false;
    if (!Array.isArray(cls[f].structures)) return false;      // classified before shapes existed
    try { return JSON.parse(sigs[f]).v === HEADER_FORMAT; } catch { return false; }
  });
  if (!classified) why.push('classify');

  // The teaching block is written FOR a header. If the header moved - a rename, a
  // re-classification, an edit to the code - the block describes something else.
  const taught = p.curatedFiles.length && p.curatedFiles.every((f) =>
    teach[f] && sigs[f] && teach[f] === sigs[f] &&
    existsSync(join(p.dir, f)) && splitTrailingTeach(readFileSync(join(p.dir, f), 'utf8')).had);
  if (!taught) why.push('teach');

  // "Has a visualizer" was the whole test once, and it is why every array-shaped
  // visualizer in the repo counted as finished work. A visualizer built before the
  // structural panels draws a BST as rows of chips: it exists, and it is exactly
  // what the backfill is for. A hand-built one carries no record and is queued too.
  if (!existsSync(join(p.dir, `${p.slug}-visualizer.html`))) why.push('visualizer');
  else if ((rec.visualizer?.v ?? 1) !== VISUALIZER_FORMAT) why.push('visualizer');

  return why;
}

/** The single field the index shows. */
export const atStandard = (p, state) => needs(p, state).length === 0;

export const STANDARD_LABEL = { current: 'Current', rebuild: 'Needs rebuilt' };
