// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(n) space
// -  BFS level-order traversal counting levels   [bfs-level-order]
// -  ranks above optimal-variant.cs (O(n) time / O(n) space)
// -
// -  Reference solution - not one you solved yourself (from submission-1)
// -
// -  visits each node once via queue; queue can hold up to O(n) nodes in
// -  the worst-case (widest level)
// --------------------------------------------------------------------------

//Breadth-First Search (BFS) find level.
public class Solution
{
    public int MaxDepth(TreeNode root)
    {
        Queue<TreeNode> queue = new Queue<TreeNode>();
        if (root != null)
        {
            queue.Enqueue(root);
        }

        int level = 0;
        while (queue.Count > 0)
        {
            int levelSize = queue.Count;
            for (int i = 0; i < levelSize; i++)
            {
                TreeNode node = queue.Dequeue();
                if (node.left != null)
                {
                    queue.Enqueue(node.left);
                }
                if (node.right != null)
                {
                    queue.Enqueue(node.right);
                }
            }
            level++;
        }
        return level;
    }
}

/*
================================================================================
 PATTERN : BFS level-order - count levels via frontier snapshot
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-1.cs when it was first processed
 STATUS  : Optimal
================================================================================
WHY THIS SHAPE
  Depth is a property of levels, not of individual nodes, so the traversal that
  materializes one level at a time answers it directly: the answer is just how
  many times you drained the frontier. Nothing here inspects a node's value or
  identity - node is dequeued only to harvest its two children. That is the tell
  that this is a counting problem riding on top of a traversal, and it is why no
  depth is ever stored per node.
THE ONE LINE THAT MATTERS
  int levelSize = queue.Count; taken BEFORE the inner for loop. The inner loop
  enqueues children into the same queue it is draining, so queue.Count is moving
  the whole time. Hoisting it into levelSize freezes the boundary between the
  current level and the next one. Everything else in the method is bookkeeping
  around that snapshot.
INVARIANT
  At the top of every while iteration: the queue contains exactly the nodes at
  depth level+1 (1-indexed), left to right, and nothing else; level holds the
  number of levels already fully drained. The inner for restores the invariant
  by removing all levelSize nodes of the current level and appending precisely
  their children, which are the entire next level. level++ then re-syncs the
  counter to the new queue contents. When the queue empties, the last level had
  no children, so level is the count of levels on the deepest root-to-leaf path.
NULL HANDLING
  Two separate guards, both load-bearing. The root != null check means an empty
  tree never enqueues anything, the while body never runs, and 0 is returned by
  fallthrough rather than by a special case. Inside the loop, children are
  null-tested before Enqueue, so the queue holds only real nodes and node.left /
  node.right are always safe to touch. If nulls were enqueued instead, levelSize
  would count phantom slots and the dequeued null would fault on node.left.
WATCH OUT
  1. Writing for (int i = 0; i < queue.Count; i++) instead of using levelSize.
  The loop then chases a queue that grows as it shrinks, swallows the entire
  tree in one pass, and returns 1 for any non-empty tree. This is the classic
  mutation of the exact bug this template exists to prevent.
  2. Incrementing level inside the inner for. That counts nodes, not levels.
  3. Moving level++ before the drain while also seeding the queue
  unconditionally - an empty root would then report 1.
INTERVIEWER FOLLOW-UPS
  Q: why not the three-line recursion 1 + Max(MaxDepth(root.left),
  MaxDepth(root.right))? Both visit every node once. The difference is what the
  auxiliary memory scales with: the recursion's call stack scales with tree
  height, this queue scales with the widest level. A degenerate chain of n nodes
  keeps at most one node in this queue but n frames on the stack, which is where
  the recursion risks a StackOverflowException; a complete tree inverts that,
  since the bottom level alone is about half the nodes. BFS is the safe pick
  when depth is unbounded and the tree may be skewed.
  Q: adapt this to minimum depth? Then the level structure pays off properly -
  inside the inner loop, return level + 1 the moment a dequeued node has neither
  child, because BFS reaches the shallowest leaf first and can stop early. Here
  there is no early exit to be had: max depth requires seeing the whole tree
  either way.
TRIGGER
  Reach for the levelSize snapshot whenever the question says level, depth, row,
  or width - level averages, right-side view, zigzag order, widest level, min
  depth. The queue plus hoisted count plus per-level for loop is one reusable
  skeleton; only the work done inside the inner loop and the value returned
  change.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
