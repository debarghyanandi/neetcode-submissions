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

/*
================================================================================
 PROBLEM : Input is a grid of 0s (water) and 1s (land). An island is land
           cells joined up, down, left or right. Diagonal cells do not join.
           Return the size of the largest island, or 0 if there is no land.
           Example: [[1,1,0],[1,0,1],[0,0,1]] -> 3
 PATTERN : Grid DFS (flood fill) + running max
================================================================================
IDEA
  Scan every cell. When a cell is land and not yet in visited, start Dfs.
  Dfs marks the cell, then returns 1 plus the sizes from its 4 neighbours.
  Bad cells (out of bounds, water, already visited) return 0.
  maxSize keeps the largest islandSize seen.
  It is correct because visited stops double counting, so each island is
  counted once, as a whole.
EXAMPLE
  grid [[1,1,0],[1,0,1],[0,0,1]]
  (0,0): Dfs -> self 1 + down (1,0) 1 + right (0,1) 1 = 3, maxSize=3
  (1,2): Dfs -> self 1 + down (2,2) 1 = 2. It touches (0,1) only diagonally.
  Answer: 3
COMPLEXITY
  Time  O(m * n)  each cell is marked once; each Dfs call does O(1) work, 4
                  tries
  Space O(m * n)  visited array plus a recursion stack that can hold every
                  cell
PATH TO OPTIMAL
  Brute force: from every land cell, flood fill its island again with a new
  visited set - O((m*n)^2) - the same island is counted many times.
  Shared visited array across all starts (this file) - O(m*n) - each cell is
  expanded only once.
KEYWORDS
  flood fill, DFS, BFS, connected components, grid graph, 4-directional
WATCH OUT
  - Mark visited before you recurse. If you mark after, two neighbours call
    each other forever.
  - On a big all-land grid the recursion depth reaches m*n. This can cause
    a stack overflow; use an explicit stack or BFS.
  - grid[0].Length fails on an empty grid. Check rows == 0 first.
  - Do not add diagonal moves. Only 4 directions join cells.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How do you save the O(m*n) visited memory?
     -> Set grid[row][col] = 0 when you visit a cell. Extra space is then only
        the stack. The trade-off: you change the caller's input.
  2. BFS instead of DFS?
     -> Use a queue and mark cells when you enqueue them. It is still O(m*n)
        time. The queue holds about one frontier, and there is no stack risk.
  3. Cells turn into land one by one; report the max island after each step.
     -> Use Union-Find with a size per root. Merge with the 4 neighbours and
        update the max. Each step is near O(1) amortized.
  4. You may flip one 0 to 1. What is the biggest island now?
     -> Label each island with an id and store its size. For each 0, add 1
        plus the sizes of the distinct neighbour ids. This is O(m*n) (Making A
        Large Island).
TRIGGER
  A grid where touching cells form groups, and you must count or measure
  the groups: use flood fill with DFS or BFS.
================================================================================
*/
