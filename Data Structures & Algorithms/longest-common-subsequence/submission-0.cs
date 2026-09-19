public class Solution {
    public int LongestCommonSubsequence(string text1, string text2) {
        //My solution
        //Memoization
        int n = text1.Length;
        int m = text2.Length;
        
        int[,] dp = new int[n, m];
        for (int i = 0; i < n; i++){
            for (int j =0; j < m; j++){
                dp[i,j] = -1;
            }
        }
        return Lcs(n-1, m-1, text1, text2, dp);
    }

    private int Lcs(int i, int j, string s, string t, int [,] dp){
        if( i < 0 || j < 0 )
            return 0;

        if(dp[i, j] != -1)
            return dp[i, j];

        if(s[i] == t[j])
            return dp[i, j] = 1 + Lcs(i-1, j-1, s, t, dp);

        return dp[i, j] = Math.Max(Lcs(i-1, j, s, t, dp), Lcs(i, j-1, s, t, dp)); 
    }
}
