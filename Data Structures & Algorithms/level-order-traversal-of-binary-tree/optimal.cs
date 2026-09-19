// ##########################################################################
// #  optimal.cs            O(n) time / O(n) space
// #  Recursive DFS with implicit level tracking   [dfs-implicit-levels]
// #  ties with optimal-variant.cs on O(n) time / O(n) space
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  Recursion depth becomes the tree height; worst-case call stack is O(n)
// #  for a skewed tree.
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
 PATTERN : DFS preorder with depth index - build level buckets
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-0.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  res      res[level] = all node values at that depth, left to right
  level    depth of the current node, root is 0
WHY THIS PATTERN
  The problem asks for values grouped by depth, so every node needs to know its
  own depth and write into the bucket for that depth. Passing `level + 1` down
  the recursion gives each node its depth for free, and `res[level].Add` puts
  the value in the right group. A queue-based BFS is the usual answer, but any
  traversal works as long as nodes at the same depth are visited left before
  right - preorder does that, because the whole left subtree is walked before
  the right one.
BRUTE FORCE
  The first thing most people write is: compute the height, then for each depth
  d from 0 to height-1, walk the whole tree again and collect only the nodes at
  depth d. That is O(n) per level and O(n * h) total, which degrades to O(n^2)
  on a skewed tree. This file touches every node exactly once instead.
INVARIANT
  Before visiting a node at depth `level`, `res` already holds a list for every
  depth from 0 up to `level - 1`, so `res.Count` is either `level` (this is the
  first node seen at this depth) or greater. That is why `res.Count == level` is
  a safe test for "create the bucket now" - depths are always reached in
  increasing order, never skipped. Because the left child is visited before the
  right child, values are appended to each bucket in left-to-right order, which
  is exactly level order.
WATCH OUT
  `res` is an instance field, not a local. If the same `Solution` object is used
  for two trees, the second call appends to the first call's lists and returns
  both mixed together. The early `return res` for a null root has the same
  problem: it returns whatever was left from before, not an empty list.
  Recursion depth equals the tree height, so a long chain of left children (a
  degenerate tree) can overflow the stack. The method also hands the caller its
  own private list, so the caller can mutate the solver's state.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How would you remove the recursion?
     Use a queue: push the root, then on each round record `queue.Count` as the
     size of the current level, pop exactly that many nodes into one list and
     push their children. Same O(n) time, and the memory becomes heap queue
     space instead of call-stack frames, which survives a skewed tree.
  2. What changes for zigzag level order (left-to-right, then right-to-left)?
     Keep this traversal unchanged and reverse `res[i]` for every odd `i` at the
     end, or insert at the front of the bucket on odd levels. Reversing at the
     end is O(n) total; front-insertion into a List is O(n) per insert, so
     prefer the reverse or use a LinkedList.
  3. What if you only need the rightmost node of each level (right side view)?
     Recurse right child first, and only write when `res.Count == level`, so the
     first node seen at each depth is the rightmost. That drops the per-level
     lists and the space becomes O(h) for the stack plus O(h) for the answer.
  4. The tree is huge and you only need level sums, not the values.
     Replace `List<List<int>>` with a `List<long>` of sums and do `res[level] +=
     root.val`, growing with `res.Add(0)` on a new level. Space falls from O(n)
     to O(h).
TRIGGER
  The output must be grouped by distance from the root, or by "layer" - hand the
  depth down the recursion and index a list of buckets by it.
C# NOTE
  Make `res` a local inside `LevelOrder` and pass it as a parameter to
  `Traverse`; `List` is a reference type, so the helper still appends to the
  same list, and the stale-state bug disappears with no copying cost.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
