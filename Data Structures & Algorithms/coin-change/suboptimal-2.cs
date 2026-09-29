// --------------------------------------------------------------------------
// -  suboptimal-2.cs       O(n * m) time / O(n * m) space
// --------------------------------------------------------------------------

public class Solution
{
    const int INF = 1_000_000_000;

    public int CoinChange(int[] coins, int amount)
    {
        int n = coins.Length;

        int[,] dp = new int[n, amount + 1];

        // Base case: using only coins[0]
        for (int target = 0; target <= amount; target++)
        {
            dp[0, target] =
                target % coins[0] == 0
                    ? target / coins[0]
                    : INF;
        }

        for (int i = 1; i < n; i++)
        {
            for (int target = 0; target <= amount; target++)
            {
                int notTake = dp[i - 1, target];

                int take = INF;

                if (target >= coins[i])
                {
                    take = 1 + dp[i, target - coins[i]];
                }

                dp[i, target] = Math.Min(notTake, take);
            }
        }

        int ans = dp[n - 1, amount];

        return ans >= INF ? -1 : ans;
    }
}

/*
================================================================================
 PROBLEM : You get coin values coins[] and a target amount. Each coin can be
           used any number of times. Return the fewest coins that sum to
           amount, or -1 if no mix works. Example: coins = [1,2,5], amount =
           11 -> 3 (5+5+1).
 PATTERN : Unbounded Knapsack DP (2D bottom-up tabulation)
================================================================================
IDEA
  dp[i, target] is the fewest coins for target using only coins[0..i].
  Row 0 is direct: target is reachable only if coins[0] divides it.
  For each later i, notTake = dp[i-1, target] and take = 1 +
  dp[i, target - coins[i]], and we keep the smaller one. take reads the
  SAME row i, so coin i can be used again, which makes it unbounded.
  Each cell sees every choice for the last coin type, so the min is
  correct. optimal.cs keeps a single 1D row instead of this full table.
EXAMPLE
  coins = [5,2], amount = 6. Row 0 (coin 5): only t0=0 and t5=1 are real.
  Row 1 (coin 2): t2=1, t3=INF+1 (1+INF, but still >= INF), t4=2,
  t5=min(1, ...)=1, t6=min(INF, 1+dp[1,4]=3)=3.
  Answer dp[1,6] = 3 (2+2+2). Greedy would take the 5 and get stuck.
COMPLEXITY
  Time  O(n * m)  n rows times amount+1 columns, O(1) work per cell.
  Space O(n * m)  the full n by (amount+1) dp table is kept.
WATCH OUT
  - take must read dp[i, ...], not dp[i-1, ...]. Reading row i-1 turns it
    into 0/1 knapsack, where each coin is used at most once.
  - Unreachable cells can hold INF+1, INF+2, ... So test ans >= INF, not
    ans == INF. INF = 1e9 keeps 1 + INF from overflowing int.
  - The base row runs target % coins[0], so a coin of 0 crashes it. An
    empty coins array also crashes, because it reads coins[0].
  - amount = 0 must return 0. It works here because 0 % coins[0] == 0.
================================================================================
*/
