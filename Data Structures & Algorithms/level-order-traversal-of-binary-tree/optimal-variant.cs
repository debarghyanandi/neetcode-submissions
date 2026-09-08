// --------------------------------------------------------------------------
// -  optimal-variant.cs    O(n) time / O(n) space
// -  BFS with queue, processing level by level using queue size snapshot
// -  [bfs-queue-level-size]
// -  ties with optimal.cs on O(n) time / O(n) space
// -
// -  Reference solution - not one you solved yourself (from submission-1)
// -
// -  Each node enqueued/dequeued once; queue holds up to O(n) nodes worst
// -  case (e.g. last level of a complete tree).
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
 PATTERN : BFS with per-level count snapshot; nulls ride the queue
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-1.cs when it was first processed
 STATUS  : Optimal variant - ties the best complexity by another route
================================================================================
LEVEL BOUNDARY
  The whole trick is one line: for (int i = queue.Count; i > 0; i--). The
  initializer runs exactly once, so i freezes the queue size at the moment the
  level begins. Every child enqueued inside the body lands after that frozen
  window and is therefore deferred to the next while pass. That single snapshot
  is what separates depth d from depth d+1 - there is no depth counter, no
  sentinel node, no pairing of (node, depth) tuples.
INVARIANT
  At the top of every while iteration, queue holds exactly the left/right slots
  of all real nodes at the previous depth, in left-to-right order. Some of those
  slots are null. The non-null ones are precisely the nodes at the current
  depth. The loop body consumes that whole run, appends every non-null node.val
  to level, and refills the queue with the next depth's slots - re-establishing
  the invariant. FIFO order plus enqueueing node.left before node.right is what
  makes level come out left-to-right; nothing sorts it afterward.
THE NULL-TOLERANT CHOICE
  This variant enqueues node.left and node.right unconditionally and filters at
  the dequeue with if (node != null). The consequence to remember: queue.Count
  is not the width of the level - it is the width plus the null slots. Both
  readings work because the count is only used as a boundary, never as a node
  tally. Cost of the choice: the queue can hold up to twice the widest level,
  since every real node contributes two slots regardless of whether they are
  children. The gain: exactly one null test in the method instead of two guards
  around the enqueues.
WHY THE EMPTY-LEVEL GUARD EXISTS
  if (level.Count > 0) is not defensive padding - it is load-bearing. The leaf
  row enqueues 2 * (number of leaves) nulls, so the while condition is still
  true one pass after the last real level. That final pass dequeues only nulls,
  builds an empty level, and would append an empty list to result without the
  guard. The same pass drains the queue (nulls enqueue nothing), which is also
  what terminates the loop.
A REDUNDANT LINE
  The if (root == null) return result; early exit is dead weight given the rest
  of the code. Trace it: queue gets one null, the while runs once, node is null
  so nothing is added, level.Count is 0 so nothing is appended, and the empty
  result is returned anyway. Worth knowing it is removable - and worth knowing
  that in the null-checked-before-enqueue sibling, that guard is not removable,
  because there a null root would be dequeued and dereferenced.
THE TRAP
  Writing for (int i = 0; i < queue.Count; i++) instead. That re-reads
  queue.Count every iteration, so children enqueued during the level extend the
  same loop and the levels bleed together - you get one flat list of the whole
  tree in BFS order, split at arbitrary points. The countdown form makes the
  snapshot impossible to get wrong by accident.
FOLLOW-UPS THIS SETS UP
  Zigzag order: reverse level on odd depths, or insert at index 0 - the boundary
  logic is untouched. Right-side view: emit the last element of each level.
  Maximum width: track level.Count per pass, which here means the guard already
  gives you the non-null count for free. Level averages: sum level before
  appending. All of them are one change inside the if (level.Count > 0) block.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
