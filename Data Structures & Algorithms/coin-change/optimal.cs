// --------------------------------------------------------------------------
// -  optimal.cs            O(n * m) time / O(m) space
// -  space-optimized rolling array   [space-optimized-rolling-array]
// -  ranks above suboptimal.cs (O(n * m) time / O(n * m) space)
// -
// -  Reference solution - not one you solved yourself (from submission-5)
// -
// -  Rolling two 1D arrays instead of full 2D table; saves space by reusing
// -  rows after each coin.
// --------------------------------------------------------------------------

public class Solution
{
    const int INF = 1_000_000_000;

    public int CoinChange(int[] coins, int amount)
    {
        //space optimization
        int n = coins.Length;

        int[] prev = new int[amount + 1];
        int[] curr = new int[amount + 1];

        // Base case: using only coins[0]
        for (int target = 0; target <= amount; target++)
        {
            prev[target] =
                target % coins[0] == 0
                    ? target / coins[0]
                    : INF;
        }

        for (int i = 1; i < n; i++)
        {
            for (int target = 0; target <= amount; target++)
            {
                int notTake = prev[target];

                int take = INF;

                if (target >= coins[i])
                {
                    take = 1 + curr[target - coins[i]];
                }

                curr[target] = Math.Min(notTake, take);
            }
            (prev, curr) = (curr, prev);
        }

        int ans = prev[amount];

        return ans >= INF ? -1 : ans;
    }
}

/*
================================================================================
 PATTERN : Unbounded Knapsack DP - two rolling rows, min coins
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-5.cs when it was first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  INF      1_000_000_000, the mark for "this target cannot be made"
  prev     prev[target] = fewest coins from coins[0..i-1] that sum to target
  curr     curr[target] = fewest coins from coins[0..i] that sum to target
  notTake  prev[target], the best count when coins[i] is not used
  take     1 + curr[target - coins[i]], the best count when we use one more coins[i]
WHY THIS PATTERN
  The problem asks for the fewest coins to reach amount, and each coin can be
  used any number of times. That is an unbounded knapsack: at each coin you
  either skip it or use it one more time. Coins are handled one at a time. Each
  coin gets a full pass over target from 0 to amount. The answer for each target
  is built from smaller targets that are already solved.
BRUTE FORCE
  The first correct idea is a recursion: f(t) = 1 + min over every coin c of f(t
  - c), with f(0) = 0. It finds the right answer. But it solves the same
  sub-amounts again and again, so the time grows exponentially with amount.
  Memoizing f(t) fixes this. It becomes the same O(n * m) DP, just with
  recursion stack depth up to amount.
INVARIANT
  After the pass for coin i, row [target] holds the fewest coins from
  coins[0..i] that sum to target, or INF if no mix of those coins can make it.
  The base row is right because a single coin makes target only when target %
  coins[0] == 0. Each later cell takes the min of two cases that cover every
  option: never use coins[i] (notTake), or use it at least once (take). So after
  the last pass, prev[amount] is the true minimum.
TAKE READS CURR, NOT PREV
  take uses curr[target - coins[i]], which is the row for the same coin, and
  that cell was already filled earlier in this pass. This is what lets one coin
  be used many times. If it read prev instead, each coin could be used at most
  once, and you would have 0/1 knapsack.
SWAP MEANS THE ANSWER IS IN PREV
  After each pass, (prev, curr) = (curr, prev), so the newest row is always in
  prev. That is why ans reads prev[amount]. It also works when n == 1: the loop
  never runs, and prev still holds the base row. After a swap, curr holds an old
  row. That is safe, because every curr[target] is written before any later cell
  in the same pass reads it.
WATCH OUT
  An empty coins array throws on coins[0] in the base case. There is no guard
  for it. INF must stay under int.MaxValue - 1, because take adds 1 to a value
  that can be INF. With 1_000_000_000 this does not overflow, but a bigger INF
  such as int.MaxValue would wrap to a negative number. The check ans >= INF
  depends on every real answer being below 1_000_000_000.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you do it with one array?
     Yes. Use dp[t] = Math.Min(dp[t], 1 + dp[t - c]) for each coin, with t going
     upward. Reading dp[t - c] from the same pass gives the unbounded reuse.
     This drops the second array, and the time stays the same.
  2. How would you count the number of ways to make amount instead (Coin Change
  II)?
     Replace min with sum, set ways[0] = 1, and keep coins in the outer loop. If
     you swap the loops, you count orderings (permutations) instead of
     combinations.
  3. How would you return which coins were used?
     Store, for each target, the last coin that gave the best value. Then walk
     back from amount. It costs O(m) extra space.
  4. Is there a different way to see the problem?
     Run BFS over sums from 0, one level per coin added. The first time you
     reach amount, the level is the answer. It uses the same O(m) visited space,
     and it can stop early.
TRIGGER
  Reach for this when you must hit an exact total using items that can be reused
  without limit, and you want the min, the max, or the number of ways.
C# NOTE
  The tuple swap (prev, curr) = (curr, prev) only swaps the two array
  references. It copies no elements, so each pass costs no extra memory or time
  for the swap.
COMPLEXITY
  Time  : O(n * m)
  Space : O(m)
================================================================================
*/
