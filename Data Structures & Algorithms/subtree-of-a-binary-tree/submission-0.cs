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
    //My solution 
    public bool IsSubtree(TreeNode root, TreeNode subRoot) {
       if(root == null && subRoot != null)
        return false;

        if(IsSameTree(root, subRoot))
            return true;
        
        else{
            return IsSubtree(root.left, subRoot) || IsSubtree(root.right, subRoot);
        } 
    }
    
    public bool IsSameTree(TreeNode first, TreeNode second){
        if(first == null && second == null)
            return true;
        
        if(first != null && second != null && first.val == second.val)
            return IsSameTree(first.left, second.left) && IsSameTree(first.right, second.right);
        return false;
    }
}
