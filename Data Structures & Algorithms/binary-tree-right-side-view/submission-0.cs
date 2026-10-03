/**
 * Definition for a binary tree node.
 * public class TreeNode {
 *     public int val;
 *     public TreeNode left;
 *     public TreeNode right;
 *     public TreeNode(int val=0, TreeNode left=null, TreeNode right=null) {
 *         this.val = val;
 *         this.left = left;
 *         this.right = right;
 *     }
 * }
 */

public class Solution {
    public List<int> RightSideView(TreeNode root) {
        //My Solution
        Queue<TreeNode> queue = new Queue<TreeNode> ();
        
        List <int> ans = new List <int> ();

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
