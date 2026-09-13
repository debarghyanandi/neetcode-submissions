// --------------------------------------------------------------------------
// -  optimal-variant.cs    O(n) time / O(n) space
// -  BFS level-order traversal counting levels   [bfs-level-order-count]
// -  ties with optimal.cs on O(n) time / O(n) space
// -
// -  Reference solution - not one you solved yourself
// -
// -  each node enqueued/dequeued once; queue holds at most the widest
// -  level, O(n) worst case
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
 PATTERN : BFS level-order - count levels via size snapshot
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-1.cs when it was first processed
 STATUS  : Optimal variant - ties the best complexity by another route
================================================================================
WHY THIS PATTERN
  The short route to max depth is recursion: 1 + max(MaxDepth(left),
  MaxDepth(right)). This file deliberately takes the iterative road. The win is
  that nothing here recurses, so a degenerate tree - 100k nodes each with only a
  left child - cannot overflow the call stack the way the recursive version can.
  The cost is the queue: this trades call frames for heap storage, and for a
  bushy tree the queue holds a whole bottom level at once, which the recursive
  version never materializes. Neither wins outright; pick by the shape you fear.
INVARIANT
  At the top of every while iteration, queue contains exactly the non-null nodes
  at one depth, and nothing else. level counts the depths already fully drained.
  Each pass of the body consumes precisely that depth and enqueues precisely the
  next one, preserving the invariant. Therefore when queue.Count hits 0, no
  further depth exists, and level equals the number of depths that held at least
  one node - which is the max depth.
ALGORITHM
  1. If root is non-null, seed queue with it; otherwise leave queue empty.
  2. While the queue is non-empty: snapshot levelSize = queue.Count.
  3. Dequeue exactly levelSize nodes. For each, enqueue node.left and node.right
  if non-null.
  4. Increment level once after the inner for loop, not once per node.
  5. Return level.
WHY LEVELSIZE IS SNAPSHOTTED
  This single line carries the whole algorithm. queue.Count is read once, before
  the for loop, and stored. The loop body mutates the same queue it is draining,
  so if the loop condition re-read queue.Count each iteration it would keep
  chasing the newly enqueued children, drain the entire tree in one pass, and
  return 1 for every non-empty tree. The frozen levelSize is the boundary marker
  that tells one depth from the next; there is no sentinel node and no depth
  stored per node because the count does that job.
EDGE CASES
  Null root: the guard skips the Enqueue, the while never runs, and 0 is
  returned - correct, and note that without the guard a null would be enqueued
  and the first Dequeue would dereference it. Single leaf: one pass, both
  children null so nothing is enqueued, level becomes 1. The null checks sit at
  the enqueue site, not the dequeue site, which is why node is used unguarded
  after Dequeue - the queue is guaranteed to contain no nulls.
INTERVIEW FOLLOW-UP
  If the question flips to min depth, BFS stops being a tie and becomes strictly
  better: return level + 1 the moment you dequeue a node with node.left == null
  && node.right == null, because BFS reaches the shallowest leaf before
  descending further. Max depth has no such early exit - every node must be
  visited either way. If asked for the values level by level, this skeleton is
  already the answer: allocate a List of size levelSize inside the while and add
  node.val in the for loop.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
