// ##########################################################################
// #  optimal.cs            O(m * n) time / O(m * n) space
// #  DFS flood fill, track max island size   [dfs-flood-fill]
// #  the only solution in this folder
// #
// #  YOU SOLVED THIS YOURSELF (from submission-0)
// #
// #  each cell visited once via recursive DFS, worst-case recursion depth
// #  and visited matrix both O(m*n)
// ##########################################################################

public class Solution {
    public int MaxAreaOfIsland(int[][] grid) {
        // My Solution
        int rows = grid.Length;
        int cols = grid[0].Length;
        
        int[,] visited = new int [rows, cols];
        int size = 0;
        
        for(int row = 0; row < rows; row++){
            for(int col = 0; col < cols; col++){
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

        //mark this node visisted
        visited[row, col] = 1;

        int size = 1;

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
 PATTERN : Grid flood fill - DFS with a separate visited matrix
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-0.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
ALGORITHM
  1. Scan every cell in row-major order from MaxAreaOfIsland.
  2. When a cell is land (grid[row][col] == 1) and unvisited (visited[row, col]
  == 0), it is the first cell of an island never touched before, so call Dfs on
  it.
  3. Dfs marks the cell visited, seeds size = 1, and adds the returned size of
  each qualifying orthogonal neighbour.
  4. The returned islandSize folds into the running size with Math.Max.

  visited is a rectangular int[,] allocated once for the whole scan, not per
  island - that is what makes the outer double loop a single sweep rather than a
  restart per component. size stays 0 for an all-water grid, which is the
  required answer.
INVARIANT
  visited[row, col] = 1 happens on the FIRST line of Dfs, before any recursion,
  not after the children return. Two consequences, and both are the correctness
  argument:

  - No cycle. A child looking back at its parent sees visited[nRow, nCol] == 1
  and skips it. Move that assignment below the neighbour loop and a two-cell
  island recurses forever.
  - No double count. Every cell executes the body of Dfs at most once, and each
  execution contributes exactly the literal 1 in size = 1. So the value bubbling
  up is the number of distinct cells reached, i.e. the component's area.

  The guard is checked twice on purpose: at the call site before recursing, and
  implicitly by the fact that a cell is only ever entered through such a guard.
THE NEIGHBOUR TRICK
  The 3x3 delRow/delCol sweep with

      if (Math.Abs(delRow) == Math.Abs(delCol)) continue;

  is a single predicate doing two jobs. Abs(0) == Abs(0) kills the centre (the
  cell itself); Abs(1) == Abs(1) kills all four diagonals. What survives is
  exactly the four orthogonal offsets, where one delta is zero and the other is
  not.

  The comment says 6 neighbours - it is 4. Fix the comment before this file is
  read again. If you ever want 8-directional connectivity, the filter becomes
  delRow == 0 && delCol == 0, not a tweak of the Abs test.
SUBTLE POINT WORTH KNOWING
  The visited[row, col] == 0 test in the OUTER loop is not load-bearing for the
  answer. Drop it and a re-entered cell would mark itself (already 1), find
  every neighbour visited, and return 1 - so Math.Max sees a 1 that the real
  island maximum already dominates. The test is there to keep islandSize
  meaningful and to avoid re-calling Dfs on every interior cell of every island.

  The visited test inside Dfs is a different story: that one is required, per
  the invariant above.
WATCH OUT
  - Stack depth. Dfs recurses one frame per cell, and a grid that is entirely 1s
  forms a single snake-like chain, so the frame count grows with the cell count.
  A large dense grid can blow the call stack - this is the practical failure
  mode of the recursive version, not a speed problem.
  - grid[0].Length assumes at least one row and treats the jagged int[][] as
  rectangular. A zero-row grid throws before the loops start.
  - rows and cols are recomputed from grid.Length and grid[0].Length at the top
  of every Dfs call. Harmless, but they are already known in the caller - pass
  them in, or make Dfs a local function that closes over them, if you clean this
  up.
FOLLOW-UP AN INTERVIEWER WILL ASK
  "Can you drop the visited array?" Yes - write grid[row][col] = 0 in place of
  visited[row, col] = 1 and test grid[nRow][nCol] == 1 alone. Water and visited
  become the same state, so the auxiliary matrix disappears. The cost is that
  you have destroyed the caller's input; say that trade out loud, and ask
  whether mutating the argument is acceptable.

  "Can you avoid recursion?" Replace Dfs with an explicit Stack<(int, int)>
  (DFS) or Queue<(int, int)> (BFS), marking visited at push time rather than pop
  time so a cell is never enqueued twice. Same traversal, bounded by heap
  instead of call stack.

  "What if the islands were queried repeatedly or merged incrementally?" That is
  the union-find variant: union each land cell with its right and down
  neighbour, track component sizes, answer with the largest root.
COMPLEXITY
  Time  : O(m * n)
  Space : O(m * n)
================================================================================
*/
