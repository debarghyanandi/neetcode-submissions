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
        return DFS(root).balanced;
    }

    // returns balanced (1 or 0) and height as 2 element int array
    private (bool balanced, int height) DFS(TreeNode node) {
    if (node == null) return (true, 0);
    var left = DFS(node.left);
    var right = DFS(node.right);
    bool balanced = left.balanced && right.balanced && Math.Abs(left.height - right.height) <= 1;
    int height = 1 + Math.Max(left.height, right.height);
    return (balanced, height);
}
}
