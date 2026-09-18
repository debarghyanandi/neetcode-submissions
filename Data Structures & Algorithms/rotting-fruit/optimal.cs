// ##########################################################################
// #  optimal.cs            O(m * n) time / O(m * n) space
// #  Multi-source BFS level-by-level spread   [multi-source-bfs]
// #  the only solution in this folder
// #
// #  YOU SOLVED THIS YOURSELF (from submission-0)
// #
// #  Each cell visited once; queue stores O(m·n) cells in worst case.
// ##########################################################################

public class Solution
{
    public int OrangesRotting(int[][] grid)
    {
        //My solution
        int rows = grid.Length;
        int cols = grid[0].Length;
        Queue<(int row, int col, int time)> q = new();
        int[,] vis = new int[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                vis[i, j] = grid[i][j];
                if (vis[i, j] == 2)
                    q.Enqueue((i, j, 0));
            }
        }

        List<int> dRow = new List<int> { -1, 0, 1, 0 };
        List<int> dCol = new List<int> { 0, 1, 0, -1 };

        int tm = 0;
        while (q.Count > 0)
        {
            var (row, col, time) = q.Dequeue();
            tm = Math.Max(time, tm);

            for (int i = 0; i < 4; i++)
            {
                int nRow = row + dRow[i];
                int nCol = col + dCol[i];

                if (nRow >= 0 && nCol >= 0 && nRow < rows && nCol < cols
                    && vis[nRow, nCol] == 1)
                {
                    q.Enqueue((nRow, nCol, time + 1));
                    vis[nRow, nCol] = 2;
                }
            }
        }

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (vis[i, j] == 1)
                    return -1;
            }
        }

        return tm;
    }
}


/*
================================================================================
 PATTERN : Multi-source BFS on a grid - time carried per node
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-0.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  The question asks for the number of minutes until the last fresh orange rots,
  and every rotten orange spreads at the same time. That is a shortest-distance
  problem with many starting points, so all cells holding 2 go into the queue at
  time 0 and BFS expands them together. Because BFS visits cells in
  non-decreasing time order, the time value on the last dequeued cell is the
  answer, which is what tm collects.
BRUTE FORCE
  The first thing most people write is a simulation loop: scan the whole grid
  each minute, rot every 1 that touches a 2, repeat until a full pass changes
  nothing. That is correct but costs a full m*n scan per minute, and the number
  of minutes can itself grow with m*n, so it is O((m*n)^2). BFS does the same
  spreading in one pass because the queue already holds exactly the cells that
  just rotted.
INVARIANT
  Every cell in the queue is rotten, and its stored time is the exact minute at
  which it rotted. A cell is set to 2 in vis at the moment it is enqueued, not
  when it is dequeued, so no cell can ever enter the queue twice and no cell
  gets a larger time than its true one. When the queue drains, every cell
  reachable from an initial rotten orange has been marked, so any remaining 1 in
  vis is unreachable and the method returns -1.
WHY MARK AT ENQUEUE, NOT AT DEQUEUE
  vis[nRow, nCol] = 2 sits right next to the Enqueue call. If you moved
  that line into the dequeue step instead, two neighbours of the same fresh
  orange could both push it, and the queue could grow far past m*n. Marking on
  push is what keeps each cell in the queue at most once.
THE COPY INTO STATE
  The code copies grid into a separate int[,] vis instead of writing over the
  caller's int[][] grid. That keeps the input unchanged, which is polite and
  safe if the caller reuses the grid. It costs one extra m*n array; say this out
  loud in an interview, because the obvious memory saving is to drop vis and
  mutate grid directly.
WATCH OUT
  cols comes from grid[0].Length, so a null grid or a grid with zero rows throws
  before any logic runs; mention the guard if the interviewer cares. The final
  double loop is not optional - without it a grid that still has a 1 stranded
  behind empty cells would wrongly return tm instead of -1. Also note
  tm starts at 0 and the all-empty grid never enters the loop, so 0 is
  returned, which is the expected answer but only by luck of the initial value,
  not by any explicit check.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you drop the extra time field in the queue tuple?
     Yes - queue only (row, col) and process the queue one level at a time using
     the count at the start of each round, incrementing a minute counter per
     level. Same complexity, smaller queue entries, but you must be careful to
     increment only when the next level is non-empty or you overcount by one.
  2. How would you avoid the final full scan of the grid?
     Count the fresh oranges during the first pass and decrement that counter
     each time you mark a cell rotten; at the end return -1 if the counter is
     not zero. It removes one m*n pass, though the overall complexity is
     unchanged.
  3. What changes if rot also spreads diagonally?
     Extend dRow and dCol to the eight offsets and change the inner loop
     bound from 4 to 8; nothing else in the BFS changes, since the algorithm
     never assumes only four neighbours.
  4. The grid is far too big to hold in memory at once - now what?
     Stream it in row bands and run BFS on a frontier of boundary cells, or move
     to a disk-backed or distributed level-synchronous BFS. The per-level
     structure of this algorithm survives; only the storage of vis has to be
     paged.
TRIGGER
  Many starting points spread outward at the same speed and you need the time
  for the last cell to be reached - push all sources into one queue at distance
  0.
C# NOTE
  Queue<(int row, int col, int time)> uses a value tuple, so each entry is
  stored inline in the queue's backing array with no per-node object, and var
  (row, col, time) = queue.Dequeue() deconstructs it in one line; the List<int>
  dRow and dCol could be int[] literals since they are never resized.
COMPLEXITY
  Time  : O(m * n)
  Space : O(m * n)
================================================================================
*/
