public class Solution
{
    public int MaxAreaOfIsland(int[][] grid)
    {
        // My Solution
        int rows = grid.Length;
        int cols = grid[0].Length;

        int[,] visited = new int[rows, cols];

        int maxSize = 0;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                // Start DFS only from unvisited land
                if (visited[row, col] == 0 && grid[row][col] == 1)
                {
                    int islandSize = Dfs(row, col, visited, grid);

                    maxSize = Math.Max(maxSize, islandSize);
                }
            }
        }

        return maxSize;
    }

    private int Dfs(int row, int col, int[,] visited, int[][] grid)
    {
        int rows = grid.Length;
        int cols = grid[0].Length;

        // Invalid position
        if (row < 0 || row >= rows ||
            col < 0 || col >= cols)
        {
            return 0;
        }

        // Already visited
        if (visited[row, col] == 1)
            return 0;

        // Water cell
        if (grid[row][col] == 0)
            return 0;

        // Mark current land cell as visited
        visited[row, col] = 1;

        int size = 1;

        // Visit all 4 neighbours
        size += Dfs(row - 1, col, visited, grid); // Up
        size += Dfs(row + 1, col, visited, grid); // Down
        size += Dfs(row, col - 1, visited, grid); // Left
        size += Dfs(row, col + 1, visited, grid); // Right

        return size;
    }
}