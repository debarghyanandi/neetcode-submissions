// --------------------------------------------------------------------------
// -  optimal-variant.cs    O(n) time / O(n) space
// -  Postorder DFS returning height and diameter as tuple
// -  [postorder-height-diameter]
// -  ties with optimal.cs on O(n) time / O(n) space
// -
// -  Reference solution - not one you solved yourself
// -
// -  Single postorder traversal visits each node once; returns both values
// -  as tuple rather than using mutable state.
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
 PATTERN : Post-order DFS returning (height, diameter) tuple
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-1.cs when it was first processed
 STATUS  : Optimal variant - ties the best complexity by another route
================================================================================
WHY THIS PATTERN
  The longest path in a tree bends at exactly one node: it goes down the left
  side and down the right side of that node. So for every node you need
  left.height + right.height, and the answer is the biggest such sum over all
  nodes. One post-order pass (children first, then the node) gives every node
  its two child heights for free, and DFS carries both the
  height and the best diameter seen in that subtree back up in one return value.
BRUTE FORCE
  The first thing most people write is a Height(node) helper, then a recursion
  that visits every node and calls Height(node.left) + Height(node.right). That
  re-walks each subtree once per ancestor, so it is O(n^2) on a skewed tree and
  O(n log n) on a balanced one. It loses because the height information is
  recomputed instead of being returned alongside the partial answer.
INVARIANT
  Every call to DFS(node) returns the true height of that
  subtree in edges-plus-one (null is 0, a leaf is 1) and the largest left.height
  + right.height over all nodes inside that subtree. The node combines three
  candidates: the path bending here (diameterThroughHere), the best in the left
  subtree, and the best in the right subtree. Since the true longest path bends
  at some single node, and every node is the "here" case in exactly one call,
  the max at the root covers all of them.
WHY DIAMETER MUST TRAVEL UP TOO
  Returning only height is not enough, because the best path may live deep in
  one subtree and never touch the root. That is why the tuple carries
  left.diameter and right.diameter upward and the node takes the max of all
  three. The alternative is a mutable field updated as a side effect during the
  height recursion; this file keeps the method pure by folding that field into
  the return value.
WATCH OUT
  The answer is counted in edges, not nodes: a single-node tree returns height 1
  and diameter 0. If you change the null base case to return -1 for height,
  diameterThroughHere must become left.height + right.height + 2, so the two
  pieces are coupled. Recursion depth equals tree height, so a long chain of
  nodes shaped like a linked list can overflow the call stack. Also note
  DiameterOfBinaryTree throws away the height with the discard, so the height
  value only matters inside the recursion.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. What changes if the diameter is defined as the number of nodes on the path
  instead of edges?
     Use diameterThroughHere = left.height + right.height + 1 and keep
     everything else the same; the height base case of 0 for null already counts
     nodes.
  2. Can you do this without recursion, for a very deep tree?
     Do an explicit post-order traversal with a Stack<TreeNode> and a
     Dictionary<TreeNode,int> (or a stack of frames) holding computed heights,
     updating a running max when both children are done. Same O(n) time, but
     heap memory replaces stack frames so depth no longer risks a stack
     overflow.
  3. How would you return the actual path, not just its length?
     Store the deepest-descendant node along with each height, and remember the
     bend node when a new max is found; then walk down from the bend to those
     two descendants. That adds O(n) extra storage for parent or child pointers.
  4. How would you extend this to an N-ary tree?
     Compute the heights of all children, keep the largest two, and use their
     sum as the path bending at that node. Finding the top two is O(k) per node
     with two running variables, so the total stays linear.
TRIGGER
  When a tree question asks for the best path that may bend at any node and
  never reaches the root, return a tuple of (what the parent needs, best answer
  so far) from one post-order pass.
C# NOTE
  The named ValueTuple (int height, int diameter) is a struct, so each recursive
  call returns it by value with no object allocation, and the named fields read
  better than left.Item1; the discard in var (_, diameter) makes it clear the
  root height is intentionally unused.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
