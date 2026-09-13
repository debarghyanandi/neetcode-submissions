// --------------------------------------------------------------------------
// -  optimal-variant.cs    O(n) time / O(n) space
// -  BFS with queue, snapshotting queue size per level
// -  [bfs-queue-level-size]
// -  ties with optimal.cs on O(n) time / O(n) space
// -
// -  Reference solution - not one you solved yourself
// -
// -  Each node (plus null children) enqueued/dequeued exactly once; queue
// -  holds up to O(n) entries at the widest level, giving O(n) time and
// -  space.
// --------------------------------------------------------------------------

public class Solution
{
    public List<List<int>> LevelOrder(TreeNode root)
    {
        List<List<int>> result = new List<List<int>>();

        if (root == null)
            return result;

        Queue<TreeNode> queue = new Queue<TreeNode>();
        queue.Enqueue(root);

        while (queue.Count > 0)
        {
            List<int> level = new List<int>();

            for (int i = queue.Count; i > 0; i--)
            {
                TreeNode node = queue.Dequeue();
                if (node != null)
                {
                    level.Add(node.val);
                    queue.Enqueue(node.left);
                    queue.Enqueue(node.right);
                }
            }
            if (level.Count > 0)
            {
                result.Add(level);
            }
        }
        return result;
    }
}

/*
================================================================================
 PATTERN : BFS queue with null placeholders, size snapshot per level
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-1.cs when it was first processed
 STATUS  : Optimal variant - ties the best complexity by another route
================================================================================
WHAT MAKES THIS THE VARIANT
  The usual BFS tests a child for null before enqueueing it. This one does the
  opposite: queue.Enqueue(node.left) and queue.Enqueue(node.right) fire
  unconditionally, so the queue is allowed to hold nulls, and the filtering
  happens on the consuming side with if (node != null). That single move
  relocates the null test from producer to consumer, and correctness then rests
  entirely on two guards further down: the if (node != null) skip and the if
  (level.Count > 0) skip. Recall the file by that trade, not by the BFS.
INVARIANT
  At the top of every while iteration, the queue holds exactly the child slots
  produced by the previous level: one entry for every real node at the current
  depth, plus one null entry for every missing child of the level above. So
  queue.Count at that moment is a slot count, not a node count. Reading it into
  i makes the for loop drain precisely that frontier; everything enqueued during
  the drain sits behind the boundary and belongs to the next iteration.
WHY THE LOOP BOUND IS CORRECT
  for (int i = queue.Count; i > 0; i--) evaluates queue.Count once, in the
  initializer, and then counts down against a fixed number. The two Enqueue
  calls in the body grow the queue without moving the target. Write it instead
  as for (int i = 0; i < queue.Count; i++) and Count is re-read on every test,
  so the children you just appended get pulled into the same level list and the
  whole tree collapses into one row. This is the most likely follow-up question
  on the file: state that the bound is a snapshot, not a live read.
WHY LEVEL.COUNT > 0 IS LOAD-BEARING
  The deepest real level enqueues only nulls - one or two per leaf, never zero.
  The queue is therefore non-empty, so while runs one extra time, dequeues that
  all-null frontier, adds nothing to level, and enqueues nothing. Without the
  guard, result would end with a stray empty list and be wrong by one element.
  That same all-null pass is also what terminates the algorithm: a null dequeue
  produces no successors, so the queue drains and cannot refill.
THE ROOT NULL CHECK IS REDUNDANT
  if (root == null) return result is defensive, not required. Delete it and the
  null-root case still works: the queue starts as [null], the first pass
  dequeues it, if (node != null) skips it, level stays empty, if (level.Count >
  0) drops it, the queue is empty, and an empty result is returned. Worth
  knowing so you can answer honestly if asked which checks are structural - only
  the two inside the loop are.
WHAT THIS COSTS
  Total enqueues are roughly 2n + 1 rather than n, and the queue peaks near
  twice the maximum tree width because the null siblings occupy slots alongside
  the real ones. The asymptotics do not move, but if an interviewer asks what
  you would change, the answer is to test each child before enqueueing: that
  removes the null filter, removes the empty-level guard, removes the extra
  trailing pass, and halves peak queue occupancy. This variant exists to show
  the pattern, not to beat the direct form.
TRIGGER
  Reach for snapshot-the-queue-size BFS whenever the output is grouped by depth
  rather than flattened - right side view, level averages, zigzag ordering,
  minimum depth, bottom-up levels. The instant the problem says per level, the
  fixed-count inner drain is the shape you want; the level list that gets
  appended to result is just the accumulator hung off it.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
