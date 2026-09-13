// --------------------------------------------------------------------------
// -  optimal-variant.cs    O(n) time / O(n) space
// -  BFS with queue, swap children   [bfs-queue-swap]
// -  ties with optimal.cs on O(n) time / O(n) space
// -
// -  Reference solution - not one you solved yourself
// -
// -  each node enqueued and dequeued exactly once, swapping its two
// -  children on dequeue; queue peaks at the widest level, up to ~n/2 for a
// -  balanced tree
// --------------------------------------------------------------------------

public class Solution {
    //BFS
    public TreeNode InvertTree(TreeNode root)
    {
        if (root == null) return null;
        Queue<TreeNode> queue = new Queue<TreeNode>();
        queue.Enqueue(root);
        while (queue.Count > 0)
        {
            TreeNode node = queue.Dequeue();
            TreeNode leftChild = node.left;
            node.left = node.right;
            node.right = leftChild;
            if (node.left != null) queue.Enqueue(node.left);
            if (node.right != null) queue.Enqueue(node.right);
        }
        return root;
    }
}

/*
================================================================================
 PATTERN : Iterative BFS - swap each node's children at dequeue
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-1.cs when it was first processed
 STATUS  : Optimal variant - ties the best complexity by another route
================================================================================
WHY THIS PATTERN
  Inverting a tree is a purely local edit: every node needs the same
  constant-time change, swapping its two child pointers. Nothing about one
  node's swap depends on the result of another's. So the algorithm reduces to
  "visit every node exactly once, apply the edit" - and any complete traversal
  will do. The queue here is just the visit-everything driver, chosen because it
  needs no call stack.
INVARIANT
  At the top of every while iteration: every node already dequeued has had its
  children swapped, and every node sitting in queue has not yet been touched but
  is reachable and will be. The queue also holds each node at most once - in a
  tree each node has exactly one parent, so it can only ever be enqueued by that
  parent's iteration, and root is seeded once before the loop. That is what
  makes "each node swapped exactly once" true, and why the loop terminates: each
  iteration removes one node and adds only nodes never before added.
CORRECTNESS - WHY ORDER IS FREE
  The mirror of a tree is defined recursively (new left = mirror of old right),
  which makes it look like the order of work matters. It does not. Swapping
  node.left and node.right rearranges the arrangement of node's descendants but
  does not change which nodes are descendants of node - the same two subtrees
  hang off it either way. So the set of pending swaps is identical no matter
  when node is processed. Since every node gets swapped exactly once (see the
  invariant), the final tree is the mirror regardless of whether you go
  breadth-first, depth-first, or in any arbitrary order. That is a genuinely
  useful thing to be able to say out loud in an interview.
WATCH OUT
  1. leftChild is not optional. Writing node.left = node.right first destroys
  the only reference to the original left subtree; the temp is the entire reason
  this works.
  2. The two enqueues read node.left and node.right AFTER the swap, so they
  enqueue the old right subtree then the old left subtree. Harmless - it is the
  same pair of nodes, just visited in the opposite order within the level, and
  by the argument above order is irrelevant.
  3. The null guards on enqueue are load-bearing, not cosmetic. A null in the
  queue would be dequeued and immediately dereferenced at node.left, throwing
  NullReferenceException.
  4. The root == null early return handles the empty tree; without it
  queue.Enqueue(root) would put a null in and hit the same crash on the first
  iteration.
  5. This mutates the caller's tree in place and returns the same root object,
  not a copy. Any reference the caller held is now pointing at an inverted tree.
VERSUS THE RECURSIVE VERSION
  The obvious solution is a three-line recursive DFS. It is shorter and does the
  same amount of work. The one real difference is where the bookkeeping lives:
  recursion consumes call stack proportional to tree height, so a degenerate
  chain-shaped tree can blow the stack, while this version's queue is bounded by
  the widest level and never touches the stack. The tradeoff runs the other way
  on a balanced tree, where recursion depth stays logarithmic but the queue can
  hold roughly half the nodes at the bottom level. Neither dominates; pick BFS
  when you are worried about depth, recursion when you are worried about width.
TRIGGER
  Reach for this shape whenever a tree transform is node-local and
  order-independent - mirror, mark every node, sum or clamp values, attach
  parent pointers. The tell is that you can describe the operation on a single
  node without referring to what any other node became. If the operation DOES
  depend on a child's post-transform state (computing heights, pruning subtrees,
  returning a value upward), this bare queue loop is the wrong tool and you want
  a post-order DFS instead.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
