// --------------------------------------------------------------------------
// -  optimal-variant.cs    O(n) time / O(n) space
// -  BFS level-order traversal, count levels   [bfs-level-order]
// -  ties with optimal.cs on O(n) time / O(n) space
// -
// -  Reference solution - not one you solved yourself (was optimal.cs)
// -
// -  queue visits each node once; worst-case queue size scales with the
// -  widest level, up to O(n)
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
 PATTERN : BFS level-order - count levels, not depths
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-1.cs when it was first processed
 STATUS  : Optimal variant - ties the best complexity by another route
================================================================================
WHY THIS PATTERN
  Max depth is the number of levels in the tree, so you never need a per-node
  depth value at all. If you can drain the queue one full level at a time, the
  answer is just how many times you drained it. That reframing is the whole
  solution: level is a counter of completed drains, not a maximum of anything.
INVARIANT
  At the top of each while iteration, queue holds exactly the nodes at depth
  level+1 (1-indexed), and nothing else.

  Base: before the first iteration queue holds root alone (depth 1) and level ==
  0.
  Step: the inner for loop runs exactly levelSize times, dequeuing every node of
  the current level and enqueuing every non-null child. Those children are
  precisely the next level, so after level++ the invariant holds again.
  Exit: queue empties only when the last level produced no children, so level
  equals the count of levels, which is the max depth.
ALGORITHM
  1. Enqueue root, but only if it is non-null. level starts at 0.
  2. While queue is non-empty: capture levelSize = queue.Count BEFORE touching
  the queue.
  3. Loop i from 0 to levelSize-1: dequeue node, enqueue node.left and
  node.right if each is non-null.
  4. level++ once per outer iteration, after the level is fully drained.
  5. Return level.
WATCH OUT
  levelSize must be snapshotted. Writing for (int i = 0; i < queue.Count; i++)
  instead reads a Count that shrinks by one dequeue and grows by up to two
  enqueues on every pass, so the loop bleeds into the next level and level stops
  counting levels. This is the single line the whole correctness argument rests
  on.

  The null guards on enqueue are load-bearing too, not defensive noise. If nulls
  entered the queue, queue.Count would no longer be a count of real nodes,
  levelSize would overcount, and the final drain of an all-null level would add
  a phantom level to the answer.

  The root == null guard is what makes the empty tree return 0: skip the
  enqueue, the while loop never runs, level stays 0. Enqueuing an unchecked root
  would instead crash on node.left.
DFS TRADE-OFF
  The recursive form, 1 + Max(MaxDepth(root.left), MaxDepth(root.right)), ties
  this on time and is shorter. What it holds is different: the call stack grows
  with the tree's height, while this queue grows with the tree's widest level.
  On a degenerate list-shaped tree of 10^5 nodes, recursion risks a stack
  overflow and this queue never holds more than one node. On a perfect tree the
  positions flip - the queue holds roughly half the nodes at the bottom level
  while the stack stays at log n. Pick by the shape you expect, and say so when
  asked.
FOLLOW-UP HOOK
  You can also do BFS with (node, depth) pairs and track a running max, skipping
  levelSize entirely. Same asymptotics, but it stores a depth alongside every
  queued node and loses the level boundary. Keep the levelSize form: the moment
  a question asks for anything per level - level order lists, right side view,
  minimum depth via early return at the first leaf - the boundary is already
  there. Minimum depth in particular is where BFS beats DFS outright, since you
  can return level as soon as you dequeue a node with no children instead of
  exploring the whole tree.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
