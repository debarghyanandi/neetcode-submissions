// ##########################################################################
// #  optimal.cs            O(m * n) time / O(m * n) space
// #  DFS island traversal   [dfs-island-area]
// #  the only solution in this folder
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  Each cell visited once during DFS traversal; visited matrix and
// #  recursion depth both O(m*n) in worst case
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
 PATTERN : Graph DFS / Flood Fill - size of each connected component
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-0.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  visited     visited[r, c] = 1 once cell (r, c) has been counted in some island
  size        in MaxAreaOfIsland: the largest island area seen so far; in Dfs: the area found from this cell
  islandSize  the area of the island that starts at (row, col)
  delRow      row step to a neighbour, from -1 to 1
  delCol      column step to a neighbour, from -1 to 1
  nRow, nCol  the neighbour cell being checked
WHY THIS PATTERN
  An island is a group of 1-cells joined up, down, left or right. In graph
  terms, it is a connected component: a set of nodes where each one can reach
  every other one. The problem asks for the biggest group, so each island must
  be counted once and its cells added up. The outer double loop finds a land
  cell with visited == 0 that has not been counted yet. Then Dfs spreads to
  every reachable land cell and returns the count, and size keeps the maximum.
BRUTE FORCE
  Start a new flood fill from every land cell. Give each one a fresh visited
  array, and take the largest count you get. This is correct, but each island of
  k cells is walked k times, so the worst case is O((m * n)^2). The shared
  visited array in this file removes that repeated work: each cell is counted
  exactly once, across all islands.
INVARIANT
  A cell is marked in visited at the moment it is added to some island's count,
  and never before. Because Dfs marks visited[row, col] = 1 before it looks at
  the neighbours, no cell can be counted twice, even when the island has cycles.
  Dfs reaches every land cell that is connected to the start cell, so its return
  value is the exact island area. The outer loop only starts from unvisited
  land, so every island is measured exactly once, and size ends up as the true
  maximum.
THE ABS FILTER PICKS 4 OF 9 OFFSETS
  The two loops produce 9 (delRow, delCol) pairs. The check Math.Abs(delRow) ==
  Math.Abs(delCol) skips (0,0) and the four diagonals. That leaves exactly up,
  down, left and right. The code matches its comments here.
WATCH OUT
  The recursion goes as deep as the path the DFS takes, and in the worst case
  that is the size of the biggest island. A large all-land grid could cause a
  StackOverflowException, and C# cannot catch that exception. Also,
  grid[0].Length is read with no guard, so an empty grid throws
  IndexOutOfRangeException before any work is done.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you drop the extra O(m * n) visited array?
     Yes. Set grid[nRow][nCol] = 0 when you visit a cell, so "visited" becomes
     "no longer land". This saves memory, but it changes the caller's input.
     Some interviewers do not allow that.
  2. How do you write it without recursion?
     Push cells onto an explicit Stack<(int, int)> or Queue<(int, int)> (BFS).
     Mark each cell when you push it, and count it when you pop it. The
     complexity is the same, and the depth now lives on the heap instead of the
     call stack.
  3. What if diagonal cells also connect an island?
     Change the filter so it skips only (0,0). All 8 neighbours then count, and
     nothing else changes.
  4. What if land cells are added one at a time, and you need the max area after
  each one?
     Use Union-Find (disjoint set union) with a size count at each root. Union
     each new cell with its land neighbours, and keep a running maximum. Each
     addition then costs close to O(1), instead of a full new scan.
TRIGGER
  A grid where cells join through their neighbours, and the question asks you to
  count, measure or compare the connected regions.
C# NOTE
  visited is an int[,] but only ever holds 0 or 1. A bool[,] states that intent
  more clearly and uses 1 byte per cell instead of 4.
COMPLEXITY
  Time  : O(m * n)
  Space : O(m * n)
================================================================================
*/
