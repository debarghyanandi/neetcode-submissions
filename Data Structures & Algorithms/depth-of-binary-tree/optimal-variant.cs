// --------------------------------------------------------------------------
// -  optimal-variant.cs    O(n) time / O(n) space
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
 PROBLEM : Given the root of a binary tree, return its maximum depth. Depth is
           the number of nodes on the longest path from the root down to a
           leaf. An empty tree has depth 0. Example: [3,9,20,null,null,15,7]
           -> 3.
 PATTERN : BFS (level-order traversal) with level counting
================================================================================
IDEA
  Walk the tree one level at a time with a queue. At the start of each round,
  levelSize stores how many nodes are on the current level. We dequeue exactly
  that many and enqueue their non-null children, then add 1 to level. When the
  queue is empty, level is the number of levels, which is the max depth.
  optimal.cs uses DFS recursion instead.
EXAMPLE
  Tree [3,9,20,null,null,15,7]:
  queue [3], levelSize 1 -> level 1; queue [9,20], levelSize 2 -> level 2
  queue [15,7], levelSize 2 -> level 3; queue empty, loop ends
  Answer: 3
COMPLEXITY
  Time  O(n)  each node is enqueued and dequeued exactly once
  Space O(n)  queue holds one full level, up to about n/2 leaves
WATCH OUT
  - Save levelSize before the for loop. If the loop tests queue.Count, it
    also counts children added in this round, and levels merge together.
  - Increment level once per round, after the for loop, not once per node.
  - If you enqueue null children, you must skip them on dequeue, or
    node.left throws. This code avoids that by checking for null first.
  - The if (root != null) check keeps an empty tree at 0 and avoids a crash.
================================================================================
*/
