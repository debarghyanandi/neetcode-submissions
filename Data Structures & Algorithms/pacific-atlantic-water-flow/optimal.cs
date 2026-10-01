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
 PROBLEM : Given an m x n grid heights, water flows from a cell to a
           4-neighbor with equal or lower height. Pacific touches the top and
           left edges; Atlantic touches the bottom and right edges. Return
           [row, col] of every cell whose water can reach both oceans.
           [[1,2],[2,1]] -> [[0,1],[1,0]] (any order).
 PATTERN : Multi-source DFS from borders (reverse flow)
================================================================================
IDEA
  Do not ask "where can this cell's water go?" Ask "which cells can each
  ocean reach by climbing?" Dfs starts from every border cell of an ocean and
  moves only to cells with height >= prevHeight, marking pacific or atlantic.
  Cells marked in both arrays go into res. This is correct because a reverse
  uphill path from the ocean is exactly a downhill path for the water.
EXAMPLE
  heights = [[1,2,3],[8,9,4],[7,6,5]]
  pacific: climbs 1->8->9 and 1->2->3->4->5->6->7, so all 9 cells marked.
  atlantic: 7->8->9 and 3->4->5 climb, but 1 and 2 are lower than 8 and 3.
  res = [0,2],[1,0],[1,1],[1,2],[2,0],[2,1],[2,2] (all but (0,0),(0,1))
COMPLEXITY
  Time  O(m * n)  each cell is marked at most once per ocean, 4 edges checked
                  each
  Space O(m * n)  two m x n visited arrays plus recursion stack up to m * n
                  deep
PATH TO OPTIMAL
  Brute force: DFS/BFS downhill from every cell - O((m*n)^2) - repeated work.
  Reverse search from ocean borders - O(m*n) - each cell is visited once per
  ocean (this file; no sibling file in this folder).
KEYWORDS
  graph traversal, DFS, BFS, multi-source, reverse flow, grid, matrix
WATCH OUT
  - Direction flip: from the ocean you go UP. Writing grid > prevHeight to
    stop (downhill) gives wrong answers. The check here is < prevHeight.
  - Recursion depth can reach m*n on a snake-shaped climb, which can cause a
    stack overflow on big grids. Use an explicit stack or a BFS queue.
  - An empty heights makes heights[0].Length throw. Guard it if allowed.
  - Use int.MinValue as the start prevHeight so every border cell is entered.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you do it with BFS instead of DFS?
     -> Push all border cells of one ocean into a queue, then expand to higher
        or equal neighbors. Same O(m*n) time and space, and no recursion risk.
  2. Why not run a search from each cell?
     -> Each search can touch the whole grid, so it costs O((m*n)^2). Reverse
        search shares the work, because one ocean search marks every cell at
        once.
  3. Can you use less memory?
     -> Use one byte per cell with bit 1 for Pacific and bit 2 for Atlantic.
        Still O(m*n), but one array instead of two.
  4. What if there are k oceans or water may flow 8 ways?
     -> Run one multi-source search per ocean, which costs O(k*m*n). For 8-way
        flow, add the diagonal moves to the neighbor list.
TRIGGER
  When many start cells ask "can I reach a boundary or target set?", search
  backward once from the targets instead of forward from every start.
================================================================================
*/
