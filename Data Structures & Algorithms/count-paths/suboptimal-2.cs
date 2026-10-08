// --------------------------------------------------------------------------
// -  suboptimal-2.cs       O(m * n) time / O(m * n) space
// --------------------------------------------------------------------------

public class Solution
{
    public int UniquePaths(int m, int n)
    {

        int[,] dp = new int[m + 1, n + 1];

        dp[m - 1, n - 1] = 1;

        for (int i = m - 1; i >= 0; i--)
        {
            for (int j = n - 1; j >= 0; j--)
            {
                dp[i, j] += dp[i + 1, j] + dp[i, j + 1];
            }
        }

        return dp[0, 0];
    }
}

/*
================================================================================
 PROBLEM : A robot starts at the top-left cell of an m x n grid. It can only
           move right or down. Return how many different paths reach the
           bottom-right cell. Example: m = 3, n = 2 -> 3.
 PATTERN : 2D Dynamic Programming (bottom-up, grid paths)
================================================================================
IDEA
  dp[i, j] is the number of paths from cell (i, j) to the target. We seed
  dp[m-1, n-1] = 1 and fill the table from the bottom-right corner back to
  (0, 0). Each cell adds the path counts of its down cell dp[i+1, j] and its
  right cell dp[i, j+1]. The extra row m and column n stay 0, so no bounds
  checks are needed. This is correct because every path takes exactly one
  first step, down or right. Unlike optimal.cs, this file keeps the whole
  grid instead of one row.
EXAMPLE
  m = 2, n = 3. Seed dp[1,2] = 1.
  Row i=1: dp[1,2] = 1+0+0 = 1, dp[1,1] = 0+1 = 1, dp[1,0] = 0+1 = 1.
  Row i=0: dp[0,2] = 1+0 = 1, dp[0,1] = 1+1 = 2, dp[0,0] = 1+2 = 3.
  Answer: dp[0,0] = 3.
COMPLEXITY
  Time  O(m * n)  two nested loops visit each of the m * n cells once, O(1)
                  work each
  Space O(m * n)  the full (m+1) x (n+1) table dp is stored
WATCH OUT
  - The "+=" matters. With "=", the target cell becomes 0 + 0 and loses its
    seed of 1, so every answer is 0.
  - The padding row and column are required. With new int[m, n], reading
    dp[i+1, j] or dp[i, j+1] goes out of bounds.
  - The result is C(m+n-2, m-1) in an int. It overflows for large grids,
    e.g. m = n = 18 gives 2333606220. Use long if no limits are given.
  - If m or n is 0, dp[m-1, n-1] throws an index error.
================================================================================
*/
