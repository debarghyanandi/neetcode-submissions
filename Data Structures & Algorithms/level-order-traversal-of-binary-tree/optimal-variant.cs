// --------------------------------------------------------------------------
// -  optimal-variant.cs    O(n) time / O(n) space
// -  BFS with queue, level-by-level using queue size snapshot
// -  [bfs-queue-level-size]
// -  ties with optimal.cs on O(n) time / O(n) space
// -
// -  Reference solution - not one you solved yourself
// -
// -  Each node enqueued/dequeued once (plus null children), queue holds up
// -  to O(n) entries at the widest level.
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
 PATTERN : BFS by level - null-padded queue, snapshotted count
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-1.cs when it was first processed
 STATUS  : Optimal variant - ties the best complexity by another route
================================================================================
INVARIANT
  At the top of every while pass, the queue holds exactly the child slots
  produced by the previous pass: each real node of the current level,
  interleaved with null placeholders where a parent was missing a child. Nothing
  from an older level is still in there, and nothing from the next level has
  been added yet. So queue.Count at that instant is precisely the number of
  dequeues needed to drain this level - the level boundary is a number, never a
  marker you have to search for.
WHY THE COUNTDOWN LOOP IS THE WHOLE TRICK
  for (int i = queue.Count; i > 0; i--) evaluates queue.Count once, in the
  initializer, before any child is enqueued. i is a frozen copy of the level
  width; the condition i > 0 compares against a local, so the bound cannot drift
  while the body grows the queue. Write it the natural-looking way instead - for
  (int i = 0; i < queue.Count; i++) - and the bound slides forward every time
  you enqueue two children, so the loop never exits until the whole tree is
  drained and result collapses to a single level containing every node.
THE NULL-PADDED QUEUE
  This version enqueues node.left and node.right unconditionally and moves the
  null test to the dequeue side (if (node != null)). Two consequences follow
  directly. First, every leaf contributes two nulls, so the queue carries up to
  about twice the widest level's worth of entries rather than exactly the widest
  level. Second, after the deepest real level is processed the queue is
  non-empty - it holds only nulls - so the while loop runs one extra phantom
  pass that dequeues them all and builds an empty level. That pass is inherent
  to the design, not a defect.
THE TWO GUARDS ARE NOT EQUALLY LOAD-BEARING
  if (level.Count > 0) is doing real work: it is what swallows the phantom pass.
  Delete it and result ends with a spurious empty list on every non-empty input.
  The early if (root == null) return result is, by contrast, redundant. Trace
  it: a null root gets enqueued, the single pass dequeues it, the node != null
  test skips it, level stays empty, the guard drops it, the queue is now empty
  and the while exits with an empty result. Same answer. Keep the check as a
  statement of intent and a one-comparison fast path, but do not claim it as the
  reason null input is safe - the guard below is.
TRACE ON 3 / 9 20 / NULL NULL 15 7
  1. queue [3]. i=1: pop 3, level [3], push 9,20. result [[3]].
  2. queue [9,20]. i=2: pop 9, level [9], push null,null; pop 20, level [9,20],
  push 15,7. result gains [9,20]. queue [null,null,15,7].
  3. i=4: two nulls popped and discarded, then 15 and 7 append to level and push
  four nulls. result gains [15,7]. queue is four nulls.
  Note what pass 3 proves: the padding nulls left by level 2 sit in the same
  queue pass as the real nodes of level 3, and because i was snapshotted at 4
  they are consumed by exactly that pass rather than leaking into the next one.
  4. i=4: four nulls, level empty, guard discards, queue empty, loop exits.
WATCH OUT
  The null-sentinel variant of level-order BFS - enqueue one null after each
  level and treat a dequeued null as end-of-level - is incompatible with this
  file. Here null already means "absent child", so a sentinel null would be
  indistinguishable from padding and every level boundary would fire at the
  wrong place. If you want to switch to sentinels, you must first go back to
  testing node.left != null before enqueueing. Pick one meaning for null in the
  queue and hold it.
FOLLOW-UPS THIS SHAPE INVITES
  Every per-level variant reuses the snapshotted for loop untouched and changes
  only what happens to level. Bottom-up order: result.Insert(0, level), or build
  as here and reverse at the end. Zigzag: level.Reverse() on alternate passes,
  toggled by result.Count % 2. Right-side view: take the last element of level.
  Maximum width: compare level.Count. Average per level: sum level. The only
  thing the interviewer can attack is the boundary logic, and that is the one
  line you must be able to justify from memory.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
