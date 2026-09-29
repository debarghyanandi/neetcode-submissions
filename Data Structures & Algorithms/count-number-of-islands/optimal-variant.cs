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
 PATTERN : Graph BFS / Flood Fill - count connected components
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-1.cs when it was first processed
 STATUS  : Optimal variant - ties the best complexity by another route
================================================================================
VARIABLES
  vis      vis[r,c] = true once cell (r,c) has been put in some queue
  cnt      number of islands found so far (one per new BFS start)
  dRow     row offsets for up, down, left, right
  dCol     column offsets that go with dRow at the same index
  nRow     row of the neighbor being checked
  nCol     column of the neighbor being checked
WHY THIS PATTERN
  The problem asks how many groups of '1' cells touch each other up, down, left
  or right. That is the number of connected components in a grid graph. Each
  land cell is a node, and each pair of touching land cells is an edge. The
  outer loops find a land cell that is not yet in vis, add 1 to cnt, and run a
  BFS (breadth-first search: visit cells level by level using a queue). The BFS
  marks the whole island, so no later start can count it again.
BRUTE FORCE
  The simplest correct approach is union-find (a structure that merges sets).
  Give each land cell its own set, then union it with its right and down land
  neighbors. The answer is the number of distinct roots. With path compression
  it is about O(m * n) too, but it needs more code and a parent array. A more
  naive approach restarts a full search from every land cell and removes
  duplicates. That costs O((m * n)^2) and loses badly.
INVARIANT
  When a BFS starts, every cell already in vis belongs to an island that cnt has
  already counted. During the BFS, a cell goes into the queue only if it is
  land, inside the grid, and not yet in vis, and it is marked at that moment. So
  each land cell is enqueued exactly once, in total. When the queue is empty,
  every land cell reachable from (row, col) is in vis. So each island adds
  exactly 1 to cnt.
MARK ON ENQUEUE
  vis is set when a cell is enqueued, both for the start cell and inside the
  neighbor loop. If you marked it only on dequeue, the same cell could be
  enqueued by two neighbors before it is processed. The result would still be
  correct, but the queue could hold duplicates and do extra work. The code
  matches its own comment here.
WATCH OUT
  grid[0].Length throws if grid is empty (rows == 0), because there is no row 0.
  Add a guard that returns 0 before reading cols. The code also assumes the grid
  is rectangular, because cols comes from row 0 only. A jagged grid with a
  shorter later row would throw at grid[nRow][nCol].
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you drop the vis array to save memory?
     Yes. Write '0' into grid when you enqueue a cell. This saves the O(m * n)
     bool array, but it changes the caller's input. Ask first if that is
     allowed.
  2. Why BFS and not recursive DFS?
     Recursive DFS is shorter, but one long island can make the recursion very
     deep and overflow the call stack. BFS with a queue keeps that state on the
     heap. The queue grows only to about the width of the BFS frontier.
  3. What if diagonal cells also connect?
     Extend dRow and dCol to 8 entries by adding the four diagonal offsets.
     Nothing else changes.
  4. What if land cells are added one at a time and you must report the count
  after each?
     Use union-find. Each add makes a new set (cnt++), and each union with a
     land neighbor does cnt--. Each step costs almost O(1), instead of a full
     rescan.
TRIGGER
  When a grid question asks you to count or measure groups of touching cells,
  run a flood fill from each unvisited cell and count how many times you start
  one.
C# NOTE
  bool[,] is one rectangular block indexed as vis[r, c], while grid is a jagged
  char[][] indexed as grid[r][c]. Keep the two index styles apart when you edit.
  The value tuple (int, int) in Queue avoids allocating a new object for each
  cell, and var (r, c) unpacks it in one line.
COMPLEXITY
  Time  : O(m * n)
  Space : O(m * n)
================================================================================
*/
