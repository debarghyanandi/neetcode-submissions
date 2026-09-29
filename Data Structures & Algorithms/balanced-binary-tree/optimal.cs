// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(n) space
// --------------------------------------------------------------------------

public class Solution
{
    public bool IsBalanced(TreeNode root)
    {
        return CheckBalance(root).balanced;
    }

    // returns balanced (1 or 0) and height as 2 element int array
    private (bool balanced, int height) CheckBalance(TreeNode node)
    {
        if (node == null)
            return (true, 0);
        var left = CheckBalance(node.left);
        var right = CheckBalance(node.right);
        bool balanced = left.balanced && right.balanced && Math.Abs(left.height - right.height) <= 1;
        int height = 1 + Math.Max(left.height, right.height);
        return (balanced, height);
    }
}

/*
================================================================================
 PROBLEM : Given the root of a binary tree, return true if it is
           height-balanced. Balanced means: at EVERY node, the heights of the
           left and right subtrees differ by at most 1. An empty tree is
           balanced. Example: [1,2,2,3,null,null,null,4] -> false (node 2 has
           heights 2 vs 0).
 PATTERN : Post-order DFS returning (balanced, height) tuple
================================================================================
IDEA
  CheckBalance visits the children first, then the node (post-order).
  Each call returns two facts: is this subtree balanced, and its height.
  A node is balanced if left.balanced, right.balanced, and the heights
  differ by at most 1. Its height is 1 + Math.Max(left.height, right.height).
  Correct because a subtree is balanced only if every node inside it is.
EXAMPLE
  Tree: 1 -> left 2 -> left 3 -> left 4; 1 -> right 2 (a leaf).
  4:(T,1) 3: heights 1 vs 0 -> (T,2) left 2: heights 2 vs 0 -> (F,3)
  right 2:(T,1) root: left.balanced is false -> (F,4)
  Answer: false, even though the root's own heights (3 vs 1) are checked too.
COMPLEXITY
  Time  O(n)  each node is visited once, O(1) work per node
  Space O(n)  recursion stack, depth equals tree height (n for a skewed tree)
PATH TO OPTIMAL
  Top-down: at each node call a separate Height() on both sides - O(n^2) on
  a skewed tree - simple but recomputes heights again and again (no file).
  Bottom-up (this file, optimal.cs) - O(n) - height and balance come back
  together in one pass, so no height is computed twice.
KEYWORDS
  binary tree, height-balanced, post-order DFS, bottom-up recursion, tree
  height
WATCH OUT
  - Checking only the root's two heights is wrong. The example above passes
    at the root side-by-side logic only if you forget left.balanced.
  - The comment says "1 or 0 ... 2 element int array", but the code returns
    a (bool, int) tuple. The comment is stale.
  - No early exit: after a subtree is unbalanced, the code still walks the
    whole tree. Correct, but wasted work.
  - A very deep skewed tree can cause a stack overflow with this recursion.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you stop early once you find an unbalanced subtree?
     -> Return height -1 as a flag and return -1 at once if a child gives -1.
        Still O(n) worst case, but faster in practice and uses a single int.
  2. The tree is very deep. Recursion may overflow. What now?
     -> Do an iterative post-order with an explicit stack and a dictionary of
        node -> height. Same O(n) time and space, but on the heap.
  3. Why is the top-down approach O(n^2)?
     -> On a skewed tree each node calls Height() over all nodes below it, so
        the sum is n + (n-1) + ... = O(n^2).
  4. How is this like Diameter of Binary Tree?
     -> Same shape: post-order returns height, and you update an extra answer
        (here a bool, there a max of left + right heights) at each node.
TRIGGER
  When a tree answer at a node depends on facts from both subtrees (height,
  size, sum), return those facts upward in one post-order DFS.
================================================================================
*/
