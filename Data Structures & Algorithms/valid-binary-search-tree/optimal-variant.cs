// --------------------------------------------------------------------------
// -  optimal-variant.cs    O(n) time / O(n) space
// -  pre-order recursion passing down valid (lower,upper) bounds
// -  [bounds-topdown]
// -  ties with optimal.cs on O(n) time / O(n) space
// -
// -  Reference solution - not one you solved yourself (from submission-3)
// -
// -  each node checked once against inherited long-typed bounds, avoiding
// -  int overflow; recursion stack is O(n) worst case for a skewed tree
// --------------------------------------------------------------------------

public class Solution
{
    public bool IsValidBST(TreeNode root)
    {
        return Validate(root, long.MinValue, long.MaxValue);
    }

    private bool Validate(TreeNode node, long lower, long upper)
    {
        if (node == null) return true;
        if (node.val <= lower || node.val >= upper) return false;

        return Validate(node.left, lower, node.val) &&
               Validate(node.right, node.val, upper);
    }
}

/*
================================================================================
 PATTERN : DFS carrying an inherited (lower, upper) bound
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-3.cs when it was first processed
 STATUS  : Optimal variant - ties the best complexity by another route
================================================================================
WHY THIS PATTERN
  BST validity is a global property, but the tempting check is local: confirm
  each node is greater than its left child and less than its right child. That
  is not enough. Take root 5, left child 4, and 4's right child 6. Every
  parent/child pair passes, yet 6 lives in the root's left subtree and must be
  under 5. Validate fixes this by shipping the ancestors' constraints down the
  recursion as lower and upper, so 6 arrives with upper = 5 and is rejected.
INVARIANT
  Whenever Validate(node, lower, upper) is called, every value in node's subtree
  is required to satisfy lower < value < upper. IsValidBST seeds the root with
  (long.MinValue, long.MaxValue), meaning unconstrained. The node tests itself
  against that window, then hands each child a narrowed one.

  Why the window is sufficient rather than just necessary: along any
  root-to-node path, lower is the value of the closest ancestor we turned right
  at and upper is the value of the closest ancestor we turned left at. Those are
  the tightest of all ancestor constraints - every other ancestor's bound is
  looser and already implied. So checking two numbers per node checks the node
  against the whole path above it.
BOUND UPDATES
  Exactly one bound moves per descent, and node.val is always the new one:
    left -> Validate(node.left, lower, node.val) upper tightens, lower is
    inherited unchanged
    right -> Validate(node.right, node.val, upper) lower tightens, upper is
    inherited unchanged
  Once node.val is installed as a bound it is never re-tested; the strict
  comparison at the child does that work. The null case returns true, which
  doubles as the leaf terminator and as the verdict for an empty tree. The &&
  short-circuits, so a failing left subtree means the right subtree is never
  walked.
WHY THE BOUNDS ARE LONG
  node.val is int, but lower and upper are long. That is deliberate. With int
  bounds and int.MinValue as the open sentinel, a perfectly valid tree whose
  root holds int.MinValue would fail node.val <= lower on the very first
  comparison - the sentinel collides with real data. Widening to long parks both
  sentinels one step outside the reachable int range, so the root's test can
  never fire spuriously.

  The alternative, if an interviewer bans the widening trick, is nullable bounds
  (int?) with null meaning unbounded, at the cost of a null check before each
  comparison.
STRICT INEQUALITY
  node.val <= lower and node.val >= upper reject equality on both sides, so
  duplicate values anywhere in the tree are invalid under this definition. If
  asked to permit duplicates in the left subtree, only the upper test relaxes:
  node.val > upper becomes the failure condition, and node.val >= upper is
  dropped. Getting this backwards - relaxing both - lets equal values sit on
  either side and destroys the ordering.
THE OTHER ROUTE
  The equivalent classic is an in-order traversal: a tree is a BST exactly when
  in-order yields a strictly increasing sequence, so you keep a prev pointer and
  compare each visited value against it. Same tree walk, but the verdict comes
  from the emitted sequence instead of from inherited windows. Its edge is that
  Morris threading can run it with O(1) auxiliary space, which the bounds
  recursion cannot match. Its cost is the mutable prev state and the same
  long/nullable sentinel problem for the first node.
WATCH OUT
  Recursion depth here tracks tree height, not a balanced log. A degenerate tree
  - values inserted in sorted order, so a right-leaning chain of 10^5 nodes - is
  exactly the shape that makes each Validate frame stack up. If a problem
  statement admits that size, convert to an explicit Stack of (node, lower,
  upper) triples; the logic transfers unchanged.
TRIGGER
  Reach for bound propagation whenever a node's legality depends on ancestors
  you have already passed, not just on its immediate neighbors. The recall
  phrase is "pass the feasible interval down." Same machinery drives
  range-restricted BST queries, trimming a BST to a value range, and
  constructing or counting BSTs over an interval.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
