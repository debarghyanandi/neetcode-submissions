public class Solution {
    //MemoIzation
    public int UniquePaths(int m, int n) {
        int [,] dp = new int [m,n];
        
        for (int i = 0; i < m; i++){
            for (int j = 0; j < n; j++){
                dp[i, j] = -1;
            }
        }
        return Dfs(0, 0, m, n, dp);
    }

    private int Dfs(int i, int j, int m, int n, int[,] dp){

        if(i >= m || j >= n) return 0;
        
        if (dp[i, j] != -1)
            return dp[i, j];
        
        if(i == m - 1 && j == n - 1)
            return 1;
        
        return dp[i, j] = Dfs(i + 1, j, m, n, dp) + Dfs(i, j + 1, m, n, dp);
    }
}
