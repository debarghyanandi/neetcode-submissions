// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(n) space
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
 PROBLEM : Given the root of a binary tree, return the length of its diameter.
           The diameter is the longest path between any two nodes, counted in
           EDGES, not nodes. The path does not have to pass through the root.
           Example: [1,2,3,4,5] -> 3 (path 4-2-1-3).
 PATTERN : DFS post-order (height) + global running max
================================================================================
IDEA
  Height(node) returns the number of nodes on the longest downward path.
  At each node, left + right is the longest path that bends at this node,
  in edges. We keep the best of these in res.
  Every path has exactly one highest node where it bends. So checking
  left + right at every node covers every possible path.
EXAMPLE
  [1,2,null,3,4,5,null,6]: 2 has children 3,4; 3 has 5; 4 has 6.
  H(5)=1, H(3)=2, H(6)=1, H(4)=2 (res=1 so far)
  H(2): left=2, right=2 -> res=4, returns 3; H(1): 3+0=3, res stays 4
  Answer 4 (path 5-3-2-4-6). The root is not on this path.
COMPLEXITY
  Time  O(n)  each node is visited once by Height, with O(1) work per visit
  Space O(n)  recursion stack depth equals tree height, n for a skewed tree
PATH TO OPTIMAL
  Brute - for each node, call a separate height() on both children -
    O(n^2) on a skewed tree, because heights are recomputed many times.
  One post-order DFS - O(n) - the height is computed once and reused for
    both the return value and the res update (this file).
  optimal-variant.cs gets the same O(n) bound in another form.
KEYWORDS
  binary tree, diameter, height, DFS, post-order, global max, recursion
WATCH OUT
  - Returning Height(root) instead of res. That is only the path through
    the root, which fails on the example above.
  - Off by one: Height counts nodes, so left + right is edges. Do not add 1.
  - res is an instance field and is never reset. Reusing the same Solution
    object for a second tree returns a stale result.
  - A very deep skewed tree can overflow the call stack.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Return the actual path, not just its length?
     -> At each node also return the deepest leaf, and record the node where
        res improved. Rebuild the path from both sides. Still O(n) / O(n).
  2. Maximum path SUM with negative values (LC 124)?
     -> Same shape. Return node.val + max(0, left, right) and update res with
        val + max(0,left) + max(0,right). Still O(n).
  3. N-ary tree, or a general tree given as a graph?
     -> N-ary: keep the top two child heights at each node. Graph tree: BFS
        from any node to the farthest node u, then BFS from u. The farthest
        distance is the diameter. O(n).
  4. Avoid recursion depth problems?
     -> Do an iterative post-order with an explicit stack and a map from node
        to height. Same O(n) time and space, but no call-stack overflow.
TRIGGER
  When a tree answer is "the best path through some node, combining both
  subtrees", return one value up and update a global max at every node.
================================================================================
*/
