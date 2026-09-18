// ##########################################################################
// #  optimal.cs            O(n) time / O(n) space
// #  post-order DFS returning subtree (min,max,valid)   [minmax-bottomup]
// #  ties with optimal-variant.cs on O(n) time / O(n) space
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  each node visited once, combining child min/max tuples bottom-up;
// #  recursion depth O(n) worst case for skewed tree
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
 PATTERN : Post-order DFS returning (min, max, found) upward
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-2.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
CORE IDEA
  Validity of a BST is a global property, so each recursive call reports back
  everything the parent could possibly need: the smallest value in the subtree,
  the largest value, and whether that subtree is itself a BST. The parent then
  tests root.val against left.max and right.min - the extremes of entire
  subtrees, not the values of its immediate children. IsValidBST itself is a
  thin wrapper that calls IsValidBSTWithMinMax(root) and throws away min and
  max, keeping only result.found.
INVARIANT
  When found is true, min is the minimum value over the whole subtree and max is
  the maximum, and every ancestor may rely on those two numbers alone. When
  found is false, min and max are meaningless - the code returns the placeholder
  (0, 0, false) - and the invariant holds anyway because the parent checks
  !left.found / !right.found first and returns false without ever reading those
  fields. The garbage never propagates upward.
WHY THE SENTINELS WORK
  The null case returns (int.MaxValue, int.MinValue, true), which looks
  backwards but is exactly the identity element for the two operations performed
  on it. In the comparisons it is neutral: a null left child gives left.max =
  int.MinValue, which is below every legal node value, so left.max >= root.val
  is false; a null right child gives right.min = int.MaxValue, so right.min <=
  root.val is false. In the propagation it is also neutral:
  Math.Min(int.MaxValue, root.val) is root.val and Math.Max(int.MinValue,
  root.val) is root.val. An empty subtree therefore contributes nothing without
  needing a single extra branch.
THE TRAP IT AVOIDS
  The classic wrong answer compares root.val only with root.left.val and
  root.right.val. Take root 5, left child 4, and 4's right child 6. Every
  parent-child pair is locally ordered, yet 6 sits in the left subtree of 5, so
  it is not a BST. Here the call on node 4 returns (4, 6, true), and at the root
  left.max = 6 >= 5 fires and returns false. Note also that both comparisons are
  strict rejections (>= and <=), so duplicates are invalid: root 5 with a left
  child 5 returns (5, 5, true) from the leaf and then 5 >= 5 rejects it.
ASYMMETRIC PROPAGATION
  min is Math.Min(left.min, root.val) and max is Math.Max(right.max, root.val) -
  min never consults the right subtree and max never consults the left. That is
  sound only because it runs after both checks have passed: at that point every
  value in the right subtree exceeds root.val, so it cannot be the subtree
  minimum, and every value in the left subtree is below root.val, so it cannot
  be the maximum. Move either line above the validity checks and the reasoning
  collapses.
WATCH OUT
  The sentinel trick is only correct while node values stay strictly inside
  (int.MinValue, int.MaxValue). A node holding int.MinValue with a null left
  child and a non-null right child compares left.max >= root.val as int.MinValue
  >= int.MinValue, which is true, and the whole tree is wrongly rejected.
  Symmetrically, int.MaxValue with a left child but no right child trips
  right.min <= root.val. The leaf shortcut accidentally hides half of this - a
  bare leaf holding int.MinValue returns before the comparison - which makes the
  bug harder to find, not less real. The header comment is the mitigation: it
  holds because the constraint caps values at +/- 1000000000.
DEAD WEIGHT
  The leaf branch is redundant, not load-bearing. Delete it and a leaf falls
  through the general path: both children return the sentinels, both checks fail
  to fire, and min and max both collapse to root.val - the same (root.val,
  root.val, true). It is worth keeping only as the accidental guard described
  above, and worth knowing it is not correctness-critical.
FOLLOW-UPS
  Two standard alternatives sidestep the sentinel fragility entirely. Top-down
  bound passing carries (low, high) down the tree using long or nullable bounds,
  where an absent bound is null rather than a magic int. In-order traversal
  walks the tree and checks that the sequence is strictly increasing against a
  single prev variable. Both also allow a genuine early exit, which this version
  does not have: left and right are always both fully computed before any check
  runs, so a violation buried in the left subtree still pays for a complete walk
  of the right subtree.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
