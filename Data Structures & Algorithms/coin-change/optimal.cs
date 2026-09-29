// --------------------------------------------------------------------------
// -  optimal.cs            O(n * m) time / O(m) space
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
 PROBLEM : Given coin values coins[] (unlimited copies of each) and a target
           amount, return the fewest coins that sum exactly to amount, or -1
           if impossible. amount = 0 needs 0 coins. Example: coins=[1,2,5],
           amount=11 -> 3 (5+5+1).
 PATTERN : DP (unbounded knapsack), two rolling rows
================================================================================
IDEA
  State: best coins to make target using only coins[0..i].
  Base row: with only coins[0], target works if it divides evenly, else INF.
  For coin i: notTake = prev[target], take = 1 + curr[target - coins[i]].
  take reads curr (this row), so coin i can be reused any number of times.
  Correct because every multiset of coins is "some copies of i + the rest".
EXAMPLE
  coins=[1,3,4], amount=6 (greedy 4+1+1 gives 3, which is wrong)
  coin 1: prev=[0,1,2,3,4,5,6]; coin 3: [0,1,2,1,2,3,2]
  coin 4: t4=min(2,1+0)=1, t5=min(3,1+1)=2, t6=min(2,1+curr[2]=3)=2
  answer prev[6] = 2 (3+3)
COMPLEXITY
  Time  O(n * m)  n coins x (amount+1) targets, O(1) work per cell
  Space O(m)      only two rows prev and curr of size amount+1
PATH TO OPTIMAL
  Plain recursion take/skip - exponential - baseline, recomputes states.
  2D memo / 2D table (suboptimal.cs, suboptimal-2.cs) - O(n*m) - no repeats.
  Two rows (this file) - O(m) space - row i needs only row i-1 and itself.
KEYWORDS
  unbounded knapsack, minimum coins, bottom-up DP, space optimization, INF
WATCH OUT
  - take must read curr, not prev. Reading prev lets each coin be used only
    once (that is 0/1 knapsack), and gives wrong answers.
  - Greedy (largest coin first) fails: [1,3,4], 6 gives 3, not 2.
  - Keep INF far below int.MaxValue: 1 + INF must not overflow. Then check
    ans >= INF, not ans == INF, because take can be INF + 1.
  - Empty coins throws at coins[0]. Guard it if the input can be empty.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Count the number of ways instead (Coin Change II)?
     -> Use + instead of min, with base dp[0]=1. Keep coins as the outer loop,
        so orders are not counted twice. Time O(n*m), space O(m).
  2. Can you use one array?
     -> Yes: dp[t] = min(dp[t], 1 + dp[t-coin]), loop t upward. Same time,
        O(m).
  3. Other technique?
     -> BFS over amounts from 0. The first level that reaches amount is the
        answer. Also O(n*m), and it can stop early.
  4. Return the coins used, not just the count?
     -> Store the last coin chosen for each target, then walk back from
        amount.
TRIGGER
  Reach for this when you must build a target sum from reusable items and
  want the min, max, or count of ways.
================================================================================
*/
