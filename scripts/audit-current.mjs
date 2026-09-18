#!/usr/bin/env node
/**
 * audit-current.mjs - prove that every folder marked "current" really is finished.
 *
 * The index has three standings: current, needs reshape, no visualizer. Only the
 * first is a promise, and it is a strong one - a folder marked current is one the
 * owner will never run through the pipeline again. So it has to be right NOW: the
 * variable names are the ones that were written, the teaching block describes those
 * names, and the visualizer's code panel is the solution file copied verbatim.
 *
 * Everything else can be demoted. A folder that needs reshape costs one backfill
 * pass and comes back correct; a folder wrongly left at "current" is a page that is
 * quietly wrong forever. So this checks, and anything it cannot prove is demoted:
 * the visualizer record is aged so the index shows "needs reshape", and the teaching
 * signatures are dropped so the block is rewritten too.
 *
 *   node scripts/audit-current.mjs                          # report only
 *   node scripts/audit-current.mjs --apply                  # demote what fails
 *   node scripts/audit-current.mjs --stale-from c815095     # also check old names
 *
 * --stale-from names a revision of state.json to read lint's historical renames
 * from, so the audit can look for a name the repair should have removed. Without
 * it the audit still runs; it just cannot check that particular thing.
 *
 * THE CODE CHECK IS THE PIPELINE'S OWN. validate() with sourceBodies is the same
 * comparison visualize.mjs makes before it writes a visualizer at all - the one
 * that exists because a model was caught inventing a comment and reflowing a
 * statement while every other check passed. Reusing it means the audit cannot
 * drift from what the pipeline considers a valid visualizer.
 */

import { readFileSync, existsSync, readdirSync } from 'node:fs';
import { execFileSync } from 'node:child_process';
import { join } from 'node:path';
import { loadState, saveState, scanRepo, REPO } from './lib/scan.mjs';
import { stripHeader } from './lib/header.mjs';
import { splitTrailingTeach } from './lib/teach.mjs';
import { validate, selectForVisualizer } from './lib/visualizer.mjs';
import { VISUALIZER_FORMAT } from './lib/shapes.mjs';

const argv = process.argv.slice(2);
const doApply = argv.includes('--apply');
const staleFrom = argv.includes('--stale-from') ? argv[argv.indexOf('--stale-from') + 1] : null;

/** Pull the PROBLEM definition back out of a built visualizer. Mirrors visualize.mjs. */
function definitionFromFile(path) {
  const src = readFileSync(path, 'utf8').split(/\r?\n/);
  const s = src.findIndex((l) => /^const PROBLEM = \{/.test(l));
  if (s < 0) return null;
  let depth = 0, end = -1;
  for (let i = s; i < src.length && end < 0; i++) {
    for (const ch of src[i]) { if (ch === '{') depth++; else if (ch === '}' && --depth === 0) { end = i; break; } }
  }
  return end < 0 ? null : src.slice(s, end + 1).join('\n');
}

// Names lint renamed TO. If one of these is still anywhere in a folder, the repair
// did not reach it and the folder is not finished, whatever else passes.
const staleNames = new Map();
if (staleFrom) {
  try {
    const old = JSON.parse(execFileSync('git', ['--no-optional-locks', 'show', `${staleFrom}:.agent/state.json`], { encoding: 'utf8', maxBuffer: 64 * 1024 * 1024 }));
    for (const [slug, r] of Object.entries(old.problems ?? {})) {
      const set = new Set();
      for (const l of Object.values(r.lint ?? {}))
        for (const rn of (l.renames ?? [])) {
          const to = String(rn).split('->')[1];
          if (to && to !== 'rows' && to !== 'cols') set.add(to);   // rows/cols were kept on purpose
        }
      if (set.size) staleNames.set(slug, set);
    }
    console.log(`\nreading lint's old rename targets from ${staleFrom} (${staleNames.size} folder(s))`);
  } catch (e) { console.log(`\ncannot read state.json at ${staleFrom}: ${e.message}`); process.exit(1); }
}

const state = loadState();
const problems = scanRepo(state).filter((p) => p.curatedFiles.length);

const demote = [];
let current = 0, ok = 0;

console.log(`\n${doApply ? 'APPLY' : 'REPORT'} - auditing every folder the index calls "current"\n`);

for (const p of problems) {
  const rec = state.problems[p.slug] ?? {};
  if (!p.hasVisualizer) continue;                                   // already "no visualizer"
  if ((rec.visualizer?.v ?? 1) !== VISUALIZER_FORMAT) continue;     // already "needs reshape"
  current++;

  const why = [];

  // 1. Does anything still carry a name lint invented?
  const stale = staleNames.get(p.slug);
  if (stale) {
    const files = [...p.curatedFiles, ...readdirSync(p.dir).filter((f) => f.endsWith('-visualizer.html'))];
    const found = new Set();
    for (const f of files) {
      const src = readFileSync(join(p.dir, f), 'utf8');
      for (const n of stale) if (new RegExp(`\\b${n}\\b`).test(src)) found.add(n);
    }
    if (found.size) why.push(`still carries lint's names: ${[...found].join(', ')}`);
  }

  // 2. Is the teaching block there, and written for the code as it stands?
  for (const f of p.curatedFiles) {
    const raw = readFileSync(join(p.dir, f), 'utf8');
    if (!splitTrailingTeach(raw).had) { why.push(`${f} has no teaching block`); continue; }
    if (rec.teachSignatures?.[f] !== rec.headerSignatures?.[f]) why.push(`${f}'s teaching block is not current`);
  }

  // 3. Is the visualizer's code panel the solution file, verbatim? The pipeline's
  //    own check, run against the files as they are on disk right now.
  const cls = rec.classification ?? {};
  const { chosen } = selectForVisualizer(p.curatedFiles, cls);
  const vizFile = readdirSync(p.dir).find((f) => f.endsWith('-visualizer.html'));
  if (!chosen.length) why.push('nothing classified to visualise');
  else if (!vizFile) why.push('no visualizer file on disk');
  else {
    const src = definitionFromFile(join(p.dir, vizFile));
    if (!src) why.push('the visualizer has no readable PROBLEM definition');
    else {
      const bodies = chosen.map((f) => stripHeader(splitTrailingTeach(readFileSync(join(p.dir, f), 'utf8')).code).body);
      const structures = [...new Set(chosen.flatMap((f) => cls[f]?.structures ?? []))];
      const v = validate(src, structures, chosen.map((f) => cls[f]?.structures ?? []), bodies);
      for (const e of v.errors.slice(0, 3)) why.push(e.length > 110 ? e.slice(0, 110) + '…' : e);
    }
  }

  if (!why.length) { ok++; console.log(`  ok        ${p.slug}`); continue; }
  demote.push(p.slug);
  console.log(`  DEMOTE    ${p.slug}`);
  for (const w of why) console.log(`              - ${w}`);
}

if (doApply && demote.length) {
  for (const slug of demote) {
    const rec = state.problems[slug];
    if (rec.visualizer) rec.visualizer.v = 1;
    if (rec.teachSignatures) for (const f of Object.keys(rec.teachSignatures)) delete rec.teachSignatures[f];
  }
  saveState(state);
}

console.log(`\n${current} folder(s) were marked current · ${ok} verified · ${demote.length} ${doApply ? 'demoted to needs reshape' : 'would be demoted'}.`);
if (!doApply && demote.length) console.log('\nReport only - re-run with --apply to demote them.\n');
else console.log('');
