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
 PATTERN : Grid DFS flood fill - largest connected component
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-0.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  visited      visited[row, col] = 1 once that cell has been counted
  size         in MaxAreaOfIsland: best island area found so far
  islandSize   area of the one island reached from (row, col)
  size         in Dfs: area of the component reached from this cell, counting itself
  delRow/delCol  offset pair (-1..1) used to generate the 4 neighbours
WHY THIS PATTERN
  The problem asks for the largest group of 1s joined side by side, so each
  answer is the size of one connected component in a grid graph. DFS from an
  unvisited land cell walks the whole component exactly once and returns its
  area, which is what Dfs gives back as size. The outer double loop starts a new
  DFS at every land cell that visited has not marked, so every component is
  measured exactly once and Math.Max keeps the biggest in size.
BRUTE FORCE
  The naive version is to re-run a fresh search from every land cell with a
  fresh visited array and keep the largest count. That is O((m*n)^2) time
  because each of the m*n starts can touch the whole grid. It loses because it
  recomputes the same component once per cell inside it; sharing one visited
  array across all starts is what makes the whole sweep linear.
INVARIANT
  A cell is marked visited[row, col] = 1 at the moment Dfs enters it, before any
  neighbour is explored, so no cell is ever entered twice and no cell is counted
  twice in size. When Dfs(r, c) returns, every land cell reachable from (r, c)
  is marked and its 1 is included in the returned total. So when the outer loop
  reaches a cell with visited == 0 and grid == 1, that cell belongs to a
  component not yet measured, and islandSize is that component's full area.
THE ABS TRICK FOR 4 NEIGHBOURS
  Instead of a directions array, the code loops delRow and delCol over -1..1
  (nine pairs) and skips any pair where Math.Abs(delRow) == Math.Abs(delCol).
  That single test throws out the centre (0,0) and all four diagonals, because
  those are exactly the cases where the two absolute values match, leaving the
  four orthogonal moves. It is compact, but a reader has to decode it; an
  explicit int[][] dirs = {{1,0},{-1,0},{0,1},{0,-1}} reads faster and does
  fewer iterations.
WATCH OUT
  grid[0].Length is read without checking grid.Length first, so an empty outer
  array throws IndexOutOfRangeException; a null or empty grid needs a guard. The
  recursion depth can reach m*n on one long snake-shaped island, which can
  overflow the call stack on a large grid. The comment says "Visit all 4
  neighbours" but the loop actually walks 9 offsets and discards 5, so it is 9
  iterations of work per cell, not 4. Dfs also recomputes rows and cols from
  grid on every call instead of receiving them. Minor: the comment "visisted" is
  a typo, and jagged rows of unequal length would break cols.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How do you remove the recursion?
     Push cells on an explicit Stack<(int,int)>, pop, and add 1 per popped cell;
     mark visited when you push, not when you pop, or the same cell can be
     pushed twice. Same time, and the stack size is bounded by the grid instead
     of the call stack.
  2. Can you drop the visited array?
     Yes, write 0 into grid[row][col] as you enter, since a 0 cell is never
     revisited. That removes the m*n extra ints but destroys the caller's input,
     so it needs the caller's permission or a copy.
  3. What if the grid is huge and streamed row by row, so you cannot hold it
  all?
     Use union-find over two rows at a time: union each land cell with its left
     and up neighbour, keep component sizes in the parent structure, and drop
     rows once they can no longer be joined.
  4. What changes if islands may wrap around the left and right edges?
     Only neighbour generation changes: compute nCol as (col + delCol + cols) %
     cols instead of rejecting out-of-range columns; the DFS and the visited
     logic stay the same.
TRIGGER
  A grid of 0s and 1s where you must measure or count groups of cells joined
  edge to edge.
C# NOTE
  visited is int[,], a true rectangular array, which is one allocation and
  indexes faster than the int[][] jagged form that grid uses; bool[,] would say
  the intent better and use one byte per cell instead of four.
COMPLEXITY
  Time  : O(m * n)
  Space : O(m * n)
================================================================================
*/
