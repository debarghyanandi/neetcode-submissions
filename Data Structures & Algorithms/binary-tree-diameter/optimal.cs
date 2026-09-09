// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(n) space
// -  postorder DFS computing height, tracking diameter in shared field
// -  [postorder-height-diameter]
// -  ties with optimal-variant.cs on O(n) time / O(n) space
// -
// -  Reference solution - not one you solved yourself (from submission-0)
// -
// -  single postorder traversal visits each node once, updating a max via
// -  mutable instance field; recursion stack depth is O(n) worst-case for a
// -  skewed tree
// --------------------------------------------------------------------------

public class Solution
{
    public int res = 0;

    public int DiameterOfBinaryTree(TreeNode root)
    {
        Height(root);
        return res;
    }

    private int Height(TreeNode root)
    {
        if (root == null)
            return 0;

        int left = Height(root.left);
        int right = Height(root.right);

        res = Math.Max(res, left + right);

        return 1 + Math.Max(left, right);
    }
}

/*
================================================================================
 PATTERN : Post-order DFS - return height, update diameter globally
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  The quantity you want at a node (longest path bending through it) and the
  quantity the parent needs from you (height) are different numbers, but both
  are computable from the same two recursive calls. So Height does double duty:
  its return value 1 + Math.Max(left, right) feeds the parent, and its side
  effect res = Math.Max(res, left + right) records the answer. One traversal,
  because nothing here needs a second look at a subtree.
BRUTE FORCE
  The literal reading of the problem is: for every node, diameter-through-node =
  height(left) + height(right), then take the max over all nodes. Written
  directly that calls a separate height routine at each node, and height itself
  walks the whole subtree - the top node's subtrees get walked once for the top
  node, again for each child, and so on. On a path-shaped tree that is O(n^2).
  This file kills the repetition by computing each height exactly once and
  folding the max in on the way back up.
CORRECTNESS ARGUMENT
  Every path in a tree has exactly one highest node - the node where the path
  stops going up and starts going down (a straight downward path degenerates to
  this too, with one side empty). Fix that node r. The best path peaking at r
  goes down the deepest chain on the left and the deepest chain on the right,
  which is exactly left + right edges. Since Height is called on every node, res
  is maximized over every possible peak, so it is maximized over every path.
  Nothing is missed and nothing invalid is counted.
INVARIANT
  Two things hold at the moment Height(root) returns. (1) The return value is
  the exact edge-height of the subtree at root: 0 for null, otherwise one more
  than the deeper child. (2) res is at least the diameter of every subtree fully
  visited so far, including root's. Note res only ever grows - it is a running
  maximum, never reset and never read during the recursion. That is why the
  order matters: both child calls must finish before the res update, so left and
  right are final heights, not partial ones.
UNITS: EDGES, NOT NODES
  Height returns 0 for null rather than -1, which makes it count edges above a
  real node: a leaf returns 1 (one edge to its parent's slot), and a leaf's own
  res contribution is 0 + 0 = 0, correct since a single node has diameter 0.
  Because left and right are each edge-counts from root down, left + right is
  the edge-count of the joined path with no off-by-one adjustment. If you ever
  switch Height to return node-counts, left + right becomes one too many and you
  need left + right - 1 at the peak, or subtract 1 at the end.
WATCH OUT
  res is a public instance field, not a local. It is initialized once at
  construction and never reset in DiameterOfBinaryTree, so calling that method
  twice on the same Solution object returns the max over both trees, not the
  second tree's diameter. The judge hands you a fresh instance per case so it
  passes, but say this out loud in an interview and offer the fix: reset res = 0
  at the top of DiameterOfBinaryTree, or drop the field and have the helper
  return a (height, diameter) pair or take a ref int. Also, recursion depth
  tracks tree height, so a degenerate one-child-per-node tree with 10^5 nodes is
  a real stack-overflow story - the iterative rewrite is an explicit post-order
  stack with a memo of computed heights.
TRIGGER
  Reach for this shape whenever the answer lives at some unknown node but each
  node can only report a summary upward: the node-local candidate is a
  combination of both children (left + right), while the upward report is a
  choice between them (max). Same skeleton as max path sum, longest univalue
  path, and the count-good-nodes family - only the two formulas change.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
