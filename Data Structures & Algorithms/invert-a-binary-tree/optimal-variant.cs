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
 PATTERN : Tree BFS - swap children level by level with a queue
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-1.cs when it was first processed
 STATUS  : Optimal variant - ties the best complexity by another route
================================================================================
VARIABLES
  queue      nodes that are still waiting to have their children swapped
  leftChild  saved copy of node.left, kept so the swap does not lose it
WHY THIS PATTERN
  Inverting a tree means doing the same small job at every node: swap its left
  and right child. The order does not matter, so any full traversal works. This
  file visits nodes with a queue, so it goes level by level. Each node is
  swapped exactly once when it leaves the queue.
BRUTE FORCE
  The simplest correct version is recursive: swap root.left and root.right, then
  call InvertTree on both children. It is also O(n) time. Its space is O(h),
  where h is the tree height, because each open call sits on the call stack. It
  does not lose on complexity. Its risk is stack overflow on a very deep, skewed
  tree. The queue version keeps its work on the heap, so it has no such limit.
INVARIANT
  When a node is dequeued, it has not been swapped yet, and every node already
  dequeued has been swapped. Each non-null child is enqueued exactly once, by
  its parent. So every node in the tree is dequeued and swapped exactly once. A
  tree where every node has its children swapped is the mirror image, so root is
  the correct answer.
ENQUEUE AFTER THE SWAP
  The children are enqueued after the swap, so node.left here is the old right
  child. This does not change correctness, because every child is still visited.
  It only changes the visit order, and this problem does not care about order.
WATCH OUT
  The swap needs the temporary leftChild. If you write node.left = node.right
  before saving the old left child, you lose it and the tree breaks. The early
  return for root == null is required. Without it, null is enqueued and the loop
  throws a NullReferenceException on node.left. The code changes the input tree
  in place and returns the same root. A caller that still needs the original
  tree must copy it first.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you do it with less memory than the queue?
     The queue can hold a whole level, up to about n/2 nodes in a full tree. A
     Stack (iterative DFS) holds O(h) nodes instead, which is less for a
     balanced tree. Morris traversal gives O(1) extra space, but it temporarily
     rewires pointers, which is harder to get right while you are also swapping
     them.
  2. How would you check if a tree is symmetric, using this idea?
     Do not invert anything. Compare the tree with its own mirror: enqueue pairs
     (left, right) and check that a.left matches b.right and a.right matches
     b.left. This leaves the tree unchanged.
  3. What if the original tree must not change?
     Build a new mirrored tree. For each node, create a copy whose left is the
     mirror of the old right and whose right is the mirror of the old left. This
     costs O(n) extra memory for the new nodes.
TRIGGER
  Use this when every node needs the same local change, independent of the
  others, and any traversal order will do.
C# NOTE
  Queue<T> in System.Collections.Generic is a circular buffer, so Enqueue and
  Dequeue are O(1) amortized, which means O(1) on average over many calls. To
  switch this to DFS, swap Queue for Stack and use Push/Pop; the rest of the
  code stays the same.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
