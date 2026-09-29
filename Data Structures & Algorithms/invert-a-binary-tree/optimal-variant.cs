// --------------------------------------------------------------------------
// -  optimal-variant.cs    O(n) time / O(n) space
// --------------------------------------------------------------------------

public class Solution
{
    //BFS
    public TreeNode InvertTree(TreeNode root)
    {
        if (root == null)
            return null;

        Queue<TreeNode> queue = new Queue<TreeNode>();
        queue.Enqueue(root);
        while (queue.Count > 0)
        {
            TreeNode node = queue.Dequeue();
            TreeNode leftChild = node.left;
            node.left = node.right;
            node.right = leftChild;
            if (node.left != null)
                queue.Enqueue(node.left);
            if (node.right != null)
                queue.Enqueue(node.right);
        }
        return root;
    }
}

/*
================================================================================
 PROBLEM : You get the root of a binary tree. Mirror it: at every node, swap
           the left and right child. Return the root of the changed tree.
           Example: [4,2,7,1,3,6,9] -> [4,7,2,9,6,3,1] (level order).
 PATTERN : BFS (level-order traversal with a queue)
================================================================================
IDEA
  Put root in queue. Take out one node at a time and swap node.left and
  node.right, using leftChild as the temp. Then enqueue the non-null
  children so they get swapped later. A tree is mirrored exactly when every
  node has its two children swapped, and BFS reaches every node once.
  Unlike a recursive DFS, this is iterative, so a very deep tree cannot
  overflow the call stack.
EXAMPLE
  [4,2,7,1,3,6,9]: pop 4 -> left=7,right=2; queue [7,2]
  pop 7 -> left=9,right=6; pop 2 -> left=3,right=1; queue [9,6,3,1]
  leaves 9,6,3,1 swap null with null. Result: [4,7,2,9,6,3,1]
COMPLEXITY
  Time  O(n)  each node is enqueued and dequeued once, with an O(1) swap
  Space O(n)  queue holds up to one full level, about n/2 nodes at the bottom
WATCH OUT
  - Swapping without a temp (node.left = node.right; node.right =
    node.left) loses the old left child. That is why leftChild exists.
  - Keep the root == null check. Without it, queue.Enqueue(null) runs and
    node.left then throws a NullReferenceException.
  - The tree is changed in place. Return root, not a new tree, and do not
    expect the caller's original shape to survive.
  - Enqueueing after the swap is fine: both children get visited either
    way, so order of the two Enqueue calls does not matter.
================================================================================
*/
