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
 PATTERN : Unbounded Knapsack - top-down memo over (coin index, target)
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-2.cs when it was first processed
 STATUS  : Suboptimal
================================================================================
VARIABLES
  INF       1e9 sentinel meaning "target cannot be built"
  dp        dp[i, target] = fewest coins from coins[0..i] summing to target
  notTake   best count when coin i is skipped entirely
  take      1 + best count when coin i is used once more (i stays)
WHY THIS PATTERN
  The problem asks for the minimum number of coins for one exact amount, and
  each coin may be used any number of times. That is unbounded knapsack: at each
  coin you choose "skip it forever" or "use one and stay on the same coin". The
  recursion F(i, target) encodes exactly those two moves, and because the same
  (i, target) pair is reached by many different coin orders, dp caches it so
  each pair is solved once.
BETTER APPROACH
  The better version is the 1-D bottom-up loop: a single int[amount + 1] where
  dp[t] = min(dp[t], dp[t - c] + 1) for each coin c, ascending t. It visits the
  same states but drops the memo table to one row and removes recursion. This
  file loses on two counts: it allocates the full n by (amount + 1) int table,
  and it recurses, so the call depth can reach amount / min(coins) frames and
  risk a stack overflow on a large amount with a small coin.
INVARIANT
  Whenever F(i, target) returns, the value is the true minimum coin count using
  only coins[0..i] to reach exactly target, or INF if impossible. The base row i
  == 0 is true by direct arithmetic: only coins[0] is available, so the amount
  must be divisible by it. Every later row is the min of the two legal moves
  over already-correct subresults, so induction on i and on decreasing target
  gives the final answer at F(n - 1, amount).
WHY TAKE RECURSES ON I, NOT I - 1
  The line take = 1 + F(i, target - coins[i], ...) keeps the same index i. That
  is the single change that turns 0/1 knapsack into unbounded: after spending
  one coin of that type you are allowed to spend it again. Recursion still
  terminates because target strictly drops by coins[i], which is positive.
INF ARITHMETIC IS SAFE HERE
  take is built as 1 + a child result, and that child may be INF, so sums like
  INF + 1 flow upward. With INF at 1_000_000_000 and int max near 2.14e9, the
  additions stay inside int as long as the chain of +1 on top of an INF stays
  short, which it does because any path adding many 1s would have found a real
  answer. Choosing int.MaxValue instead of 1e9 would overflow on the very first
  1 + INF.
WATCH OUT
  The memo uses -1 as "not computed", which works only because a coin count is
  never negative; if the sentinel scheme ever changed, the check dp[i, target]
  != -1 would silently return garbage. The base case divides and mods by
  coins[0], so a zero-valued coin throws DivideByZeroException, and coins being
  empty makes n - 1 equal to -1 and the very first dp read go out of range. The
  explicit double loop that fills dp with -1 costs n * (amount + 1) writes
  before any real work, on top of the allocation itself. The final check is ans
  >= INF rather than ans == INF, which is the correct guard precisely because
  INF values can be incremented on the way up.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Cut the memory down.
     Go bottom-up with one int[amount + 1] row, init to INF except dp[0] = 0,
     then for each coin loop t from coins[i] up to amount. Because the loop is
     ascending, the row already holds results that include coin i, which is the
     unbounded reuse the recursion gets by passing i again. Space drops from n
     rows to one.
  2. Return the actual coins used, not just the count.
     Keep a parallel int[amount + 1] choice array that records which coin gave
     the winning value at each t, then walk back from amount subtracting the
     stored coin until t hits 0. That adds one array and no extra time.
  3. Count the number of distinct ways to make the amount instead of the
  minimum.
     Replace Math.Min with addition and change the base to dp[0] = 1. The same
     coin-outer, amount-inner loop order then counts combinations rather than
     permutations.
  4. The amount is huge but there are only a handful of coin values.
     This table grows with amount, so it stops being practical. You would switch
     to a BFS over remainders or to number-theory arguments (the Chicken
     McNugget / Frobenius style bound), where past a threshold the greedy choice
     on the largest coin becomes safe.
TRIGGER
  Minimum or count over an exact target where every item may be picked an
  unlimited number of times.
C# NOTE
  int[,] is a true rectangular array, so dp[i, target] is one bounds-checked
  multiply-add rather than the pointer chase of int[][]; the trade-off is that
  you cannot hand a single row to a helper, which is exactly what the 1-D
  rewrite would want.
COMPLEXITY
  Time  : O(n * m)
  Space : O(n * m)
================================================================================
*/
