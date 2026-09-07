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

import { standing } from './header.mjs';

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

console.log(`\n${pass} passed, ${fail} failed.`);
process.exit(fail ? 1 : 0);
