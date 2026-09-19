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

import { assignNames, canonical, rank, CHOICES, COMPLEXITY, ALIAS } from './complexity.mjs';

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

// The real longest-common-subsequence folder, which refused for want of an O(m) rung.
// Same time for all three; the rolling two-row tabulation is the only one that is not
// O(m * n) in space, so it is the one that earns optimal.cs. Before the rung existed the
// whole folder was left untouched instead.
names('a space-optimised two-input DP is the optimal one',
  [S('submission-0.cs', 'O(m * n)', 'O(m * n)', true),
   S('submission-2.cs', 'O(m * n)', 'O(m * n)', true),
   S('submission-4.cs', 'O(m * n)', 'O(m)', true)],
  { 'submission-4.cs': 'optimal.cs', 'submission-0.cs': 'suboptimal.cs', 'submission-2.cs': 'suboptimal-2.cs' });

// O(m) and O(n) are one tier wearing two names, so a folder holding both is a TIE - two
// interchangeable solutions - not a ranking. This is the whole reason O(m) is an alias
// rather than a rung of its own: a rung would have forced a false ordering.
names('O(n) and O(m) space are the same tier, so both stay in the best tier',
  [S('optimal.cs', 'O(n)', 'O(n)'), S('optimal-variant.cs', 'O(n)', 'O(m)')],
  { 'optimal.cs': 'optimal.cs', 'optimal-variant.cs': 'optimal-variant.cs' });

{
  const r = assignNames([S('optimal.cs', 'other', 'O(1)'), S('suboptimal.cs', 'O(n)', 'O(1)')]);
  if (!r.ok && r.names === null) { pass++; console.log('ok    a complexity off the ladder is refused, not guessed'); }
  else { fail++; console.log('FAIL  a complexity off the ladder is refused, not guessed'); }
}


// ---- the ladder does not stop the pipeline for a new letter -------------------------
// This is the class of failure that took longest-common-subsequence's run down: a real
// tier the ladder happened to spell with another letter. Resolution is by variable
// rename only - never by moving a magnitude - so a genuinely new shape still refuses.

const canon = (label, text, want) => {
  const got = canonical(text);
  if (got === want) { pass++; console.log(`ok    ${label}`); }
  else { fail++; console.log(`FAIL  ${label}\n      got:  ${got}\n      want: ${want}`); }
};

canon('a rung is itself', 'O(n log n)', 'O(n log n)');
canon('the other input is the same tier', 'O(m)', 'O(n)');
canon('and so is a log of it', 'O(log m)', 'O(log n)');
canon('the two-input sum, written the other way round', 'O(m + n)', 'O(n + m)');
canon('the product, written the other way round', 'O(n * m)', 'O(m * n)');
canon('spacing is not meaning', 'O(m*n)', 'O(m * n)');
canon('nor is case', 'o(M * N)', 'O(m * n)');
canon('nor is a stray space', '  O(n)  ', 'O(n)');
canon('a letter nobody listed still resolves', 'O(p)', 'O(n)');
canon('two letters nobody listed, renamed onto the rung', 'O(a * b)', 'O(m * n)');
canon('a binary search per row, whichever letters were used', 'O(a log b)', 'O(m log n)');

// k is a rung whose meaning IS "bounded by n but not n". Renaming it would erase that.
canon('k is left alone, so O(n log k) stays its own rung', 'O(n log k)', 'O(n log k)');
canon('and the second input paired with k still resolves', 'O(m log k)', 'O(n log k)');

// The refusal has to survive, or the ladder stops meaning anything.
canon('a genuinely new shape is not guessed at', 'O(n * 2^n)', null);
canon('nor is prose', 'depends on the input', null);
canon('nor is nothing at all', '', null);
canon('nor a non-string', undefined, null);

// Ranking, not just resolving: an alias must land on exactly its target's rung.
{
  const same = rank('O(m)') === rank('O(n)') && rank('O(n * m)') === rank('O(m * n)');
  if (same) { pass++; console.log('ok    an alias ranks equal to its target, not one rung off'); }
  else { fail++; console.log('FAIL  an alias ranks equal to its target, not one rung off'); }
}
{
  const off = rank('O(n * 2^n)') >= COMPLEXITY.length;
  if (off) { pass++; console.log('ok    an unresolvable complexity ranks off the ladder'); }
  else { fail++; console.log('FAIL  an unresolvable complexity ranks off the ladder'); }
}

// Every alias must point at a real rung, or the model can pick a value that ranks nowhere.
{
  const bad = Object.entries(ALIAS).filter(([, t]) => !COMPLEXITY.includes(t)).map(([a]) => a);
  if (!bad.length) { pass++; console.log('ok    every alias points at a rung that exists'); }
  else { fail++; console.log(`FAIL  every alias points at a rung that exists\n      dangling: ${bad.join(', ')}`); }
}
// ...and no alias may shadow a rung, or one tier would have two different ranks.
{
  const clash = Object.keys(ALIAS).filter((a) => COMPLEXITY.includes(a));
  if (!clash.length) { pass++; console.log('ok    no alias shadows a rung'); }
  else { fail++; console.log(`FAIL  no alias shadows a rung\n      clashing: ${clash.join(', ')}`); }
}
// CHOICES is what the model is actually allowed to answer.
{
  const ok = CHOICES.length === COMPLEXITY.length + Object.keys(ALIAS).length
    && CHOICES.at(-1) === 'other'
    && CHOICES.every((c) => c === 'other' || rank(c) < COMPLEXITY.length);
  if (ok) { pass++; console.log('ok    every value the model may answer ranks somewhere'); }
  else { fail++; console.log('FAIL  every value the model may answer ranks somewhere'); }
}

// The real folder, once more, with O(m) arriving as an alias rather than a rung.
names('longest-common-subsequence: the rolling-row DP is the optimal one',
  [S('submission-0.cs', 'O(m * n)', 'O(m * n)', true),
   S('submission-2.cs', 'O(m * n)', 'O(m * n)', true),
   S('submission-4.cs', 'O(m * n)', 'O(m)', true)],
  { 'submission-4.cs': 'optimal.cs', 'submission-0.cs': 'suboptimal.cs', 'submission-2.cs': 'suboptimal-2.cs' });

console.log(`\n${pass} passed, ${fail} failed.`);
process.exit(fail ? 1 : 0);
