// ##########################################################################
// #  suboptimal-2.cs       O(n * m) time / O(n * m) space
// ##########################################################################

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

/*
================================================================================
 PROBLEM : Given two strings text1 and text2, return the length of their
           longest common subsequence. A subsequence keeps the order of
           characters but may skip some; it need not be contiguous. Return 0
           if nothing is shared. Example: text1 = "abcde", text2 = "ace" -> 3
           ("ace").
 PATTERN : 2D DP (tabulation) on two prefixes
================================================================================
IDEA
  dp[i, j] is the LCS length of the first i chars of text1 and the first j
  chars of text2. Indexes are shifted right by one, so row 0 and column 0
  mean "empty prefix" and hold 0. If text1[i-1] == text2[j-1], that char
  extends the LCS of both shorter prefixes: 1 + dp[i-1, j-1]. Otherwise one
  of the two chars is unused, so take max(dp[i-1, j], dp[i, j-1]).
  Unlike optimal.cs, it keeps the whole (n+1) x (m+1) table, not one row.
EXAMPLE
  text1 = "abcde" (rows), text2 = "ace" (cols). Rows i=1..5 end as:
  a: 1 1 1 | b: 1 1 1 | c: 1 2 2 (match: 1+dp[2,1]) | d: 1 2 2
  e: 1 2 3 (match at j=3: 1+dp[4,2] = 3)
  Answer dp[5, 3] = 3.
COMPLEXITY
  Time  O(n * m)  each of the n*m cells is filled once in O(1)
  Space O(n * m)  the full dp table has (n+1)*(m+1) ints
WATCH OUT
  - Off by one: dp[i, j] compares text1[i-1] and text2[j-1]. Using
    text1[i] overruns the string. Return dp[n, m], not dp[n-1, m-1].
  - On a mismatch, do not add 1 or use dp[i-1, j-1]. Take only the max of
    skipping one char: dp[i-1, j] or dp[i, j-1].
  - The base-case loops are redundant: new int[,] is already all zeros.
    The commented -1 fill is memoization leftover, unused in tabulation.
================================================================================
*/
