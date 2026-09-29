// --------------------------------------------------------------------------
// -  suboptimal.cs         O(n * m) time / O(n * m) space
// --------------------------------------------------------------------------

public class Solution
{
    const int INF = 1_000_000_000;

    public int CoinChange(int[] coins, int amount)
    {
        int n = coins.Length;

        int[,] dp = new int[n, amount + 1];

        for (int i = 0; i < n; i++)
        {
            for (int target = 0; target <= amount; target++)
            {
                dp[i, target] = -1;
            }
        }

        int ans = F(n - 1, amount, coins, dp);

        return ans >= INF ? -1 : ans;
    }

    private int F(int i, int target, int[] coins, int[,] dp)
    {
        if (i == 0)
        {
            return target % coins[0] == 0
                ? target / coins[0]
                : INF;
        }

        if (dp[i, target] != -1)
            return dp[i, target];

        int notTake = F(i - 1, target, coins, dp);

        int take = INF;

        if (target >= coins[i])
        {
            take = 1 + F(i, target - coins[i], coins, dp);
        }

        return dp[i, target] = Math.Min(notTake, take);
    }
}

/*
================================================================================
 PROBLEM : Given coin values coins and a target amount, return the fewest
           coins that sum exactly to amount. Each coin can be used any number
           of times. Return -1 if no mix works. Example: coins = [1,2,5],
           amount = 11 -> 3 (5+5+1).
 PATTERN : DP (unbounded knapsack), top-down memoization
================================================================================
IDEA
  F(i, target) is the fewest coins to make target using only coins[0..i].
  For each coin choose: notTake (move to i-1) or take (stay at i, since the
  coin can be reused, and reduce target by coins[i]). Base case i == 0 is
  direct: target / coins[0] if it divides evenly, otherwise INF.
  Every multiset of coins is one path of take/notTake choices, so the min
  is correct. Unlike optimal.cs, this keeps a 2D dp table plus recursion.
EXAMPLE
  coins = [2,3], amount = 7. F(1,7): notTake F(0,7) = INF (7 odd);
  take = 1 + F(1,4). F(1,4): notTake F(0,4) = 2; take = 1 + F(1,1) = INF.
  So F(1,4) = 2, and F(1,7) = min(INF, 1+2) = 3. Answer 3 (2+2+3).
COMPLEXITY
  Time  O(n * m)  n * (amount+1) states, each does O(1) work once memoized
  Space O(n * m)  dp table is n by amount+1; recursion stack adds up to n +
                  amount
WATCH OUT
  - dp starts at -1, not 0, because 0 is a real answer (target 0).
  - take can be INF+1, INF+2, ... when no solution exists below it, so the
    final check must be ans >= INF, not ans == INF.
  - Recursion depth reaches about amount / coins[i] + n. A big amount with
    a coin of 1 can overflow the call stack. Bottom-up avoids this.
  - Empty coins gives n - 1 = -1 and crashes at F. Guard it if needed.
================================================================================
*/
