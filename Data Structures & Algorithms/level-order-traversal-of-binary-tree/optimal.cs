// ##########################################################################
// #  optimal.cs            O(n) time / O(n) space
// #  DFS recursion tracking depth/level   [dfs-track-level]
// #  ties with optimal-variant.cs on O(n) time / O(n) space
// #
// #  YOU SOLVED THIS YOURSELF (from submission-0)
// #
// #  Visits each node once via preorder DFS, appending to the list for its
// #  level; recursion stack depth is O(h) which is O(n) worst case for a
// #  skewed tree.
// ##########################################################################

public class Solution
{
    private List<List<int>> res = new List<List<int>>();

    public List<List<int>> LevelOrder(TreeNode root)
    {
        //My Solution
        if (root == null)
            return res;

        DFS(root, 0);
        return res;
    }

    private void DFS(TreeNode root, int level)
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

        DFS(root.left, level + 1);
        DFS(root.right, level + 1);
    }
}

/*
================================================================================
 PATTERN : DFS preorder + depth index - new row on first visit
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-0.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  Level order does not require a queue. It requires that every value land in the
  bucket for its depth, and that within a bucket the values sit left to right.
  DFS gets both if you carry the depth down as the level parameter and let res
  double as the bucket list. The line that makes it work is the res.Count ==
  level test: it is the "have I ever been this deep before" question, answered
  without a visited set, a depth counter, or a pre-pass to measure height.
INVARIANT
  At every entry into DFS, level <= res.Count.

  It holds at the root: level 0, res empty, 0 <= 0. It is preserved because
  level only ever grows by exactly 1 per recursive call, and the moment level
  equals res.Count the code appends one list, restoring level < res.Count before
  res[level] is touched. So res.Count is exactly the number of distinct depths
  already reached, and res[level] can never throw an out-of-range. Note the
  check is == and not >=; anything else would be papering over a broken
  invariant rather than relying on it.
CORRECTNESS - WHY THE ROWS COME OUT LEFT TO RIGHT
  This is the follow-up an interviewer will actually ask, because DFS visiting
  order looks nothing like level order.

  Take two nodes u and v at the same depth, with u to the left of v. Let a be
  their lowest common ancestor. Since they are distinct and equally deep,
  neither is an ancestor of the other, so u sits in a.left's subtree and v sits
  in a.right's subtree. DFS(a) runs DFS(a.left, ...) to completion before
  DFS(a.right, ...), so u's Add fires before v's Add. Both Adds target the same
  res[level] and List.Add appends. Therefore the row is built in left-to-right
  order. The preorder-vs-inorder choice does not matter for the row contents -
  only that left recurses before right.
THE TRAP
  res is an instance field, not a local. Nothing clears it. Call LevelOrder
  twice on the same Solution object and the second call appends onto the first
  result's rows - a genuine bug that a judge harness hides because it constructs
  a fresh Solution per test case. The null-root branch makes it visible: it
  returns the same shared, possibly non-empty list. The fix is to make res a
  local passed into DFS, or to assign res = new List<List<int>>() at the top of
  LevelOrder.

  Second, smaller point: the null guard in LevelOrder is redundant with the one
  at the top of DFS. It is harmless, but if you keep it, keep it for the early
  return, not for safety.
WATCH OUT - THE SPACE IS RECURSION, NOT JUST OUTPUT
  The output alone is n ints spread across the rows. The extra cost is the call
  stack, which is one frame per level of depth: fine on a balanced tree, but a
  fully skewed tree (every node has only a left child) puts n frames on the
  stack and can actually overflow it in C#, where you cannot raise the limit
  from inside the method. The BFS-with-a-Queue version has the same asymptotic
  space (the widest level can hold about n/2 nodes) but spends it on the heap
  instead of the stack. That is the real trade you are making here, and it is
  the honest answer if asked which version you would ship.
WHAT THIS UNLOCKS
  The res.Count == level idiom generalizes to any "first node encountered at
  each depth" problem, and those are common follow-ups:

  1. Right side view: recurse right before left and, instead of appending, only
  act when res.Count == level - the first node reached at each depth is then the
  rightmost one.
  2. Max depth: res.Count after the traversal is the height.
  3. Level sums or averages: replace the row list with a running total indexed
  by level.

  Each is the same skeleton with the Add line swapped, which is why it is worth
  remembering as a shape rather than as this one problem's answer.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
