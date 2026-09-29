// --------------------------------------------------------------------------
// -  suboptimal.cs         O(n * m) time / O(n * m) space
// -  recursive memoization   [recursive-memoization]
// -  ranks below optimal.cs (O(n * m) time / O(m) space)
// -
// -  Reference solution - not one you solved yourself (from submission-2)
// -
// -  Memoization table has n×(amount+1) cells, each computed once with O(1)
// -  work; recursion depth is O(n).
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
 PATTERN : Unbounded Knapsack DP - memoized take / not-take
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-2.cs when it was first processed
 STATUS  : Suboptimal
================================================================================
VARIABLES
  INF     1_000_000_000, a marker that means "this target cannot be made"
  dp      dp[i, target] = fewest coins to make target using only coins[0..i]; -1 = not computed yet
  F       F(i, target) = the same value as dp[i, target], computed by recursion
  notTake best count when coins[i] is never used, so we drop to i - 1
  take    1 + best count after using coins[i] once; we stay on i, so the coin can be used again
WHY THIS PATTERN
  The problem gives a set of coins with unlimited copies of each and asks for
  the minimum count that reaches amount. That is an unbounded knapsack. At each
  coin i you either skip it (notTake, move to i - 1) or use it once more (take,
  stay at i with target - coins[i]). The same (i, target) pair is reached
  through many paths, so dp stores each answer once. This turns an exponential
  search into one computation per cell.
BETTER APPROACH
  The better solution is a bottom-up 1D table: best[t] = fewest coins for amount
  t. Set best[0] = 0, then for each coin c and each t from c to amount, best[t]
  = min(best[t], best[t - c] + 1). The time is the same, but it uses O(amount)
  memory instead of the n by amount+1 grid, and it has no recursion. This file
  loses on memory and on call-stack depth.
INVARIANT
  Once dp[i, target] is set, it holds the exact minimum number of coins from
  coins[0..i] that sum to target, or a value of at least INF if it cannot be
  done. Every valid combination either never uses coins[i] or uses it at least
  once. The first case is exactly notTake. The second case is exactly 1 plus an
  optimal answer for target - coins[i] with the same coins, which is take. So
  Math.Min of the two covers every choice.
CLOSED-FORM BASE CASE AT I == 0
  With only coins[0] left, you do not need more recursion. If target % coins[0]
  == 0 the answer is target / coins[0], and otherwise it is impossible (INF).
  This is also why row 0 of dp is never filled in or read.
WHY THE CHECK IS ANS >= INF
  An impossible result can grow above INF: take = 1 + F(...) adds 1 to an INF
  from further down the chain, so a failed branch can return INF + k. For this
  reason CoinChange tests ans >= INF and not ans == INF. INF is 1e9, so there is
  room for about 1.1e9 of these additions before int overflows.
WATCH OUT
  Recursion depth: the take branch keeps the same i and lowers target by only
  coins[i]. With a small coin, the depth reaches about amount / coins[i] + n
  frames, so a large amount can cause a StackOverflowException. If coins is
  empty, n = 0 and F(-1, ...) runs. It skips the i == 0 base case and then reads
  dp[-1, target], which throws IndexOutOfRangeException. When amount is 0, the
  code correctly returns 0 through the base case (0 % coins[0] == 0).
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How does this change if you must count the NUMBER of combinations that make
  amount (Coin Change II)?
     Replace Math.Min with a sum, and make the base case 1 when target %
     coins[0] == 0, otherwise 0. In the 1D bottom-up version, the coin loop must
     be outside the amount loop. If you swap the loops, you count orderings
     (permutations) instead of combinations.
  2. What if each coin may be used at most once?
     The take branch becomes 1 + F(i - 1, target - coins[i]), so you move on
     after taking. In 1D, loop t downward from amount to c, so a coin is not
     reused in the same pass.
  3. How would you return the actual coins used, not just the count?
     Keep the dp table. Walk from (n - 1, amount): if dp[i, target] equals the
     take value, record coins[i] and lower target; otherwise move to i - 1. This
     needs the full table, so you lose the 1D memory saving.
  4. Is there a non-DP view?
     BFS over amounts. Start at 0, and each edge adds one coin. The first level
     that reaches amount is the answer. The cost is the same order, but it can
     stop early when a short answer exists.
TRIGGER
  Reach for this pattern when the problem says "minimum number of items,
  unlimited copies of each, that sum exactly to a target".
C# NOTE
  int[,] cannot be passed to Array.Fill, which is why the file needs a nested
  loop to set -1. A jagged int[][] (or the 1D table) lets you write
  Array.Fill(row, -1) in one call per row.
COMPLEXITY
  Time  : O(n * m)
  Space : O(n * m)
================================================================================
*/
