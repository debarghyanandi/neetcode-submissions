// ##########################################################################
// #  optimal.cs            O(n) time / O(n) space
// #  DFS recursion, pre-order traversal   [dfs-recursion]
// #  ties with optimal-variant.cs on O(n) time / O(n) space
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  Each node visited once; space bounded by call stack depth, worst case
// #  O(n) in skewed tree.
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
  res      res[level] = values of all nodes at that depth, left to right
  level    depth of the current node, 0 for root
WHY THIS PATTERN
  The task asks for node values grouped by depth, so every node needs one label:
  its distance from the root. That label is easy to carry down a recursive call
  - each child gets level + 1 - so a plain depth-first walk can drop each value
  into the right bucket. The grouping work is done by res, not by the traversal
  order, so breadth-first order is not required.
BRUTE FORCE
  The first thing most people write is: measure the tree height, then for each
  depth d from 0 to height, walk the whole tree again and collect nodes whose
  depth equals d. That is O(n * h) time, and on a skewed tree h is n, so it
  becomes quadratic. This file visits each node exactly once instead.
INVARIANT
  When Traverse is entered for a node at depth level, res already holds exactly
  one list for every depth strictly less than level that has been reached, so
  res.Count is either level (this node is the first one seen at its depth) or
  greater (a list is already there). That makes the single check res.Count ==
  level a correct "new level" test without any extra bookkeeping. Because the
  recursion always does left before right, values arrive in each res[level] in
  left-to-right order.
WHY PREORDER STILL GIVES LEFT-TO-RIGHT
  Depth-first does not visit a level in one sweep, yet the output per level is
  still correct. For two nodes at the same depth, the one in the left subtree of
  their lowest common ancestor is always reached first, because
  Traverse(root.left, ...) runs before Traverse(root.right, ...). Appending to
  the end of res[level] therefore preserves that order.
WATCH OUT
  res is an instance field, not a local. If the same Solution object is used for
  a second tree, the old results are still in res and the new values get
  appended to them. The root == null path also returns that shared list rather
  than a fresh empty one. And the recursion depth equals the tree height, so a
  long chain of left children can overflow the call stack.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Rewrite it without recursion.
     Use a Queue<TreeNode>, and at each outer step record count = queue.Count,
     then pop exactly count nodes into one new list while pushing their
     children. Same O(n) time, and the stack depth problem disappears, but peak
     memory is now the widest level instead of the height.
  2. Return the levels bottom-up (deepest first).
     Keep this exact traversal, then call res.Reverse() at the end, or insert
     each new level list at index 0. Reverse is O(n) and cheap; inserting at
     index 0 shifts the list every time.
  3. Zigzag order - left to right on even levels, right to left on odd ones.
     Keep building res the same way, then reverse res[level] for every odd level
     at the end. Do not flip the recursion order, since that would break the
     invariant above for the even levels too.
  4. Only the rightmost node of each level (right side view).
     Swap the two recursive calls so right runs first, and push root.val only
     when res.Count == level. Each level then keeps just its first-seen node,
     which is the rightmost one.
TRIGGER
  The output must be grouped by depth or distance from a start node, and each
  element belongs to exactly one group.
C# NOTE
  LeetCode's signature for this problem is normally IList<IList<int>>; returning
  the concrete List<List<int>> works only if the harness matches it, and the two
  types are not interchangeable in C# because generics are invariant here.
  Moving res out of the field and into a local passed to Traverse would remove
  the reuse problem at no cost.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
