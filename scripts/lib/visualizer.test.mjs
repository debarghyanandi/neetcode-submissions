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

import { renameInVisualizer } from './visualizer.mjs';

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

console.log(`\n${pass} passed, ${fail} failed.`);
process.exit(fail ? 1 : 0);
