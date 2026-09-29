// --------------------------------------------------------------------------
// -  optimal-variant-2.cs  O(n) time / O(n) space
// --------------------------------------------------------------------------

public class Solution
{
    // Iterative DFS
    public TreeNode InvertTree(TreeNode root)
    {
        if (root == null)
            return null;

        Stack<TreeNode> stack = new Stack<TreeNode>();
        stack.Push(root);
        while (stack.Count > 0)
        {
            TreeNode node = stack.Pop();
            TreeNode leftChild = node.left;
            node.left = node.right;
            node.right = leftChild;
            if (node.left != null)
                stack.Push(node.left);
            if (node.right != null)
                stack.Push(node.right);
        }

        return root;
    }
}

/*
================================================================================
 PROBLEM : You are given the root of a binary tree. Mirror it: at every node,
           swap the left and right child. Return the root of the same, now
           inverted, tree. Example: [4,2,7,1,3,6,9] -> [4,7,2,9,6,3,1]. Empty
           tree -> null.
 PATTERN : Tree DFS (iterative, explicit stack)
================================================================================
IDEA
  Put root on stack. Pop a node, swap node.left and node.right using the
  temp leftChild, then push each non-null child. Every node is popped once
  and swapped once. So every node ends up mirrored, and the whole tree is
  mirrored. It is the same work as recursive optimal.cs, but an explicit
  Stack replaces the call stack.
EXAMPLE
  Tree [4,2,7,1,3,6,9]. Pop 4: now left=7, right=2; push 7, push 2.
  Pop 2: left=3, right=1; push 3, 1. Pop 1, pop 3 (leaves, nothing to do).
  Pop 7: left=9, right=6; push 9, 6. Pop 6, pop 9.
  Pop order 4,2,1,3,7,6,9. Result [4,7,2,9,6,3,1].
COMPLEXITY
  Time  O(n)  each node is pushed and popped once, with an O(1) swap per pop
  Space O(n)  the stack can hold up to n nodes in the worst case
WATCH OUT
  - Skipping the temp: node.left = node.right; node.right = node.left
    loses the old left subtree. Keep the swap in leftChild.
  - The null-root check must come first, or Push(null) then crashes on
    node.left.
  - The tree changes in place. The caller's original tree is gone.
    Clone the tree first if the caller still needs the original.
  - Popping order does not matter. Pushing children after the swap or before
    it both work, because each node swaps only its own two child links.
================================================================================
*/
