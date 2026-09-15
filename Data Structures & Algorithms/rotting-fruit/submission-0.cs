public class Solution {
    public int OrangesRotting(int[][] grid) {
        //My solution
        int n = grid.Length;
        int m = grid[0].Length;
        Queue<(int row, int col, int time)> q = new();
        int [,] vis = new int [n, m];
        for (int i = 0; i < n; i++){
            for(int j = 0; j < m; j++){
                vis [i, j] = grid[i][j];
                if (vis[i, j] == 2)
                q.Enqueue((i, j, 0));
            }
        }
        List<int> dRow = new List<int> { -1, 0, 1, 0 };
        List<int> dCol = new List<int> { 0, 1, 0, -1 };

        int tm = 0;
        while(q.Count > 0){
            var (row, col, time) = q.Dequeue();
            tm = Math.Max(time, tm);
            for(int i = 0; i < 4; i++){
                int nRow = row + dRow[i];
                int nCol = col + dCol[i];

                if(nRow >= 0 && nCol >= 0 && nRow < n && nCol < m
                    && vis[nRow, nCol] == 1){
                        q.Enqueue((nRow, nCol, time+1));
                        vis[nRow, nCol] = 2;
                    }
            }
        }

        for(int i = 0; i < n; i++){
            for(int j = 0; j < m; j++){
                if(vis[i,j] == 1)
                return -1;
            }
        }
        return tm;
    }
}
