// --------------------------------------------------------------------------
// -  optimal-variant.cs    O(n) time / O(n) space
// -  pre-order DFS passing down valid (lower,upper) bounds
// -  [bounds-topdown]
// -  ties with optimal.cs on O(n) time / O(n) space
// -
// -  Reference solution - not one you solved yourself
// -
// -  each node checked once against inherited bounds; recursion depth O(n)
// -  worst case for skewed tree
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
 PATTERN : Top-down DFS carrying an open (lower, upper) interval
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-3.cs when it was first processed
 STATUS  : Optimal variant - ties the best complexity by another route
================================================================================
WHY THIS PATTERN
  BST-ness is not a property you can check one parent at a time. A node deep in
  the left subtree is constrained by every ancestor it hangs under, not just its
  immediate parent. The fix is to carry that accumulated constraint down the
  recursion: Validate receives the exact range the current node is allowed
  to occupy, so each node is validated against all of its ancestors in one
  comparison pair.
THE INVARIANT
  When Validate(node, lower, upper) is called, lower and upper are the
  intersection of every ancestor constraint on node's subtree: lower < every
  value in the subtree < upper. The recursion preserves this. Going left, the
  window narrows to (lower, node.val) because everything left of node must stay
  below it while still respecting the inherited lower. Going right, it narrows
  to (node.val, upper). The window only ever shrinks, and node.val itself
  becomes the new wall on the side it splits.
ALGORITHM
  1. Seed the root with the widest possible window: Validate(root,
  long.MinValue, long.MaxValue).
  2. A null node vacuously satisfies any window, so return true.
  3. Reject immediately if node.val <= lower or node.val >= upper.
  4. Recurse left with upper tightened to node.val, recurse right with lower
  tightened to node.val, and require both.
  5. The && short-circuits, so a violation in the left subtree stops the right
  subtree from being walked at all.
WHY LONG AND NOT INT
  The sentinels must be values no node can equal, and the comparisons here are
  non-strict (<=, >=). If the bounds were int.MinValue and int.MaxValue, a
  legitimate single-node tree whose root holds int.MinValue would hit node.val
  <= lower and be rejected. Widening to long puts the sentinels strictly outside
  the range of any int val. The alternative, if you were forced to stay in int,
  is nullable bounds (long?/int?) or passing the bounding TreeNode references
  and skipping the check when null.
WHY THE COMPARISONS ARE STRICT
  node.val <= lower and node.val >= upper reject equality, which encodes the
  no-duplicates definition of a BST. A node equal to an ancestor fails: if
  node.val equals the parent that set the wall, it lands exactly on lower or
  upper. If the problem variant allowed duplicates on one side, exactly one of
  these two comparisons would relax to < or >, and which one it is tells you
  which side duplicates live on.
THE TRAP
  The tempting wrong answer is a purely local check: node.val > node.left.val
  and node.val < node.right.val, recursed everywhere. It passes on trees that
  are not BSTs. Take root 5 with left child 4, and give that 4 a right child of
  6. Locally 5 > 4 holds and 6 > 4 holds, so the local check accepts, but 6 sits
  in the left subtree of 5 and must be below 5. The bounds version catches it: 6
  arrives with the window (4, 5) inherited from both ancestors and fails 6 >= 5.
FOLLOW-UP
  Two questions an interviewer reaches for next. First, the other route to the
  same bound: an in-order traversal of a BST emits values in strictly increasing
  order, so you validate by keeping one prev variable and rejecting any val <=
  prev. Same work, no bounds threading, but you must handle the first node's
  absent prev. Second, this version recurses once per node, so a fully skewed
  tree drives recursion depth equal to node count; converting to an explicit
  Stack of (node, lower, upper) triples removes the call-stack dependency
  without changing the logic.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
