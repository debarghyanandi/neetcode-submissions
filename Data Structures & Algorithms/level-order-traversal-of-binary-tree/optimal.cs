// ##########################################################################
// #  optimal.cs            O(n) time / O(n) space
// #  DFS recursion carrying depth as level index   [dfs-track-level]
// #  ties with optimal-variant.cs on O(n) time / O(n) space
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  Preorder DFS visits each node once, appending into res[level] using
// #  the depth parameter; recursion stack depth is O(h) which is O(n) worst
// #  case for a skewed tree.
// ##########################################################################

public class Solution
{
    private List<List<int>> res = new List<List<int>>();

    public List<List<int>> LevelOrder(TreeNode root)
    {
        //My Solution
        if (root == null)
            return res;

        Traverse(root, 0);
        return res;
    }

    private void Traverse(TreeNode root, int level)
    {
        if (root == null)
            return;
        // When we first reach a new level, level == res.Count.
        // For example, at the first node of level 1, both are 1,
        // so we create a new list for that level.

        // When we reach the next node at the same level,
        // level is still 1, but res.Count is now 2 because
        // the list for level 1 was already created.
        if (res.Count == level)
            res.Add(new List<int>());

        res[level].Add(root.val);

        Traverse(root.left, level + 1);
        Traverse(root.right, level + 1);
    }
}

/*
================================================================================
 PATTERN : DFS preorder, res.Count as the new-level marker
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-0.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  The output is grouped by depth, which normally says "BFS with a queue,
  snapshot the queue size once per level." This file skips the queue entirely by
  carrying the depth as an explicit parameter: Traverse(root, 0) at the top,
  level + 1 on both recursive calls. Once every node knows its own depth,
  grouping is just an index into res - no queue, no per-level count, no null
  sentinel between levels. Remember the trick, not the problem: any "bucket the
  nodes by depth" question collapses into a plain DFS the moment you pass the
  depth down.
CORRECTNESS - WHY DFS PRODUCES LEFT-TO-RIGHT ORDER
  The non-obvious part is that res[level] comes out in the correct left-to-right
  order even though the traversal never visits a level contiguously. Take two
  nodes u and v on the same level with u to the left of v. Their lowest common
  ancestor a has u somewhere in a.left's subtree and v somewhere in a.right's
  subtree (they cannot share a side, or a would not be lowest). Traverse(a.left,
  ...) runs to completion before Traverse(a.right, ...) is ever called, so
  root.val for u is appended to res[level] before v's. That holds for every such
  pair, so each inner list ends up in exactly the order BFS would have produced.
INVARIANT
  res.Count is always exactly the number of distinct levels discovered so far,
  and every call to Traverse is entered with level <= res.Count.

  It holds inductively: the root enters with level 0 and res.Count 0. Any child
  is only reached after its parent has already run the res.Add guard and the
  res[level].Add, so at that moment res.Count >= parentLevel + 1 = childLevel.
  Two things fall out. First, res.Count == level is a complete and exact test
  for "this is the first node of a level I have never seen" - it can never be a
  false positive, because level can never exceed res.Count. Second, res[level]
  is guaranteed in range on the very next line, so no bounds check or
  ContainsKey is needed.
ALGORITHM
  1. If root is null, return res (empty).
  2. Call Traverse(root, 0).
  3. In Traverse: return immediately on a null node.
  4. If res.Count == level, this is the first node seen at this depth - append a
  fresh List<int> to res, which makes res.Count equal level + 1.
  5. Append root.val to res[level].
  6. Recurse into root.left with level + 1, then root.right with level + 1. Left
  before right is load-bearing, see the correctness argument.
  7. Return res.
WATCH OUT
  res is an instance field, not a local. Calling LevelOrder twice on the same
  Solution object appends the second tree's nodes onto the first tree's lists,
  and the null-root early return hands back whatever the previous call left
  behind instead of an empty list. The judge constructs a fresh Solution per
  test case so it passes, but an interviewer will ask about reuse. The fix is to
  declare List<List<int>> res inside LevelOrder and pass it into Traverse, or
  reassign res = new List<List<int>>() as the first statement of LevelOrder.

  Also: the root == null check in LevelOrder is dead weight. Traverse already
  returns on a null node, so the guard only skips a call that would have done
  nothing.

  And the real difference from the BFS version - this recurses to the depth of
  the tree. A degenerate 100k-node chain blows the call stack here; the queue
  version does not care.
INTERVIEWER FOLLOW-UPS
  "Do it iteratively" - queue, take int count = queue.Count at the top of each
  outer iteration, drain exactly that many nodes into one list, enqueue their
  non-null children.

  "Bottom-up level order" - res.Reverse() at the end, nothing else changes.

  "Zigzag" - with this DFS you still fill res the same way, then reverse every
  list at an odd index; the depth parameter is what makes that a one-liner.

  "Right side view" - keep the level parameter but recurse right before left and
  append only when res.Count == level, so the first node reached at each depth
  wins.

  "Why not a Dictionary<int, List<int>>" - it would work, but the levels are
  dense 0..h with no gaps and are discovered in increasing order, so a List
  indexed by level is strictly simpler and needs no final sort by key.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
