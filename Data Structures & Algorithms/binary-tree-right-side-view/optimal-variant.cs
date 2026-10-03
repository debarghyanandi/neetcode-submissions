// --------------------------------------------------------------------------
// -  optimal-variant.cs    O(n) time / O(n) space
// --------------------------------------------------------------------------

public class Solution
{
    public List<int> RightSideView(TreeNode root)
    {
        List<int> ans = new List<int>();

        Dfs(root, 0, ans);

        return ans;
    }

    private void Dfs(TreeNode node, int level, List<int> ans)
    {
        if (node == null)
            return;

        // We reached this level for the first time.
        if (level == ans.Count)
        {
            // this is the rightmost node because we visit right first.
            ans.Add(node.val);
        }

        // Visit right before left
        Dfs(node.right, level + 1, ans);
        Dfs(node.left, level + 1, ans);
    }
}

/*
================================================================================
 PROBLEM : Given the root of a binary tree, imagine standing on its right
           side. Return the values you can see, from top to bottom. That is
           one value per level: the rightmost node on that level. Example:
           [1,2,3,4] -> [1,3,4] (4 is a left child but is still seen).
 PATTERN : DFS (preorder, right child first) + depth index
================================================================================
IDEA
  Dfs walks the tree in the order node, right, left, and passes the depth
  as level. ans holds one value per level that has been seen. When level
  == ans.Count, this is the first node to reach that depth, so its value
  is added. It is correct because right-first preorder reaches every
  level through its rightmost node before any other node on that level.
  optimal.cs does the same job with BFS, level by level.
EXAMPLE
  Tree [1,2,3,4]: 1 has children 2 (left) and 3 (right); 2 has left 4.
  Dfs(1,0): 0==0, add 1 -> Dfs(3,1): 1==1, add 3. 3 has no children.
  Dfs(2,1): 1!=2, skip -> Dfs(4,2): 2==2, add 4.
  Answer: [1,3,4]
COMPLEXITY
  Time  O(n)  each node is entered by Dfs exactly once
  Space O(n)  recursion depth is the tree height, n for a chain
WATCH OUT
  - Swap the two Dfs calls and you get the LEFT side view, not the right.
  - A view node can be a left child (the 4 above). Code that only follows
    node.right misses it.
  - The check must be level == ans.Count. A flag like "seen this value"
    fails, because values can repeat in the tree.
  - A very deep, chain-like tree can overflow the call stack. BFS avoids
    this.
================================================================================
*/
