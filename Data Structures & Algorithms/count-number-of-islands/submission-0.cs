public class Solution {
    public int NumIslands(char[][] grid) {
        //My solution
        int n = grid.Length;
        int m = grid[0].Length;

        // Visited array
        int [,] vis = new int [n, m];
        int cnt = 0;
        for(int row = 0; row < n; row++){
            for(int col = 0; col < m; col++){
                if(vis[row, col] == 0 && grid[row][col] == '1'){
                    Dfs(row, col, vis, grid);
                    cnt++;
                }
            }
        }
        return cnt;
    }

    private void Dfs (int row, int col, int [,] vis, char[][] grid)
    {
        int n = grid.Length;
        int m = grid[0].Length;

        //mark this node visisted
        vis[row, col] = 1;

        //visit all 6 neighbours
        for(int delRow = -1; delRow <= 1; delRow++){
            for(int delCol = -1; delCol <= 1; delCol++){
                //exclude the node itself(0,0) and diagonal nodes
                if (Math.Abs(delRow) == Math.Abs(delCol))
                    continue;
                
                int nRow = row + delRow;
                int nCol = col + delCol;

                //invalid neighbour validation
                //land validation
                //visited validation
                if (nRow >= 0 && nRow < n &&
                    nCol >= 0 && nCol < m &&
                    vis[nRow, nCol] == 0 &&
                    grid[nRow][nCol] == '1'){
                        Dfs(nRow, nCol, vis, grid);
                    }
            }
        }

    }
}
