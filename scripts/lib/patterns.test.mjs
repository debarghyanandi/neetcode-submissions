#!/usr/bin/env node
/**
 * patterns.test.mjs - the index page's grouping.
 *
 * Every case below is a real problem in this repo. Two of them are here because
 * the first version of the rules got them wrong, and both failures were of the
 * same kind: a substring match inside a word. They are the reason the rules are
 * anchored, and the reason this file exists - a misgrouped problem does not
 * throw, it just sits quietly in the wrong section of the page.
 *
 *   node scripts/lib/patterns.test.mjs
 */

import { groupFor, patternLine, GROUPS, GROUP_NOTE, groupRank } from './patterns.mjs';

let pass = 0, fail = 0;
const is = (name, got, want) => {
  if (got === want) { pass++; console.log(`ok    ${name}`); }
  else { fail++; console.log(`FAIL  ${name}\n      got:  ${got}\n      want: ${want}`); }
};
const g = (slug, pattern, structures) => groupFor({ slug, pattern, structures });

// ---- the two that were actually wrong ------------------------------------
is('"substring" does not contain a BST',
  g('longest-substring-without-duplicates', 'Sliding window - jump left past the last duplicate'),
  'Sliding Window');
is('"rotated" is not "rotting"',
  g('find-minimum-in-rotated-sorted-array', 'Binary Search on the Break - compare mid against right'),
  'Binary Search');

is('rotting-fruit is a graph, not a stack - a queue on a grid is BFS',
  g('rotting-fruit', 'Multi-source BFS on a grid - time carried per node', ['array', 'matrix', 'queue']),
  'Graphs');
is('and it is still a graph on its slug alone, before it is classified',
  g('rotting-fruit', '', []),
  'Graphs');
is('a queue alone still does not make something a graph',
  g('implement-queue-using-stacks', 'Two Stacks - lazy drain, refill only when output empty', ['stack', 'queue']),
  'Stack');

// ---- the ordering the rules depend on ------------------------------------
is('a monotonic deque is a window technique, not a stack one',
  g('sliding-window-maximum', 'Monotonic deque of indices - front is the window max'),
  'Sliding Window');
is('a monotonic stack still lands in Stack',
  g('daily-temperatures', 'Monotonic Stack - decreasing stack of unresolved indices'),
  'Stack');
is('a two-pointer problem over a list is a list problem',
  g('remove-node-from-end-of-linked-list', 'Two Pointers - fixed n-gap window plus dummy head'),
  'Linked List');
is('fast and slow pointers over a list, likewise',
  g('linked-list-cycle-detection', "Fast and Slow Pointers - Floyd's cycle detection"),
  'Linked List');
is('a tree named "binary" is not a binary search',
  g('binary-tree-diameter', 'Post-order DFS - return height, update diameter globally'),
  'Trees');
is('a bucket sort is not a heap, whatever the problem is called',
  g('top-k-elements-in-list', 'Bucket sort by frequency - count, bucket, walk down'),
  'Arrays & Hashing');
is('a bounded heap is',
  g('k-closest-points-to-origin', 'Top-K via bounded max-heap: evict the current farthest'),
  'Heap / Priority Queue');
is('"greedy buy day" does not make it a greedy problem',
  g('buy-and-sell-crypto', 'Running minimum - fix the sell day, greedy buy day'),
  'Sliding Window');
is('Kadane does', g('maximum-subarray', 'Kadane - reset the running sum when it goes negative'), 'Greedy');
is('a 2-D binary search is still a binary search',
  g('search-2d-matrix', 'Binary search over the flattened 2D index'), 'Binary Search');
is('LRU is pointer surgery', g('lru-cache', 'Hash map + doubly linked list, sentinels at both ends'), 'Linked List');

// ---- recorded structures outrank the prose -------------------------------
is('a recorded tree wins even with no pattern line',
  g('mystery-problem', '', ['tree', 'call-stack']), 'Trees');
is('a recorded heap wins even with no pattern line',
  g('mystery-problem', '', ['heap']), 'Heap / Priority Queue');
is('a recorded graph wins even with no pattern line',
  g('mystery-problem', '', ['graph']), 'Graphs');

// ---- degrading -----------------------------------------------------------
// A folder that has been submitted but not yet taught has no PATTERN line at
// all. It must still be placed, and must never throw.
is('an untaught folder still groups off its slug',
  g('remove-duplicates-from-sorted-array', ''), 'Two Pointers');
is('something genuinely unknown falls through', g('quantum-rope-balancing', ''), 'Other');
is('no pattern and no structures is fine', g('', ''), 'Other');
is('undefined fields do not throw', groupFor({ slug: 'x' }), 'Other');

// ---- the teaching block reader -------------------------------------------
is('the PATTERN line is read out of a teaching block',
  patternLine('// ====\n// PATTERN : Two Pointers - converge inward\n// SOURCE  : ...'),
  'Two Pointers - converge inward');
is('a file with no teaching block returns empty', patternLine('public class Solution {}'), '');

// ---- the group table is self-consistent ----------------------------------
for (const name of GROUPS) {
  is(`"${name}" has a note`, typeof GROUP_NOTE[name] === 'string' && GROUP_NOTE[name].length > 10, true);
}
is('every group ranks inside the list', GROUPS.every((n) => groupRank(n) < GROUPS.length), true);
is('an unknown group sorts last', groupRank('Nonsense') >= GROUPS.length, true);
is('"Other" is last so unfiled work is visible at the bottom', GROUPS[GROUPS.length - 1], 'Other');

// ---- the classifier's own words count ------------------------------------
//
// house-robber-ii landed in Other on the day it was solved. classify had called it
// "Dynamic programming, space-optimized, circular constraint split" - exactly right -
// but groupFor was never handed that field. It saw structures ["array"], because a
// space-optimised DP allocates no table, and a PATTERN line reading "two linear DP
// runs", which does not contain the literal words "dynamic programming".
const ga = (slug, pattern, algorithm, structures) => groupFor({ slug, pattern, algorithm, structures });

is('a space-optimised DP has no dp-table, and is still 1-D DP',
  ga('house-robber-ii', 'House Robber II - circular array split into two linear DP runs',
     'Dynamic programming, space-optimized, circular constraint split', ['array']),
  '1-D DP');
is('the algorithm field alone is enough',
  ga('some-problem', '', 'Bottom-up dynamic programming', ['array']), '1-D DP');
is('and so is a bare "DP" in the teach block',
  ga('some-problem', 'bottom-up DP over the prefix', '', ['array']), '1-D DP');
is('a dp-table still lands there with nothing else said',
  ga('x', '', '', ['array', 'dp-table']), '1-D DP');

// Word-bounded, so it cannot fire on a substring of something else.
is('"dpi" is not DP - it falls through rather than being filed as one',
  ga('x', 'scale by dpi', '', ['array']) === '1-D DP', false);

// 2-D is tested first and no longer needs a dp-table either, since a rolling-row
// grid DP has none.
is('a grid DP is 2-D, not 1-D',
  ga('unique-paths', 'DP over a grid of cells', 'Dynamic programming on a matrix', ['matrix']), '2-D DP');
is('and one that keeps its table, likewise',
  ga('edit-distance', 'tabulate the edit distance', 'Dynamic programming', ['dp-table']), '2-D DP');

// Greedy is tested AFTER 1-D DP, so a Kadane scan that called itself DP would now be
// filed as DP. It does not, and this pins that.
is('Kadane is still greedy',
  ga('maximum-subarray', "Kadane's algorithm, one rolling sum", "Kadane's algorithm", ['array']), 'Greedy');

console.log(`\n${pass} passed, ${fail} failed.`);
process.exit(fail ? 1 : 0);
