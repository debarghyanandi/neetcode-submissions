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
 PATTERN : Tree DFS - match whole subtree at every node
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-0.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  root      current node of the big tree being tested as a match start
  subRoot   the pattern tree; never changes during the search
  first     node from the big tree in the pairwise identity walk
  second    node from the pattern tree, walked in lockstep with first
WHY THIS PATTERN
  The question asks whether the pattern tree appears somewhere inside the big
  tree as a complete subtree, and a subtree is identified by the node it hangs
  from. So there are only as many candidate positions as there are nodes, and
  each candidate is a yes/no full-equality test. IsSubtree walks every node of
  root to pick candidates; IsSameTree does the strict shape-and-value comparison
  of first against second. The "or" of root.left and root.right means one match
  anywhere is enough.
BRUTE FORCE
  The first idea most people write is exactly this: try every node, compare
  fully. There is no simpler correct version to fall back to, so the honest
  brute force is the same shape with a wasteful comparison, for example
  serializing the subtree at every node into a string and comparing strings,
  which costs extra memory per node and still visits every pair. This file
  avoids that by comparing nodes directly and stopping at the first mismatch.
INVARIANT
  When IsSubtree is called on a node, every ancestor of that node has already
  failed its own IsSameTree test, so any match must lie in the current node or
  below it. IsSameTree(first, second) returns true only when both trees end at
  exactly the same places and every paired value is equal, so it can never
  accept a pattern that is only a prefix of a branch. Since IsSubtree tests
  every node once, if a match exists it is reached and reported.
THE NULL GUARD IS LOAD-BEARING
  The first if is what makes the recursion safe. If root is null and subRoot is
  null, IsSameTree(null, null) returns true and the method returns before
  touching root.left. If root is null and subRoot is not, the guard returns
  false. Remove that guard and a null root reaches root.left and throws a
  NullReferenceException.
WATCH OUT
  A null subRoot makes the method return true for any tree, including a null
  root, because IsSameTree(null, null) is true. That may not be what the problem
  wants, and it is worth stating your assumption out loud. Duplicate values are
  handled correctly only because the code keeps searching with the "or" after a
  failed IsSameTree, not because the search stops at the first equal value. The
  else after a return is dead weight; it reads as if there were two branches
  when there is only one path left.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you do better than testing every node?
     Serialize both trees with explicit null markers into strings, then run KMP
     or Z-algorithm to look for the pattern string inside the tree string. That
     drops the time to linear in the two sizes, but costs linear extra memory
     for the two strings and needs careful separators so that values like 1 and
     12 cannot blur together.
  2. Remove the recursion.
     Replace the outer walk with an explicit Stack<TreeNode> pushing left and
     right, and make IsSameTree iterative with a stack of node pairs. Same work,
     but the depth is bounded by heap memory instead of the call stack, which
     matters for a long skewed chain.
  3. Instead of yes/no, count how many nodes start a matching subtree.
     Keep the same walk but replace the short-circuit "or" with a sum, so both
     children are always explored and no match is skipped after the first one.
  4. What if only the pattern's shape must match, values ignored?
     Drop the first.val == second.val test from IsSameTree and keep the null
     checks; the outer search is unchanged.
TRIGGER
  When a problem asks whether one tree occurs inside another as a complete
  subtree, pair a node-by-node search with a strict whole-tree equality check.
C# NOTE
  IsSameTree uses no instance state and is not part of the required API, so it
  can be private static; leaving it public widens the class surface for no
  reason. The explicit first != null && second != null test is doing the work
  that ?. cannot do here, since you need both sides checked before reading .val.
COMPLEXITY
  Time  : O(n * m)
  Space : O(n + m)
================================================================================
*/
