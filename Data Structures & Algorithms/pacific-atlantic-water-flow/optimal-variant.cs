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
 PROBLEM : Given an m x n grid heights, water flows from a cell to a
           4-neighbor whose height is equal or lower. The Pacific touches the
           top and left edges. The Atlantic touches the bottom and right
           edges. Return [row, col] of every cell that can reach both oceans:
           [[1,2],[4,3]] -> [[0,1],[1,0],[1,1]].
 PATTERN : Multi-source BFS from borders (reverse flow)
================================================================================
IDEA
  Do not start from every cell. Start from each ocean and climb uphill.
  pacificQueue holds the top row and left column. atlanticQueue holds the
  bottom row and right column. Bfs moves only to neighbors with height >=
  the current cell, and marks them in pacific or atlantic. A reverse step
  uphill is exactly a forward flow downhill, so a marked cell can drain to
  that ocean. The answer is every cell marked in both grids. This variant
  uses iterative BFS with queues, so deep recursion is not a risk.
EXAMPLE
  heights = [[1,2,3],[8,9,4],[7,6,5]] (a rising spiral)
  pacific: from 2 -> 9, and 3 -> 4 -> 5 -> 6, so all 9 cells are marked.
  atlantic: from 6 -> 9 and 7 -> 8, but 8 -> 1 and 3 -> 2 go down. So (0,0)
  and (0,1) stay false. Result: [0,2],[1,0],[1,1],[1,2],[2,0],[2,1],[2,2].
COMPLEXITY
  Time  O(m * n)  each cell is enqueued at most once per ocean, and has 4
                  neighbors.
  Space O(m * n)  two bool grids plus queues that can hold up to m*n cells.
WATCH OUT
  - The check is heights[nRow][nCol] < heights[row][col] -> skip. If you
    write <= instead, plateaus of equal height break and cells are missed.
  - Mark visited when you enqueue, not when you dequeue. Otherwise the
    same cell enters the queue many times and the time blows up.
  - heights[0].Length throws on an empty grid. Guard rows == 0 if needed.
  - Corner cells are seeded twice by the two loops. This is harmless, but
    it shows the seeds are not deduplicated.
================================================================================
*/
