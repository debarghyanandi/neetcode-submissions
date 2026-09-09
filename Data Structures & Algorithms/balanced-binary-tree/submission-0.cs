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
    public bool IsBalanced(TreeNode root) {
        return DFS(root)[0] == 1;
    }

    // returns balanced (1 or 0) and height as 2 element int array
    private int[] DFS(TreeNode root){
        if(root == null){
            return new int[]{1, 0};
        }
        var left = DFS(root.left);
        var right = DFS(root.right);

        bool balanced = (left[0] == 1 && right[0] == 1 && 
                        Math.Abs(left[1] - right[1]) <= 1);
        int height = 1 + Math.Max(left[1], right[1]);

        return new int[] {balanced ? 1 : 0, height};
    }
}
