// ##########################################################################
// #  optimal.cs            O(n) time / O(n) space
// ##########################################################################

public class Solution
{
    public List<int> RightSideView(TreeNode root)
    {
        //My Solution
        Queue<TreeNode> queue = new Queue<TreeNode>();

        List<int> ans = new List<int>();

        if (root != null)
        {
            queue.Enqueue(root);
        }

        while (queue.Count > 0)
        {
            int levelSize = queue.Count;

            for (int i = 0; i < levelSize; i++)
            {
                TreeNode node = queue.Dequeue();

                if (i == levelSize - 1) // last node of every level
                {
                    ans.Add(node.val);
                }

                if (node.left != null)
                {
                    queue.Enqueue(node.left);
                }

                if (node.right != null)
                {
                    queue.Enqueue(node.right);
                }
            }
        }
        return ans;
    }
}

/*
================================================================================
 PROBLEM : Given the root of a binary tree, imagine standing on its right
           side. Return the values of the nodes you can see, from top to
           bottom. That is the rightmost node of each level, even if it is a
           left child. Example: [1,2,3,null,5,null,4] -> [1,3,4].
 PATTERN : BFS level-order traversal (take last node per level)
================================================================================
IDEA
  Run BFS with a queue, one level at a time.
  Before each level, save levelSize = queue.Count, so the loop handles
  exactly that level. Children are enqueued left then right, so the node
  with i == levelSize - 1 is the rightmost one on the level. Add its val to
  ans. This is correct because BFS keeps each level in left-to-right order.
EXAMPLE
  Tree [1,2,3,4]: 1 has children 2 and 3, and 4 is the left child of 2.
  Level 0 queue [1] -> last is 1. Level 1 [2,3] -> last is 3.
  Level 2 [4] -> last is 4 (a left child, but still seen from the right).
  Answer: [1,3,4].
COMPLEXITY
  Time  O(n)  each node is enqueued and dequeued exactly once
  Space O(n)  the queue can hold a whole level, up to about n/2 nodes
PATH TO OPTIMAL
  Store every level in its own list, then take the last of each - O(n) /
  O(n) - works, but keeps lists you never use.
  This file: same BFS, but only record i == levelSize - 1 - O(n) / O(n).
  optimal-variant.cs: same O(n), a different route to the same answer.
KEYWORDS
  binary tree, BFS, level order traversal, queue, right side view, DFS
WATCH OUT
  - Do not just follow node.right from the root. That misses levels where
    the rightmost node is a left child, like 4 in [1,2,3,4].
  - Save levelSize before the for loop. Reading queue.Count inside the loop
    is wrong, because the children enqueued there change the count.
  - If you enqueue right before left, the visible node is i == 0, not the
    last one. The check must match the enqueue order.
  - An empty tree must return an empty list. The root != null guard does it.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How do you get the left side view?
     -> Keep the same BFS and record the node at i == 0. Time and space stay
        O(n).
  2. Can you do it with DFS?
     -> Visit right before left and pass the depth. Add node.val when depth ==
        ans.Count. O(n) time, O(h) stack space, so it is better on wide trees but
        worse on very deep ones.
  3. Why is BFS a natural fit here?
     -> The answer is defined per level, and levelSize gives clear level
        borders. So "last of each level" is a one-line check.
  4. How do you return the bottom or top view instead?
     -> Track a column index (left -1, right +1) and keep one value per column
        in a map. Then sort the columns: O(n log n), or O(n) if you track the min
        and max column.
TRIGGER
  The problem asks for one value per depth of a tree (first, last, max,
  average), so reach for BFS level order with a saved levelSize.
================================================================================
*/
