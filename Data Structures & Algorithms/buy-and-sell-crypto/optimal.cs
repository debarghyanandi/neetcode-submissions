// ##########################################################################
// #  optimal.cs            O(n) time / O(1) space
// #  Single pass with minimum tracking   [single-pass-min-tracking]
// #  the only solution in this folder
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  One pass through prices tracks minimum and computes maximum profit in
// #  constant auxiliary space.
// ##########################################################################

public class Solution
{
    public int MaxProfit(int[] prices)
    {
        int buyIndex = 0;      // the cheapest day seen so far
        int maxProfit = 0;     // 0 is always achievable: simply never trade

        for (int sellIndex = 1; sellIndex < prices.Length; sellIndex++)
        {
            if (prices[sellIndex] > prices[buyIndex])
            {
                // Profitable pair - record it, but keep the same buy day,
                // a later price could be even higher.
                maxProfit = Math.Max(maxProfit, prices[sellIndex] - prices[buyIndex]);
            }
            else
            {
                // A new all-time low. Every future sale should start here,
                // so move the buy day forward in ONE step.
                buyIndex = sellIndex;
            }
        }

        return maxProfit;
    }
}

/*
================================================================================
 PATTERN : Greedy / One Pass - track the cheapest buy day so far
 SOURCE  : YOUR OWN SOLUTION - your own annotation at c76939d
 STATUS  : Optimal
================================================================================
VARIABLES
  buyIndex    index of the lowest price seen in prices[0..sellIndex-1]
  maxProfit   best profit from any buy day before any sell day so far
  sellIndex   the day we test as the sell day
WHY THIS PATTERN
  You must buy once and then sell once on a later day. So for each sell day, the
  only buy day that matters is the cheapest day before it. Keep that day in
  buyIndex while you scan. Then each prices[sellIndex] needs only one
  subtraction to find its best profit.
BRUTE FORCE
  Try every pair (i, j) with i < j, compute prices[j] - prices[i], and keep the
  largest result, or 0 if none is positive. It is clearly correct, but it runs
  in O(n^2) time because it checks every pair. The one-pass version skips all
  pairs whose buy day is not the cheapest so far, because those pairs can never
  win.
INVARIANT
  At the start of each loop step, prices[buyIndex] is the minimum of
  prices[0..sellIndex-1]. maxProfit is the best profit from any buy-then-sell
  pair that ends before sellIndex. Every sell day is paired with its cheapest
  earlier buy day, and that pair is the best one for that sell day. So after the
  loop, maxProfit is the best profit over all pairs. It starts at 0, which means
  "never trade".
WATCH OUT
  The else branch runs when prices[sellIndex] <= prices[buyIndex], so a price
  that only ties the minimum also moves buyIndex. The comment "A new all-time
  low" is therefore not exactly true on ties. The answer is still right, because
  an equal price gives the same profit later. If prices is null, prices.Length
  throws. An empty or one-element array returns 0, which is correct.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. What if you can buy and sell many times, holding at most one share?
     Add every positive step prices[i] - prices[i-1]. It is still one pass, but
     it is a different greedy idea: take every rise instead of the single best
     gap.
  2. What if you may make at most two transactions (or k transactions)?
     Use a small state machine DP (dynamic programming, where each state stores
     the best result so far): buy1, sell1, buy2, sell2, updated each day. For k
     transactions this costs O(nk) time and O(k) space.
  3. What if you must also return which days to buy and sell?
     Save buyIndex and sellIndex whenever maxProfit gets a new best value. The
     work is the same, plus two more integers.
  4. What if prices arrive as a stream?
     The code already keeps only the running minimum and the best profit. It
     works as-is by storing the minimum price instead of an index.
TRIGGER
  The answer is the best "later value minus earlier value" (or a pair where one
  element must come before the other), so a running minimum or maximum of the
  prefix removes the inner loop.
C# NOTE
  buyIndex holds an index, but it is only used to read prices[buyIndex]. Storing
  int minPrice directly would remove one array read per step and make the
  invariant easier to see. prices[sellIndex] - prices[buyIndex] cannot overflow
  here, because that line only runs when the result is positive and the two
  values are ints of the same sign range.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
