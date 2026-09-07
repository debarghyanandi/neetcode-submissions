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
    public List<List<int>> LevelOrder(TreeNode root) {
        List<List<int>> res = new List<List<int>>();

        if(root == null)
        return res;

        Queue<TreeNode> q = new Queue<TreeNode>();
        q.Enqueue(root);

        while(q.Count > 0){
            List<int> level = new List<int>();

            for(int i = q.Count; i > 0; i--){
                TreeNode node = q.Dequeue();
                if(node != null){
                    level.Add(node.val);
                    q.Enqueue(node.left);
                    q.Enqueue(node.right);
                }
            }
            if(level.Count > 0){
                res.Add(level);
            }
        }
        return res;
    }
}
