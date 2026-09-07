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
//Breadth-First Search (BFS) find level.
public class Solution {
    public int MaxDepth(TreeNode root) {
        Queue<TreeNode> q = new Queue<TreeNode>();
        if (root != null) {
            q.Enqueue(root);
        }

        int level = 0;
        while (q.Count > 0) {
            int size = q.Count;
            for (int i = 0; i < size; i++) {
                TreeNode node = q.Dequeue();
                if (node.left != null) {
                    q.Enqueue(node.left);
                }
                if (node.right != null) {
                    q.Enqueue(node.right);
                }
            }
            level++;
        }
        return level;
    }
}