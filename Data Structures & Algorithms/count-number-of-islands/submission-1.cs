public class Solution {
    public int NumIslands(char[][] grid) {
        int n = grid.Length;
        int m = grid[0].Length;
        bool[,] vis = new bool[n, m];
        int cnt = 0;

        int[] dRow = { -1, 1, 0, 0 };
        int[] dCol = { 0, 0, -1, 1 };

        for (int row = 0; row < n; row++) {
            for (int col = 0; col < m; col++) {
                if (!vis[row, col] && grid[row][col] == '1') {
                    cnt++;
                    Queue<(int, int)> queue = new();
                    queue.Enqueue((row, col));
                    vis[row, col] = true; // mark visited when ENQUEUED, not when dequeued

                    while (queue.Count > 0) {
                        var (r, c) = queue.Dequeue();
                        for (int i = 0; i < 4; i++) {
                            int nRow = r + dRow[i];
                            int nCol = c + dCol[i];
                            if (nRow >= 0 && nRow < n && nCol >= 0 && nCol < m &&
                                !vis[nRow, nCol] && grid[nRow][nCol] == '1') {
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