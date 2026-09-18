// ##########################################################################
// #  optimal.cs            O(m * n) time / O(m * n) space
// #  DFS flood fill with visited matrix   [dfs-flood-fill]
// #  the only solution in this folder
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  Each of the m*n cells is visited exactly once via recursive DFS; the
// #  visited matrix and call-stack depth are both O(m*n) in the worst case.
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
 PATTERN : Grid DFS Flood Fill - size of each connected region
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-0.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  The question asks for the largest group of 1s joined side by side, so "island"
  means a connected component in a grid graph where each cell has up to 4 edges.
  Flood fill is the direct tool: from any unvisited land cell, walk the whole
  component once and return its cell count. The outer double loop finds one
  starting cell per island, Dfs returns islandSize, and size keeps the
  running best.
BRUTE FORCE
  The simplest first attempt is: for every land cell, run a flood fill with a
  fresh visited array and take the max size. That is O((m*n)^2) time because
  every island is recounted once for each of its own cells. This code keeps one
  shared visited array across the whole scan, so each cell is expanded exactly
  once and the total work collapses to one pass.
INVARIANT
  A cell is written into visited at the very top of Dfs, before any recursion,
  and a neighbour is only recursed into when visited[nRow, nCol]
  == 0 and the cell is land. So every land cell enters Dfs at most once, and
  size = 1 plus the sizes returned by its children counts each cell of the
  component exactly once. In the outer loop, a cell that is already visited
  never starts a new Dfs, so each island contributes one islandSize value to
  size, not several partial ones.
THE NEIGHBOUR TRICK
  Instead of a directions array, the inner loops run delRow and delCol over
  -1..1 (nine pairs) and skip when Math.Abs(delRow) == Math.Abs(delCol). That
  single test removes the centre (0,0) and all four diagonals (1,1), (1,-1),
  (-1,1), (-1,-1), leaving exactly the four side neighbours. It is correct, but
  it does nine iterations per cell to use four; a static int[][] dirs =
  {{1,0},{-1,0},{0,1},{0,-1}} says the same thing more plainly and is easier to
  change if the problem switches to 8-directional.
WATCH OUT
  Depth is the danger here: on a grid that is all 1s the recursion nests
  rows*cols deep, which can blow the call stack - the algorithm is fine but the
  runtime stack is not. int cols = grid[0].Length throws if grid has zero rows,
  so an empty grid is not handled. Also rows and cols are recomputed from grid
  on every single Dfs call, which is pure repeated work; pass them in or make
  them fields. The visited array is int[,] but only ever holds 0 or 1, so it
  uses four bytes per cell for one bit of information.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you do this without recursion?
     Push the start cell on an explicit Stack<(int row, int col)>, mark visited
     when you push, and pop-and-expand in a loop while counting; same time and
     space bounds, and the stack lives on the heap so deep islands cannot
     overflow the call stack.
  2. Can you drop the visited array entirely?
     Yes - set grid[row][col] = 0 as you enter a cell, so sunk land can never be
     revisited; that removes the O(m*n) extra array but destroys the caller's
     input, so you should say so or restore it afterwards.
  3. What changes if islands may also connect diagonally?
     Delete the Math.Abs(delRow) == Math.Abs(delCol) skip and replace it with a
     skip only when delRow == 0 && delCol == 0, giving all 8 neighbours; nothing
     else in the logic changes.
  4. How would you answer this if the grid were streamed row by row and too
  large to hold in memory?
     Use union-find over two rows at a time: union each land cell with its left
     and up neighbour, carry component ids and sizes forward as rows retire, and
     track the max size, so memory is O(cols) instead of O(m*n).
TRIGGER
  A 2D grid of 0/1 where the answer depends on groups of cells joined side by
  side - count them, size them, or colour them.
C# NOTE
  Note the two different array shapes in play: grid is int[][], a jagged array
  of separate row objects, while visited is int[,], a single rectangular block -
  that is why grid uses grid[row][col] and visited uses visited[row, col].
  Switching visited to bool[,] gives the same logic at one byte per cell.
COMPLEXITY
  Time  : O(m * n)
  Space : O(m * n)
================================================================================
*/
