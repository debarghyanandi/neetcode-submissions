// --------------------------------------------------------------------------
// -  optimal-variant-2.cs  O(n) time / O(n) space
// -  iterative BFS with two queues, level-by-level compare
// -  [bfs-pairwise-compare]
// -  ties with optimal.cs on O(n) time / O(n) space
// -
// -  Reference solution - not one you solved yourself
// -
// -  each node enqueued/dequeued once from twin queues advanced in
// -  lockstep, including null placeholders to preserve shape
// --------------------------------------------------------------------------

public class Solution
{
    public bool IsSameTree(TreeNode p, TreeNode q)
    {
        var queueP = new Queue<TreeNode>(new[] { p });
        var queueQ = new Queue<TreeNode>(new[] { q });

        while (queueP.Count > 0 && queueQ.Count > 0)
        {
            for (int i = queueP.Count; i > 0; i--)
            {
                var nodeP = queueP.Dequeue();
                var nodeQ = queueQ.Dequeue();

                if (nodeP == null && nodeQ == null) continue;
                if (nodeP == null || nodeQ == null || nodeP.val != nodeQ.val)
                {
                    return false;
                }

                queueP.Enqueue(nodeP.left);
                queueP.Enqueue(nodeP.right);
                queueQ.Enqueue(nodeQ.left);
                queueQ.Enqueue(nodeQ.right);
            }
        }

        return true;
    }
}

/*
================================================================================
 PATTERN : BFS lockstep - two queues, nulls as placeholders
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-2.cs when it was first processed
 STATUS  : Optimal variant - ties the best complexity by another route
================================================================================
WHY THIS PATTERN
  Tree identity is a shape claim plus a value claim. Walking both trees in the
  same order and comparing position by position settles both at once, so the
  traversal order itself is the proof - no serialization, no hashing. queueP and
  queueQ are stepped in exact lockstep: one Dequeue from each per iteration, so
  the i-th node pulled from queueP and the i-th from queueQ are always the same
  coordinate in their respective trees.
INVARIANT
  queueP.Count == queueQ.Count at the top of every iteration. It holds initially
  (both seeded with one element) and is preserved by the body: each pass
  dequeues exactly one from each, and either enqueues two into each (the matched
  non-null case) or two into neither (the both-null continue, and the return
  false path exits). Everything else in the method leans on this. It is why
  bounding the inner loop with queueP.Count is safe for queueQ.Dequeue(), and
  why the while condition testing both counts is redundant - either test alone
  would do.
WHY THE NULLS GO IN
  queueP.Enqueue(nodeP.left) runs unconditionally, so nulls sit in the queue as
  real entries. That is what encodes shape. Drop them - enqueue only non-null
  children - and take p = node 1 with left child 2 versus q = node 1 with right
  child 2: each queue receives exactly one entry holding value 2, every
  comparison passes, and the method wrongly returns true. With nulls kept,
  queueP holds [2, null] and queueQ holds [null, 2], and the next level hits
  nodeP != null with nodeQ == null and returns false. The null placeholder is
  the only thing distinguishing a left child from a right child here.
THE THREE-WAY CHECK
  Order matters in the two guards. Both-null is tested first and continues,
  which is the accept case for a missing subtree. The second line then catches
  exactly one null (mismatched shape) and, only once both are known non-null,
  dereferences val. Reversing them would null-deref on the both-null pair. Note
  also that the seed accepts p == null and q == null with no special case: the
  first iteration compares them, continues, and the queues drain to the final
  return true.
TERMINATION
  Each non-null node is dequeued once and enqueues two entries; each null is
  dequeued once and enqueues nothing. So total enqueues are bounded at 1 + 2n
  and every entry is dequeued exactly once - the loop cannot spin. The both-null
  continue is the drain: a tree of n nodes puts n+1 nulls into the queue as its
  frontier, and those nulls consume themselves without producing successors.
WATCH OUT
  for (int i = queueP.Count; i > 0; i--) evaluates queueP.Count once, in the
  initializer. That snapshot is load-bearing: the body grows queueP, so a
  condition of the form i < queueP.Count re-read each pass would chase a moving
  target and never exit. Also note the level grouping this for loop creates is
  decorative - nothing in the body reads the level boundary, and collapsing it
  to a flat while over the queue would behave identically.
WHY THIS ROUTE OVER RECURSION
  The recursive DFS on this problem does the same work, but its stack depth
  tracks tree height, so a 10^4-node right spine risks blowing the call stack.
  Here a skewed tree keeps the queue at roughly two entries per level, and the
  peak cost lands on the widest level instead - a real trade, not a wash. The
  price is bookkeeping: two explicit containers and the count invariant to
  maintain, versus one line of recursion.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
