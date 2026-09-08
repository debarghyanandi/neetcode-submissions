// ##########################################################################
// #  optimal.cs            O(n) time / O(n) space
// #  post-order recursion computing subtree min/max bottom-up
// #  [minmax-bottomup]
// #  ties with optimal-variant.cs on O(n) time / O(n) space
// #
// #  YOU SOLVED THIS YOURSELF (from submission-2)
// #
// #  each node visited once, returning (min,max,found) tuples validated
// #  against child extremes; recursion stack is O(n) worst case for a
// #  skewed tree
// ##########################################################################

// Sentinel values are safe only because Node.val is restricted to [-1000000000, 1000000000];
// if the value range can reach int.MinValue/MaxValue, use long or nullable bounds.
public class Solution
{
    //my solution
    public bool IsValidBST(TreeNode root)
    {
        var result = IsValidBSTWithMinMax(root);
        return result.found;
    }

    public (int min, int max, bool found) IsValidBSTWithMinMax(TreeNode root)
    {
        //Empty
        if (root == null)
            return (int.MaxValue, int.MinValue, true);

        //Leaf Node
        if (root.right == null && root.left == null)
            return (root.val, root.val, true);

        var left = IsValidBSTWithMinMax(root.left);
        var right = IsValidBSTWithMinMax(root.right);

        // left subtree must be valid and its max < root
        if (!left.found || left.max >= root.val)
            return (0, 0, false);

        if (!right.found || right.min <= root.val)
            return (0, 0, false);

        //we came to here means valid bst
        int min = Math.Min(left.min, root.val);
        int max = Math.Max(right.max, root.val);

        return (min, max, true);
    }
}

/*
================================================================================
 PATTERN : Post-order DFS returning (min, max, found) per subtree
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-2.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
WHY BOTTOM-UP
  The tempting version checks only root.left.val < root.val < root.right.val at
  each node. That is a local check and it passes on this tree:

        5
       /
      1
       \
        6

  1 < 5 holds, 6 > 1 holds, yet 6 sits in the left subtree of 5. Returning the
  whole subtree's extremes is what catches it: the call on node 1 reports max =
  6, and the parent's test left.max >= root.val fires 6 >= 5 and returns false.
  Each node is compared against every descendant that could conflict with it,
  not just its two children.
INVARIANT
  Every return from IsValidBSTWithMinMax means: found is true iff the subtree
  rooted here is a valid BST, and when found is true, min and max are the
  smallest and largest values actually present in that subtree. Both facts must
  be true together at every return site, which is why the (0, 0, false) path is
  allowed to lie about min and max - found is false, so no caller may read them.
  The || short-circuit in !left.found || left.max >= root.val is what enforces
  that: when !left.found is true, left.max is never evaluated. Same shape on the
  right side.
THE ASYMMETRIC FOLD
  min = Math.Min(left.min, root.val) never looks at right.min, and max =
  Math.Max(right.max, root.val) never looks at left.max. This is correct only
  because both ordering checks have already passed by the time those two lines
  run: left.max < root.val <= right.min, so the subtree minimum can only come
  from the left side or from root.val itself, and the maximum only from the
  right side or root.val. If you moved these two lines above the validity
  checks, the fold would be wrong. Worth being able to say out loud - an
  interviewer will ask why you did not take Math.Min of all three.
THE EMPTY SENTINEL
  The null case returns (int.MaxValue, int.MinValue, true) - an inverted,
  deliberately empty interval. It does double duty. In the comparisons: left.max
  >= root.val becomes int.MinValue >= root.val, always false, so a missing left
  child never rejects; right.min <= root.val becomes int.MaxValue <= root.val,
  also always false. In the fold: Math.Min(int.MaxValue, root.val) collapses to
  root.val and Math.Max(int.MinValue, root.val) collapses to root.val, so a node
  with one child reports itself as the bound on the missing side. Flip the two
  sentinels and every single-child node breaks.
WATCH OUT
  1. The sentinel is only safe inside the value range noted in the file comment.
  A node holding int.MinValue makes the empty-left test int.MinValue >=
  int.MinValue true, and a perfectly valid tree is rejected. The fix is long
  bounds or nullable bounds, not a wider int.
  2. Duplicates are rejected on purpose: >= and <= rather than > and <. A tree
  with two 3s is not a valid BST under this problem's definition, and loosening
  either comparison silently accepts it.
  3. min/max here are subtree extremes flowing upward, not permitted bounds
  flowing downward. Do not mix this up with the (low, high) top-down variant -
  the parameters look alike and the direction is opposite.
SLACK IN THE CODE
  The leaf branch is a shortcut, not a requirement: with both children null, the
  general path computes left = right = the sentinel, both checks pass, and the
  fold yields exactly (root.val, root.val, true). Deleting it changes nothing
  but the number of calls.

  There is also no pruning. Both recursive calls are made before either check
  runs, so an invalid left subtree still costs a full traversal of the right
  one. Every node is still visited exactly once, so this does not change the
  bound - but if asked to add early exit, the move is to test left.found before
  recursing right.
TRIGGER
  Reach for this shape whenever a node's validity depends on values arbitrarily
  deep beneath it and a single summary of each subtree is enough to decide: BST
  validation, largest-BST-subtree, balanced-tree checks, subtree sum or
  diameter. The tell is that you can name a small fixed tuple that summarizes
  any subtree and that a parent can combine two children's tuples plus its own
  value in O(1). If the parent instead needs context from above, that is the
  top-down bounds version, not this one.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
