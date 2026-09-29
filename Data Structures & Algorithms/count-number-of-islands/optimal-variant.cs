// --------------------------------------------------------------------------
// -  optimal-variant.cs    O(m * n) time / O(m * n) space
// --------------------------------------------------------------------------

public class Solution
{
    public int NumIslands(char[][] grid)
    {
        int rows = grid.Length;
        int cols = grid[0].Length;
        bool[,] vis = new bool[rows, cols];
        int cnt = 0;

        int[] dRow = { -1, 1, 0, 0 };
        int[] dCol = { 0, 0, -1, 1 };

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                if (!vis[row, col] && grid[row][col] == '1')
                {
                    cnt++;
                    Queue<(int, int)> queue = new();
                    queue.Enqueue((row, col));
                    vis[row, col] = true; // mark visited when ENQUEUED, not when dequeued

                    while (queue.Count > 0)
                    {
                        var (r, c) = queue.Dequeue();
                        for (int i = 0; i < 4; i++)
                        {
                            int nRow = r + dRow[i];
                            int nCol = c + dCol[i];
                            if (nRow >= 0 && nRow < rows && nCol >= 0 && nCol < cols &&
                                !vis[nRow, nCol] && grid[nRow][nCol] == '1')
                            {
                                vis[nRow, nCol] = true;
                                queue.Enqueue((nRow, nCol));
                            }
                        }
                    }
                }
            }
        }
        return cnt;
    }
}

/*
================================================================================
 PROBLEM : You get a 2D grid of chars: '1' is land and '0' is water. Return
           how many islands there are. An island is land cells joined up,
           down, left or right. Diagonal cells do not join.
           [["1","1","0"],["0","0","0"],["0","1","1"]] -> 2
 PATTERN : Grid BFS (flood fill) + visited matrix
================================================================================
IDEA
  Scan every cell. When a cell is '1' and not yet in vis, it starts a new
  island,
  so cnt++. A BFS from that cell visits the whole island through a queue. It
  uses dRow/dCol for the 4 moves and marks each cell in vis when it is
  enqueued.
  Every land cell is marked exactly once, so each island is counted once, by
  its first cell in scan order. This file uses a separate vis array and does
  not change grid, so the input stays intact.
EXAMPLE
  grid = 110 / 010 / 101
  (0,0): cnt=1, BFS (0,0)->(0,1)->(1,1), then the queue is empty
  (2,0): cnt=2. It touches (1,1) only diagonally, so it is a new island
  (2,2): cnt=3. Answer = 3
COMPLEXITY
  Time  O(m * n)  each cell is enqueued at most once and checks 4 neighbours
  Space O(m * n)  the vis array, plus a queue that can hold many cells of one
                  island
WATCH OUT
  - Mark vis when you ENQUEUE, not when you dequeue. Otherwise one cell can
    be added many times by its neighbours. That is still correct, but slower.
  - An empty grid (rows == 0) crashes on grid[0].Length. Guard it if asked.
  - Compare with the char '1', not the int 1. grid[r][c] == 1 is always false.
  - Check the bounds before you read vis or grid. The code relies on && to
    short-circuit (skip the rest once one test is false).
================================================================================
*/
