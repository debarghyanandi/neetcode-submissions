// ##########################################################################
// #  optimal.cs            O(n) time / O(n) space
// ##########################################################################

public class Solution
{
    private List<List<int>> res = new List<List<int>>();

    public List<List<int>> LevelOrder(TreeNode root)
    {
        //My Solution
        if (root == null)
            return res;

        Traverse(root, 0);
        return res;
    }

    private void Traverse(TreeNode root, int level)
    {
        if (root == null)
            return;
        // When we first reach a new level, level == res.Count.
        // For example, at the first node of level 1, both are 1,
        // so we create a new list for that level.

        // When we reach the next node at the same level,
        // level is still 1, but res.Count is now 2 because
        // the list for level 1 was already created.
        if (res.Count == level)
            res.Add(new List<int>());

        res[level].Add(root.val);

        Traverse(root.left, level + 1);
        Traverse(root.right, level + 1);
    }
}

/*
================================================================================
 PROBLEM : Given the root of a binary tree, return its node values level by
           level. Each level is one list, ordered left to right, from the top
           level down. An empty tree returns an empty list. Example:
           [3,9,20,null,null,15,7] -> [[3],[9,20],[15,7]]
 PATTERN : DFS (preorder) with depth index, instead of BFS
================================================================================
IDEA
  Traverse visits nodes in preorder and carries the depth as level.
  If res.Count == level, this is the first node seen at this depth,
  so a new empty list is added. Then root.val is appended to res[level].
  It is correct because left is visited before right at every node, so
  within any one level, values are appended in left-to-right order.
EXAMPLE
  Input [1,2,3,4] (4 is the left child of 2). Visit order: 1, 2, 4, 3.
  1: L0, Count 0 -> new list. 2: L1, Count 1 -> new. 4: L2, Count 2 -> new
  3: L1, Count 3 != 1 -> just append to res[1] = [2,3]
  Answer: [[1],[2,3],[4]]
COMPLEXITY
  Time  O(n)  each node is visited once, and each append is O(1)
  Space O(n)  output holds n values; recursion stack is tree height, n if
              skewed
PATH TO OPTIMAL
  Height first, then one DFS per depth d - O(n*h) - simple but repeats work.
  One DFS that passes level (this file) - O(n) - each node is touched once.
  BFS with a queue, one level per round - O(n) - same cost, no recursion;
  compare it with optimal-variant.cs.
KEYWORDS
  level order traversal, BFS, queue, DFS with depth, binary tree, levels
WATCH OUT
  - res is a class field. A second LevelOrder call on the same Solution
    object adds to the old results. Make res local, or clear it first.
  - The comment says res.Count "is now 2" at the next level-1 node. It can
    be larger (it is 3 in the example), so the test must be ==, not >=.
  - Visit left before right. Swapping them reverses every level.
  - A deep skewed tree can overflow the call stack. BFS avoids this.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you do it with BFS instead?
     -> Use a queue. Each round, read count = queue.Count, pop that many nodes
        into one list, and push their children. O(n) time, O(width) extra.
  2. Zigzag order (alternate directions)?
     -> Same traversal. Reverse every odd level at the end, or insert at the
        front on odd levels. It stays O(n).
  3. Right side view, or the average of each level?
     -> Keep the per-level grouping. Take the last value, or the sum / count
        of each level. O(n) time. DFS can also do it by visiting right first.
  4. Return the levels bottom-up?
     -> Build the lists as here, then reverse res once. Still O(n).
TRIGGER
  The answer must be grouped by depth or distance from a start node.
================================================================================
*/
