// --------------------------------------------------------------------------
// -  suboptimal.cs         O(m * n) time / O(m * n) space
// --------------------------------------------------------------------------

public class Solution
{
    //MemoIzation
    public int UniquePaths(int m, int n)
    {
        int[,] dp = new int[m, n];

        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < n; j++)
            {
                dp[i, j] = -1;
            }
        }
        return Dfs(0, 0, m, n, dp);
    }

    private int Dfs(int i, int j, int m, int n, int[,] dp)
    {

        if (i >= m || j >= n)
            return 0;

        if (dp[i, j] != -1)
            return dp[i, j];

        if (i == m - 1 && j == n - 1)
            return 1;

        return dp[i, j] = Dfs(i + 1, j, m, n, dp) + Dfs(i, j + 1, m, n, dp);
    }
}

/*
================================================================================
 PROBLEM : A robot starts at the top-left cell of an m x n grid. It can only
           move right or down. Return how many different paths reach the
           bottom-right cell. Example: m = 3, n = 2 -> 3.
 PATTERN : 2D DP, top-down DFS + memoization
================================================================================
IDEA
  Dfs(i, j) returns the number of paths from cell (i, j) to the target.
  Stepping off the grid returns 0. The target cell returns 1.
  Every other cell adds Dfs(i + 1, j) and Dfs(i, j + 1), and the sum is
  cached in dp[i, j]. The value -1 means "not computed yet".
  This is correct because each path makes its first move either down or
  right, so the two sets of paths never overlap. optimal.cs instead fills
  the table bottom-up and keeps only one row.
EXAMPLE
  m = 3, n = 3. The last row and the last column all give 1 path.
  dp[1,1] = dp[2,1] + dp[1,2] = 1 + 1 = 2
  dp[1,0] = 1 + 2 = 3, dp[0,1] = 2 + 1 = 3
  dp[0,0] = dp[1,0] + dp[0,1] = 3 + 3 = 6 -> answer 6
COMPLEXITY
  Time  O(m * n)  each cell is computed once, then read from dp in O(1)
  Space O(m * n)  the m x n dp table, plus a recursion stack of depth m + n
WATCH OUT
  - The bounds check must come before reading dp[i, j]. If you swap the
    order, the code throws IndexOutOfRange when i == m or j == n.
  - Without dp the recursion is exponential. Always store the result with
    "return dp[i, j] = ...", not just "return ...".
  - The recursion depth is m + n - 1, so a very large grid can overflow the
    call stack. The bottom-up version has no such risk.
  - The answer is C(m+n-2, m-1). It overflows int on big grids.
================================================================================
*/
