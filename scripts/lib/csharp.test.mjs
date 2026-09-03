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

import { sameShape } from './csharp.mjs';

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
refuses('var swapped for an explicit type',
  'public class S { public void F() { var x = 1; } }',
  'public class S { public void F() { int x = 1; } }', 'kw changed');

console.log(`\n${pass} passed, ${fail} failed.`);
process.exit(fail ? 1 : 0);
