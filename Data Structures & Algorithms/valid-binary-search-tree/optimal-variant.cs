// --------------------------------------------------------------------------
// -  optimal-variant.cs    O(n) time / O(n) space
// -  recursive bounds validation with parameters   [recursive-bounds-check]
// -  ties with optimal.cs on O(n) time / O(n) space
// -
// -  Reference solution - not one you solved yourself
// -
// -  Each node visited once; recursion depth O(n) in worst case.
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
 PATTERN : DFS with inherited range bounds - validate against (lower, upper)
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-3.cs when it was first processed
 STATUS  : Optimal variant - ties the best complexity by another route
================================================================================
VARIABLES
  lower    exclusive lower bound every value in this subtree must beat
  upper    exclusive upper bound every value in this subtree must stay under
  node     current subtree root being checked
WHY THIS PATTERN
  A BST is not defined by a local rule. It is not enough that node.left.val <
  node.val; every value in the whole left subtree must be smaller. That "whole
  subtree" condition is exactly a range, so each call carries the open interval
  (lower, upper) that the current subtree is allowed to live in. Going left
  tightens upper to node.val, going right tightens lower to node.val, and one
  comparison per node then covers all ancestor constraints at once.
BRUTE FORCE
  The first idea most people write is to check, for each node, the maximum of
  its left subtree and the minimum of its right subtree by walking those
  subtrees again. That is correct but costs O(n) work per node, so O(n^2) on a
  skewed tree. The range-passing version pushes the same information downward
  instead of pulling it upward, so each node is touched once.
INVARIANT
  When Validate(node, lower, upper) is called, every ancestor constraint on this
  subtree has already been folded into the pair (lower, upper): a value is legal
  here if and only if lower < val < upper. The two strict checks reject node.val
  immediately, and the recursive calls preserve the invariant because the left
  child inherits (lower, node.val) and the right child inherits (node.val,
  upper). By induction, if the root call returns true then every node satisfied
  all of its ancestors, which is the BST definition.
WHY LONG AND NOT INT
  The bounds are long, not int, only because the sentinels long.MinValue and
  long.MaxValue must sit strictly outside the range of node.val. If lower and
  upper were int, a root holding int.MinValue would fail node.val <= lower
  against the int.MinValue sentinel and a valid tree would be rejected. The
  alternative is nullable int? bounds with an explicit null means "no bound"
  check, which costs more branches but no widening.
WATCH OUT
  The comparisons are strict on both sides, so equal values are rejected
  everywhere, including a left child equal to its parent. That matches the usual
  "strictly less / strictly greater" definition, but if the problem allows
  duplicates on one side the check has to loosen to < or > on that side only.
  Depth is recursion depth: a fully skewed tree of n nodes makes n nested frames
  and can overflow the call stack, and there is no iterative fallback here. The
  && between the two recursive calls short-circuits, so a failure in the left
  subtree skips the right one entirely - fine for a boolean answer, but wrong if
  you later add counting or side effects inside Validate.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Remove the recursion - how?
     Do an iterative inorder traversal with an explicit Stack<TreeNode>, keeping
     prev as the last visited value and returning false if prev >= current. Same
     O(n) time, and the stack is now heap memory so a deep tree no longer kills
     the call stack.
  2. Can you get it down to O(1) extra space?
     Morris inorder traversal: temporarily rewire the rightmost node of each
     left subtree to point back to the current node, then undo the link on the
     way through. No stack and no recursion, but it mutates the tree during the
     walk, which is unacceptable if the tree is shared across threads.
  3. Instead of a yes/no, return the size of the largest BST subtree.
     Switch from top-down bounds to bottom-up: each call returns (isBst, min,
     max, size) for its subtree and the parent combines the children. Still
     O(n), but you can no longer short-circuit, since both children must always
     be visited.
  4. The tree is huge and stored on disk, one node per page.
     Prefer the inorder-with-prev form, since it reads nodes in sorted order and
     touches each exactly once, which is friendlier to sequential access than
     the two-sided range recursion.
TRIGGER
  A tree condition that talks about an entire subtree rather than a parent and
  its direct child - push a min/max window down the recursion instead of
  recomputing subtree extremes.
C# NOTE
  node.val is int and the bounds are long, so node.val <= lower silently widens
  the int to long at each comparison; that widening is the whole reason the
  sentinel trick is safe, and it is worth saying out loud rather than leaving it
  as an accident of the signature.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
