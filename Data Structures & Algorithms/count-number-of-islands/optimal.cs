// ##########################################################################
// #  optimal.cs            O(m * n) time / O(m * n) space
// #  recursive DFS flood fill   [dfs-recursive-flood-fill]
// #  ties with optimal-variant.cs on O(m * n) time / O(m * n) space
// #
// #  YOU SOLVED THIS YOURSELF (from submission-0)
// #
// #  each cell visited once via recursive DFS; recursion depth can be
// #  O(m*n) in worst case (e.g. spiral-filled grid)
// ##########################################################################

public class Solution
{
    public int NumIslands(char[][] grid)
    {
        //My solution
        int rows = grid.Length;
        int cols = grid[0].Length;

        // Visited array
        int[,] visited = new int[rows, cols];
        int islandCount = 0;
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                if (visited[row, col] == 0 && grid[row][col] == '1')
                {
                    Dfs(row, col, visited, grid);
                    islandCount++;
                }
            }
        }
        return islandCount;
    }

    private void Dfs(int row, int col, int[,] visited, char[][] grid)
    {
        int rows = grid.Length;
        int cols = grid[0].Length;

        //mark this node visisted
        visited[row, col] = 1;

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
                    grid[nRow][nCol] == '1')
                {
                    Dfs(nRow, nCol, visited, grid);
                }
            }
        }
    }
}

/*
================================================================================
 PATTERN : Grid flood fill - count DFS launches, not cells
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-0.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  The question is "how many connected components", and the answer here is a
  counting trick rather than a graph build: the grid is its own adjacency list,
  so no nodes or edges are ever materialized. The outer double loop touches
  every cell once; a Dfs is launched only when a cell is land and unmarked. Each
  launch consumes one whole component, so islandCount moves exactly once per
  component. Note that Dfs returns void - its only job is to paint visited, and
  the counting lives entirely in the caller.
WHY THE COUNT IS EXACT
  Two halves, and an interviewer will ask for both.

  1. No component counted twice. Dfs sets visited[row, col] = 1 on entry and
  only stops at the component boundary, so by the time the launching call
  returns, every cell reachable from (row, col) via orthogonal land steps has
  visited == 1. When the outer scan later walks over those cells, the
  visited[row, col] == 0 half of the guard fails and islandCount does not move.

  2. No component missed. The outer loop visits every (row, col), so whichever
  cell of a component comes first in row-major order is the one that launches.
  Water cells fail grid[row][col] == '1' and are never marked at all - visited
  stays 0 for them forever, which is harmless because the same char test also
  guards the increment.
TERMINATION
  Dfs has no early return. There is nothing at the top saying "if already
  visited, return". The single thing preventing infinite recursion is the
  visited[nRow, nCol] == 0 test inside the neighbor guard - without it, two
  adjacent land cells would call each other forever. So the mark-on-entry and
  the check-at-the-call-site are one mechanism split across two places. If you
  ever refactor this into the guard-at-top style, you have to move both, not
  one.
THE NEIGHBOR FILTER
  Instead of a hardcoded offsets array, delRow and delCol each sweep -1..1,
  giving 9 pairs, and Math.Abs(delRow) == Math.Abs(delCol) continues past 5 of
  them: the center (0,0) and the four diagonals (1,1), (1,-1), (-1,1), (-1,-1).
  Equal absolute values is precisely the "diagonal or self" condition. What
  survives is the 4 orthogonal moves, the ones where exactly one of the two
  deltas has magnitude 1. The bounds tests sit ahead of the array reads in the
  same && chain, so an out-of-range nRow or nCol short-circuits before
  grid[nRow][nCol] is ever indexed.
WATCH OUT
  The comment says "visit all 6 neighbours" - it is 4. Stale line, ignore it.

  Recursion depth is the real exposure. An all-land grid is one component, and
  the DFS can descend through every cell before it unwinds, so frame depth is
  bounded only by rows * cols - 90,000 at the usual 300x300 input limit. This is
  the first thing an interviewer probes on a recursive flood fill.

  cols is taken from grid[0].Length, which assumes at least one row and uniform
  row lengths. Fine for the given input, not something char[][] guarantees.
  Also, rows and cols are recomputed from grid at the top of every Dfs call
  rather than being passed down with row and col.
FOLLOW-UPS TO HAVE READY
  1. Make it iterative. Push (row, col) onto an explicit Stack<(int, int)>, or
  switch to a Queue<(int, int)> for BFS - same count, no frame-depth exposure.
  Mark visited on push, not on pop, or the same cell lands in the container
  multiple times.

  2. Drop the visited array and write grid[row][col] = '0' as the mark. Land
  becomes water, so the '1' test does the visited test's job and the two guard
  conditions collapse into one. The cost is that the caller's grid is destroyed,
  which is exactly why the separate int[,] here is the safer default - say that
  trade out loud rather than presenting it as a strict win.

  3. Union-Find. Union each land cell with its right and down neighbor, then
  count distinct roots. More code for this problem, but it is the only structure
  that answers the streaming variant (number of islands after each addLand)
  without rescanning the grid.
TRIGGER
  Reach for this skeleton whenever the input is a grid and the question counts,
  sizes, or recolors maximal regions of matching cells: number of islands, max
  area of island, flood fill, closed islands, surrounded regions. The frame
  never changes - scan all cells, launch only from qualifying unmarked ones, let
  the traversal paint the whole component. What varies is the per-launch action:
  increment a counter here, take a max over returned sizes for max-area, or seed
  only from border cells for the surrounded and closed-island variants.
COMPLEXITY
  Time  : O(m * n)
  Space : O(m * n)
================================================================================
*/
