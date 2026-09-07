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
    public bool IsSameTree(TreeNode p, TreeNode q) {
        var q1 = new Queue<TreeNode>(new[] { p });
        var q2 = new Queue<TreeNode>(new[] { q });

        while (q1.Count > 0 && q2.Count > 0) {
            for (int i = q1.Count; i > 0; i--) {
                var nodeP = q1.Dequeue();
                var nodeQ = q2.Dequeue();

                if (nodeP == null && nodeQ == null) continue;
                if (nodeP == null || nodeQ == null || nodeP.val != nodeQ.val) {
                    return false;
                }

                q1.Enqueue(nodeP.left);
                q1.Enqueue(nodeP.right);
                q2.Enqueue(nodeQ.left);
                q2.Enqueue(nodeQ.right);
            }
        }

        return true;
    }
}