// ##########################################################################
// #  optimal.cs            O(m * n) time / O(m * n) space
// #  Recursive DFS island search   [dfs-grid-islands]
// #  ties with optimal-variant.cs on O(m * n) time / O(m * n) space
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  Each cell visited once via recursive DFS; call stack depth bounded by
// #  grid dimensions.
// ##########################################################################

public class Solution
{
    public int NumIslands(char[][] grid)
    {
        //My solution
        int rows = grid.Length;
        int cols = grid[0].Length;

        // Visited array
        int[,] vis = new int[rows, cols];
        int cnt = 0;
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
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
        int rows = grid.Length;
        int cols = grid[0].Length;

        //mark this node visisted
        vis[row, col] = 1;

        //visit all 6 neighbours
        for (int delRow = -1; delRow <= 1; delRow++)
        {
            for (int delCol = -1; delCol <= 1; delCol++)
            {
                //exclude the node itself(0,0) and diagonal nodes
                if (Math.Abs(delRow) == Math.Abs(delCol))
                    continue;

                int nRow = row + delRow;
                int nCol = col + delCol;

                //invalid neighbour validation
                //land validation
                //visited validation
                if (nRow >= 0 && nRow < rows &&
                    nCol >= 0 && nCol < cols &&
                    vis[nRow, nCol] == 0 &&
                    grid[nRow][nCol] == '1')
                {
                    Dfs(nRow, nCol, vis, grid);
                }
            }
        }
    }
}

/*
================================================================================
 PATTERN : Grid Flood Fill (DFS) - count connected components
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-0.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  vis        vis[r,c] = 1 if cell (r,c) is already part of a counted island
  cnt        number of islands found so far (one per new DFS start)
  delRow     row step to a neighbour, from -1 to 1
  delCol     column step to a neighbour, from -1 to 1
  nRow       row of the neighbour cell being checked
  nCol       column of the neighbour cell being checked
WHY THIS PATTERN
  An island is a group of '1' cells joined up, down, left or right. In graph
  terms, each cell is a node and each shared edge is a link, so each island is
  one connected component (a group of nodes you can reach from each other). The
  outer loops look for a land cell with vis == 0. From there, Dfs marks the
  whole island, and cnt goes up by one. Each island is counted exactly once,
  because after its first cell starts a DFS, every other cell of it is already
  marked.
BRUTE FORCE
  A simple correct first idea: for each land cell, run a new search with a fresh
  visited set. This finds its whole island. Count the cell only if it is the
  top-left-most cell of that island. This is correct, but it costs O((m*n)^2),
  because every cell can walk the whole island again. The shared vis array
  removes this repeated work. Each cell is visited once in the whole run.
INVARIANT
  When the outer loop reaches (row, col), every land cell of every island
  already counted has vis = 1. No cell of an island not yet found has been
  marked. So a cell with vis == 0 and grid == '1' must be the first cell seen of
  a new island. Dfs marks a cell before it recurses, so no cell is entered
  twice. It stops only at water, at the grid edge, or at cells already marked,
  so it marks exactly one island.
3X3 LOOP WITH ABS FILTER
  Dfs loops delRow and delCol over -1..1 (9 pairs). It skips a pair when
  Math.Abs(delRow) == Math.Abs(delCol). That one test removes the centre (0,0)
  and the 4 diagonals (|1| == |1|). Only the 4 side neighbours are left. If you
  remove the filter and skip only (0,0), you get the 8-direction version.
WATCH OUT
  The comment "visit all 6 neighbours" is wrong. The code visits 4 neighbours
  (up, down, left, right), and the 3x3 loop checks 9 pairs in total.
  grid[0].Length throws an exception if grid is empty (rows == 0), and
  NumIslands reads it before any check. Dfs is recursive, so one very large
  island (for example, a grid of all '1') can make the call stack very deep and
  cause a StackOverflowException. In .NET you cannot catch that exception.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you do it without the extra vis array?
     Yes. Set grid[r][c] = '0' when you visit a cell, and test only grid == '1'.
     This saves the O(m*n) visited memory, but it changes the caller's input.
     Ask if that is allowed.
  2. What if land cells are added one at a time, and you must report the count
  after each one?
     Use Union-Find (disjoint set: each cell points to a leader for its group).
     Each new cell adds 1 to the count, and each merge with a land neighbour
     takes 1 away. Each add is close to O(1), so you do not run a full flood
     fill again every time.
  3. How would you return the size of the largest island instead?
     Make Dfs return 1 plus the sum of what its neighbour calls return. Keep a
     running max in the outer loop instead of cnt++. The traversal stays the
     same.
TRIGGER
  A grid or graph where you must count or measure groups of cells that are
  "connected" should make you think of flood fill with a visited mark.
C# NOTE
  vis only ever holds 0 or 1, so bool[,] shows the intent better, and each
  element uses 1 byte instead of 4. Also, Dfs computes rows and cols again on
  every call. Keep them in fields, or pass them in as parameters.
COMPLEXITY
  Time  : O(m * n)
  Space : O(m * n)
================================================================================
*/
