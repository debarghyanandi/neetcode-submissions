// ##########################################################################
// #  optimal.cs            O(m * n) time / O(m * n) space
// ##########################################################################

public class Solution
{
    public int MaxAreaOfIsland(int[][] grid)
    {
        // My Solution
        int rows = grid.Length;
        int cols = grid[0].Length;

        int[,] visited = new int[rows, cols];
        int size = 0;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                if (visited[row, col] == 0 && grid[row][col] == 1)
                {
                    int islandSize = Dfs(row, col, visited, grid);
                    size = Math.Max(size, islandSize);
                }
            }
        }
        return size;
    }

    private int Dfs(int row, int col, int[,] visited, int[][] grid)
    {
        int rows = grid.Length;
        int cols = grid[0].Length;

        // Mark this node visisted
        visited[row, col] = 1;

        int size = 1;

        // Visit all 4 neighbours
        for (int delRow = -1; delRow <= 1; delRow++)
        {
            for (int delCol = -1; delCol <= 1; delCol++)
            {
                // Exclude the node itself (0,0) and diagonal nodes
                if (Math.Abs(delRow) == Math.Abs(delCol))
                    continue;

                int nRow = row + delRow;
                int nCol = col + delCol;

                // Invalid neighbour validation
                // Land validation
                // Visited validation
                if (nRow >= 0 && nRow < rows &&
                    nCol >= 0 && nCol < cols &&
                    visited[nRow, nCol] == 0 &&
                    grid[nRow][nCol] == 1)
                {
                    size += Dfs(nRow, nCol, visited, grid);
                }
            }
        }
        return size;
    }
}

/*
================================================================================
 PROBLEM : You get a grid of 0 (water) and 1 (land). An island is a group of
           land cells joined up, down, left or right (not diagonally). Area is
           the number of cells in an island. Return the largest area, or 0 if
           there is no land. Example: [[1,1,0],[0,1,0],[0,0,1]] -> 3
 PATTERN : Grid DFS (flood fill) + running max
================================================================================
IDEA
  Scan every cell. When a cell is land and not yet in visited, start Dfs.
  Dfs marks the cell, counts it as 1, and adds the Dfs result of each valid
  4-direction neighbour. The delRow/delCol loop skips cells where
  Math.Abs(delRow) == Math.Abs(delCol): the center and the diagonals.
  Each Dfs returns one whole island, and visited stops a recount, so the
  running max in size is the true answer.
EXAMPLE
  Grid [[1,1,0],[0,1,0],[0,0,1]]: Dfs(0,0) -> (0,1) -> (1,1), area 3.
  (1,1) touches (2,2) only diagonally, so it is not added.
  Later Dfs(2,2) gives area 1. size = max(3, 1) = 3.
COMPLEXITY
  Time  O(m * n)  each cell starts or enters Dfs at most once, 4 checks each
  Space O(m * n)  visited matrix plus a recursion stack as deep as one island
PATH TO OPTIMAL
  Fresh flood fill from every land cell, no shared visited - O((m*n)^2).
  Shared visited across all starts - O(m*n) - each cell is done once.
  Only optimal.cs is in this folder, so no sibling file shows step one.
KEYWORDS
  flood fill, DFS, BFS, connected components, grid graph, visited, union-find
WATCH OUT
  - Mark visited before you recurse (Dfs does it first). If you mark later,
    two cells can call each other forever.
  - Empty grid: grid[0].Length throws when rows == 0. Check this first.
  - A big all-land grid makes Dfs recurse m*n deep and can overflow the
    stack. Use an explicit stack or BFS queue.
  - The Math.Abs check removes diagonals. If you drop it, you get
    8-direction islands and the wrong area.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you do it without the extra visited array?
     -> Set grid[r][c] = 0 when you visit a cell. The space drops to just the
        stack, but the input is changed. Ask if that is allowed.
  2. Can you avoid recursion?
     -> Use BFS with a queue, or DFS with your own stack. It is still O(m*n)
        time, and there is no risk of call-stack overflow.
  3. What if cells turn into land one by one, and you report the max area?
     -> Use union-find with a size per root. Each added cell unions with its
        land neighbours in near O(1) amortized time, and you track the max.
  4. What if you may flip one 0 to 1 (Making A Large Island)?
     -> Label each island with an id and store its area. For each 0, add 1 to
        the areas of its distinct neighbour ids. Still O(m*n).
TRIGGER
  A grid of cells where you must find, count, or measure connected regions.
================================================================================
*/
