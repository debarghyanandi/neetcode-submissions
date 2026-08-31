#!/usr/bin/env node
/**
 * migrate-provenance.mjs - record who wrote what, once, while the evidence exists.
 *
 * The //My solution marker lives in the raw NeetCode submission. Curating that
 * submission into optimal.cs rewrites the code, and the marker usually does not
 * survive - buy-and-sell-crypto is the proof: its banner records the marker
 * from submission-2, but no marker remains in the curated file.
 *
 * So absence of a marker in a curated file means the evidence was edited away,
 * NOT that the solution is not yours. Re-deriving provenance from a curated
 * file therefore gets it wrong, and gets it wrong in the direction that quietly
 * disowns your work.
 *
 * This reads the repo as it stood BEFORE the pipeline touched anything and
 * records, per file, what was true then. From here on the recorded value is
 * authoritative and is never re-derived.
 *
 *   node scripts/migrate-provenance.mjs            # report
 *   node scripts/migrate-provenance.mjs --write
 */

import { execFileSync } from 'node:child_process';
import { loadState, saveState, scanRepo, REPO } from './lib/scan.mjs';
import { stripHeader } from './lib/header.mjs';
import { splitTrailingTeach } from './lib/teach.mjs';
import { isSelfMarked } from './lib/complexity.mjs';

const argv = process.argv.slice(2);
const REF = argv.includes('--ref') ? argv[argv.indexOf('--ref') + 1] : 'c76939d';
const write = argv.includes('--write');

// Your own words, in the banner styles this repo used before the pipeline.
const CLAIMS_YOURS = /YOU SOLVED THIS YOURSELF|YOUR OWN SOLUTION/i;
const CLAIMS_REFERENCE = /Reference solution|Not one you solved yourself/i;

const show = (path) => {
  try {
    return execFileSync('git', ['show', `${REF}:${path}`], { cwd: REPO, encoding: 'utf8', stdio: ['pipe','pipe','pipe'] });
  } catch { return null; }
};

const state = loadState();
let yours = 0, reference = 0, unknown = 0, missing = 0;

for (const p of scanRepo(state)) {
  const rows = [];
  for (const file of p.curatedFiles) {
    const src = show(`${p.path}/${file}`);
    if (src === null) { rows.push([file, 'not present at ref - left unrecorded']); missing++; continue; }

    // Annotation blocks are excluded before looking for a marker, so a banner
    // quoting "marked '//My solution'" cannot manufacture one.
    const code = stripHeader(splitTrailingTeach(src).code).body;
    const annotations = src.replace(code, '');

    let selfMarked = null, evidence = null;
    if (isSelfMarked(code)) { selfMarked = true; evidence = 'marker present in the code itself'; }
    else if (CLAIMS_YOURS.test(annotations)) { selfMarked = true; evidence = `your own annotation at ${REF}`; }
    else if (CLAIMS_REFERENCE.test(annotations)) { selfMarked = false; evidence = `your own annotation at ${REF}`; }
    else { evidence = 'no marker and no annotation - genuinely unknown'; }

    if (selfMarked === true) yours++; else if (selfMarked === false) reference++; else unknown++;
    rows.push([file, `${selfMarked === null ? 'UNKNOWN' : selfMarked ? 'yours' : 'reference'}  -  ${evidence}`]);

    if (write) {
      const rec = state.problems[p.slug] ?? (state.problems[p.slug] = {});
      (rec.provenance ?? (rec.provenance = {}))[file] = { selfMarked, evidence, recordedFrom: REF };
    }
  }
  if (rows.length) {
    console.log(p.path);
    for (const [f, r] of rows) console.log(`  ${f.padEnd(22)} ${r}`);
  }
}

console.log(`\nyours ${yours} · reference ${reference} · unknown ${unknown} · absent at ref ${missing}`);
if (write) { saveState(state); console.log('written to .agent/state.json\n'); }
else console.log('report only - pass --write to record\n');
