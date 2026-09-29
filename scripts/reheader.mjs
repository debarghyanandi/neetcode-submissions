#!/usr/bin/env node
/**
 * reheader.mjs - push a header format change into the files that already have one.
 *
 * Same idea as reskin.mjs for visualizers. Headers are written by classify, and
 * classify only rewrites one when the folder is re-classified - a model call per
 * folder to reproduce facts that are already on record. Every fact the banner
 * prints (name, time, space, correct, the # / - provenance mark) is already in
 * state.json's headerSignatures, so the new banner is rebuilt from there.
 *
 * Deterministic, no model, no cost. It also stamps each signature with the new
 * HEADER_FORMAT, which is what marks the teaching block stale: teach compares its
 * record against the header signature, so after this runs, teach --backfill knows
 * every block needs rewriting in the new shape.
 *
 *   node scripts/reheader.mjs                    # dry run: count, and show one
 *   node scripts/reheader.mjs --apply
 *   node scripts/reheader.mjs --apply --slug trapping-rain-water
 */

import { readFileSync, writeFileSync } from 'node:fs';
import { join } from 'node:path';
import { loadState, saveState, scanRepo } from './lib/scan.mjs';
import { buildHeader, applyHeader, stripHeader, HEADER_FORMAT } from './lib/header.mjs';

const argv = process.argv.slice(2);
const arg = (n, d = null) => (argv.includes(n) ? argv[argv.indexOf(n) + 1] : d);
const doApply = argv.includes('--apply');
const only = arg('--slug')?.split(',').map((s) => s.trim()).filter(Boolean) ?? null;

const state = loadState();
let wrote = 0, current = 0, skipped = 0, shown = false;

for (const p of scanRepo(state)) {
  if (only && !only.includes(p.slug)) continue;
  const rec = state.problems[p.slug];
  const sigs = rec?.headerSignatures;
  if (!sigs) continue;
  for (const file of p.curatedFiles) {
    let sig;
    try { sig = JSON.parse(sigs[file]); } catch { skipped++; console.log(`  ${p.slug}/${file}: no header signature - left for classify`); continue; }
    if (sig.v === HEADER_FORMAT) { current++; continue; }

    const full = join(p.dir, file);
    const src = readFileSync(full, 'utf8');
    if (!stripHeader(src).had) { skipped++; console.log(`  ${p.slug}/${file}: no header in the file - left for classify`); continue; }

    const header = buildHeader(file, null, { time: sig.time, space: sig.space, correct: sig.correct }, sig.selfMark, []);
    const next = applyHeader(src, header);
    if (!shown) { console.log(`\n${p.slug}/${file} - new top of file:\n`); console.log(next.split(/\r?\n/).slice(0, 6).map((l) => '    ' + l).join('\n')); console.log(''); shown = true; }
    if (doApply) {
      writeFileSync(full, next, 'utf8');
      sigs[file] = JSON.stringify({ ...sig, v: HEADER_FORMAT });
    }
    wrote++;
  }
}

if (doApply && wrote) saveState(state);
console.log(`${doApply ? 'APPLY' : 'DRY RUN'} - header format ${HEADER_FORMAT}: ${wrote} ${doApply ? 'rewritten' : 'would be rewritten'}, ${current} already current, ${skipped} skipped.`);
