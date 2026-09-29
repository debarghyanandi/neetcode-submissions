// --------------------------------------------------------------------------
// -  optimal-variant-2.cs  O(n) time / O(n) space
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

                if (nodeP == null && nodeQ == null)
                    continue;
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
 PATTERN : BFS in lockstep - compare two trees level by level
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-2.cs when it was first processed
 STATUS  : Optimal variant - ties the best complexity by another route
================================================================================
VARIABLES
  queueP   the nodes of tree p still waiting to be checked, null children included
  queueQ   the nodes of tree q at the same positions as queueP, in the same order
  i        counts down the nodes left in the current level, using the size of queueP only
WHY THIS PATTERN
  Two trees are the same when every position has the same shape and the same
  value. So you must visit matching positions in both trees at the same time.
  BFS (breadth-first search: visit nodes level by level with a queue) does this
  if you move queueP and queueQ together. Each pair nodeP / nodeQ comes from the
  same place in its tree, so one comparison per pair checks the whole tree.
BRUTE FORCE
  The first thing most people write is recursive DFS: IsSameTree(p.left, q.left)
  && IsSameTree(p.right, q.right), with the null checks as base cases. It is
  correct, and its big-O is no worse than this file. Its weak point is the call
  stack. The depth equals the tree height, so a tree that is one long chain can
  overflow the stack. Another simple way is to turn both trees into strings with
  null markers and compare the strings. It is correct too, but it builds two
  full strings before it can find any difference.
INVARIANT
  Each time the loop dequeues, nodeP and nodeQ sit at the same path from the
  root in their trees. This holds because children go in only when the parents
  matched, and always in the same order: left, then right, into both queues.
  When the loop finds a mismatch it returns false right away. If it empties both
  queues with no mismatch, every position has been compared and matched, so true
  is correct.
NULL CHILDREN ARE ENQUEUED ON PURPOSE
  The code adds nodeP.left and nodeP.right even when they are null. The nulls
  hold each position open, and this is how the code checks shape. If you skipped
  the nulls, [1,2] and [1,null,2] would give the same queue contents and wrongly
  compare as equal. The check "nodeP == null || nodeQ == null" then catches a
  node that exists in one tree but not the other.
WATCH OUT
  The for loop takes its level size from queueP.Count only. That is safe only
  because both queues always get the same number of items. If you ever enqueue
  into one queue on its own, the two queues fall out of step without any error.
  The level loop also does no real work here: one plain while loop over single
  pairs gives the same result. So do not claim the solution needs level-by-level
  processing. If p and q are both null, the first dequeue hits the "both null"
  continue and the method returns true, which is correct. Keep that behavior if
  you refactor.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How would you check whether one tree is a mirror of the other (Symmetric
  Tree)?
     Keep the same lockstep loop, but pair nodeP.left with nodeQ.right and
     nodeP.right with nodeQ.left when you enqueue. The shape and value checks
     stay the same.
  2. Can you use one queue instead of two?
     Yes. Use a queue of pairs, Queue<(TreeNode, TreeNode)>. Then the two sides
     cannot fall out of step. The memory is the same, and the code is harder to
     break.
  3. How would you use this for Subtree of Another Tree?
     Run this check from every node of the big tree. That costs O(m*n) in the
     worst case. For O(m+n), turn both trees into strings with null markers and
     search with KMP (a linear-time string search). The cost is extra memory for
     the strings.
  4. When would you pick recursion over this?
     When the tree is known to be balanced. Then the stack depth is small, the
     code is three lines, and the memory is O(h) instead of the width of the
     widest level.
TRIGGER
  Two structures must be compared position by position, and you want an
  iterative walk: move two queues or stacks in lockstep and keep a null marker
  for every missing child.
C# NOTE
  new Queue<TreeNode>(new[] { p }) creates a one-item array only to seed the
  queue. new Queue<TreeNode>() followed by Enqueue(p) does the same job without
  the array. With nullable reference types turned on, declare the queues as
  Queue<TreeNode?>, because they hold nulls on purpose.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
