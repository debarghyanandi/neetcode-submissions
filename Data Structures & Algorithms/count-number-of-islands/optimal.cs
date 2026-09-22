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
 PATTERN : Grid DFS flood fill - count connected components
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-0.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  vis      vis[row, col] == 1 means the cell was already swallowed by some island
  cnt      number of DFS launches so far = number of islands found
  delRow   row offset of the candidate neighbour, -1..1
  delCol   column offset of the candidate neighbour, -1..1
  nRow     row + delRow, the neighbour row before bounds checking
  nCol     col + delCol, the neighbour column before bounds checking
WHY THIS PATTERN
  The problem asks how many separate groups of '1' cells touch each other side
  by side. That is exactly "count connected components" in a graph whose nodes
  are land cells and whose edges join neighbours. The outer double loop finds a
  land cell no one has reached yet, and the single Dfs call from it marks the
  whole component in vis, so every component is entered exactly once and cnt is
  the answer.
BRUTE FORCE
  The naive version is union-find without any union by size or path compression:
  give each land cell an id, union it with its right and down land neighbour,
  then count distinct roots. It is correct but a chain of unions can degrade
  each Find to a walk up a long parent chain, so it is slower for no gain here.
  Flood fill already touches every cell a constant number of times, which is the
  floor for a problem that must read the whole grid.
INVARIANT
  At the moment the outer loop reaches (row, col), every land cell belonging to
  an island already counted has vis == 1. So the test vis == 0 && grid == '1'
  fires only on a cell of a brand new island. Inside Dfs, a cell is written vis
  = 1 before its neighbours are explored, so no cell is ever entered twice and
  the recursion cannot loop forever on a cycle.
THE ABS TRICK FOR 4-DIRECTIONS
  The loops over delRow and delCol generate all 9 offsets, and Math.Abs(delRow)
  == Math.Abs(delCol) throws away the ones that are wrong. It removes (0,0)
  because 0 == 0, and it removes the four diagonals because their absolute
  values are 1 and 1. What survives is exactly up, down, left, right - which is
  what this problem means by "adjacent".
WATCH OUT
  The comment says "visit all 6 neighbours" but the code visits 4; there is no
  6-neighbour case in a 2D grid, so trust the abs filter, not the comment. int
  cols = grid[0].Length throws if grid is empty, and it assumes every row has
  the same length - a ragged char[][] would break the bounds check for longer
  rows. Dfs recursion depth can reach rows * cols on a grid that is all '1',
  which can overflow the call stack. Dfs recomputes rows and cols on every call
  instead of taking them as parameters.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Remove the recursion - the stack overflows on a big all-land grid.
     Push (row, col) pairs onto an explicit Stack<(int,int)> (or a Queue for
     BFS) and mark vis at push time, not at pop time, or the same cell gets
     queued many times. Same work, but the depth now lives on the heap.
  2. Drop the vis array to save memory.
     Overwrite grid[nRow][nCol] = '0' when you visit it; the '0' then doubles as
     "visited". Cheaper in space, but it destroys the caller's input, so it
     needs the caller's permission.
  3. Now return the size of the largest island instead of the count.
     Have Dfs return 1 plus the sum of its recursive calls, and keep a running
     max instead of incrementing cnt. The traversal is unchanged.
  4. The grid does not fit in memory and arrives one row at a time.
     Switch to union-find over just the previous and current row: union each
     land cell with its left neighbour and with the cell above, then retire ids
     that can no longer be extended. Memory drops to O(cols).
TRIGGER
  A 2D grid where cells of one type touching side by side form a region, and you
  must count or measure those regions.
C# NOTE
  vis is a rectangular int[,] indexed once as vis[row, col], while grid is a
  jagged char[][] needing two dereferences per read; a bool[,] would carry the
  same information in one byte per cell instead of four.
COMPLEXITY
  Time  : O(m * n)
  Space : O(m * n)
================================================================================
*/
