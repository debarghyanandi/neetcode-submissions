// --------------------------------------------------------------------------
// -  optimal.cs            O(m * n) time / O(m * n) space
// --------------------------------------------------------------------------

public class Solution
{
    public List<List<int>> PacificAtlantic(int[][] heights)
    {
        int rows = heights.Length;
        int cols = heights[0].Length;

        bool[,] pacific = new bool[rows, cols];
        bool[,] atlantic = new bool[rows, cols];

        //Pacific
        for (int col = 0; col < cols; col++)
        {
            Dfs(0, col, pacific, heights, int.MinValue);
        }

        for (int row = 0; row < rows; row++)
        {
            Dfs(row, 0, pacific, heights, int.MinValue);
        }

        //Atlantic
        for (int col = 0; col < cols; col++)
        {
            Dfs(rows - 1, col, atlantic, heights, int.MinValue);
        }

        for (int row = 0; row < rows; row++)
        {
            Dfs(row, cols - 1, atlantic, heights, int.MinValue);
        }


        List<List<int>> res = new List<List<int>>();

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                if (pacific[row, col] && atlantic[row, col])
                {
                    res.Add(new List<int> { row, col });
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
        if (row < 0 || row >= rows || col < 0 || col >= cols)
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

/*
================================================================================
 PROBLEM : Given an m x n grid heights, the Pacific touches the top and left
           edges. The Atlantic touches the bottom and right edges. Water flows
           to a neighbor (up/down/left/right) of equal or lower height. Return
           every [row, col] whose water can reach both oceans. [[1,2],[4,3]]
           -> [[0,1],[1,0],[1,1]].
 PATTERN : Multi-source DFS from borders (reverse flow)
================================================================================
IDEA
  Do not ask "where can this cell flow?". Ask "which cells can reach this
  ocean?" Start Dfs from every border cell of each ocean and climb uphill:
  move only to a cell with height >= prevHeight. This fills the pacific and
  atlantic boolean grids. The answer is every cell marked in both. It is
  correct because each uphill step backward is a legal downhill step forward.
EXAMPLE
  heights = [[1,2],[4,3]]
  pacific: from (0,0) climb to (1,0)=4 and (0,1)=2, then (1,1)=3 -> all 4
  atlantic: (1,0),(1,1),(0,1) seeded; 1 < 2 and 1 < 4, so (0,0) is never hit
  answer (row-major scan): [[0,1],[1,0],[1,1]]
COMPLEXITY
  Time  O(m * n)  vis stops repeats; each cell is marked at most once per
                  ocean
  Space O(m * n)  two bool grids plus a recursion stack up to m*n deep
PATH TO OPTIMAL
  DFS from every cell to test both oceans - O((m*n)^2) - repeats all work.
  Reverse DFS from the borders, one pass per ocean - O(m*n) - this file.
  optimal-variant.cs reaches the same O(m*n) bound with a different version.
KEYWORDS
  graph traversal, multi-source DFS, BFS, reverse flow, grid, flood fill
WATCH OUT
  - Equal heights must pass. Using <= in "grid[row][col] < prevHeight"
    wrongly blocks flat plateaus.
  - Recursion can go m*n deep on a long uphill snake path, which can
    overflow the stack. Use an explicit stack or BFS for big grids.
  - heights[0].Length throws on an empty grid. Return an empty list first.
  - Set vis before recursing. Otherwise equal neighbors loop forever.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you do it without recursion?
     -> Use BFS with a queue seeded with all border cells of one ocean. Same
        O(m*n) time and space, with no stack-overflow risk.
  2. Why not run DFS from each cell forward?
     -> Each search can touch the whole grid, so it is O((m*n)^2). Reverse
        search shares work: one pass per ocean covers every cell.
  3. Can you save memory?
     -> Store bits in one int grid (1 = Pacific, 2 = Atlantic, 3 = both). This
        is still O(m*n) space but uses one array instead of two.
TRIGGER
  When you must find which cells can reach a boundary, search backward from
  the boundary as many sources at once.
================================================================================
*/
