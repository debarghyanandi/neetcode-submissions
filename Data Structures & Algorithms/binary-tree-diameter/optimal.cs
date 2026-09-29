// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(n) space
// -  Post-order DFS height calculation   [dfs-postorder-height]
// -  ties with optimal-variant.cs on O(n) time / O(n) space
// -
// -  Reference solution - not one you solved yourself
// -
// -  Each node visited once during recursive descent; call stack depth
// -  equals tree height (worst case n for skewed tree).
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
 PATTERN : Tree DFS / Post-order - combine child heights at each node
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  res      the longest path seen so far, counted in edges
  left     Height(root.left) = number of nodes on the longest path down from the left child
  right    Height(root.right) = number of nodes on the longest path down from the right child
WHY THIS PATTERN
  The diameter is the longest path between any two nodes, and that path does not
  have to pass through the root. Every such path has one highest node, where it
  turns from going up to going down. At that node the path length is the left
  depth plus the right depth. A post-order DFS (visit both children first, then
  the node) gives left and right to each node, so every node can be tested as
  the turning point in one pass while res keeps the best value.
BRUTE FORCE
  For each node, call a separate height function on its left and right subtrees
  and compute left + right, then take the maximum over all nodes. This is
  correct, but height is computed again and again for the same subtrees. It
  costs O(n^2) on a skewed tree (one where every node has only one child) and
  O(n log n) on a balanced one. This file gets each height once and uses it for
  two things: the answer at this node and the value returned to the parent.
INVARIANT
  When Height(root) returns, it has given back the number of nodes on the
  longest downward path from root. Also, res already holds the best left + right
  over every node in that subtree. Because every path has exactly one highest
  node, and every node gets checked once, the final res is the true diameter.
NODES VS EDGES
  Height counts nodes (a null child gives 0, a leaf gives 1), but res counts
  edges. These match because left is the node count below root on the left side,
  which equals the number of edges from root down that side. So left + right is
  the edge length of the path through root, and no -1 or +1 fix is needed. If
  you change Height to count edges, you must change this sum too.
WATCH OUT
  res is a public instance field and is never reset. If the same Solution object
  is called on a second tree, a larger old value can be returned. Set res = 0 at
  the start of DiameterOfBinaryTree, or make it a local passed by ref. The
  recursion depth equals the tree height, so a very deep skewed tree can cause a
  StackOverflowException. That cannot be caught in .NET, so the process ends.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How do you do this without recursion?
     Do an iterative post-order traversal with an explicit stack and a
     Dictionary<TreeNode,int> that maps each node to its height. It is safe for
     deep trees, but the code is longer and the map costs O(n) memory.
  2. What if each edge has a weight?
     Height returns the max weighted depth instead: max(left + w(root,left
     child), right + w(root,right child)). At each node, res compares the sum of
     the two weighted sides. The structure stays the same.
  3. What is the diameter of a general tree (N-ary, or an undirected graph with
  no cycles)?
     At each node, keep the two largest child depths and add them. Another way
     is two BFS runs: go from any node to the farthest node, then from there to
     the farthest again. The two-BFS method only works when all edge weights are
     zero or positive.
  4. How do you return the actual path, not just its length?
     Store the node where res was last improved. Then walk down from it, always
     taking the deeper child, once on the left side and once on the right side.
     This needs heights you can look up, so cache them or compute them again.
TRIGGER
  The answer is a path that can bend at any node of a tree, and the best path
  through a node can be built from values its children return.
C# NOTE
  To avoid the shared field, write a private helper with the signature int
  Height(TreeNode node, ref int best). This keeps the state local to one call
  and makes the method safe to call more than once.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
