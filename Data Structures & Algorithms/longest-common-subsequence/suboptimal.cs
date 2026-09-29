// ##########################################################################
// #  suboptimal.cs         O(n * m) time / O(n * m) space
// ##########################################################################

public class Solution
{
    public int LongestCommonSubsequence(string text1, string text2)
    {
        //My solution
        //Memoization
        int n = text1.Length;
        int m = text2.Length;

        int[,] dp = new int[n, m];
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < m; j++)
            {
                dp[i, j] = -1;
            }
        }
        return Lcs(n - 1, m - 1, text1, text2, dp);
    }

    private int Lcs(int i, int j, string s, string t, int[,] dp)
    {
        if (i < 0 || j < 0)
            return 0;

        if (dp[i, j] != -1)
            return dp[i, j];

        if (s[i] == t[j])
            return dp[i, j] = 1 + Lcs(i - 1, j - 1, s, t, dp);

        return dp[i, j] = Math.Max(Lcs(i - 1, j, s, t, dp), Lcs(i, j - 1, s, t, dp));
    }
}

/*
================================================================================
 PROBLEM : Given two strings text1 and text2, return the length of their
           longest common subsequence. A subsequence keeps the order of
           characters but may skip some; it does not have to be contiguous.
           Return 0 if none exists. Example: "abcde", "ace" -> 3 ("ace").
 PATTERN : 2D DP, top-down (recursion + memoization)
================================================================================
IDEA
  Lcs(i, j) is the LCS of the prefixes s[0..i] and t[0..j], walking from the
  ends. If s[i] == t[j], that char is used: 1 + Lcs(i-1, j-1). Otherwise
  drop one end: max(Lcs(i-1, j), Lcs(i, j-1)). Any index below 0 means an
  empty prefix, so it returns 0. dp[i, j] caches each answer (-1 = unknown),
  so each state is solved once. optimal.cs does the same bottom-up in 1D.
EXAMPLE
  text1="abcde", text2="ace", start Lcs(4,2): e==e -> 1 + Lcs(3,1).
  Lcs(3,1): d!=c -> max(Lcs(2,1), Lcs(3,0)). Lcs(2,1): c==c -> 1+Lcs(1,0).
  Lcs(1,0)=max(Lcs(0,0)=1, 0)=1, so Lcs(2,1)=2; Lcs(3,0)=1 (memo hit).
  Lcs(3,1)=2, answer Lcs(4,2)=3.
COMPLEXITY
  Time  O(n * m)  n*m states, each computed once with O(1) work plus memo hits
  Space O(n * m)  dp table is n*m; the recursion stack adds up to n+m frames
WATCH OUT
  - Recursion depth reaches about n+m. Very long strings can overflow the
    call stack in C#. Say this, and offer the bottom-up table as the fix.
  - On a match, do not also try the skip branches. Taking s[i]==t[j] is
    always safe, and extra calls only waste time.
  - The sentinel must be -1, not 0. 0 is a real LCS value, so a 0 sentinel
    recomputes those states and loses the memo speedup.
  - Empty input is fine: n-1 = -1 hits the i < 0 base case before dp[i, j].
================================================================================
*/
