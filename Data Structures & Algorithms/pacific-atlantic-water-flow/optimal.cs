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
 PROBLEM : Given an m x n grid of heights, the Pacific touches the top and
           left edges. The Atlantic touches the bottom and right edges. Water
           flows to a neighbor (up/down/left/right) of equal or lower height.
           Return every [row, col] that can reach both oceans. Example:
           [[1,2],[4,3]] -> [[0,1],[1,0],[1,1]].
 PATTERN : Multi-source DFS from borders (reverse flow)
================================================================================
IDEA
  Do not ask "where can this cell flow?" for every cell. Ask the reverse
  question: start at each ocean's border cells and climb uphill. Dfs moves to
  a cell only if grid[row][col] >= prevHeight, and it marks reached cells in
  pacific or atlantic. A cell is in res when both arrays are true. This is
  correct because reverse uphill paths are exactly the downhill water paths.
EXAMPLE
  heights = [[1,2],[4,3]]
  pacific: from (0,0)=1 climb to (1,0)=4, (0,1)=2, then (1,1)=3 -> all 4
  atlantic: (1,0),(1,1),(0,1) start; (0,0)=1 is lower, so it is never reached
  res = [[0,1],[1,0],[1,1]] ((0,0) reaches only the Pacific)
COMPLEXITY
  Time  O(m * n)  vis stops repeats, so each cell is expanded at most once per
                  ocean
  Space O(m * n)  two bool grids plus a recursion stack up to m * n deep
PATH TO OPTIMAL
  Brute force: downhill DFS from every cell - O((m*n)^2) - repeats much work.
  Reverse search from the borders, one pass per ocean - O(m*n) - each cell is
  marked once per ocean (this file; optimal-variant.cs is the same cost).
KEYWORDS
  graph traversal, grid DFS, BFS, multi-source search, reverse flow, matrix
WATCH OUT
  - The check is < prevHeight, not <=. Equal heights must still flow.
  - Do not climb downhill from the ocean. That is the forward direction.
  - Deep recursion: a long snake-shaped path can overflow the call stack.
  - heights[0].Length throws on an empty grid. Return early if rows == 0.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How do you avoid stack overflow on a huge grid?
     -> Use iterative BFS with a queue, seeded by all border cells of one
        ocean. The cost stays O(m*n) time and space, with no recursion depth
        limit.
  2. Can you use less memory?
     -> Use one byte grid with bit 1 for Pacific and bit 2 for Atlantic, and
        collect cells equal to 3. Still O(m*n), but one array instead of two.
  3. What if there are k oceans, or water flows in 8 directions?
     -> Run one reverse search per ocean, which is O(k*m*n). For 8 directions,
        add the four diagonals to the neighbor list. The logic does not change.
TRIGGER
  When many start cells ask "can I reach target X?", search backward from X
  once.
================================================================================
*/
