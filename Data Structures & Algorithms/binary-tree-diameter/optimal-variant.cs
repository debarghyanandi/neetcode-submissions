// --------------------------------------------------------------------------
// -  optimal-variant.cs    O(n) time / O(n) space
// -  Post-order DFS height calculation   [dfs-postorder-height]
// -  ties with optimal.cs on O(n) time / O(n) space
// -
// -  Reference solution - not one you solved yourself
// -
// -  Each node visited once; returns tuple instead of global variable; call
// -  stack depth equals tree height (worst case n for skewed tree).
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
 PATTERN : Tree DFS (post-order) - return height and best together
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-1.cs when it was first processed
 STATUS  : Optimal variant - ties the best complexity by another route
================================================================================
VARIABLES
  left                 (height, diameter) tuple for the left subtree
  right                (height, diameter) tuple for the right subtree
  height               number of nodes on the longest downward path from node; null = 0, leaf = 1
  diameterThroughHere  length in edges of the longest path that bends at node
  diameter             longest path in edges found anywhere inside this subtree
WHY THIS PATTERN
  The longest path between any two nodes must bend at exactly one highest node.
  At that node, the path is the deepest path down the left side joined to the
  deepest path down the right side. So each node needs its children's heights
  first. That is post-order DFS: finish both children, then work on the parent.
  The file also returns the best diameter seen so far in the same tuple, so it
  needs no shared variable.
BRUTE FORCE
  For every node, call a separate Height function on its left and right child,
  add the two results, and keep the maximum. This is correct, but it measures
  each subtree again for every ancestor above it. On a skewed tree (a tree that
  is really one long chain) this costs O(n^2) time. The version here computes
  each height once and reuses it on the way back up.
INVARIANT
  When DFS(node) returns, height is the true height of that subtree. diameter is
  the longest path that lies fully inside that subtree. The longest path either
  bends at node (diameterThroughHere) or stays fully inside one child. The child
  case is already correct in left.diameter or right.diameter. So taking the max
  of the three values is correct, and by induction the value at root is the
  answer.
HEIGHT IN NODES, DIAMETER IN EDGES
  height counts nodes: null gives 0 and a leaf gives 1. Because of this,
  left.height + right.height is exactly the number of edges on the path through
  node, and you do not need +1 or -1. If you change the base case to return -1
  (height counted in edges), the sum becomes wrong and you must add 2.
WATCH OUT
  The recursion goes as deep as the tree is tall. On a long chain, a very deep
  tree can overflow the call stack. An empty tree (root == null) correctly
  returns 0. The problem normally wants the answer in edges. If an interviewer
  asks for the answer in nodes, you must add 1 to diameterThroughHere.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How do you do this without recursion?
     Do an iterative post-order walk with an explicit Stack<TreeNode>, and store
     each finished node's height in a Dictionary<TreeNode,int>. There is no
     stack-overflow risk now, but the code is longer and uses a dictionary on
     top of the stack.
  2. What if the tree is N-ary (a node can have any number of children)?
     At each node, keep only the two largest child heights. diameterThroughHere
     is their sum. The cost is still one pass over all children.
  3. What if the edges have weights?
     height becomes the heaviest downward path sum: max(child height + edge
     weight). diameterThroughHere adds the best two of those. The structure
     stays the same. With negative weights you may also need to stop a branch at
     0.
  4. How do you return the actual path and not just its length?
     Store the node where the best bend happens. Then walk down from that node,
     following the taller child on each side, and join the two chains.
TRIGGER
  The answer is a path that can bend at any node in a tree, and each node needs
  a combined result from both of its children.
C# NOTE
  The named value tuple (int height, int diameter) lets you write left.height
  and right.diameter clearly. ValueTuple is a struct, so it passes both results
  up without a mutable class field that you would have to reset between calls to
  DiameterOfBinaryTree.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
