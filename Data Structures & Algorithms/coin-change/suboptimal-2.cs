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
 PATTERN : Unbounded Knapsack DP - 2D coin/target table
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-3.cs when it was first processed
 STATUS  : Suboptimal
================================================================================
VARIABLES
  INF       1_000_000_000, stands for "this target is unreachable"
  dp        dp[i, target] = fewest coins from coins[0..i] that sum to exactly target
  notTake   dp[i - 1, target], the answer if coin i is never used
  take      1 + dp[i, target - coins[i]], reuse coin i once more on the same row
  ans       dp[n - 1, amount], the final cell before the -1 translation
WHY THIS PATTERN
  The problem asks for the minimum number of coins for an exact amount, and each
  coin may be used any number of times. That is an unbounded knapsack: at every
  cell there are only two choices, skip coin i forever, or take coin i and stay
  on coin i. Both choices point at strictly smaller sub-targets, so a table over
  (coin index, target) covers every case with no repeated work. dp[n - 1,
  amount] is the answer for all coins and the full amount.
BETTER APPROACH
  The better version keeps one int[] dp of length amount + 1 and loops the coins
  on the outside, writing dp[target] = Math.Min(dp[target], 1 + dp[target -
  coins[i]]). Because take reads dp[i, ...] on the same row, never the previous
  row, the row can be updated in place - the old row is not needed. That is the
  same number of operations but drops the space from n rows to one, and this
  file loses only on memory.
INVARIANT
  When the loop finishes row i, every dp[i, target] holds the true minimum coin
  count using only coins[0..i], or INF if that target cannot be made. Row 0 is
  set directly by the divisibility test on coins[0]. Each later cell combines
  notTake (row i - 1, already final) and take (same row, smaller target, already
  final in this left-to-right pass), so the induction holds and the last row is
  correct for all coins.
TAKE READS THE SAME ROW
  take uses dp[i, target - coins[i]], not dp[i - 1, ...]. That one index is what
  allows a coin to be used many times. Change it to i - 1 and the code silently
  becomes 0/1 knapsack - each coin usable once - which still compiles and still
  returns numbers, just wrong ones.
WHY 1 + INF NEVER OVERFLOWS
  take can be 1 + 1_000_000_000, which fits in int with room to spare. More
  important, Math.Min with notTake clamps the stored value back to at most INF,
  since row 0 never exceeds INF and each row inherits that bound. So INF never
  grows row by row and the final test ans >= INF is safe.
WATCH OUT
  An empty coins array breaks this immediately: dp is allocated as new int[0,
  amount + 1] and the first loop writes dp[0, target], throwing
  IndexOutOfRangeException, and coins[0] would throw too. A zero or negative
  value inside coins would also break things - target % coins[0] divides by
  zero, and a coin of 0 makes take read its own cell. amount = 0 is fine and
  returns 0 through the target % coins[0] == 0 branch. The comment "Base case:
  using only coins[0]" is accurate, but note it also does the unbounded logic by
  division rather than by the recurrence, so a bug fixed in the main loop must
  be checked against this loop separately.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How do you report which coins were used, not just how many?
     Keep a parallel choice table, or walk back from dp[n - 1, amount]: if dp[i,
     t] equals dp[i - 1, t] move up a row, otherwise record coins[i] and move to
     t - coins[i]. The walk-back costs no extra memory but needs the full 2D
     table, so it conflicts with the 1D rolling version.
  2. What changes if the question asks for the number of ways to make the amount
  instead of the fewest coins?
     Replace Math.Min with addition, set the base row to 1 where target %
     coins[0] == 0, and drop INF entirely. The shape of the loops and the
     same-row read stay identical.
  3. What if each coin may be used at most once?
     take becomes 1 + dp[i - 1, target - coins[i]], and if you then compress to
     1D you must loop target downward so a coin is not reused inside the same
     pass.
  4. amount is large but the coin values are small - any other angle?
     Treat it as a shortest path and run BFS from 0 over edges of length
     coins[i], stopping at the first time amount is reached. Same worst case,
     but it can stop early when the answer is small, at the cost of a visited
     array and a queue.
TRIGGER
  Reach for this when an exact target must be hit by items that can be picked an
  unlimited number of times and you must minimise or count the picks.
C# NOTE
  int[,] is a single rectangular block, so you cannot swap or reassign rows the
  way you can with int[][] or a plain int[] - if you later compress to the
  rolling version, the type has to change too.
COMPLEXITY
  Time  : O(n * m)
  Space : O(n * m)
================================================================================
*/
