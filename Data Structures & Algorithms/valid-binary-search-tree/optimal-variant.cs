// --------------------------------------------------------------------------
// -  optimal-variant.cs    O(n) time / O(n) space
// --------------------------------------------------------------------------

public class Solution
{
    public bool IsValidBST(TreeNode root)
    {
        return Validate(root, long.MinValue, long.MaxValue);
    }

    private bool Validate(TreeNode node, long lower, long upper)
    {
        if (node == null)
            return true;
        if (node.val <= lower || node.val >= upper)
            return false;

        return Validate(node.left, lower, node.val) &&
               Validate(node.right, node.val, upper);
    }
}

/*
================================================================================
 PROBLEM : Given the root of a binary tree, return true if it is a valid
           binary search tree. Every node in the left subtree must be strictly
           less than the node. Every node in the right subtree must be
           strictly greater. Equal is invalid. Example: [2,1,3] -> true,
           [5,1,4,null,null,3,6] -> false.
 PATTERN : DFS (preorder) with min/max bounds
================================================================================
IDEA
  Each node must lie inside an open range (lower, upper) set by its ancestors.
  Validate starts at the root with range (long.MinValue, long.MaxValue).
  Going left, node.val becomes the new upper. Going right, it becomes the new
  lower. This is correct because the range holds every rule from every
  ancestor, not only the parent. So a deep node that breaks a rule set far
  above it is still caught.
EXAMPLE
  Tree [5,4,6,null,null,3,7]. Here 3 is the left child of 6.
  5 in (-inf,inf) ok -> 4 in (-inf,5) ok -> 6 in (5,inf) ok
  3 in (5,6): 3 <= lower 5 -> false. The && short-circuits, so 7 is skipped.
  Answer: false. A parent-only check would wrongly pass, since 3 < 6.
COMPLEXITY
  Time  O(n)  each node is checked once, with O(1) work per node
  Space O(n)  recursion stack depth equals tree height, which is n for a
              skewed tree
WATCH OUT
  - Comparing a node only with its parent is the classic bug. It misses the
    3-under-6 case in the example above.
  - Using int bounds (int.MinValue/MaxValue) fails when a node equals them.
    This code uses long to avoid that, so keep the long parameters.
  - Use strict checks: <= lower and >= upper. Duplicates must return false,
    for example [2,2] -> false.
  - A very deep, skewed tree can overflow the C# call stack. The fix is an
    explicit stack of (node, lower, upper).
================================================================================
*/
