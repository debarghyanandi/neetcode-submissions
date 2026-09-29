// --------------------------------------------------------------------------
// -  suboptimal.cs         O(n) time / O(n) space
// --------------------------------------------------------------------------

public class Solution
{
    public int ClimbStairs(int n)
    {
        if (n <= 2)
        {
            return n;
        }

        int[] dp = new int[n + 1];
        dp[1] = 1;
        dp[2] = 2;

        for (int i = 3; i <= n; i++)
        {
            dp[i] = dp[i - 1] + dp[i - 2];
        }

        return dp[n];
    }
}

/*
================================================================================
 PROBLEM : You climb a staircase with n steps. Each move is 1 step or 2 steps.
           Return how many distinct ways reach the top (order of moves
           matters). Example: n = 3 -> 3 (1+1+1, 1+2, 2+1).
 PATTERN : 1-D Dynamic Programming (bottom-up tabulation)
================================================================================
IDEA
  dp[i] holds the number of ways to reach step i.
  The last move onto step i is either 1 step (from i-1) or 2 steps (from i-2).
  So dp[i] = dp[i - 1] + dp[i - 2]. This is the Fibonacci recurrence.
  Fill dp from i = 3 up to n, starting from dp[1] = 1 and dp[2] = 2.
  Unlike optimal.cs, it keeps the whole table, not just the last two values.
EXAMPLE
  n = 5: dp[1]=1, dp[2]=2
  i=3: 2+1=3, i=4: 3+2=5, i=5: 5+3=8
  return dp[5] = 8
COMPLEXITY
  Time  O(n)  one loop from 3 to n, O(1) work per step
  Space O(n)  the dp array has n + 1 slots
WATCH OUT
  - Keep the n <= 2 guard. Without it, n = 1 makes dp of size 2, and
    dp[2] = 2 throws IndexOutOfRangeException.
  - n = 0 returns 0 here. Some versions expect 1 (one way: do nothing).
    Ask the interviewer.
  - int overflows from n = 46 (the answer is 2971215073). Use long if n can
    be large.
  - Base cases are dp[1]=1, dp[2]=2, not Fibonacci's 1, 1. Mixing them up
    shifts every answer by one index.
================================================================================
*/
