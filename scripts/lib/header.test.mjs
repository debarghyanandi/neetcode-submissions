#!/usr/bin/env node
/**
 * header.test.mjs - the header's deterministic half.
 *
 * standing() is derived from the ranked list, never asked of the model, which
 * means it is testable and must be tested: it is the one line of the header
 * that makes a claim about a FILE OTHER THAN ITS OWN, so a mistake shows up as
 * two headers in one folder contradicting each other.
 *
 *   node scripts/lib/header.test.mjs
 */

import { standing, solutionBody } from './header.mjs';
import { isSelfMarked } from './complexity.mjs';

let pass = 0, fail = 0;
const is = (name, got, want) => {
  if (got === want) { pass++; console.log(`ok    ${name}`); }
  else { fail++; console.log(`FAIL  ${name}\n      got:  ${got}\n      want: ${want}`); }
};

const S = (n, t, sp) => ({ name: n, time: t, space: sp });

is('a folder with one solution',
  standing('optimal.cs', [S('optimal.cs', 'O(n)', 'O(1)')]),
  'the only solution in this folder');

const BETTER = [S('optimal.cs', 'O(n)', 'O(n)'), S('suboptimal.cs', 'O(n^2)', 'O(1)')];
is('the faster one ranks above',
  standing('optimal.cs', BETTER), 'ranks above suboptimal.cs (O(n^2) time / O(1) space)');
is('the slower one ranks below',
  standing('suboptimal.cs', BETTER), 'ranks below optimal.cs (O(n) time / O(n) space)');

// depth-of-binary-tree: BFS and recursive DFS, both O(n)/O(n). The order
// between them is decided by filename, not by merit, so neither may claim to
// rank above the other.
const TIED = [S('optimal.cs', 'O(n)', 'O(n)'), S('optimal-variant.cs', 'O(n)', 'O(n)')];
is('a tie does not become a ranking',
  standing('optimal.cs', TIED), 'ties with optimal-variant.cs on O(n) time / O(n) space');
is('and the other side agrees',
  standing('optimal-variant.cs', TIED), 'ties with optimal.cs on O(n) time / O(n) space');

// Equal time, worse space is NOT a tie - the ladder is (time, space).
const SPACE = [S('optimal.cs', 'O(n)', 'O(1)'), S('optimal-variant.cs', 'O(n)', 'O(n)')];
is('same time but more space still ranks',
  standing('optimal.cs', SPACE), 'ranks above optimal-variant.cs (O(n) time / O(n) space)');
is('and reads as below from the other side',
  standing('optimal-variant.cs', SPACE), 'ranks below optimal.cs (O(n) time / O(1) space)');

// ---- solutionBody, and the //My solution marker -------------------------
//
// The marker is how a solution is recorded as YOURS rather than a reference, and
// it is read by matching //My solution against the BODY. Nothing else in the
// pipeline decides this: classify never asks the model, and once provenance is
// recorded it is never re-derived. So what the body is matters, and these pin it.

const CURATED = [
  '// ------------------------------------------------------------------',
  '// -  optimal.cs         O(n) time / O(1) space',
  '// ------------------------------------------------------------------',
  '',
  'public class Solution',
  '{',
  '    //My solution',
  '    public int Rob(int[] nums)',
  '    {',
  '        return nums[0];',
  '    }',
  '}',
  '',
  '/*',
  '================================================================================',
  ' PATTERN : something',
  '================================================================================',
  'WHY THIS PATTERN',
  '  A reader might well write "my solution" in here while discussing it.',
  '================================================================================',
  '*/',
].join('\n');

const body = solutionBody(CURATED);
is('solutionBody drops the header banner', body.includes('optimal.cs         O(n)'), false);
is('and the teaching block', body.includes('WHY THIS PATTERN'), false);
is('and keeps the code', body.includes('public int Rob(int[] nums)'), true);
is('the marker inside the code is still found', isSelfMarked(body), true);

// The reason the search must run on the body and not the whole file. Prose alone
// cannot trip the marker - it is anchored to a // comment - but a teaching block
// that QUOTES the marker can, and quoting the code is exactly what teach does.
const REFERENCE = CURATED.replace('    //My solution\n', '')
  .replace('A reader might well write "my solution" in here while discussing it.',
           'The author tags solutions they wrote themselves with //My solution at the top.');
is('a reference solution stays unmarked when the teaching block merely quotes the marker',
  isSelfMarked(solutionBody(REFERENCE)), false);
is('...and it would be marked if the whole file were searched', isSelfMarked(REFERENCE), true);
// Prose without the // is not a marker in either place, which is why the regex is anchored.
is('plain prose is never a marker', isSelfMarked('this is my solution, roughly'), false);

is('an empty input is empty, not a crash', solutionBody('').trim(), '');
is('and so is a nullish one', solutionBody(null).trim(), '');
is('a bare file with no header or block comes through whole',
  solutionBody('public class S { }').trim(), 'public class S { }');

console.log(`\n${pass} passed, ${fail} failed.`);
process.exit(fail ? 1 : 0);
