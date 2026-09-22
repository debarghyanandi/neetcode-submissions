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
 PATTERN : Unbounded Knapsack DP - two rolling rows over coins
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-5.cs when it was first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  INF      1_000_000_000, stands for "target not reachable"
  n        number of distinct coin values
  prev     prev[target] = fewest coins for target using coins[0..i-1]
  curr     curr[target] = fewest coins for target using coins[0..i]
  notTake  answer for this target when coin i is skipped entirely
  take     1 + best answer for target - coins[i], coin i still allowed again
WHY THIS PATTERN
  The problem asks for the minimum number of coins that sum to exactly amount,
  and each coin value may be used any number of times. That is a choose/skip
  decision per coin value with unlimited repeats, which is the unbounded
  knapsack shape: for every coin index i and every target, either skip coin i
  (notTake) or spend one of it and stay on the same coin (take). Only the
  previous coin's row is ever read for notTake, so the full 2D table collapses
  to prev and curr. The final prev[amount] is the answer over all coins.
BRUTE FORCE
  The first thing most people write is plain recursion f(i, target) that returns
  min(f(i+1, target), 1 + f(i, target - coins[i])). It is correct but re-solves
  the same (i, target) pairs over and over, so it runs in exponential time.
  Adding memoization on that pair already fixes it; this file is the same table
  filled bottom-up, with the row dimension dropped.
INVARIANT
  At the moment curr[target] is assigned, prev holds the exact best answers for
  every target using only coins[0..i-1], and curr[0..target-1] already holds the
  exact best answers using coins[0..i]. Since take reads curr[target -
  coins[i]], which is strictly to the left and already final, and notTake reads
  prev[target], every way to build target from coins[0..i] is covered. After the
  swap, prev is again a complete, correct row for one more coin, so after the
  last coin prev[amount] is the global minimum.
TAKE READS THE CURRENT ROW
  take = 1 + curr[target - coins[i]] uses curr, not prev. That is what makes
  coins reusable: the smaller target has already been solved with coin i
  available, so spending coin i again costs just one more. Reading prev there
  would allow each coin value at most once and turn this into 0/1 knapsack.
NO DP[0] = 0 SENTINEL
  There is no "all INF except index 0" initialization. The base row is seeded
  directly from coins[0] by divisibility: target / coins[0] when it divides, INF
  otherwise. prev[0] becomes 0 for free because 0 % coins[0] == 0, which is what
  makes every later take chain terminate at a real count.
WATCH OUT
  coins[0] is read before any length check, so an empty coins array throws
  IndexOutOfRangeException, and a coin value of 0 throws DivideByZeroException
  in the base row. INF is not clamped: when both branches are unreachable,
  curr[target] can become INF + 1, and that +1 can accumulate once per coin row,
  so the stored value drifts above 1_000_000_000. It stays far below
  int.MaxValue here and the final test is ans >= INF rather than ans == INF, so
  the drift is handled - but changing that test to == would silently break the
  -1 case. curr is never cleared between rows; this is safe only because the
  inner loop writes every index 0..amount.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can this run on one array instead of two?
     Yes. notTake is just "leave the value alone", so a single dp looping target
     forward gives dp[target] = Math.Min(dp[target], 1 + dp[target - coins[i]]).
     Same time, half the memory, and the swap disappears; the cost is that the
     "previous coin" row is no longer available if you later need it for
     debugging or reconstruction.
  2. How would you return the actual coins used, not just the count?
     Keep a parallel array choice[target] recording which coin index produced
     the minimum, then walk back from amount subtracting that coin until you hit
     0. That adds O(amount) memory and keeps the same time.
  3. What if the question were "how many different combinations sum to amount"
  instead of the minimum?
     Replace Math.Min with addition and change the base row to prev[target] =
     target % coins[0] == 0 ? 1 : 0. The loop structure and the same-row read
     stay exactly as they are, since combinations also allow unlimited reuse.
TRIGGER
  Exact target, an unlimited supply of a small set of item values, and you want
  the minimum (or the count) of items.
C# NOTE
  (prev, curr) = (curr, prev) is tuple deconstruction and swaps only the two
  array references, so no element copying happens between rows. If you ever move
  to the single-array version, note that new int[amount + 1] already zero-fills,
  which is why the explicit base-row loop has to overwrite every slot rather
  than assume anything.
COMPLEXITY
  Time  : O(n * m)
  Space : O(m)
================================================================================
*/
