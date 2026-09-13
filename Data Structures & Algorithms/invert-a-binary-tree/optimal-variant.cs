// --------------------------------------------------------------------------
// -  optimal-variant.cs    O(n) time / O(n) space
// -  iterative BFS with queue, swap children   [bfs-queue-swap]
// -  ties with optimal.cs on O(n) time / O(n) space
// -
// -  Reference solution - not one you solved yourself
// -
// -  each node enqueued and dequeued exactly once, swapping children on
// -  dequeue; queue can hold up to ~n/2 nodes at the widest level
// --------------------------------------------------------------------------

public class Solution {
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
 PATTERN : BFS level-order - swap children at each dequeued node
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-1.cs when it was first processed
 STATUS  : Optimal variant - ties the best complexity by another route
================================================================================
WHY THIS WORKS
  Mirroring is defined top-down: mirror(node) has left = mirror(old right) and
  right = mirror(old left). Unrolled over the whole tree, that is exactly
  "exchange the two child pointers of every node, exactly once."

  The swap at a node touches only that node's two fields, and no other node
  reads those fields. So no node's swap depends on whether any other node has
  been swapped yet. Any enumeration that visits every node once produces the
  same result - the queue is just one such enumeration.

  The check: after the loop, the node you reach by walking left-right-left from
  root is the node you originally reached by right-left-right. Depth is
  preserved, the path is bit-flipped.
ALGORITHM
  1. Guard root == null and return null - the only null check outside the loop.
  2. Enqueue root.
  3. While queue.Count > 0: dequeue node, stash leftChild = node.left, assign
  node.left = node.right, then node.right = leftChild.
  4. Enqueue node.left if non-null, then node.right if non-null.
  5. Return root - the same object that came in, mutated.
THE SWAP-THEN-ENQUEUE SUBTLETY
  The two enqueue guards read node.left and node.right after the assignment.
  This is safe: a swap exchanges the two fields, so the unordered pair
  {node.left, node.right} is identical before and after. The same two children
  get queued, just in mirrored order - and since order does not matter (see the
  invariant), the result is unaffected.

  The bug to watch for is enqueuing leftChild and node.right instead. The line
  node.right = leftChild has already aliased them, so you would enqueue the
  original left child twice and never descend the original right subtree at all.
  leftChild is dead the instant that assignment runs - do not reuse it below.
WHY BFS RATHER THAN RECURSION
  The recursive two-line version is shorter, but its call stack is as deep as
  the tree is tall. A degenerate 10^5-node chain - every node with only a right
  child - overflows it, while this queue holds at most one node at a time on
  that same input.

  The trade runs the other way on a perfectly balanced tree: the queue peaks at
  the widest level, roughly half the nodes, where recursion would have held only
  log n frames. Pick BFS when the input can be skewed, recursion when you know
  the tree is bushy and shallow.

  Because correctness does not depend on visit order, swapping Queue for Stack
  and Enqueue/Dequeue for Push/Pop gives an iterative DFS that is equally
  correct and bounds memory by height instead of width.
WATCH OUT
  This mutates the caller's tree in place. root is never reassigned, so the
  returned reference is the same object the caller passed in - any other
  variable pointing at that tree now sees it inverted. The return statement
  satisfies the signature; it does not hand back a copy.

  The queue never holds null, since filtering happens at enqueue. The
  alternative - enqueue unconditionally and skip nulls after dequeue - is
  equally correct but moves the branch inside the loop body; do not mix the two
  and end up with neither check.
INTERVIEW FOLLOW-UPS
  "Make it non-destructive." Then local swapping is no longer enough - you must
  allocate a new TreeNode per visit and wire its left to the copy of the old
  right. BFS still works but the queue has to carry pairs of (source node,
  destination node).

  "Is the tree a mirror of itself?" Different problem - do not invert then
  compare, that destroys the input and costs two passes. Walk two pointers down
  in tandem, comparing a.left against b.right.

  "What if nodes had parent pointers?" The swap would no longer be purely local;
  each child's parent link is unchanged here, but any sibling-order-dependent
  field would need fixing in the same visit.
TRIGGER
  Reach for this shape when the transformation at each node is independent of
  its neighbours - a pure local edit repeated over every node. The moment a
  node's update needs a value computed from its children, order stops being free
  and you need post-order recursion instead of a queue.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
