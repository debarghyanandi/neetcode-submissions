#!/usr/bin/env node
/**
 * shapes.test.mjs - the coverage check's deterministic half.
 *
 * coverage() is the rule that stopped every problem in this repo coming out
 * array-shaped. It is also the rule most likely to be wrong in the annoying
 * direction: too strict and a legitimate iterative solution cannot be drawn at
 * all, too loose and it enforces nothing and we are back where we started. So
 * the cases below are mostly about the EDGES of the rule rather than its
 * middle - what it lets through, and why.
 *
 *   node scripts/lib/shapes.test.mjs
 */

import { coverage, required, panelsFor, STRUCTURES, STRUCTURE_HELP, PANELS, ENFORCED, catalogueSection, contractSection } from './shapes.mjs';

let pass = 0, fail = 0;
const is = (name, got, want) => {
  const g = JSON.stringify(got), w = JSON.stringify(want);
  if (g === w) { pass++; console.log(`ok    ${name}`); }
  else { fail++; console.log(`FAIL  ${name}\n      got:  ${g}\n      want: ${w}`); }
};
const ok = (name, cond) => is(name, !!cond, true);

// ---- the catalogue is internally consistent -------------------------------
// Cheap, and it is the failure mode of a table split across two constants: a
// structure listed as enforced whose panel nobody ever wrote fails every run
// with no way to pass.
for (const p of PANELS) {
  for (const s of p.renders) {
    ok(`${p.fn} renders ${s}, which is a real structure`, STRUCTURES.includes(s));
  }
}
for (const s of ENFORCED) {
  ok(`enforced structure "${s}" has at least one panel that can draw it`, panelsFor(s).length > 0);
}
for (const s of STRUCTURES) {
  ok(`"${s}" has a help line for the classify prompt`, typeof STRUCTURE_HELP[s] === 'string' && STRUCTURE_HELP[s].length > 5);
}

// ---- the bug this whole thing exists to prevent ---------------------------
is('a tree drawn as rows of chips is rejected',
  coverage(['tree'], ['chips', 'pills'], null).errors.length, 1);

is('a recursive solution with no frames on screen is rejected',
  coverage(['tree', 'call-stack'], ['tree', 'pills'], null).errors.length, 1);

is('drawing both satisfies both',
  coverage(['tree', 'call-stack'], ['tree', 'stack'], null).errors, []);

// ---- deliberately weak: any capable panel counts --------------------------
// The check is "the structure is on screen", never "you called this function".
is('a queue may be drawn by the bucket panel in queue mode',
  coverage(['queue'], ['stack'], null).errors, []);
is('a graph may be satisfied by its adjacency list rather than a node-link drawing',
  coverage(['graph'], ['chips'], null).errors, []);
is('a grid-shaped graph may be satisfied by the grid panel',
  coverage(['graph'], ['grid'], null).errors, []);

// ---- not everything is enforced, on purpose -------------------------------
// An array IS a row of boxes and a counter IS a row of chips. Enforcing those
// would be ceremony, and ceremony is what makes a check get switched off.
is('an array needs no special panel', coverage(['array'], ['tiles'], null).errors, []);
is('a hash map needs no special panel', coverage(['hash-map'], ['chips'], null).errors, []);
is('intervals need no special panel', coverage(['interval'], ['ranges'], null).errors, []);
is('only structures with real geometry are required',
  required(['array', 'string', 'hash-map', 'tree']).map((r) => r.structure), ['tree']);

// ---- the override --------------------------------------------------------
is('an override with a real reason waives the requirement',
  coverage(['call-stack'], ['tree'],
    { skip: ['call-stack'], reason: 'the loop rebinds one variable and keeps no frames at all' }),
  { errors: [], waived: ['call-stack'] });

is('an override with a token reason does not',
  coverage(['call-stack'], ['tree'], { skip: ['call-stack'], reason: 'n/a' }).errors.length, 1);

is('an override only waives what it names',
  coverage(['tree', 'call-stack'], ['pills'],
    { skip: ['call-stack'], reason: 'the iterative walk keeps no frames, which is the point of it' }).errors.length, 1);

is('an override for something that was drawn anyway is not reported as waived',
  coverage(['call-stack'], ['stack'],
    { skip: ['call-stack'], reason: 'this reason is long enough to pass the length check' }).waived, []);

// ---- unrecorded state degrades, it does not explode -----------------------
// Every folder classified before the structures field existed arrives here.
is('no structures on record enforces nothing', coverage([], ['tiles'], null).errors, []);
is('an unknown structure is ignored rather than crashing', coverage(['quantum-rope'], ['tiles'], null).errors, []);
is('a missing override object is fine', coverage(['tree'], ['tree'], undefined).errors, []);

// ---- prompt text is built, not hand-maintained ---------------------------
ok('the catalogue names every panel', PANELS.every((p) => catalogueSection(['tree']).includes(p.sig)));
ok('the catalogue marks the panels that suit the problem', catalogueSection(['tree']).includes('<- suits this problem'));
ok('the contract names the structures on record', contractSection(['tree', 'heap']).includes('tree, heap'));
ok('the contract explains itself when nothing is on record', contractSection([]).includes('no structures on record'));

console.log(`\n${pass} passed, ${fail} failed.`);
process.exit(fail ? 1 : 0);
