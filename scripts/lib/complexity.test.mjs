#!/usr/bin/env node
/**
 * complexity.test.mjs - which file gets to be called optimal.cs.
 *
 * assignNames decides filenames, and filenames are the most visible thing the
 * pipeline produces: they are in the README table, in index.md, in every link
 * anyone has to a solution. A rule that churns them, or that quietly demotes
 * the solution you wrote, is worth catching here rather than in a diff.
 *
 *   node scripts/lib/complexity.test.mjs
 */

import { assignNames } from './complexity.mjs';

let pass = 0, fail = 0;
const S = (file, time, space, selfMarked = false) => ({ file, time, space, selfMarked });

function names(label, solutions, expected) {
  const r = assignNames(solutions);
  const got = r.ok ? [...r.names].map(([f, n]) => `${f}=>${n}`).sort().join(' ') : `REFUSED: ${r.reason}`;
  const want = Object.entries(expected).map(([f, n]) => `${f}=>${n}`).sort().join(' ');
  if (got === want) { pass++; console.log(`ok    ${label}`); }
  else { fail++; console.log(`FAIL  ${label}\n      got:  ${got}\n      want: ${want}`); }
}

// A tie on BOTH axes means nothing separates them but who wrote it.
names('a tie goes to the solution you wrote',
  [S('optimal.cs', 'O(n)', 'O(n)'), S('optimal-variant.cs', 'O(n)', 'O(n)', true)],
  { 'optimal-variant.cs': 'optimal.cs', 'optimal.cs': 'optimal-variant.cs' });

names('and stays there - the swap happens once',
  [S('optimal.cs', 'O(n)', 'O(n)', true), S('optimal-variant.cs', 'O(n)', 'O(n)')],
  { 'optimal.cs': 'optimal.cs', 'optimal-variant.cs': 'optimal-variant.cs' });

names('with nothing marked, the incumbent keeps the name',
  [S('optimal.cs', 'O(n)', 'O(n)'), S('optimal-variant.cs', 'O(n)', 'O(n)')],
  { 'optimal.cs': 'optimal.cs', 'optimal-variant.cs': 'optimal-variant.cs' });

// The tie-break lives INSIDE the best tier. It is not a promotion.
names('your slower solution is not promoted',
  [S('optimal.cs', 'O(n)', 'O(1)'), S('suboptimal.cs', 'O(n^2)', 'O(1)', true)],
  { 'optimal.cs': 'optimal.cs', 'suboptimal.cs': 'suboptimal.cs' });

names('equal time but more space is not a tie',
  [S('optimal.cs', 'O(n)', 'O(1)'), S('optimal-variant.cs', 'O(n)', 'O(n)', true)],
  { 'optimal.cs': 'optimal.cs', 'optimal-variant.cs': 'suboptimal.cs' });

names('fresh submissions: yours wins whatever its number',
  [S('submission-1.cs', 'O(n)', 'O(n)'), S('submission-2.cs', 'O(n)', 'O(n)', true)],
  { 'submission-2.cs': 'optimal.cs', 'submission-1.cs': 'optimal-variant.cs' });

names('and without a marker, the lower number wins',
  [S('submission-1.cs', 'O(n)', 'O(n)'), S('submission-2.cs', 'O(n)', 'O(n)')],
  { 'submission-1.cs': 'optimal.cs', 'submission-2.cs': 'optimal-variant.cs' });

// Numeric collation puts optimal-variant-2.cs first, which renamed these two
// past each other on every run. invert-a-binary-tree has exactly this shape.
names('three in the best tier keep the names they have',
  [S('optimal.cs', 'O(n)', 'O(n)'), S('optimal-variant.cs', 'O(n)', 'O(n)'), S('optimal-variant-2.cs', 'O(n)', 'O(n)')],
  { 'optimal.cs': 'optimal.cs', 'optimal-variant.cs': 'optimal-variant.cs', 'optimal-variant-2.cs': 'optimal-variant-2.cs' });

{
  const r = assignNames([S('optimal.cs', 'other', 'O(1)'), S('suboptimal.cs', 'O(n)', 'O(1)')]);
  if (!r.ok && r.names === null) { pass++; console.log('ok    a complexity off the ladder is refused, not guessed'); }
  else { fail++; console.log('FAIL  a complexity off the ladder is refused, not guessed'); }
}

console.log(`\n${pass} passed, ${fail} failed.`);
process.exit(fail ? 1 : 0);
