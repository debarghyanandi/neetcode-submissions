// --------------------------------------------------------------------------
// -  optimal-variant.cs    O(m * n) time / O(m * n) space
// --------------------------------------------------------------------------

public class Solution
{
    // 4 directions: down, up, right, left
    private readonly int[][] directions =
    {
        new int[] { 1, 0 },
        new int[] { -1, 0 },
        new int[] { 0, 1 },
        new int[] { 0, -1 }
    };

    public List<List<int>> PacificAtlantic(int[][] heights)
    {
        int rows = heights.Length;
        int cols = heights[0].Length;

        // Cells reachable from each ocean
        bool[,] pacific = new bool[rows, cols];
        bool[,] atlantic = new bool[rows, cols];

        Queue<int[]> pacificQueue = new Queue<int[]>();
        Queue<int[]> atlanticQueue = new Queue<int[]>();

        // Pacific touches top row
        // Atlantic touches bottom row
        for (int col = 0; col < cols; col++)
        {
            pacificQueue.Enqueue(new int[] { 0, col });
            pacific[0, col] = true;

            atlanticQueue.Enqueue(new int[] { rows - 1, col });
            atlantic[rows - 1, col] = true;
        }

        // Pacific touches left column
        // Atlantic touches right column
        for (int row = 0; row < rows; row++)
        {
            pacificQueue.Enqueue(new int[] { row, 0 });
            pacific[row, 0] = true;

            atlanticQueue.Enqueue(new int[] { row, cols - 1 });
            atlantic[row, cols - 1] = true;
        }

        // Reverse BFS from both oceans
        Bfs(pacificQueue, pacific, heights);
        Bfs(atlanticQueue, atlantic, heights);

        List<List<int>> result = new List<List<int>>();

        // A cell is valid if it can reach both oceans
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                if (pacific[row, col] && atlantic[row, col])
                {
                    result.Add(new List<int> { row, col });
                }
            }
        }

        return result;
    }

    private void Bfs(
        Queue<int[]> queue,
        bool[,] visited,
        int[][] heights)
    {
        int rows = heights.Length;
        int cols = heights[0].Length;

        while (queue.Count > 0)
        {
            int[] current = queue.Dequeue();

            int row = current[0];
            int col = current[1];

            foreach (int[] direction in directions)
            {
                int nRow = row + direction[0];
                int nCol = col + direction[1];

                // Invalid position
                if (nRow < 0 || nRow >= rows ||
                    nCol < 0 || nCol >= cols)
                {
                    continue;
                }

                // Already reachable from this ocean
                if (visited[nRow, nCol])
                    continue;

                // Reverse water flow:
                // From ocean, move only to equal or higher cells
                if (heights[nRow][nCol] < heights[row][col])
                    continue;

                visited[nRow, nCol] = true;
                queue.Enqueue(new int[] { nRow, nCol });
            }
        }
    }
}

/*
================================================================================
 PROBLEM : You get a grid of heights. The Pacific touches the top and left
           edges. The Atlantic touches the bottom and right edges. Water flows
           to a neighbour of equal or lower height. Return every [row, col]
           that can reach both oceans. Example: [[1,2],[3,4]] ->
           [[0,1],[1,0],[1,1]].
 PATTERN : Multi-source BFS from borders (reverse flow)
================================================================================
IDEA
  Do not start a search from each cell. Start from the oceans and walk uphill.
  Each ocean's border cells go into pacificQueue or atlanticQueue first. Bfs
  then moves to a neighbour only if it is equal or higher. It marks reached
  cells in pacific or atlantic. A cell is in the answer if both flags are
  true.
  This is correct because "water flows down from X to the sea" is the same
  path as "from the sea, climb up to X" read backwards. This variant uses an
  iterative BFS with queues, so it has no deep recursion.
EXAMPLE
  heights = [[1,2],[3,4]]. Pacific seeds (0,0),(0,1),(1,0); BFS adds (1,1).
  Atlantic seeds (1,0),(1,1),(0,1); (0,0)=1 is lower than 2 and 3, skipped.
  Both flags: (0,1),(1,0),(1,1). The low corner (0,0) only reaches Pacific.
  Answer: [[0,1],[1,0],[1,1]] (row-major order from the final scan).
COMPLEXITY
  Time  O(m * n)  each cell is set visited once per ocean, so it is enqueued
                  once each
  Space O(m * n)  two bool grids plus queues that hold at most all cells
WATCH OUT
  - Flip the height test and you get the wrong answer. In reverse BFS, skip
    when heights[nRow][nCol] < heights[row][col]. Equal heights must pass.
  - Set visited when you enqueue, not when you dequeue. Otherwise one cell
    can enter the queue many times.
  - An empty heights throws on heights[0].Length. Guard it if asked.
  - Corners are enqueued twice (top row loop + left column loop). This is
    harmless here, but it is extra work.
================================================================================
*/
