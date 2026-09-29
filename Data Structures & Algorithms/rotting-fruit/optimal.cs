// ##########################################################################
// #  optimal.cs            O(m * n) time / O(m * n) space
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
 PROBLEM : Grid cells are 0 (empty), 1 (fresh fruit) or 2 (rotten fruit). Each
           minute, every fresh fruit next to a rotten one (up, down, left,
           right) becomes rotten. Return the minutes until no fresh fruit is
           left, or -1 if some fruit can never rot. [[2,1,1],[0,1,1],[1,0,1]]
           -> -1
 PATTERN : Multi-source BFS on a grid (level = minute)
================================================================================
IDEA
  Put every rotten cell into q at time 0, all at once, as the BFS start.
  Each dequeued cell rots its fresh neighbours in vis and enqueues them
  with time + 1. tm keeps the largest time seen. At the end, any 1 left
  in vis was never reached, so return -1. BFS reaches each cell first by
  its shortest path, so each time is the minute it really rots.
EXAMPLE
  grid [[2,1,1],[1,1,0],[0,1,1]], start q = (0,0,t0)
  (0,0) -> (0,1)t1,(1,0)t1; (0,1) -> (1,1)t2,(0,2)t2; (1,0) adds none
  (1,1) -> (2,1)t3; (2,1) -> (2,2)t4; no 1 left in vis -> answer 4
COMPLEXITY
  Time  O(m * n)  each cell is enqueued at most once, with 4 neighbour checks
  Space O(m * n)  the vis copy of the grid plus q, both up to m * n cells
PATH TO OPTIMAL
  Simulate minute by minute, rescanning the whole grid - O((m*n)^2) -
    simple, but a long snake of fruit needs about m*n full scans.
  Multi-source BFS from all rotten cells - O(m*n) - each cell is handled
    once, not once per minute (this file, optimal.cs).
KEYWORDS
  multi-source BFS, grid BFS, shortest time spread, queue, flood fill,
  4-directional neighbours
WATCH OUT
  - Set vis[nRow, nCol] = 2 when you enqueue, not when you dequeue.
    Otherwise one fruit can be enqueued twice by two rotten neighbours.
  - Start BFS from ALL rotten cells together. BFS from each one alone
    gives wrong times and costs O((m*n)^2).
  - No fresh fruit at all must return 0, not -1. This code does it
    right, because tm stays 0.
  - grid[0].Length throws on an empty grid. Add a guard if rows can be 0.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you use O(1) extra space?
     -> Change grid in place instead of copying it to vis. Also count the
        fresh fruit first and stop when the count hits 0. Time stays O(m*n), but
        this destroys the caller's input.
  2. How do you skip storing time in each queue entry?
     -> Process q level by level: read q.Count, pop that many, then do
        minutes++. Same O(m*n) time, and the queue items are smaller.
  3. What if fruit also rots diagonally, or walls block the spread?
     -> Use 8 direction pairs in dRow/dCol, or skip wall cells in the check.
        The BFS stays the same, still O(m*n).
  4. Why BFS and not DFS?
     -> BFS pops cells in time order, so the first visit is the earliest
        minute. DFS would need to revisit cells when it finds a smaller time.
TRIGGER
  Many sources spread to their neighbours step by step at the same time,
  and you need the time or distance until everything is reached.
================================================================================
*/
