// ##########################################################################
// #  optimal.cs            O(n) time / O(n) space
// ##########################################################################

public class Solution
{
    public bool IsSameTree(TreeNode first, TreeNode second)
    {
        //my solution.
        if (first == null && second == null)
        {
            return true;
        }
        if (first != null && second != null && first.val == second.val)
        {
            return (IsSameTree(first.left, second.left) && IsSameTree(first.right, second.right));
        }
        return false;
    }
}

/*
================================================================================
 PROBLEM : You get the roots of two binary trees, p and q (here: first,
           second). Return true if they have the same shape and the same value
           in every node. Empty trees count as equal. Shape matters, not just
           the values. Example: [1,2,3] and [1,2,3] -> true; [1,2] and
           [1,null,2] -> false.
 PATTERN : Tree DFS (recursive, two trees in lockstep)
================================================================================
IDEA
  Walk both trees at the same time and compare the node pairs.
  If first and second are both null, this pair matches.
  If both exist and first.val == second.val, both left pairs and both right
  pairs must match too. In any other case (one null, or values differ),
  return false. This is correct because two trees are equal exactly when
  their roots match and both subtree pairs are equal.
EXAMPLE
  first = [1,2], second = [1,null,2] (same values, different shape)
  (1,1): both exist, vals equal -> check left pair (2,null)
  (2,null): only one is null -> false; && skips the right pair
  Answer: false
COMPLEXITY
  Time  O(n)  each node pair is compared at most once; stops at first mismatch
  Space O(n)  recursion stack is tree height, n deep for a skewed tree
PATH TO OPTIMAL
  Serialize both trees (preorder with null markers), compare strings - O(n)
  time, O(n) space - correct, but builds two full strings before comparing.
  Recursive DFS (this file) - O(n) time, O(h) stack - no extra strings, and
  it exits early. The variant files are the same-cost alternatives to compare.
KEYWORDS
  binary tree, same tree, DFS, recursion, tree equality, preorder, BFS
WATCH OUT
  - Check for null before reading .val. This code is safe only because it
    tests first != null && second != null before first.val.
  - Comparing only value lists (inorder, or preorder without nulls) fails:
    [1,2] and [1,null,2] give the same values but have different shapes.
  - Use && between the two subtree calls, not ||. With ||, one matching side
    would wrongly return true.
  - A very deep, skewed tree can cause a stack overflow in C# recursion.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How do you avoid recursion for very deep trees?
     -> Push (first, second) pairs onto an explicit stack or queue and apply
        the same three checks. Still O(n) time and O(n) space, with no call
        overflow.
  2. Is q a subtree of p (Subtree of Another Tree)?
     -> Run IsSameTree from every node of p: O(m*n). Or serialize both trees
        with null markers and use KMP to find the match: O(m+n) time, more code.
  3. Check whether one tree is symmetric?
     -> Use the same lockstep DFS on (left, right), but compare a.left with
        b.right and a.right with b.left. Same O(n) time and O(h) space.
TRIGGER
  Two trees must be compared node by node: recurse on both roots in lockstep.
================================================================================
*/
