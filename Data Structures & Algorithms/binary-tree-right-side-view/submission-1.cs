public class Solution
{
    public List<int> RightSideView(TreeNode root)
    {
        List<int> ans = new List<int>();

        Dfs(root, 0, ans);

        return ans;
    }

    private void Dfs(TreeNode node, int level, List<int> ans)
    {
        if (node == null)
            return;

        // We reached this level for the first time.
        if (level == ans.Count)
        {
            // this is the rightmost node because we visit right first.
            ans.Add(node.val);
        }

        // Visit right before left
        Dfs(node.right, level + 1, ans);
        Dfs(node.left, level + 1, ans);
    }
}