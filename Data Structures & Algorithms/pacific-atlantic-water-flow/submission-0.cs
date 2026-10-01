public class Solution {
    public List<List<int>> PacificAtlantic(int[][] heights) {
        int rows = heights.Length;
        int cols = heights[0].Length;

        bool [,] pacific = new bool[rows, cols];
        bool [,] atlantic = new bool[rows, cols];
        
        //Pacific
        for(int col = 0; col < cols; col++){
            Dfs(0, col, pacific, heights, int.MinValue);
        }

        for(int row = 0; row < rows; row++){
            Dfs(row, 0, pacific, heights, int.MinValue);
        }

        //Atlantic
        for(int col = 0; col < cols; col++){
            Dfs(rows - 1, col, atlantic, heights, int.MinValue);
        }

        for(int row = 0; row < rows; row++){
            Dfs(row, cols - 1, atlantic, heights, int.MinValue);
        }


        List<List<int>> res = new List<List<int>>();

        for (int row = 0; row < rows; row++){
            for(int col = 0; col < cols; col++){
                if(pacific[row, col] && atlantic[row, col]){
                    res.Add(new List<int>{row, col});
                }
            }
        }

        return res;
        
    }

    private void Dfs(int row, int col, bool[,] vis, int[][] grid, int prevHeight)
    {
        int rows = grid.Length;
        int cols = grid[0].Length;

        // Invalid
        if(row < 0 || row >= rows || col < 0 || col >= cols)
        {
            return;
        }

        // Already visited.
        if (vis[row, col])
            return;

        // Reverse flow
        // From ocean, we can only move to equal or high cells
        if (grid[row][col] < prevHeight)
            return;

        vis[row, col] = true;
        
        //prev height for next call
        int currentHeight = grid[row][col];

        Dfs(row - 1, col, vis, grid, currentHeight); //up
        Dfs(row + 1, col, vis, grid, currentHeight); //down
        Dfs(row, col - 1, vis, grid, currentHeight); //left
        Dfs(row, col + 1, vis, grid, currentHeight); //right-
    }
}
