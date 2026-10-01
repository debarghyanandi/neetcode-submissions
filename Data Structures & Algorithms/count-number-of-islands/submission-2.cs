public class Solution
{
    public int NumIslands(char[][] grid)
    {
        // My solution
        int n = grid.Length;
        int m = grid[0].Length;

        // Track visited cells
        int[,] vis = new int[n, m];

        int cnt = 0;

        for (int row = 0; row < n; row++)
        {
            for (int col = 0; col < m; col++)
            {
                // Start DFS only from unvisited land
                if (vis[row, col] == 0 && grid[row][col] == '1')
                {
                    Dfs(row, col, vis, grid);
                    cnt++;
                }
            }
        }

        return cnt;
    }

    private void Dfs(int row, int col, int[,] vis, char[][] grid)
    {
        int n = grid.Length;
        int m = grid[0].Length;

        // Invalid position
        if (row < 0 || row >= n ||
            col < 0 || col >= m)
        {
            return;
        }

        // Already visited
        if (vis[row, col] == 1)
            return;

        // Water cell
        if (grid[row][col] == '0')
            return;

        // Mark current land cell as visited
        vis[row, col] = 1;

        // Visit all 4 neighbours

        // Up
        Dfs(row - 1, col, vis, grid);

        // Down
        Dfs(row + 1, col, vis, grid);

        // Left
        Dfs(row, col - 1, vis, grid);

        // Right
        Dfs(row, col + 1, vis, grid);
    }
}