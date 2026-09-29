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
 PATTERN : Multi-source BFS - all rotten oranges start at time 0
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-0.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  q       queue of (row, col, time) = a rotten cell and the minute it turned rotten
  vis     vis[r, c] = current state of cell (0 empty, 1 fresh, 2 rotten); a copy of grid
  dRow    row offsets for the 4 neighbours (up, right, down, left)
  dCol    column offsets, paired with dRow by index
  tm      largest time taken from q so far = minutes until the last orange rots
WHY THIS PATTERN
  Rot spreads one step per minute to the 4 neighbours, from every rotten orange
  at once. That is a shortest-distance question on a grid with unit edges, and
  BFS answers it. Putting every starting 2 into q with time 0 is the same as one
  BFS from a single virtual source joined to all of them. So each fresh orange
  gets time = distance to its nearest rotten orange, and the answer is the
  largest one, kept in tm.
BRUTE FORCE
  Simulate minute by minute. Each minute, scan the whole grid, find fresh cells
  next to a cell that was rotten at the start of that minute, and rot them. Stop
  when a minute changes nothing. Each scan costs O(m * n), and there can be up
  to O(m * n) minutes, so the worst case is O((m * n)^2). It loses because it
  rescans cells that have already settled, while BFS touches each cell once.
INVARIANT
  q always holds cells in non-decreasing order of time. A cell is set to 2 in
  vis at the moment it is enqueued, with the time of its first discovery. BFS
  finds each cell first from the earliest possible front, so that time is the
  true minute it rots. When q is empty, every fresh orange that any rotten
  orange can reach is rotten. Any 1 still left in vis is unreachable, and -1 is
  the correct answer for it.
MARK ON ENQUEUE, NOT ON DEQUEUE
  The line vis[nRow, nCol] = 2 runs right after Enqueue. Two rotten neighbours
  in the same minute can both see the same fresh cell. Because it is marked
  right away, the second one sees 2 and skips it. If you marked on dequeue
  instead, the cell would be enqueued twice. The answer would still be right,
  but q would grow with duplicate work.
WATCH OUT
  grid[0].Length throws if grid has no rows. Add a guard before it if empty
  input is possible. The name vis is misleading: it is not a visited flag. It is
  a full copy of the grid state, and its values 1 and 2 carry meaning. tm =
  Math.Max(time, tm) is safe, but because q is in time order, tm = time would
  give the same result.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you use less extra memory?
     Write the 2s straight into grid and drop vis. This saves the copy, but it
     changes the caller's input, so ask first if that is allowed.
  2. Can you avoid the last full scan for leftover fresh oranges?
     Count fresh cells in the first loop. Subtract one each time you rot a cell.
     At the end, return count > 0 ? -1 : tm. You can also return 0 early if the
     count starts at 0.
  3. How do you drop time from the tuple?
     Process level by level. Read q.Count at the start of each round, dequeue
     exactly that many cells, then add one minute if anything rotted in that
     round. The trade-off is an extra inner loop and care to not count the last
     empty round.
  4. What if rot also spreads diagonally?
     Add the 4 diagonal offsets to dRow and dCol and loop to 8. The BFS logic
     stays the same.
TRIGGER
  Something spreads one step per unit of time from many starting cells at once,
  and you need the time or distance until it reaches everything.
C# NOTE
  dRow and dCol are built as new List<int> on every call, but they never change.
  A static readonly array such as (int dr, int dc)[] with the four pairs keeps
  each offset pair together in one place and is allocated only once.
COMPLEXITY
  Time  : O(m * n)
  Space : O(m * n)
================================================================================
*/
