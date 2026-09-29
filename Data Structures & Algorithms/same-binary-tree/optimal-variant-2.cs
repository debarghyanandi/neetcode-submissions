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
 PROBLEM : You get the roots of two binary trees, p and q. Return true if the
           trees have the same shape and the same value in every matching
           node, else false. Example: p = [1,2,3], q = [1,2,3] -> true; p =
           [1,2], q = [1,null,2] -> false.
 PATTERN : BFS (two queues in lockstep, nulls kept)
================================================================================
IDEA
  Walk both trees level by level with queueP and queueQ, moving together.
  Each step dequeues one nodeP and one nodeQ. If both are null, skip them.
  If only one is null or the vals differ, return false. Otherwise enqueue
  both children of each, including null children. Keeping nulls makes both
  queues follow the same order, so position i in queueP always matches
  position i in queueQ. Unlike optimal.cs, this is iterative, not recursive.
EXAMPLE
  p = [1,2], q = [1,null,2]
  Level 1: nodeP=1, nodeQ=1 -> same. queueP=[2,null], queueQ=[null,2]
  Level 2: nodeP=2, nodeQ=null -> only one is null -> return false
COMPLEXITY
  Time  O(n)  every node and every null child is dequeued at most once
  Space O(n)  the queues can hold a whole level, up to about n/2 nodes plus
              nulls
WATCH OUT
  - Do not skip null children when you enqueue. Then [1,2] and [1,null,2]
    both give the same queue order and the code wrongly returns true.
  - Check "both null" first, then "one null". If you read nodeP.val before
    the null check, you get a NullReferenceException.
  - The inner for loop uses only queueP.Count. This works because both
    queues always get the same number of enqueues. The level loop is not
    needed for correctness.
================================================================================
*/
