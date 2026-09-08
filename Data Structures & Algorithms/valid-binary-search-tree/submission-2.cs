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

// Sentinel values are safe only because Node.val is restricted to [-1000000000, 1000000000];
// if the value range can reach int.MinValue/MaxValue, use long or nullable bounds.
public class Solution {
    //my solution
    public bool IsValidBST(TreeNode root) {
        var res = IsValidBSTWithMinMax(root);
        return res.found;
    }

    public (int min, int max, bool found) IsValidBSTWithMinMax(TreeNode root) {
        //Empty
        if(root == null )
            return (int.MaxValue, int.MinValue, true);
        
        //Leaf Node
        if(root.right == null && root.left == null)
        return (root.val, root.val, true);

        var left = IsValidBSTWithMinMax(root.left); 
        var right = IsValidBSTWithMinMax(root.right);

        // left subtree must be valid and its max < root
        if(!left.found || left.max >= root.val)
        return (0, 0, false);

        if(!right.found || right.min <= root.val)
        return (0, 0, false);

        //we came to here means valid bst
        int min = Math.Min(left.min, root.val);
        int max = Math.Max(right.max, root.val);

        return (min, max, true);
    }
}


