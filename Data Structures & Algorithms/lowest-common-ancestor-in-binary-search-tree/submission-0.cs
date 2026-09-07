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
    public TreeNode LowestCommonAncestor(TreeNode root, TreeNode p, TreeNode q) {
        //My solution
        int min = Math.Min(p.val, q.val);
        int max = Math.Max(p.val, q.val);
        
        if(max >= root.val && min <= root.val)
            return root;
        
        if(min > root.val){
            return LowestCommonAncestor(root.right, p, q);
        }
        else
            return LowestCommonAncestor(root.left, p, q); 
    }
}
