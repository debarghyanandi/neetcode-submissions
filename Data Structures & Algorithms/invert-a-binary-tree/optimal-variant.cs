// --------------------------------------------------------------------------
// -  optimal-variant.cs    O(n) time / O(n) space
// -  BFS with queue, swap children   [bfs-queue-swap]
// -  ties with optimal.cs on O(n) time / O(n) space
// -
// -  Reference solution - not one you solved yourself (from submission-1)
// -
// -  queue holds an entire tree level, which can be up to ~n/2 nodes for a
// -  balanced tree
// --------------------------------------------------------------------------

public class Solution {
    //BFS
    public TreeNode InvertTree(TreeNode root) {
        if (root == null) return null;
        Queue<TreeNode> queue = new Queue<TreeNode>();
        queue.Enqueue(root);
        while (queue.Count > 0) {
            TreeNode current = queue.Dequeue();
            TreeNode leftChild = current.left;
            current.left = current.right;
            current.right = leftChild;
            if (current.left != null) queue.Enqueue(current.left);
            if (current.right != null) queue.Enqueue(current.right);
        }
        return root;
    }
}

/*
================================================================================
 PATTERN : BFS queue - swap children at each dequeue
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-1.cs when it was first processed
 STATUS  : Optimal variant - ties the best complexity by another route
================================================================================
WHY THIS SHAPE
  Inverting a tree is a purely local edit: at each node you exchange two
  reference fields and nothing else. The traversal is therefore just an
  enumerator - it has to reach every node exactly once, and it does not care in
  what order. That is the whole reason an explicit Queue ties the recursive
  version: BFS is not doing anything clever here, it is only the iterative way
  to enumerate. Recognizing that the work is node-local is what frees you to
  pick any traversal, and it is the sentence to lead with if an interviewer asks
  why you chose BFS.
INVARIANT
  At the top of every while iteration: the queue holds exactly the nodes that
  have been discovered but whose child pointers have not yet been swapped, and
  every node already dequeued has had its two children exchanged exactly once.
  The loop body restores this - it swaps current's children, then discovers
  current's (now reordered) children and enqueues them.

  Exactly once is the load-bearing part, not at least once. Swapping is its own
  inverse, so a node processed twice cancels out and leaves that subtree upright
  inside an otherwise inverted tree. A tree gives this for free: each node has a
  unique parent, so it is enqueued by exactly one Enqueue call and there are no
  cycles to revisit. On a general graph this code would be wrong without a
  visited set.
WHY THE ENQUEUE ORDER IS IRRELEVANT
  The two guarded Enqueue calls sit after the swap, so current.left at that
  moment is the original right child. The set of nodes pushed is identical
  either way - both children go in - only their order within the level flips.
  Moving the swap below the enqueues produces the same output tree.

  The deeper reason: the swap at current writes only current.left and
  current.right. It never touches a grandchild pointer, so no node's pending
  work is disturbed by an ancestor's swap. Independent edits commute, which is
  also why pre-order DFS, post-order DFS and this queue all agree.
THE TEMP IS NOT OPTIONAL
  leftChild exists because the two assignments are sequential. Drop it and write
  current.left = current.right followed by current.right = current.left, and the
  second line reads the already-overwritten field: you get the right subtree
  hanging off both sides and the entire left subtree unreachable. If you want it
  in one line, C# tuple deconstruction does the swap safely: (current.left,
  current.right) = (current.right, current.left). Same semantics, temp hidden by
  the language.
WATCH OUT
  1. The root == null guard is load-bearing, not defensive noise. Without it a
  null root is enqueued, dequeued, and current.left throws a
  NullReferenceException on the first iteration.
  2. Nulls never enter the queue, because both Enqueue calls are guarded. That
  is why there is no null check after Dequeue - the two designs (guard on push
  vs guard on pop) are both valid, but mixing them is how you get a crash. This
  version keeps the queue smaller.
  3. Testing trap: inverting twice returns the original tree, so a test that
  asserts InvertTree(InvertTree(t)) equals t passes even for a no-op
  implementation. Assert against the inverted shape directly.
  4. The tree is mutated in place and root is the same object the caller passed
  in. Returning it is a convenience, not a copy - any other reference the caller
  holds into this tree now sees swapped children.
BFS VS THE RECURSIVE ONE-LINER
  Both do the same amount of work; the difference is which auxiliary structure
  grows and on which input shape.

  A left-skewed chain is the worst case for recursion - depth equals node count,
  and a deep enough chain throws StackOverflowException, which in .NET cannot be
  caught and kills the process. This queue holds at most one node at a time on
  that same input.

  A wide, balanced tree flips it: the queue peaks at the widest level (about
  half the nodes for a complete tree) while the recursion stack stays at the
  height. So the honest answer to "which is better" is: this one when depth is
  the risk, recursion when width is and you want fewer lines.
TRIGGER
  Reach for this template when the per-node work is local and self-contained
  (swap, mark, increment a field, collect a value) and you need every node
  reached - no ordering constraint, no information flowing up from children. If
  the answer at a node depends on results from its subtrees (height, diameter,
  balanced check, sum of subtree), this shape does not apply: you need
  post-order, where children must be finished before the parent, and a plain
  queue gives you no place to hang that pending state.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
