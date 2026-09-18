// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(n) space
// -  Postorder DFS computing height, tracking diameter via mutable field
// -  [postorder-height-diameter]
// -  ties with optimal-variant.cs on O(n) time / O(n) space
// -
// -  Reference solution - not one you solved yourself
// -
// -  Single postorder traversal visits each node once; recursion stack
// -  depth is O(n) worst-case for skewed tree.
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
 PATTERN : Post-order DFS - return height, track best at each node
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  The diameter is the longest path between any two nodes, and every such path
  has one highest node where it turns. So the answer is the max over all nodes
  of left height plus right height. A post-order walk gives exactly that:
  Height(root) computes the children's values first, so at each node left and
  right are already known and the candidate left + right can be scored before
  returning upward.
BRUTE FORCE
  The first version most people write is a recursive DiameterOfBinaryTree that,
  for each node, calls a separate Height helper on both children and adds them,
  then recurses on both children. That recomputes heights over and over: O(n^2)
  on a skewed tree, O(n log n) when balanced. This file fixes it by having one
  traversal both return the height and update diameter, so each node is visited
  once.
INVARIANT
  Height(node) returns the number of nodes on the longest downward path from
  node, which is 1 + max(left, right), and 0 for null. At the moment
  Height(node) is about to return, diameter holds the largest left + right seen
  over every node already finished, including node itself. Since left + right at
  a node is exactly the edge count of the longest path that turns at that node,
  after Height(root) finishes diameter is the maximum over all nodes, which is
  the answer.
NODES UP, EDGES OUT
  Two different units live in the same function. The return value counts nodes
  (a leaf returns 1), but diameter is measured in edges. left + right works
  because the two node counts are also the number of edges going down each side
  from the current node. If you switch Height to return edge counts (-1 for
  null), the diameter line stays left + right but the return now builds on a -1
  base, and an off-by-one creeps in easily.
WATCH OUT
  diameter is a public instance field that is never reset inside
  DiameterOfBinaryTree. Call the same Solution object twice with a second tree
  and the old, possibly larger, value leaks into the new answer. Set diameter =
  0 at the top of DiameterOfBinaryTree, or keep it local and pass it by ref.
  Also, recursion depth equals tree height, so a long skewed chain can overflow
  the call stack; note that root == null is handled correctly, since Height
  returns 0 without touching diameter.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you do this without recursion?
     Yes: do an iterative post-order with an explicit Stack<TreeNode> plus a
     Dictionary<TreeNode,int> (or a stack of node-and-height pairs) so a node is
     scored only after both children have heights. Same O(n) time, but the code
     is longer and you pay for the dictionary instead of the call stack.
  2. How would you return the actual path, not just its length?
     Have Height also return the deepest node on its best downward side, and
     remember the turning node whenever diameter improves; then walk down from
     that node on both sides picking the taller child. It costs one extra value
     per return but keeps a single pass.
  3. What changes if you want the maximum path sum of node values instead of the
  length?
     Replace height with best downward sum, clamp negatives to zero at each
     child with Math.Max(0, Height(child)), and score root.val + left + right.
     The structure is identical; only the combine step and the base value
     change.
  4. How would you handle an N-ary tree instead of a binary one?
     Compute the heights of all children, keep the two largest, and score their
     sum; the return is still 1 + the largest. That is O(children) per node with
     two running maxima, so still O(n) overall.
TRIGGER
  A tree question asking for a best path that may bend at some node and need not
  pass through the root - compute one value per node bottom-up and score the
  combination on the way back.
C# NOTE
  Using a public mutable field for diameter works but is fragile across calls; a
  cleaner C# form is a local variable in DiameterOfBinaryTree captured by a
  local function, or having Height return a (int height, int diameter) value
  tuple so nothing outlives one call.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
