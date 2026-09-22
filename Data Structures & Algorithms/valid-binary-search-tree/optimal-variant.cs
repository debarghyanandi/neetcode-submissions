// --------------------------------------------------------------------------
// -  optimal-variant.cs    O(n) time / O(n) space
// -  Recursive bounds checking validation   [recursive-bound-checking]
// -  ties with optimal.cs on O(n) time / O(n) space
// -
// -  Reference solution - not one you solved yourself
// -
// -  Each node visited once with O(1) work; call stack depth is tree height
// -  (O(n) worst case for skewed tree).
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
 PATTERN : DFS with range bounds - validate BST by interval
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-3.cs when it was first processed
 STATUS  : Optimal variant - ties the best complexity by another route
================================================================================
VARIABLES
  lower    strict lower bound every value in this subtree must beat
  upper    strict upper bound every value in this subtree must stay under
WHY THIS PATTERN
  The BST rule is not local: a node must be greater than every value in the left
  subtree of its ancestors, not just its parent. So each recursive call carries
  the allowed open interval (lower, upper) for the whole subtree. Going left
  replaces upper with node.val; going right replaces lower with node.val. That
  single pass encodes all ancestor constraints without looking back up the tree.
BRUTE FORCE
  The first thing most people write is a check at each node only against its two
  children, which is wrong, not just slow. The simplest correct naive version
  is: for every node, walk its entire left subtree and confirm all values are
  smaller, then the right subtree for larger. That is O(n) work per node, so
  O(n^2) time on a skewed tree, and it repeats the same comparisons at every
  level.
INVARIANT
  When Validate(node, lower, upper) is called, every value in that subtree must
  lie strictly inside (lower, upper) for the tree to be a BST. The code checks
  node.val against the interval first, then narrows the interval for each child
  so the child inherits both the ancestor bound and the new bound from node.val.
  If every node passes its own check, no ancestor-descendant pair can be out of
  order, which is exactly the BST definition.
LONG BOUNDS AND STRICT COMPARISONS
  The bounds are long, not int, so long.MinValue and long.MaxValue sit outside
  the range of any int node value. If they were int.MinValue and int.MaxValue, a
  root holding int.MinValue would fail its own <= check even though a single
  node is a valid BST. The comparisons use <= and >= because duplicates are not
  allowed in this definition; node.val == lower or node.val == upper means the
  same value already appeared on the path and must be rejected.
WATCH OUT
  The recursion depth equals the height of the tree, so a completely skewed tree
  (a linked list of nodes) risks a stack overflow, which in .NET cannot be
  caught. node.val is compared as an int widened to long; that widening is what
  makes the sentinel bounds safe, so changing lower and upper to int silently
  breaks the extreme-value cases. Also note the && short-circuits: if the left
  subtree fails, the right subtree is never visited, which is fine for
  correctness but means you cannot rely on this code to visit every node if you
  later add side effects inside Validate.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you do this without recursion?
     Yes, with an explicit Stack of (node, lower, upper) tuples, or with an
     iterative in-order traversal that keeps only the previous value and checks
     prev < node.val. The in-order version drops the bounds entirely and uses
     one long prev variable, but still uses stack space up to the tree height.
  2. What if duplicates were allowed, say equal values must go in the left
  subtree?
     Change the left recursion to allow equality: the node check becomes
     node.val < lower || node.val >= upper, and going left passes upper =
     node.val while still permitting node.val to reappear below. Only the
     comparison operators move; the structure stays the same.
  3. The tree does not fit in memory as one object graph - what changes?
     Stream it in in-order from disk and keep only the last value seen,
     comparing each new value to it. That turns the space cost into O(1) beyond
     the read buffer, but you lose the early exit on a bad left subtree since
     you must read in order.
  4. How would you return the size of the largest BST subtree instead?
     Switch from top-down bounds to a bottom-up post-order return of (isBst,
     min, max, size) per subtree, combining children before deciding the parent.
     Same O(n) time, but every node must be visited, so no short-circuit is
     possible.
TRIGGER
  A tree question where a node's legality depends on all its ancestors, not just
  its parent - push the allowed range down instead of looking up.
C# NOTE
  Validate is a private instance method taking the bounds as parameters, so no
  field state is shared between calls and the method is safe to call on the same
  Solution object repeatedly; if you wanted the tuple-stack iterative form,
  ValueTuple<TreeNode, long, long> keeps it allocation-light compared to a
  class-based node wrapper.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
