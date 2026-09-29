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
 PATTERN : DFS with bounds - pass a valid (lower, upper) range down
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-3.cs when it was first processed
 STATUS  : Optimal variant - ties the best complexity by another route
================================================================================
VARIABLES
  lower    every value in this subtree must be greater than lower
  upper    every value in this subtree must be less than upper
WHY THIS PATTERN
  A BST (binary search tree) rule is not only about a parent and its child.
  Every node in the left subtree must be smaller than the ancestor, and every
  node in the right subtree must be larger. So each node has an allowed open
  range that its ancestors decide. Validate carries that range as lower and
  upper. Going left, node.val becomes the new upper. Going right, node.val
  becomes the new lower.
BRUTE FORCE
  For each node, walk its whole left subtree to find the max and its whole right
  subtree to find the min. Then check max < node.val < min. This is correct, but
  it visits each subtree again for every ancestor. That costs O(n^2) time on a
  skewed tree (a tree shaped like a long chain), while here each node is checked
  once.
INVARIANT
  When Validate(node, lower, upper) is called, lower and upper are the tightest
  bounds set by all ancestors of node. If every node falls strictly inside its
  range, then every left descendant is less than each ancestor it hangs under,
  and every right descendant is greater. That is exactly the BST definition. The
  && also stops the search at the first node that breaks the rule.
LONG SENTINELS FOR INT VALUES
  node.val is an int, but the bounds are long, and they start at long.MinValue
  and long.MaxValue. So a node that holds int.MinValue or int.MaxValue is still
  strictly inside the starting range. If you used int.MinValue as the sentinel
  (a special start value), then a single node with val = int.MinValue would fail
  the <= check and the code would return false by mistake.
WATCH OUT
  The checks use <= and >=. This means duplicate values make the tree invalid.
  That is right only if the problem defines a BST with strictly smaller left and
  strictly larger right values. If duplicates are allowed on one side, you must
  loosen one side of the check. The recursion depth equals the tree height. So a
  very deep, skewed tree can overflow the call stack.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you solve it without passing bounds?
     Yes. Do an in-order traversal (left, node, right) and check that each value
     is strictly greater than the one before it. You keep only one prev
     variable. The cost is the same, but you must track state across calls, or
     use an explicit stack.
  2. How do you avoid recursion?
     Use an explicit Stack of (node, lower, upper) tuples, or an iterative
     in-order walk with a stack. This removes the call-stack limit. The extra
     memory still grows with the tree height, but it lives on the heap.
  3. Can you do it in O(1) extra space?
     Morris in-order traversal links each node's in-order predecessor back to it
     for a short time, so no stack is needed. The downside is that it changes
     the tree while it runs and must undo those links. It is also harder to get
     right.
  4. What if the values were long, or any comparable type?
     A long sentinel no longer works. Use nullable bounds (long? or TreeNode
     references), where null means "no limit on this side".
TRIGGER
  Reach for this when a node's validity depends on all of its ancestors, not
  only its parent. Then pass the limits down as parameters.
C# NOTE
  node.val is compared to long lower and upper, so C# implicitly widens the int
  to long with no cast. Passing node.val as the new bound widens it the same
  way. This is why the sentinel trick needs no extra code.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
