// --------------------------------------------------------------------------
// -  optimal-variant-2.cs  O(n) time / O(n) space
// -  iterative BFS with two queues, level-by-level compare   [bfs-compare]
// -  ties with optimal-variant.cs on O(n) time / O(n) space
// -
// -  Reference solution - not one you solved yourself (from submission-2)
// -
// -  Each node enqueued/dequeued once; queue width can reach O(n) at the
// -  widest level, a mechanism distinct from the stack-based DFS
// -  approaches.
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
 PATTERN : Lockstep BFS - twin queues, nulls enqueued as placeholders
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-2.cs when it was first processed
 STATUS  : Optimal variant - ties the best complexity by another route
================================================================================
WHY THIS PATTERN
  Tree equality is a positional claim: whatever sits at position k of p must
  match position k of q in both value and existence. Any traversal order proves
  that, provided both trees are walked in the identical order and missing
  children are visible rather than skipped. Two queues advanced in lockstep give
  exactly that. The recursive form does the same comparison on the call stack;
  this version moves the frontier onto the heap, so depth costs nothing - a
  10^5-node right-skewed chain walks fine here and would blow the stack in the
  naive recursion. That is the real reason to keep this variant around.
INVARIANT
  queueP.Count == queueQ.Count everywhere either queue is touched. True at entry
  (one seed each, p and q). Inside the body every iteration dequeues exactly one
  from each, and the four Enqueue calls all sit under the same branch - either
  both queues grow by two or neither grows at all. There is no path that pushes
  to one without pushing to the other. That single fact carries two things: it
  makes the bound taken from queueP.Count legal to apply to queueQ.Dequeue(),
  and it makes the && in the while condition redundant, since one queue emptying
  implies the other did.
ALGORITHM
  1. Seed queueP with p and queueQ with q. A null seed is not special-cased - it
  falls into the same null handling as any other node.
  2. Freeze the level width as i = queueP.Count, then process that many pairs.
  3. nodeP == null && nodeQ == null: this position agrees and is a dead end.
  continue, enqueue nothing.
  4. Exactly one null, or nodeP.val != nodeQ.val: the trees disagree at this
  position. return false, no need to look further.
  5. Both non-null and equal: enqueue left then right of each, nulls included,
  so the next level's slots stay aligned.
  6. Both queues drain together. Falling out of the while means every position
  on every level agreed - return true. Two null trees hit this on the first
  pass: one pair, both null, continue, queues empty, true.
NULLS ARE DATA
  Guarding the Enqueue calls with a null check would quietly break the whole
  thing. p = [1,2] and q = [1,null,2] produce the same value sequence in BFS
  order once nulls are dropped, and would compare equal despite being different
  trees. Enqueueing null pins every node's children into fixed slots, so the
  left-versus-right distinction survives into the next level and a shape
  mismatch surfaces as a one-null-one-node pair. The price is that the queue
  carries a null for every absent child - each leaf contributes two of them.
WATCH OUT
  for (int i = queueP.Count; i > 0; i--) evaluates queueP.Count once, in the
  initializer, before the body starts enqueueing into that same queue. That is
  the point of the descending form. But notice that nothing in this method uses
  the level boundary - no per-level accumulator, no depth counter, no result
  flushed at the end of a row. The inner for loop is inherited from the
  level-order template and does no work here; collapsing it to a plain while
  (queueP.Count > 0) that dequeues one pair per turn is behaviorally identical.
  The correctness rests on the pairwise lockstep, not on levels, so do not let
  the batching mislead you into thinking level alignment is what is being
  checked.
FOLLOW-UPS
  Why two queues instead of one Queue<(TreeNode, TreeNode)> of pairs? A single
  queue of tuples makes the count invariant structural instead of something you
  have to argue - there is no way to desynchronize. Say so before the
  interviewer does; the two-queue version is the one that needs the proof.
  Swap BFS for a stack and you get the iterative DFS variant with the same shape
  and the same null-placeholder requirement.
  Mirror problem (Symmetric Tree): same skeleton, but seed both queues from the
  same root's two children and enqueue in opposite orders - left.left against
  right.right, left.right against right.left.
  Subtree of Another Tree: this method becomes the inner predicate, called at
  every candidate root.
TRIGGER
  Reach for lockstep twin-queue BFS when two structures must be compared
  position by position and recursion depth is a stated or suspected risk -
  degenerate skewed trees, or an environment where stack depth is capped. If
  depth is bounded and you just want the answer, the three-line recursion is
  easier to write correctly and harder to get wrong. Choose this one for a
  reason you can name, not by default.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
