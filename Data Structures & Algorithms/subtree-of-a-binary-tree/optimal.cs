// ##########################################################################
// #  optimal.cs            O(n * m) time / O(n + m) space
// #  Recursive DFS tree comparison   [recursive-dfs-subtree-check]
// #  the only solution in this folder
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  Each of m nodes in root triggers IsSameTree comparison on n nodes of
// #  subRoot; call stack accumulates height of both trees.
// ##########################################################################

public class Solution
{
    //My solution 
    public bool IsSubtree(TreeNode root, TreeNode subRoot)
    {
        if (root == null && subRoot != null)
            return false;

        if (IsSameTree(root, subRoot))
            return true;

        else
        {
            return IsSubtree(root.left, subRoot) || IsSubtree(root.right, subRoot);
        }
    }

    public bool IsSameTree(TreeNode first, TreeNode second)
    {
        if (first == null && second == null)
            return true;

        if (first != null && second != null && first.val == second.val)
            return IsSameTree(first.left, second.left) && IsSameTree(first.right, second.right);
        return false;
    }
}

/*
================================================================================
 PATTERN : Tree DFS - check a same-tree match at every node
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-0.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  first     the current node in the big tree (starts at a candidate root)
  second    the matching node in subRoot, walked at the same position
WHY THIS PATTERN
  The problem asks whether some node in root starts a copy of subRoot. That copy
  must match in both values and shape. So there are two questions. Where can the
  copy start? Any node, so IsSubtree visits every node with DFS (depth-first
  search: go deep down one branch before trying the next). Does it match from
  this node? IsSameTree walks first and second together and answers that.
BRUTE FORCE
  This file is already the simplest correct approach, so here is a different
  correct one. Collect every node of root into a list. Then call a same-tree
  check on each one. The work is the same, but it needs an extra list of n nodes
  and it never stops early. The recursive version returns as soon as || finds a
  match, and it keeps no list.
INVARIANT
  IsSameTree(first, second) is true only when both trees have the same shape and
  the same values at every position. That includes where the nulls are. So a
  match that stops before the leaves is rejected, for example when first still
  has children below the point where second ends. IsSubtree(root, subRoot) is
  true only if the match starts at root itself or somewhere in root.left or
  root.right. These are all the possible start points, so the answer is complete
  and correct.
WATCH OUT
  If subRoot is null, the code returns true. The recursion keeps going down
  until root is null, and then IsSameTree(null, null) returns true. That is fine
  only if the problem says an empty tree counts as a subtree. The first guard
  only handles the case where root is null and subRoot is not. If you delete it,
  root.left throws a NullReferenceException. Both functions use recursion, so a
  very deep, skewed tree (every node has only one child) can overflow the call
  stack.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you do it in linear time?
     Turn both trees into strings with a preorder walk. Write a marker for each
     null child and put separators around each value, so "2" cannot match inside
     "12". Then find subRoot's string inside root's string with KMP (a string
     search that never goes backward). This takes O(n + m) time. The cost is O(n
     + m) extra memory for the strings, and the code is harder to get right.
  2. How do you avoid recursion?
     Walk root with an explicit Stack<TreeNode>. For each candidate node,
     compare the two trees with a second stack that holds pairs of nodes. The
     logic stays the same and deep trees no longer overflow the call stack. The
     code gets longer.
  3. What if you must count how many times subRoot appears?
     Remove the early return. Add 1 each time IsSameTree matches, and search
     both children every time. Another option is to give each subtree a hash
     built from its children's hashes (a Merkle hash). Then compare hashes, and
     confirm each hash match with a real comparison.
TRIGGER
  The question asks whether one tree appears exactly inside another. So you try
  every node as a start point and run a same-tree check from it.
C# NOTE
  You can make IsSameTree shorter with one line: if (first is null || second is
  null) return first == second;. This handles every null case at once. After
  that line, both nodes are known to be non-null, so you only compare val and
  then recurse.
COMPLEXITY
  Time  : O(n * m)
  Space : O(n + m)
================================================================================
*/
