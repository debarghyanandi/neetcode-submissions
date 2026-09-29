// --------------------------------------------------------------------------
// -  suboptimal-2.cs       O(n * m) time / O(n * m) space
// -  iterative bottom-up tabulation   [iterative-tabulation]
// -  ranks below optimal.cs (O(n * m) time / O(m) space)
// -
// -  Reference solution - not one you solved yourself (from submission-3)
// -
// -  Fills 2D table row-by-row iteratively; n coins × (amount+1) targets,
// -  each cell O(1) work.
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
 PATTERN : DP / Unbounded Knapsack - min coins, reuse the same row
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-3.cs when it was first processed
 STATUS  : Suboptimal
================================================================================
VARIABLES
  INF      1_000_000_000, the "cannot make this amount" value
  dp       dp[i, target] = fewest coins to make target using only coins[0..i]
  notTake  dp[i-1, target], the best result when coins[i] is never used
  take     1 + dp[i, target - coins[i]], the best result when coins[i] is used at least once
  ans      dp[n-1, amount] before it is turned into -1 if it is INF
WHY THIS PATTERN
  The problem asks for the fewest coins, and each coin can be used any number of
  times. That is an unbounded knapsack with "min count" as the value. Greedy
  fails for some coin sets, so you must try every choice. For each coin i and
  each target, you either skip coins[i] (notTake) or use one more coins[i]
  (take). dp stores the best result for each smaller problem, so no work is done
  twice.
BETTER APPROACH
  The better approach uses a 1D array: dp[t] = min over all coins c of dp[t - c]
  + 1, with dp[0] = 0. The time is the same, but it needs only O(amount) memory.
  This file keeps all n rows. But row i only reads row i-1 (notTake) and row i
  itself (take). So all the older rows are memory it never needs again.
INVARIANT
  After row i is filled, dp[i, target] is the true minimum number of coins from
  coins[0..i] that sum to target, or INF if no mix of them can. Row 0 is exact:
  target is possible only if coins[0] divides it, and then it takes target /
  coins[0] coins. For row i, every valid mix either uses zero copies of coins[i]
  (covered by dp[i-1, target]) or at least one (one copy plus the best mix for
  target - coins[i]). The code takes the min of these two, so it covers every
  case, and row n-1 at amount is the answer.
TAKE READS THE SAME ROW
  take reads dp[i, target - coins[i]], not dp[i - 1, ...]. This is what lets one
  coin be used many times. The target loop goes upward, so that cell is already
  filled for row i. If you read row i-1 here, you get 0/1 knapsack (each coin
  used at most once), and the answer is wrong.
INF IS NOT INT.MAXVALUE
  1 + dp[...] is computed even when that cell is INF. With INF = 1_000_000_000,
  the result 1_000_000_001 still fits in an int. With int.MaxValue it would
  overflow to a negative number and win the Math.Min. The final check is ans >=
  INF, not ans == INF, so a value just above INF would still become -1.
WATCH OUT
  The base case reads coins[0] with no check. An empty coins array throws
  IndexOutOfRangeException, even when amount is 0. A coin of value 0 in coins[0]
  causes a divide-by-zero in target % coins[0]. A 0 coin at a later index makes
  take read dp[i, target], the same cell that is being computed.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Return how many different ways make amount (Coin Change II), not the fewest
  coins.
     Replace min with sum: dp[i, t] = dp[i-1, t] + dp[i, t - coins[i]], with
     dp[., 0] = 1. In the 1D version the coin loop must be the outer loop. If
     the amount loop is outer, you count orderings (permutations) instead of
     sets of coins.
  2. Return which coins were used, not just the count.
     Keep the full 2D table (a good use for this file's extra memory). Start at
     (n-1, amount). If dp[i, t] == dp[i-1, t], move up a row. If not, record
     coins[i] and move to t - coins[i]. With a 1D table, store the last coin
     chosen for each t instead.
  3. Can you solve it as a shortest path?
     Yes. Use BFS (breadth-first search: visit nodes level by level) from 0,
     where each edge adds one coin. The first level that reaches amount is the
     answer. It can stop early when the answer is small, but it needs a visited
     array of size amount + 1 and a queue.
TRIGGER
  When a problem asks for the fewest (or number of) items to reach an exact
  total and items can be reused, reach for unbounded knapsack DP.
C# NOTE
  int[,] is a rectangular array. It is one block, indexed as dp[i, target]. A
  jagged int[][] would need n + 1 separate allocations for the same table.
COMPLEXITY
  Time  : O(n * m)
  Space : O(n * m)
================================================================================
*/
