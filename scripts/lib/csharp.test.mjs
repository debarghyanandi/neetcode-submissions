#!/usr/bin/env node
/**
 * csharp.test.mjs - the sabotage suite for sameShape().
 *
 * sameShape is the only thing standing between the lint model and your code.
 * Every case below is either a rewrite that must be allowed through, or a
 * sabotage that must be caught. Run it after any change to csharp.mjs:
 *
 *   node scripts/lib/csharp.test.mjs
 *
 * A failure here means the guard has a hole, or has grown a false alarm that
 * will refuse honest rewrites. Both are worth stopping for.
 */

import { sameShape, shapeForm } from './csharp.mjs';

let pass = 0, fail = 0;

function accepts(name, before, after, expectRenames = null) {
  const r = sameShape(before, after);
  if (!r.ok) { fail++; console.log(`FAIL  ${name}\n      wrongly refused: ${r.errors.join(' | ')}`); return; }
  if (expectRenames) {
    const got = r.renames.map(([a, b]) => `${a}->${b}`).sort().join(',');
    const want = [...expectRenames].sort().join(',');
    if (got !== want) { fail++; console.log(`FAIL  ${name}\n      renames were [${got}], expected [${want}]`); return; }
  }
  pass++; console.log(`ok    ${name}`);
}

function refuses(name, before, after, mustMention) {
  const r = sameShape(before, after);
  if (r.ok) { fail++; console.log(`FAIL  ${name}\n      let it through`); return; }
  if (mustMention && !r.errors.some((e) => e.includes(mustMention))) {
    fail++; console.log(`FAIL  ${name}\n      refused for the wrong reason: ${r.errors.join(' | ')}`); return;
  }
  pass++; console.log(`ok    ${name}  (${r.errors[0]})`);
}

const BASE = `public class Solution
{
    public int Search(int[] nums, int t)
    {
        int l = 0, r = nums.Length - 1;
        while (l <= r)
        {
            int m = l + (r - l) / 2;   // avoid overflow
            if (nums[m] == t) return m;
            if (t < nums[m]) r = m - 1; else l = m + 1;
        }
        return -1;
    }
}`;

const ren = (s, pairs) => pairs.reduce((acc, [a, b]) => acc.replace(new RegExp(`\\b${a}\\b`, 'g'), b), s);

// ---- rewrites that must be allowed -------------------------------------
accepts('identical file', BASE, BASE, []);

accepts('reindented only', BASE, BASE.replace(/\n    /g, '\n        '), []);

accepts('honest rename', BASE,
  ren(BASE, [['t', 'target'], ['l', 'left'], ['r', 'right'], ['m', 'mid']]),
  ['t->target', 'l->left', 'r->right', 'm->mid']);

accepts('comment reworded after a rename', BASE,
  ren(BASE, [['m', 'mid']]).replace('// avoid overflow', '// mid without overflowing'),
  ['m->mid']);

accepts('an extra comment added', BASE,
  BASE.replace('while (l <= r)', '// binary search\n        while (l <= r)'), []);

// The bug this suite was written for: `where` is a contextual keyword, which
// means it is a perfectly ordinary identifier here. Renaming it away used to
// be refused as "kw -> id".
accepts('renaming a variable that happens to be a contextual keyword',
  'public class S { public int F() { int where = 1; return where; } }',
  'public class S { public int F() { int index = 1; return index; } }',
  ['where->index']);

accepts('property accessors are not renames',
  'public class S { public int Count { get; set; } public int F(int q) { return q; } }',
  'public class S { public int Count { get; set; } public int F(int count) { return count; } }',
  ['q->count']);

accepts('renaming a variable whose type is a custom class',
  'public class S { public int F(TreeNode root) { TreeNode node = root; return node.val; } }',
  'public class S { public int F(TreeNode root) { TreeNode current = root; return current.val; } }',
  ['node->current']);

// ---- sabotage that must be caught --------------------------------------
refuses('flipped comparison', BASE, BASE.replace('l <= r', 'l >= r'), 'op changed');
refuses('shortened comparison', BASE, BASE.replace('l <= r', 'l < r'), 'token count changed');
refuses('changed literal', BASE, BASE.replace('return -1;', 'return -2;'), 'num changed');
refuses('dropped statement', BASE, BASE.replace('            if (nums[m] == t) return m;\n', ''), 'token count changed');
refuses('added statement', BASE, BASE.replace('return -1;', 'Console.WriteLine(l);\n        return -1;'), 'token count changed');
refuses('renamed a member after a dot', BASE, BASE.replace('nums.Length', 'nums.Count'), 'member name changed');
refuses('renamed the class', BASE, BASE.replace('class Solution', 'class BinarySearch'), 'type name changed');
refuses('two variables collapsed into one', BASE, ren(BASE, [['t', 'target'], ['m', 'target']]), 'both became');
refuses('deleted a comment', BASE, BASE.replace('   // avoid overflow', ''), 'comment(s) deleted');
refuses('renamed onto a contextual keyword', BASE, ren(BASE, [['t', 'value']]), 'contextual keyword');
// A collection swap is a complexity change wearing a rename's clothes: seen
// maps one-to-one onto seen, List onto HashSet, and every other token matches.
const CONTAINS = 'public class S { public bool F(int[] nums) { List<int> seen = new List<int>(); foreach (var n in nums) { if (seen.Contains(n)) return true; seen.Add(n); } return false; } }';
refuses('List swapped for HashSet', CONTAINS, CONTAINS.split('List').join('HashSet'), 'type name changed');
refuses('constructed type swapped',
  'public class S { public void F() { Queue<int> q = new Queue<int>(); } }',
  'public class S { public void F() { Queue<int> q = new Stack<int>(); } }', 'type name changed');

refuses('var swapped for an explicit type',
  'public class S { public void F() { var x = 1; } }',
  'public class S { public void F() { int x = 1; } }', 'kw changed');

// ---- shapeForm: the same solution, however it is written ---------------

function same(name, a, b, want = true) {
  const got = shapeForm(a) === shapeForm(b);
  if (got === want) { pass++; console.log(`ok    ${name}`); }
  else { fail++; console.log(`FAIL  ${name}\n      shapes ${got ? 'matched' : 'differed'}, expected ${want ? 'a match' : 'a difference'}`); }
}

same('renamed variables are the same solution', BASE, ren(BASE, [['t', 'target'], ['l', 'left'], ['r', 'right'], ['m', 'mid']]));
same('recommented is the same solution', BASE, BASE.replace('// avoid overflow', '// NOTE: mid, computed safely'));
same('reindented is the same solution', BASE, BASE.replace(/\n    /g, '\n\t'));
same('a flipped comparison is NOT the same solution', BASE, BASE.replace('l <= r', 'l >= r'), false);
same('a different literal is NOT the same solution', BASE, BASE.replace('return -1;', 'return -2;'), false);
same('List and HashSet are NOT the same solution', CONTAINS, CONTAINS.split('List').join('HashSet'), false);
same('a queue and a stack are NOT the same solution',
  'public class S { void F() { Queue<int> q = new Queue<int>(); q.Enqueue(1); } }',
  'public class S { void F() { Stack<int> q = new Stack<int>(); q.Push(1); } }', false);

console.log(`\n${pass} passed, ${fail} failed.`);
process.exit(fail ? 1 : 0);
