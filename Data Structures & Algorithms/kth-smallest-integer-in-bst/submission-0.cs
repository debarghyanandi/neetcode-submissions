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

    private int count = 0;
    public int KthSmallest(TreeNode root, int k) 
    {
        if (root == null)
            return -1;

        int left = KthSmallest(root.left, k);
        if(left != -1)
            return left;

        count ++;

        if (count == k)
        {
            return root.val;
        }
        
        return KthSmallest(root.right, k);
        
    }
    
}
