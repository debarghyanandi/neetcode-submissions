#!/usr/bin/env node
/**
 * visualizer.test.mjs - the visualizer's deterministic half.
 *
 * renameInVisualizer follows a curated rename into the badge each panel shows.
 * It exists because re-keying the recorded code prints stopped the rebuild
 * that used to refresh those badges by accident, and a caption naming the
 * wrong solution is worse than a stale one - it is confidently wrong.
 *
 *   node scripts/lib/visualizer.test.mjs
 */

import { renameInVisualizer, structuresFor, validate } from './visualizer.mjs';
import { coverage } from './shapes.mjs';

let pass = 0, fail = 0;
const is = (name, got, want) => {
  if (got === want) { pass++; console.log(`ok    ${name}`); }
  else { fail++; console.log(`FAIL  ${name}\n      got:  ${got}\n      want: ${want}`); }
};

const SWAP = { 'optimal.cs': 'optimal-variant.cs', 'optimal-variant.cs': 'optimal.cs' };

// depth-of-binary-tree: the tie-break swapped the two panels' files.
const BADGES = "badge: 'O(n) time · level-order BFS · optimal.cs',\nbadge: 'O(n) time · recursive DFS · optimal-variant.cs',";
is('a swap is applied once, not twice',
  renameInVisualizer(BADGES, SWAP).html,
  "badge: 'O(n) time · level-order BFS · optimal-variant.cs',\nbadge: 'O(n) time · recursive DFS · optimal.cs',");

is('and reports what it touched', String(renameInVisualizer(BADGES, SWAP).changed), '2');

is('a rename to the same name is not a change',
  String(renameInVisualizer(BADGES, { 'optimal.cs': 'optimal.cs' }).changed), '0');

is('an empty map leaves the file alone',
  renameInVisualizer(BADGES, {}).html, BADGES);

// Longest-first alternation, and boundaries on both sides.
is('a numbered variant is not caught by its prefix',
  renameInVisualizer("· optimal-variant-2.cs'", SWAP).html, "· optimal-variant-2.cs'");

is('a longer name that merely starts the same is left alone',
  renameInVisualizer("// optimalish.cs and optimal.csv stay", SWAP).html,
  "// optimalish.cs and optimal.csv stay");

is('a submission name renames to its curated name',
  renameInVisualizer("badge: 'x · submission-2.cs'", { 'submission-2.cs': 'optimal.cs' }).html,
  "badge: 'x · optimal.cs'");

// A three-way rotation must also survive one pass.
is('a three-way rotation is applied once',
  renameInVisualizer("a=optimal.cs b=optimal-variant.cs c=optimal-variant-2.cs", {
    'optimal.cs': 'optimal-variant.cs',
    'optimal-variant.cs': 'optimal-variant-2.cs',
    'optimal-variant-2.cs': 'optimal.cs',
  }).html,
  "a=optimal-variant.cs b=optimal-variant-2.cs c=optimal.cs");

// ---- each solution is held to its own file's structures ------------------
// reverse-a-linked-list: the iterative file is a linked list, the recursive one also a call stack.
const UNION = ['linked-list', 'call-stack'];
const PER = [['linked-list'], ['linked-list', 'call-stack']];
is('the iterative tab needs only its own structures',
  JSON.stringify(structuresFor(0, UNION, PER)), JSON.stringify(['linked-list']));
is('so drawing a list and no call stack passes, with no waiver',
  coverage(structuresFor(0, UNION, PER), ['list'], null).errors.length, 0);
is('the recursive tab still must draw its call stack',
  coverage(structuresFor(1, UNION, PER), ['list'], null).errors.length > 0, true);
is('a solution with nothing recorded falls back to the union',
  JSON.stringify(structuresFor(0, UNION, [[], PER[1]])), JSON.stringify(UNION));
is('and with no per-solution list at all, nothing changes',
  JSON.stringify(structuresFor(1, UNION, null)), JSON.stringify(UNION));

// ---- the code panel must be the file, copied verbatim --------------------
// Opus at medium effort invented explanatory comments and reflowed a statement
// across two lines (2026-09-17, binary-tree-diameter). Every line number still
// resolved and every other check passed, so only a verbatim comparison sees it.
const FILE_BODY = [
  'public int Total(int[] nums)',
  '{',
  '    int sum = 0;                 // running total',
  '    foreach (int n in nums) sum += n;',
  '    return sum;',
  '}',
].join('\n');

const defWith = (codeLines) => `
const PROBLEM = {
  title: 'T', note: 'n',
  inputs: [{id:'a', label:'a', value:'1'}],
  parse(raw){ return {ok:true, value:{v:1}, normalized:{a:'1'}}; },
  shuffle(){ return {a:'1'}; },
  solutions: [{
    label: 'L', badge: 'B · optimal.cs', blurb: 'b',
    code: ${JSON.stringify(codeLines)},
    simulate(){ return [{lines:[1], msg:'m', panels:[]}]; },
    shapeOverride: {skip: ['*'], reason: 'fixture'},
  }],
};`;

const fidelityErrors = (codeLines) =>
  validate(defWith(codeLines), [], [[]], [FILE_BODY]).errors.filter((e) => /not a line of the solution file|not in the solution file/.test(e));

is('the file copied verbatim passes',
  fidelityErrors(FILE_BODY.split('\n')).length, 0);

is('a prefix of the file passes - stopping early is allowed',
  fidelityErrors(FILE_BODY.split('\n').slice(0, 4)).length, 0);

is('re-indentation alone is not a fidelity failure',
  fidelityErrors(FILE_BODY.split('\n').map((l) => l.trim())).length, 0);

is('an invented comment is caught',
  fidelityErrors([...FILE_BODY.split('\n'), '// add them all up']).length, 1);

is('a reworded comment on a real line is caught',
  fidelityErrors(['    int sum = 0;                 // start at zero']).length, 1);

is('a statement reflowed across two lines is caught',
  fidelityErrors(['    foreach (int n in nums)', '        sum += n;']).length, 2);

is('a blank line is ignored rather than flagged',
  fidelityErrors(['public int Total(int[] nums)', '', '{']).length, 0);

is('with no source bodies the check does not run',
  validate(defWith(['// entirely invented']), [], [[]], null)
    .errors.filter((e) => /not a line of the solution file/.test(e)).length, 0);

console.log(`\n${pass} passed, ${fail} failed.`);
process.exit(fail ? 1 : 0);
