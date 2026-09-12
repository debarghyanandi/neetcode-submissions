#!/usr/bin/env node
/**
 * reskin.mjs - push a chassis change into the visualizers that already exist.
 *
 * splice() writes the whole chassis into every visualizer, so each file is a
 * standalone copy. That is deliberate - a visualizer is one file you can open
 * from anywhere with no build step - but it means editing
 * templates/visualizer.chassis.html changes NOTHING that has already been
 * written. Before this script the only way to pick up a chassis improvement was
 * to regenerate the PROBLEM object too, which costs an Opus call per folder to
 * reproduce work that was already correct.
 *
 * So: lift each file's PROBLEM definition out, splice it into the current
 * chassis, put it back. Deterministic, no model, no cost. The animation is
 * untouched; only the shell around it moves forward.
 *
 * It still RUNS each definition against the new chassis before writing. A
 * chassis edit that renamed a helper would otherwise turn every visualizer in
 * the repo into a blank page at once, and re-splicing is exactly the operation
 * that would do it to all of them in one go.
 *
 *   node scripts/reskin.mjs                     # dry run over everything
 *   node scripts/reskin.mjs --apply
 *   node scripts/reskin.mjs --apply --slug two-integer-sum,binary-search
 *   node scripts/reskin.mjs --apply --force     # hand-built ones too
 */

import { readFileSync, writeFileSync, existsSync, appendFileSync } from 'node:fs';
import { join } from 'node:path';
import { loadState, scanRepo } from './lib/scan.mjs';
import { splice, validate } from './lib/visualizer.mjs';
import { report, group, endGroup } from './lib/report.mjs';

const argv = process.argv.slice(2);
const arg = (n, d = null) => (argv.includes(n) ? argv[argv.indexOf(n) + 1] : d);
const has = (n) => argv.includes(n);

const onlyRaw = arg('--slug');
const only = onlyRaw ? onlyRaw.split(',').map((x) => x.trim()).filter(Boolean) : null;
const limit = Number(arg('--limit', '0')) || 0;
const doApply = has('--apply');
const force = has('--force');

/**
 * Lift `const PROBLEM = { ... };` out of a finished visualizer.
 *
 * Brace matching rather than a regex: the definition contains braces in strings
 * and in every nested object, and a non-greedy match stops at the first one.
 */
export function extractProblem(html) {
  const lines = html.split(/\r?\n/);
  const start = lines.findIndex((l) => /^const PROBLEM = \{/.test(l));
  if (start < 0) return null;
  let depth = 0, end = -1;
  for (let i = start; i < lines.length && end < 0; i++) {
    for (const ch of lines[i]) {
      if (ch === '{') depth++;
      else if (ch === '}' && --depth === 0) { end = i; break; }
    }
  }
  if (end < 0) return null;
  // Keep the trailing semicolon when the definition carries one.
  const tail = lines[end].slice(lines[end].lastIndexOf('}') + 1).trim();
  const body = lines.slice(start, end + 1).join('\n');
  return tail.startsWith(';') ? body + ';' : body;
}

const state = loadState();
let targets = scanRepo(state).filter((p) => existsSync(join(p.dir, `${p.slug}-visualizer.html`)));
if (only) targets = targets.filter((p) => only.includes(p.slug));
if (limit) targets = targets.slice(0, limit);

if (!targets.length) { console.log('\nNo visualizers to reskin.\n'); process.exit(0); }

console.log(`\n${doApply ? 'APPLY' : 'DRY RUN'} - reskin, ${targets.length} visualizer(s)\n`);
let wrote = 0, skipped = 0, failures = 0, unchanged = 0;

for (const p of targets) {
  const file = join(p.dir, `${p.slug}-visualizer.html`);
  const html = readFileSync(file, 'utf8');
  const rec = state.problems[p.slug]?.visualizer;

  // A visualizer with no record was built by hand: its CSS and markup are its
  // own, and re-splicing would replace them with the chassis and quietly throw
  // that work away. Naming it is not enough; --force is the deliberate gesture.
  if (!rec && !force) {
    console.log(`  ${p.slug.padEnd(46)} hand-built, left alone (--force to reskin it too)`);
    skipped++;
    continue;
  }

  const def = extractProblem(html);
  if (!def) {
    group(p.path);
    console.log('  cannot find a PROBLEM definition - leaving it alone');
    report('reskin', p.slug, 'failed', 'no PROBLEM definition found');
    failures++;
    endGroup();
    continue;
  }

  const next = splice(def);
  if (next === html) { unchanged++; continue; }

  // Run it against the NEW chassis before trusting it. Coverage is checked
  // against what classify recorded, so a reskin cannot quietly downgrade a
  // visualizer that the shape rules would now reject.
  const cls = state.problems[p.slug]?.classification ?? {};
  const structures = [...new Set(Object.values(cls).flatMap((c) => c.structures ?? []))];
  const v = validate(def, structures);
  if (v.errors.length) {
    group(p.path);
    console.log('  the definition does not survive the current chassis:');
    v.errors.slice(0, 4).forEach((e) => console.log(`      ${e}`));
    report('reskin', p.slug, 'failed', v.errors[0].slice(0, 90));
    failures++;
    endGroup();
    continue;
  }

  console.log(`  ${p.slug.padEnd(46)} ${rec ? 'reskin' : 'reskin (hand-built, forced)'}`);
  if (doApply) {
    writeFileSync(file, next, 'utf8');
    report('reskin', p.slug, 'ok', 'respliced onto the current chassis');
  }
  wrote++;
}

console.log(`\n${wrote} ${doApply ? 'reskinned' : 'would be reskinned'}, ${unchanged} already current, ${skipped} hand-built skipped, ${failures} failed.\n`);
if (!doApply && wrote) console.log('Add --apply to write them.\n');
if (process.env.GITHUB_OUTPUT) appendFileSync(process.env.GITHUB_OUTPUT, `wrote=${wrote}\n`);
process.exit(failures ? 1 : 0);
