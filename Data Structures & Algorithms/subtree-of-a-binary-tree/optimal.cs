// ##########################################################################
// #  optimal.cs            O(m * n) time / O(n + m) space
// #  DFS anchor scan + recursive tree equality check
// #  [tree-compare-each-node]
// #  the only solution in this folder
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  IsSubtree recurses over every node of root as a candidate anchor, and
// #  IsSameTree does a full O(n) structural comparison at each anchor;
// #  worst-case recursion depth stacks the root traversal depth with a
// #  same-tree call depth.
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
 PATTERN : DFS anchor walk + full tree-equality check at each node
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-0.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
WHY TWO RECURSIONS
  IsSubtree and IsSameTree ask different questions and cannot be fused into one
  traversal. IsSameTree(first, second) asks "does an exact copy of second start
  right here, at first". IsSubtree asks "does any node in root anchor such a
  copy". The reason they must stay separate: the moment IsSameTree hits a
  mismatch it has to abandon that whole comparison, but the search itself is not
  over - you must restart the comparison from a fresh anchor node. If you tried
  to keep descending with a single function you would be matching subRoot's
  children against nodes that are no longer aligned with subRoot's root.
INVARIANT
  IsSameTree(first, second) returns true only when the two trees agree in shape
  and in value all the way down to their nulls. It is not a prefix or
  containment test. That is the whole definition of "subtree" here: a node plus
  ALL of its descendants, nothing extra hanging off the bottom. Look at the
  structure of the check - the recursive case requires both children to agree
  (&& of the two calls), and the only true-leaf is the first != null && second
  != null branch bottoming out at the null/null case. There is no path to true
  that ignores a remaining node on either side.

  IsSubtree(root, subRoot) returns true iff some node inside root's subtree
  satisfies that predicate against subRoot.
WHY ROOT.LEFT IS SAFE
  The else branch dereferences root without a null check, and that is
  deliberate. Case analysis on reaching it:

  1. root == null and subRoot != null - the first guard already returned false.
  2. root == null and subRoot == null - IsSameTree(null, null) hits its first
  line and returns true, so IsSubtree returned true.
  3. root != null - the only case left.

  So the first guard is load-bearing as a null-dereference shield, not just as a
  logic case. Drop it and input (null, non-null) falls through IsSameTree (which
  correctly returns false) straight into root.left and throws
  NullReferenceException. Note this also covers the recursive calls: descending
  into a null child with a non-null subRoot terminates at that same guard rather
  than crashing.
WATCH OUT
  1. Finding a node whose val equals subRoot.val does not let you commit.
  Duplicate values are the standard adversarial input: the first anchor with a
  matching val may fail deep down, and you still have to search the rest. The
  code handles this because a false from IsSameTree falls into the || over both
  children rather than returning false.

  2. The || short-circuits, so the entire left subtree is exhausted before
  root.right is touched. Correct, but it means the anchor you find is the
  leftmost one - do not reason about it as "the first in level order".

  3. The serialize-and-substring shortcut is the classic wrong answer to this
  problem. Preorder strings without explicit null markers and delimiters give
  false positives: a tree printing as 12 matches inside one printing as 122, and
  two structurally different trees can share a preorder sequence. If you go that
  route the null sentinels are mandatory.
TRIGGER
  Reach for this shape when the question is structural containment of one tree
  in another - "contains an exact copy of", "appears as a subtree". The tell is
  that the target must match completely, not partially: that forces the separate
  all-or-nothing equality helper. Contrast with problems that allow a partial
  match (path-in-tree, same-prefix), which can be done in a single recursion.
INTERVIEWER FOLLOW-UP
  The expected push is "can you beat the product of the two sizes". Two
  linear-time answers:

  1. Serialize both trees in preorder with an explicit marker for every null and
  a delimiter between values, then run KMP to find subRoot's string inside
  root's string. The null markers are what make the serialization injective,
  which is exactly what fixes the substring trap above.

  2. Merkle hashing: post-order, give each node a hash derived from its value
  and its two children's hashes, then compare against subRoot's hash. Mention
  the caveat unprompted - this is correct only up to hash collisions, so you
  either accept the probabilistic bound or verify a hit with IsSameTree, which
  is the helper you already have.
COMPLEXITY
  Time  : O(m * n)
  Space : O(n + m)
================================================================================
*/
