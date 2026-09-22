// --------------------------------------------------------------------------
// -  optimal-variant.cs    O(m * n) time / O(m * n) space
// -  Iterative BFS island search   [bfs-grid-islands]
// -  ties with optimal.cs on O(m * n) time / O(m * n) space
// -
// -  Reference solution - not one you solved yourself
// -
// -  Each cell visited once via BFS queue; queue holds frontier nodes at
// -  maximum capacity.
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
 PATTERN : BFS flood fill on a grid - count connected components
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-1.cs when it was first processed
 STATUS  : Optimal variant - ties the best complexity by another route
================================================================================
VARIABLES
  vis        vis[row, col] = true once that cell has been enqueued
  cnt        number of islands started so far
  dRow/dCol  the four neighbour offsets, paired by index i
  queue      cells of the current island still waiting to expand
  nRow/nCol  candidate neighbour of the dequeued cell (r, c)
WHY THIS PATTERN
  The problem asks how many separate groups of '1' cells exist, where cells join
  only through up/down/left/right links. That is exactly counting connected
  components in an implicit graph whose nodes are land cells and whose edges are
  the four dRow/dCol steps. The outer double loop finds a cell that has never
  been reached, bumps cnt once, then the BFS drains every cell reachable from it
  so those cells can never start a second count.
BRUTE FORCE
  The naive idea is union-find or, worse, repeatedly scanning the grid and
  merging any two adjacent land cells until nothing changes. That
  repeat-until-stable scan can cost O(m * n) per pass and O(m * n) passes in a
  snake-shaped island. BFS reaches the same answer in a single sweep because
  each cell is enqueued at most once.
INVARIANT
  Every cell with vis true is either already counted inside some island or
  sitting in queue for the current one, and no cell is ever enqueued twice. So
  when the while loop drains queue, the whole component containing the start
  cell is marked, and the outer loop can only trigger again on a genuinely new
  component. cnt therefore equals the number of components finished.
MARK ON ENQUEUE
  vis[nRow, nCol] is set at the moment of Enqueue, not after Dequeue. If you
  moved the marking to dequeue time, a cell with two already-queued neighbours
  would be pushed twice, and the queue could swell to more than m * n entries;
  the count would still be right but the memory bound would break. The same
  guard doubles as the "not yet seen" test in the if, so one array does two
  jobs.
WATCH OUT
  grid[0].Length runs before any emptiness check, so a null grid or a
  zero-length outer array throws immediately. Because grid is char[][] (jagged),
  a short inner row would make grid[row][col] throw even though nCol < cols
  passed - the bounds check trusts cols from row 0 only. Note the cells are char
  '1' and '0', not int 1 and 0; comparing against 1 would not compile the way
  you expect and comparing against '0' is the easy typo. Finally the tuple (int,
  int) has unnamed fields, so nothing stops you from later writing (c, r) in the
  wrong order.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you drop the vis array entirely?
     Yes - overwrite grid[r][c] = '0' when you enqueue. Same time, no extra
     bool[,], but it destroys the caller's input, so you would document that or
     restore it afterwards.
  2. DFS instead of BFS?
     Recursion replaces the queue and the code shrinks, but a long snaking
     island makes the call stack as deep as m * n and can overflow. The explicit
     Queue here keeps the growth on the heap.
  3. What if the grid is too large to hold in memory at once, streamed row by
  row?
     Switch to union-find over two rows at a time: keep component ids for the
     previous row, union downward as each new row arrives, and count the roots
     at the end. Memory falls to O(n).
  4. What if diagonal touching also counts as one island?
     Extend dRow and dCol to eight entries with the four diagonal pairs; nothing
     else changes, the loop bound 4 becomes dRow.Length.
TRIGGER
  A grid of two symbols where the question is "how many groups" or "how big is
  the largest group" under 4-directional adjacency.
C# NOTE
  bool[,] is a true rectangular array with one allocation and one bounds pair,
  which is why vis[row, col] is safe while grid[row][col] (jagged, separately
  allocated rows) is not; Queue<(int, int)> stores value tuples inline, so no
  per-cell object is allocated.
COMPLEXITY
  Time  : O(m * n)
  Space : O(m * n)
================================================================================
*/
