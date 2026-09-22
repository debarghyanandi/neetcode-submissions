// --------------------------------------------------------------------------
// -  optimal-variant.cs    O(n) time / O(n) space
// -  Iterative BFS with queue   [iterative-bfs-queue]
// -  ties with optimal.cs on O(n) time / O(n) space
// -
// -  Reference solution - not one you solved yourself
// -
// -  Level-order traversal visits each node once; queue width is O(n) worst
// -  case on complete tree.
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
 PATTERN : BFS level-order traversal - swap children in place
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-1.cs when it was first processed
 STATUS  : Optimal variant - ties the best complexity by another route
================================================================================
VARIABLES
  queue       nodes seen but not yet swapped
  leftChild   temporary copy of node.left, so the swap does not lose it
WHY THIS PATTERN
  The task asks for the same small edit at every node: exchange its two
  children. Nothing at a node depends on what happened at its parent or its
  siblings, so any order that reaches every node works. A queue gives that order
  without recursion: each node is dequeued once, swapped, and its two (now
  swapped) children are pushed for later. When the queue empties, every node has
  been touched exactly once, and root still points at the same top node.
BRUTE FORCE
  The first version most people write is recursive: swap root.left and
  root.right, then call yourself on both children. It is also O(n) time, but it
  uses the call stack, which is as deep as the tree height - a degenerate chain
  of nodes can overflow it. Another naive idea is to build a whole new tree with
  mirrored children; correct, but it allocates n new nodes and leaves the caller
  with two trees.
INVARIANT
  At the top of each while iteration, every node already dequeued has had its
  children swapped, and every node of the tree is either already dequeued or
  reachable from some node still in queue. The swap of node.left and node.right
  through leftChild is complete before the node's children are enqueued, so no
  node is ever swapped twice and none is skipped. When queue.Count hits 0 the
  second half of the invariant leaves nothing unreachable, so every node has
  been mirrored.
ENQUEUE AFTER SWAP IS STILL CORRECT
  The code swaps first and then enqueues node.left and node.right, which are now
  the old right and old left. That is fine: the swap changes the order of the
  pair, not its membership, so the same two children get queued either way. Only
  their arrival order in queue changes, and this algorithm never depends on
  visit order.
WATCH OUT
  This mutates the input tree in place and returns the same root reference, so a
  caller holding the original tree no longer has the un-inverted version. The
  null guard only covers root being null; the two if checks inside the loop are
  what keep null children out of the queue, so removing either one causes a
  NullReferenceException on the next Dequeue. The queue can hold close to half
  the nodes at once when the bottom level is full, so peak memory is not small
  for a wide tree. If the structure is not a real tree and some node is
  reachable twice, the loop never ends.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you do it with less extra memory?
     Swap Queue for Stack and the code is otherwise identical; depth-first keeps
     only the nodes along one root-to-leaf path plus their siblings, which is
     much less than a full level on a wide tree. On a deep skewed tree the stack
     is no better.
  2. How would you mirror only the subtree below a given depth?
     Enqueue pairs of (node, depth) instead of bare nodes, and run the swap only
     when depth is at or past the cutoff; you still enqueue children at depth +
     1 so the traversal reaches everything.
  3. How would you produce the mirrored tree without touching the input?
     Walk the original and build new nodes as you go, setting the new node's
     left from the original's right and vice versa; same O(n) time, but you pay
     n allocations and must return the new root instead of root.
  4. How do you check two trees are mirrors of each other without inverting
  either one?
     Push pairs into the queue - (a.left, b.right) and (a.right, b.left) - and
     compare values at each step, failing on the first mismatch or null shape
     difference.
TRIGGER
  Reach for this when every node of a tree needs the same local edit that
  depends on no other node, so traversal order is free and an explicit queue
  replaces recursion.
C# NOTE
  The three-line swap with leftChild can be written as one tuple assignment,
  (node.left, node.right) = (node.right, node.left), which removes the temporary
  entirely. Also note Queue<T>.Dequeue throws InvalidOperationException on an
  empty queue, so the while (queue.Count > 0) guard is doing real work here;
  TryDequeue would be the alternative.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
