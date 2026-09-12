// --------------------------------------------------------------------------
// -  optimal-variant.cs    O(m * n) time / O(m * n) space
// -  BFS flood fill with queue   [bfs-queue-flood-fill]
// -  ties with optimal.cs on O(m * n) time / O(m * n) space
// -
// -  Reference solution - not one you solved yourself (from submission-1)
// -
// -  each cell enqueued and dequeued at most once, visited marked on
// -  enqueue to avoid duplicates; queue can hold O(m*n) cells worst case
// --------------------------------------------------------------------------

public class Solution {
    public int NumIslands(char[][] grid)
    {
        int rows = grid.Length;
        int cols = grid[0].Length;
        bool[,] visited = new bool[rows, cols];
        int islandCount = 0;

        int[] rowDelta = { -1, 1, 0, 0 };
        int[] colDelta = { 0, 0, -1, 1 };

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                if (!visited[row, col] && grid[row][col] == '1')
                {
                    islandCount++;
                    Queue<(int, int)> queue = new();
                    queue.Enqueue((row, col));
                    visited[row, col] = true; // mark visited when ENQUEUED, not when dequeued

                    while (queue.Count > 0)
                    {
                        var (currRow, currCol) = queue.Dequeue();
                        for (int i = 0; i < 4; i++)
                        {
                            int nRow = currRow + rowDelta[i];
                            int nCol = currCol + colDelta[i];
                            if (nRow >= 0 && nRow < rows && nCol >= 0 && nCol < cols &&
                                !visited[nRow, nCol] && grid[nRow][nCol] == '1')
                            {
                                visited[nRow, nCol] = true;
                                queue.Enqueue((nRow, nCol));
                            }
                        }
                    }
                }
            }
        }
        return islandCount;
    }
}

/*
================================================================================
 PATTERN : BFS flood fill on a grid with a visited matrix
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-1.cs when it was first processed
 STATUS  : Optimal variant - ties the best complexity by another route
================================================================================
WHY THIS PATTERN
  The question "how many islands" is "how many connected components" in
  disguise. The grid is an implicit graph: each '1' cell is a vertex, and edges
  run to the four orthogonal neighbours that are also '1'. Counting components
  in a graph always has the same shape - sweep every vertex, and whenever you
  hit one that no traversal has reached yet, increment the counter and burn down
  everything reachable from it. The traversal itself never increments; it only
  consumes. That split is the whole solution: the double for loop over row/col
  owns the counting, the while loop over queue owns the erasing.
ALGORITHM
  1. Read rows from grid.Length and cols from grid[0].Length, allocate visited
  as bool[rows, cols].
  2. Scan every (row, col) in row-major order.
  3. If grid[row][col] == '1' and !visited[row, col], this cell belongs to a
  component nothing has touched yet. Do islandCount++.
  4. Seed a fresh Queue<(int, int)> with (row, col) and set visited[row, col] =
  true immediately.
  5. While the queue is non-empty, dequeue (currRow, currCol) and walk i from 0
  to 3, forming (nRow, nCol) from rowDelta[i] and colDelta[i]. The paired arrays
  { -1, 1, 0, 0 } and { 0, 0, -1, 1 } spell out up, down, left, right.
  6. Enqueue the neighbour only if it passes all four guards at once: in bounds
  on both axes, not visited, and equal to '1'. Mark it visited in the same
  breath as the enqueue.
  7. Return islandCount after the sweep finishes.
INVARIANT
  Every cell is enqueued at most once across the entire run. visited[r, c] is
  set to true in the same statement block that enqueues (r, c), and it is never
  reset to false anywhere, so the !visited[nRow, nCol] guard can never let the
  same coordinate through twice. Total enqueues are therefore bounded by rows *
  cols.

  The counting argument rides on top of that. When the outer scan arrives at a
  land cell with visited still false, that cell cannot be connected to any
  island already counted - if it were, the BFS for that earlier island would
  have reached it and flipped the flag before the scan got here. So islandCount
  increments exactly once per connected component: never twice for the same
  island (the BFS consumes the rest of it), never zero times (the scan visits
  every cell, so it must reach at least one member of every component).
THE TRAP
  The marking has to happen at enqueue time, and the inline comment in the file
  is there because this is the single thing most people get backwards. Suppose
  you moved visited[nRow, nCol] = true down to just after the Dequeue instead.
  Take a 2x2 block of land. Dequeue (0,0), push (0,1) and (1,0) unmarked.
  Dequeue (0,1), see (1,1) still unmarked, push it. Dequeue (1,0), see (1,1)
  STILL unmarked because it has not been dequeued yet, push it a second time.
  Every cell now enters the queue once per land neighbour - up to four times.

  Worth being precise about the damage: the returned count is still correct,
  because the duplicate dequeue finds all its neighbours already visited and
  does nothing. What you lose is the bound from the invariant above, and with it
  the clean argument for the stated runtime. The genuinely fatal version is
  dropping the !visited check on the neighbour entirely - then two adjacent land
  cells push each other forever.
BFS OR DFS
  The other route to the same complexity is recursive DFS: same visited matrix,
  same four deltas, the call stack replacing the queue. It is shorter to write
  and interviewers accept either.

  The reason to reach for the queue is stack depth. DFS recursion on a grid that
  is entirely land descends once per cell, so a 300x300 board is 90,000 nested
  frames - a real stack overflow risk, not a theoretical one. This BFS holds
  only one frontier at a time, which in a grid is O(min(rows, cols)) cells, and
  it lives on the heap. If you do argue for DFS, be ready to say you would
  convert it to an explicit Stack<(int, int)> for large inputs.
WATCH OUT
  grid[0].Length throws on a zero-row grid; guard rows == 0 first if the
  constraints allow an empty board. The parameter type is jagged char[][], not
  char[,], so cols is really just row zero's length - the code assumes
  rectangularity, which LeetCode guarantees but a general caller does not.

  The comparison is to the character literal '1', never the integer 1 or the
  string "1". And note the two indexing styles sitting side by side in the same
  condition: visited is a true rectangular array indexed visited[nRow, nCol]
  with a comma, while the grid is jagged and indexed grid[nRow][nCol] with two
  brackets. Mixing them up is a compile error here, but it is the kind of thing
  that stalls you on a whiteboard.
FOLLOW-UP
  "Can you drop the visited array?" Yes - sink the island as you traverse by
  writing grid[nRow][nCol] = '0' at the point where you currently set the flag.
  A zeroed cell fails the == '1' test, so it does the same job as the boolean.
  That removes the rows * cols auxiliary allocation and leaves only the frontier
  in the queue. The cost is that you have destroyed the caller's input, which is
  the trade to name out loud rather than perform silently.

  "What about diagonal connectivity?" That is a two-line change and the reason
  the deltas are in arrays rather than four hand-written branches: extend
  rowDelta and colDelta to the eight offsets and raise the loop bound from 4 to
  8. Nothing else in the traversal cares.
TRIGGER
  Reach for this shape whenever the input is a grid and the question counts,
  sizes, or labels regions of like-valued cells - number of islands, max area of
  island, surrounded regions, number of provinces, number of distinct islands.
  The tell is that no cell belongs to two answers, which is exactly what makes a
  single visited matrix sufficient.

  It is the wrong shape the moment the question asks for a shortest distance
  from a specific source, a multi-source expansion by rounds (rotting oranges),
  or a weighted path - those need the queue to carry a level or a cost, and this
  loop deliberately carries neither.
COMPLEXITY
  Time  : O(m * n)
  Space : O(m * n)
================================================================================
*/
