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