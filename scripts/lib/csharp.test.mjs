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

import { sameShape, shapeForm, tokenDelta, tokenize } from './csharp.mjs';

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
  // A sabotage that edits nothing is not a passing test, it is a broken one -
  // and it passes silently, because refusing an identical file is impossible.
  // Every one of these is built by string replacement on a fixture, so a
  // reworded fixture can quietly turn a sabotage into a no-op. This caught
  // exactly that when BASE's signature changed.
  if (before === after) { fail++; console.log(`FAIL  ${name}\n      the sabotage changed nothing - the fixture must have moved`); return; }
  const r = sameShape(before, after);
  if (r.ok) { fail++; console.log(`FAIL  ${name}\n      let it through`); return; }
  if (mustMention && !r.errors.some((e) => e.includes(mustMention))) {
    fail++; console.log(`FAIL  ${name}\n      refused for the wrong reason: ${r.errors.join(' | ')}`); return;
  }
  pass++; console.log(`ok    ${name}  (${r.errors[0]})`);
}

// The signature is NeetCode's stub - `Search`, `nums` and `target` are pinned.
// `l`, `r` and `m` are yours, and are what lint exists for.
const BASE = `public class Solution
{
    public int Search(int[] nums, int target)
    {
        int l = 0, r = nums.Length - 1;
        while (l <= r)
        {
            int m = l + (r - l) / 2;   // avoid overflow
            if (nums[m] == target) return m;
            if (target < nums[m]) r = m - 1; else l = m + 1;
        }
        return -1;
    }
}`;

const ren = (s, pairs) => pairs.reduce((acc, [a, b]) => acc.replace(new RegExp(`\\b${a}\\b`, 'g'), b), s);

// ---- rewrites that must be allowed -------------------------------------
accepts('identical file', BASE, BASE, []);

accepts('reindented only', BASE, BASE.replace(/\n    /g, '\n        '), []);

accepts('honest rename of the locals', BASE,
  ren(BASE, [['l', 'left'], ['r', 'right'], ['m', 'mid']]),
  ['l->left', 'r->right', 'm->mid']);

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
  'public class S { public int Count { get; set; } private int F(int q) { return q; } }',
  'public class S { public int Count { get; set; } private int F(int count) { return count; } }',
  ['q->count']);

accepts('renaming a variable whose type is a custom class',
  'public class S { public int F(TreeNode root) { TreeNode node = root; return node.val; } }',
  'public class S { public int F(TreeNode root) { TreeNode current = root; return current.val; } }',
  ['node->current']);

// ---- sabotage that must be caught --------------------------------------
refuses('flipped comparison', BASE, BASE.replace('l <= r', 'l >= r'), 'op changed');
refuses('shortened comparison', BASE, BASE.replace('l <= r', 'l < r'), 'token count changed');
refuses('changed literal', BASE, BASE.replace('return -1;', 'return -2;'), 'num changed');
refuses('dropped statement', BASE, BASE.replace('            if (nums[m] == target) return m;\n', ''), 'token count changed');
refuses('added statement', BASE, BASE.replace('return -1;', 'Console.WriteLine(l);\n        return -1;'), 'token count changed');
refuses('renamed a member after a dot', BASE, BASE.replace('nums.Length', 'nums.Count'), 'member name changed');
refuses('renamed the class', BASE, BASE.replace('class Solution', 'class BinarySearch'), 'type name changed');
refuses('two variables collapsed into one', BASE, ren(BASE, [['l', 'mid'], ['m', 'mid']]), 'both became');
refuses('deleted a comment', BASE, BASE.replace('   // avoid overflow', ''), 'comment(s) deleted');
refuses('renamed onto a contextual keyword', BASE, ren(BASE, [['l', 'value']]), 'contextual keyword');
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

// ---- the signature NeetCode gave you ----------------------------------

// The case that started this: two methods, one name, and the model was right
// about both - `root` really is the root in one and just a node in the other.
// Pinning it by name means there is nothing left to disagree about.
const TWO_METHODS = `public class Solution {
    public List<List<int>> LevelOrder(TreeNode root) {
        var res = new List<List<int>>();
        DFS(root, 0, res);
        return res;
    }
    private void DFS(TreeNode root, int level, List<List<int>> res) {
        if (root == null) return;
        DFS(root.left, level + 1, res);
    }
}`;

refuses('a public parameter is not renamed', BASE, ren(BASE, [['target', 'wanted']]), 'public signature');
refuses('nor a public parameter that is an array', BASE, ren(BASE, [['nums', 'values']]), 'public signature');
refuses('nor the public method itself', BASE, ren(BASE, [['Search', 'BinarySearch']]), 'public signature');
refuses('nor the helper copy of a public parameter name',
  TWO_METHODS, TWO_METHODS.replace('TreeNode root, int level', 'TreeNode node, int level'), 'public signature');

accepts('a private helper and its own parameters are yours',
  TWO_METHODS,
  ren(TWO_METHODS, [['DFS', 'Walk'], ['level', 'depth'], ['res', 'levels']]),
  ['DFS->Walk', 'level->depth', 'res->levels']);

// ---- scope: a local belongs to its method, a field to the whole file ------

// The real file lint refused twice on 2026-09-13. `size` is the running max in one method
// and one island's area in the other; giving them different names is the honest rename.
const MAX_AREA = `public class Solution {
    public int MaxAreaOfIsland(int[][] grid) {
        int size = 0;
        for (int row = 0; row < grid.Length; row++) {
            size = Math.Max(size, Dfs(row, 0, grid));
        }
        return size;
    }
    private int Dfs(int row, int col, int[][] grid) {
        int size = 1;
        size += Dfs(row + 1, col, grid);
        return size;
    }
}`;
const splitSize = (src) => {
  const cut = src.indexOf('private int Dfs');
  return ren(src.slice(0, cut), [['size', 'maxArea']]) + ren(src.slice(cut), [['size', 'areaCount']]);
};
accepts('the same local name in two methods may get two names', MAX_AREA, splitSize(MAX_AREA),
  ['size->maxArea', 'size->areaCount']);
refuses('but inside one method it still renames one way',
  MAX_AREA, MAX_AREA.replace('return size;\n    }\n    private', 'return best;\n    }\n    private'), 'renamed inconsistently');
refuses('a parameter renamed in the signature but not the body',
  MAX_AREA, MAX_AREA.replace('private int Dfs(int row,', 'private int Dfs(int r,'), 'renamed inconsistently');

const WITH_FIELD = `public class Solution {
    private int best = 0;
    public int MaxAreaOfIsland(int[][] grid) {
        Walk(grid);
        return best;
    }
    private void Walk(int[][] grid) {
        best = best + 1;
    }
}`;
refuses('a field is shared, so it renames the same way in every method',
  WITH_FIELD, WITH_FIELD.replace('return best;', 'return top;'), 'renamed inconsistently');
accepts('and renaming it everywhere is fine', WITH_FIELD, ren(WITH_FIELD, [['best', 'maxArea']]), ['best->maxArea']);
refuses('a local renamed onto a field name would shadow it',
  WITH_FIELD.replace('best = best + 1;', 'int step = 1; best = best + step;'),
  WITH_FIELD.replace('best = best + 1;', 'int best = 1; best = best + best;'));

// ---- shapeForm: the same solution, however it is written ---------------

function same(name, a, b, want = true) {
  const got = shapeForm(a) === shapeForm(b);
  if (got === want) { pass++; console.log(`ok    ${name}`); }
  else { fail++; console.log(`FAIL  ${name}\n      shapes ${got ? 'matched' : 'differed'}, expected ${want ? 'a match' : 'a difference'}`); }
}

same('renamed variables are the same solution', BASE, ren(BASE, [['l', 'left'], ['r', 'right'], ['m', 'mid']]));
same('recommented is the same solution', BASE, BASE.replace('// avoid overflow', '// NOTE: mid, computed safely'));
same('reindented is the same solution', BASE, BASE.replace(/\n    /g, '\n\t'));
same('a flipped comparison is NOT the same solution', BASE, BASE.replace('l <= r', 'l >= r'), false);
same('a different literal is NOT the same solution', BASE, BASE.replace('return -1;', 'return -2;'), false);
same('List and HashSet are NOT the same solution', CONTAINS, CONTAINS.split('List').join('HashSet'), false);
same('the same solution with only the helper renamed', TWO_METHODS, ren(TWO_METHODS, [['DFS', 'Walk'], ['res', 'levels']]));
same('a queue and a stack are NOT the same solution',
  'public class S { void F() { Queue<int> q = new Queue<int>(); q.Enqueue(1); } }',
  'public class S { void F() { Stack<int> q = new Stack<int>(); q.Push(1); } }', false);

// ---- locals that share a name with a member ------------------------------
// Every tree solution has locals called left and right sitting beside
// node.left and node.right. The member is correctly held fixed - but it was
// also being recorded in the rename map, so the local's legitimate rename read
// as one name mapping to two things. It rejected invert-a-binary-tree twice
// and left the file unlinted; a member and a local are different namespaces.
const TREE_SWAP = [
  'public class Solution {',
  '    public TreeNode InvertTree(TreeNode root) {',
  '        if (root == null) return null;',
  '        TreeNode left = InvertTree(root.left);',
  '        TreeNode right = InvertTree(root.right);',
  '        root.left = right;',
  '        root.right = left;',
  '        return root;',
  '    }',
  '}',
].join('\n');

const renamedLocals = TREE_SWAP
  .replace('TreeNode left = InvertTree(root.left);', 'TreeNode newLeft = InvertTree(root.left);')
  .replace('TreeNode right = InvertTree(root.right);', 'TreeNode newRight = InvertTree(root.right);')
  .replace('root.left = right;', 'root.left = newRight;')
  .replace('root.right = left;', 'root.right = newLeft;');

accepts('a local named left may be renamed beside a member called left', TREE_SWAP, renamedLocals);
refuses('but swapping which one is assigned is still a different program',
  TREE_SWAP, TREE_SWAP.replace('root.left = right;', 'root.left = left;'));
refuses('and the member itself still may not be renamed',
  TREE_SWAP, TREE_SWAP.replace(/root\.left/g, 'root.lft'));

// ---- the rejection has to say WHAT changed -----------------------------
//
// The real failure this came from: house-robber's submission-0 has a braceless
//
//     if(n == 1)
//     return nums[0];
//
// Haiku braced it, which is +2 tokens, and the refusal said only "token count
// changed: 145 before, 147 after". The retry got that same sentence back, made
// the same edit, and the file was retired permanently. A rejection that carries
// no new information is a wasted attempt - so these test the information.

const BRACELESS = `public class Solution
{
    public int Rob(int[] nums)
    {
        int n = nums.Length;
        if (n == 1)
            return nums[0];
        return n;
    }
}`;
const BRACED = BRACELESS.replace('        if (n == 1)\n            return nums[0];',
                                 '        if (n == 1)\n        {\n            return nums[0];\n        }');

// Still refused - the guard has not been loosened, only made articulate.
refuses('braces added to a braceless if', BRACELESS, BRACED, 'token count changed');
refuses('and the refusal names the braces', BRACELESS, BRACED, '"{"');
refuses('and says braces are not formatting', BRACELESS, BRACED, 'Braces are structure');
refuses('braces removed from a braced if', BRACED, BRACELESS, 'Braces are structure');

// The brace line is specific to braces. An unrelated size change must not
// collect a lecture about braces it can do nothing with.
function refusalHas(name, before, after, phrase, want) {
  const r = sameShape(before, after);
  const got = r.errors.some((e) => e.includes(phrase));
  if (got === want) { pass++; console.log(`ok    ${name}`); }
  else { fail++; console.log(`FAIL  ${name}\n      errors were: ${r.errors.join(' | ')}`); }
}
refusalHas('a dropped statement is not blamed on braces',
  BASE, BASE.replace('            if (nums[m] == target) return m;\n', ''), 'Braces are structure', false);
refusalHas('but it does say what went missing',
  BASE, BASE.replace('            if (nums[m] == target) return m;\n', ''), 'removed', true);
refusalHas('an added statement says what arrived',
  BASE, BASE.replace('return -1;', 'Console.WriteLine(l);\n        return -1;'), 'added', true);

// ---- tokenDelta on its own ---------------------------------------------
function delta(name, before, after, wantAdded, wantRemoved) {
  const A = tokenize(before).filter((t) => t.t !== 'ws' && t.t !== 'comment');
  const B = tokenize(after).filter((t) => t.t !== 'ws' && t.t !== 'comment');
  const d = tokenDelta(A, B);
  const got = `+[${d.added.join(' ')}] -[${d.removed.join(' ')}]`;
  const want = `+[${wantAdded.join(' ')}] -[${wantRemoved.join(' ')}]`;
  if (got === want) { pass++; console.log(`ok    ${name}`); }
  else { fail++; console.log(`FAIL  ${name}\n      got ${got}, wanted ${want}`); }
}
delta('an inserted brace pair', 'if (a) b();', 'if (a) { b(); }', ['{', '}'], []);
delta('a removed brace pair', 'if (a) { b(); }', 'if (a) b();', [], ['{', '}']);
delta('an identical file has no delta', BASE, BASE, [], []);
delta('a pure rename has no delta either - it is length-equal', 'int l = 0;', 'int left = 0;', ['left'], ['l']);

console.log(`\n${pass} passed, ${fail} failed.`);
process.exit(fail ? 1 : 0);
