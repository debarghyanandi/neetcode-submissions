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
 PROBLEM : Input is a 2D grid of chars, '1' for land and '0' for water. Return
           how many islands there are. An island is land cells joined up,
           down, left or right; diagonal cells do NOT connect. Example:
           [[1,1,0],[0,0,0],[0,1,1]] -> 2.
 PATTERN : Grid BFS (flood fill) + visited matrix
================================================================================
IDEA
  Scan every cell. When a cell is '1' and not yet in vis, it starts a new
  island.
  So cnt++, and a BFS from it marks every connected land cell in vis.
  The BFS uses a Queue and the dRow/dCol arrays to try the 4 neighbours.
  Later scans skip marked cells, so each island is counted exactly once.
  Unlike a DFS, it uses an explicit queue, so deep islands cannot overflow
  the call stack. It also keeps grid unchanged by using a separate vis.
EXAMPLE
  grid = [[1,1,0],[0,1,0],[1,0,1]]
  (0,0): cnt=1, BFS marks (0,0) -> (0,1) -> (1,1); (1,1) has no new nbrs.
  (2,0): cnt=2. It touches (1,1) only by a diagonal, so it is a new island.
  (2,2): cnt=3. Answer: 3.
COMPLEXITY
  Time  O(m * n)  each cell is marked once and dequeued once; 4 checks per
                  cell.
  Space O(m * n)  the vis matrix has rows*cols cells, plus the BFS queue.
WATCH OUT
  - Mark vis when you ENQUEUE, as the code does. If you mark on dequeue,
    one cell can be added many times, and the queue can grow far too big.
  - An empty grid crashes: grid[0].Length throws when rows == 0.
    Return 0 early if grid.Length == 0.
  - Cells are chars: compare with '1', not 1. The int test is never true,
    so the count is always 0.
  - Check the bounds before you read vis[nRow, nCol] or grid[nRow][nCol].
    The && short-circuit order in the if statement is what prevents a crash.
================================================================================
*/
