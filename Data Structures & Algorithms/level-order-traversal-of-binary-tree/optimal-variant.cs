// --------------------------------------------------------------------------
// -  optimal-variant.cs    O(n) time / O(n) space
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
 PROBLEM : Given the root of a binary tree, return its node values level by
           level. Each inner list holds one level, read left to right, top
           level first. An empty tree returns an empty list. Example:
           [3,9,20,null,null,15,7] -> [[3],[9,20],[15,7]]
 PATTERN : BFS with a queue (level-size snapshot)
================================================================================
IDEA
  Put root in queue. Each round of the while loop is one level: the for
  loop starts at i = queue.Count, so it dequeues only this level's entries.
  Each real node goes into level, and both children are enqueued, even
  null ones. Nulls are skipped when dequeued. This differs from optimal.cs,
  which checks children before it enqueues them. It is correct because a
  FIFO queue returns all of level k before any node of level k+1.
EXAMPLE
  Tree [1,2,3,null,4]: 1 has children 2 and 3, and 4 is the right child of 2.
  Rounds: [1] -> [2,3] -> queue [null,4,null,null], so level [4]
  -> queue [null,null], level is empty and is not added.
  Answer: [[1],[2,3],[4]]
COMPLEXITY
  Time  O(n)  each node is dequeued once, plus at most n+1 null entries
  Space O(n)  the queue holds the widest level plus its null children
WATCH OUT
  - Take the level size once, as i = queue.Count does. Using i < queue.Count
    as the loop test is wrong, because the queue grows inside the loop.
  - The last round sees only nulls. Without the level.Count > 0 check, the
    output ends with an extra empty [].
  - Create a new level list each round. Reusing and clearing one list
    empties the lists already stored in result.
================================================================================
*/
