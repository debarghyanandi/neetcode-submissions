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
    private List<List<int>> res = new List<List<int>>();
    
    public List<List<int>> LevelOrder(TreeNode root) {
        //My Solution
        if(root == null)
            return res;
        
        DFS(root, 0);
        return res;
    }

    private void DFS(TreeNode root, int level){
        if(root == null)
            return;
        // When we first reach a new level, level == res.Count.
        // For example, at the first node of level 1, both are 1,
        // so we create a new list for that level.

        // When we reach the next node at the same level,
        // level is still 1, but res.Count is now 2 because
        // the list for level 1 was already created.
        if(res.Count == level)
            res.Add(new List<int>());
        
        res[level].Add(root.val);

        DFS(root.left, level+1);    
        DFS(root.right, level+1);
        }
}
