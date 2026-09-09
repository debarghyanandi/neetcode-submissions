// --------------------------------------------------------------------------
// -  optimal-variant.cs    O(n) time / O(n) space
// -  postorder DFS returning (height, diameter) tuple
// -  [postorder-height-diameter]
// -  ties with optimal.cs on O(n) time / O(n) space
// -
// -  Reference solution - not one you solved yourself (from submission-1)
// -
// -  same single-pass postorder idea as submission-0 but avoids shared
// -  mutable state by returning height and diameter together; recursion
// -  stack depth is O(n) worst-case
// --------------------------------------------------------------------------

public class Solution
{
    public int DiameterOfBinaryTree(TreeNode root)
    {
        var (_, diameter) = DFS(root);
        return diameter;
    }

    private (int height, int diameter) DFS(TreeNode node)
    {
        if (node == null)
            return (0, 0);

        var left = DFS(node.left);
        var right = DFS(node.right);

        int height = 1 + Math.Max(left.height, right.height);
        int diameterThroughHere = left.height + right.height;
        int diameter = Math.Max(diameterThroughHere, Math.Max(left.diameter, right.diameter));

        return (height, diameter);
    }
}

/*
================================================================================
 PATTERN : Post-order DFS returning (height, diameter) pairs
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-1.cs when it was first processed
 STATUS  : Optimal variant - ties the best complexity by another route
================================================================================
WHY THIS PATTERN
  Nothing can be decided at a node until both children have reported. The
  longest path bending at node needs left.height and right.height, and the
  answer for the subtree needs left.diameter and right.diameter. Post-order is
  the only traversal order where all four values are already in hand when you
  reach the node. Each node is entered once, computes three lines of arithmetic,
  and returns.
THE INVARIANT
  DFS(node) returns exactly two facts about the subtree rooted at node, and
  nothing about anything above it:
    height = number of nodes on the longest root-to-leaf path inside that
    subtree (null = 0, leaf = 1).
    diameter = number of edges on the longest path with both endpoints inside
    that subtree.
  Both are self-contained, which is what makes the recursion compose. The parent
  never needs to look back down.
WHY IT CHECKS EVERY PATH
  This is the correctness argument an interviewer will push on. Every path in a
  tree has a unique topmost node - its LCA. So partitioning all paths by their
  topmost node covers each path exactly once. diameterThroughHere = left.height
  + right.height is the best path whose topmost node is this one: greedily take
  the deepest reach on each side, since the two sides are independent. Folding
  in Math.Max(left.diameter, right.diameter) covers every path whose top is
  strictly lower. Union over all nodes, so the value returned from the root is a
  max over all paths.
THE NODES-VS-EDGES OFFSET
  height counts nodes but diameter counts edges, and left.height + right.height
  silently converts between them. It works because the number of nodes below
  node on one side equals the number of edges from node down that side: the
  child itself contributes the node-to-child edge. Check it on a leaf - 0 + 0 =
  0 edges, right. Two leaf children - 1 + 1 = 2 edges, right. If you instead
  define null as -1 (leaf height 0), diameterThroughHere must become left.height
  + right.height + 2. Pick one convention and verify it on the leaf case.
WHAT THE TUPLE ROUTE COSTS YOU
  The common alternative keeps a mutable field or ref int, has DFS return height
  alone, and does field = Math.Max(field, left + right) as a side effect. There
  the running max is maintained for free by the field, so nothing is ever lost.
  Here it is not free: the Math.Max(diameterThroughHere, Math.Max(left.diameter,
  right.diameter)) line is load-bearing. Drop the child diameters and you
  compute only the best path through the root, which is wrong for any tree whose
  longest path lives off to one side. The payoff is that DFS is a pure function
  - no shared state, safe to reason about or reuse in isolation.
WATCH OUT
  1. The null base case (0, 0) has to be right in both slots; returning a
  nonzero diameter there poisons the max all the way up.
  2. The height slot of the top-level call is discarded by var (_, diameter) -
  that is deliberate, the root's height is not part of the answer.
  3. A single-node tree returns 0, not 1. Diameter is measured in edges, so the
  empty and single-node cases share an answer.
  4. Recursion depth equals tree height, so a fully skewed input of ~10^5 nodes
  is the case to raise if asked about robustness; the fix is an explicit stack
  with post-order ordering, not a change to the math above.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
