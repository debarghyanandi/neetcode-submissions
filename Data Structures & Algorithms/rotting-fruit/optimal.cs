// ##########################################################################
// #  optimal.cs            O(m * n) time / O(m * n) space
// #  Multi-source BFS with time tracking   [multi-source-bfs]
// #  the only solution in this folder
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  Every cell visited once via BFS queue; time propagates from all rotten
// #  oranges simultaneously.
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
 PATTERN : Multi-source BFS on a grid - time stored per cell
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-0.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  vis      vis[i,j] = current state of cell (i,j): 0 empty, 1 fresh, 2 rotten
  q        queue of (row, col, time) - time = minutes for that orange to rot
  dRow     row offsets for the 4 neighbours
  dCol     matching column offsets for the 4 neighbours
  tm       largest time value popped so far = answer in minutes
WHY THIS PATTERN
  Rot spreads to all four neighbours at the same time, one step per minute, so
  every fresh orange rots at its shortest distance from any rotten orange. That
  is exactly breadth-first search, and because every already-rotten cell starts
  spreading at minute 0, all of them go into q before the loop starts. Pushing
  all sources at once makes the BFS layers equal to minutes. The last layer
  reached is tm.
BRUTE FORCE
  Simulate minute by minute: scan the whole grid, collect every cell that is 2,
  rot its fresh neighbours, repeat until one full pass changes nothing. That
  costs a full m*n scan per minute, so O(m*n*(m+n)) in the worst case, since the
  rot front can need that many minutes to cross the grid. BFS does the same work
  but visits each cell a constant number of times instead of rescanning.
INVARIANT
  When a cell is enqueued, its time field is already the smallest number of
  minutes in which it can rot, and vis is set to 2 at enqueue time, not at
  dequeue time. That means no cell ever enters q twice, and the queue holds
  times in non-decreasing order. So tm, the running maximum over popped times,
  ends as the minute the last orange rots.
MARKING ON ENQUEUE
  Setting vis[nRow, nCol] = 2 in the same if block that enqueues is what keeps
  the work linear. If the mark moved to the dequeue step, the same fresh cell
  could be pushed by two different rotten neighbours in the same layer, and the
  queue could grow well past m*n.
WATCH OUT
  grid[0] is read before any length check, so an empty outer array throws
  IndexOutOfRangeException; a zero-length first row gives cols = 0 and the code
  returns 0, which happens to be harmless. If the grid has no rotten orange at
  all, the while loop never runs, tm stays 0, and the final scan correctly
  returns -1 only because a fresh orange exists - a grid of only empty cells and
  no oranges also returns 0, which is the expected answer. vis is a full copy of
  grid, so the input is never mutated, but that also means the 0 cells are
  copied for nothing. The comment "My solution" says nothing about the algorithm
  and will not help you in a review.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you drop vis and work on grid directly?
     Yes - write 2 into grid[nRow][nCol] instead. That saves the m*n int array,
     but it destroys the caller's input, which is only acceptable if the problem
     says so.
  2. Can you avoid storing time in every queue entry?
     Yes - process the queue one layer at a time: read q.Count into a variable,
     pop exactly that many cells, then increment a minute counter. The tuple
     shrinks to (row, col) and tm becomes the layer counter, but you must not
     increment on the final empty layer.
  3. What if rot also spread diagonally?
     Extend dRow and dCol to 8 entries and change the inner loop bound from 4 to
     8; nothing else in the BFS changes.
  4. What if you had to report which orange rots last, not just when?
     Track the (row, col) of the entry that set a new maximum in the tm =
     Math.Max line, replacing it with an explicit comparison so you can save the
     coordinates alongside.
TRIGGER
  Several starting points spread outward at the same speed over a grid and you
  need the time or distance for the last cell reached.
C# NOTE
  dRow and dCol are List<int> built at every call; int[] { -1, 0, 1, 0 } as a
  static readonly field would do the same job without the per-call allocation
  and list indirection. Also, the tuple deconstruction var (row, col, time) =
  q.Dequeue() works because Queue<T> returns the value type directly - no boxing
  occurs here.
COMPLEXITY
  Time  : O(m * n)
  Space : O(m * n)
================================================================================
*/
