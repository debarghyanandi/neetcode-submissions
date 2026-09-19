public class Solution
{
    public int LongestCommonSubsequence(string text1, string text2)
    {
        //My solution
        //Tabulation one right shift
        int n = text1.Length;
        int m = text2.Length;

        int[,] dp = new int[n + 1, m + 1];
        /*for (int i = 0; i < n+1; i++){
            for (int j =0; j < m+1; j++){
                dp[i,j] = -1;
            }
        }*/
        //base cases
        for (int j = 0; j < m + 1; j++)
            dp[0, j] = 0;
        for (int i = 0; i < n + 1; i++)
            dp[i, 0] = 0;

        for (int i = 1; i < n + 1; i++)// (or <= n)
        {
            for (int j = 1; j < m + 1; j++) //(or <= m)
            {
                if (text1[i - 1] == text2[j - 1])
                    dp[i, j] = 1 + dp[i - 1, j - 1];
                else
                    dp[i, j] = Math.Max(dp[i - 1, j], dp[i, j - 1]);
            }
        }

        return dp[n, m];
    }

}
