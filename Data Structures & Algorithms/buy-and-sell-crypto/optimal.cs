// ##########################################################################
// #  optimal.cs            O(n) time / O(1) space
// #  one-pass running minimum, greedy   [running-min-greedy]
// #  the only solution in this folder
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  single forward scan tracks the index of the lowest price seen so far
// #  and updates max profit against it in O(1) per step using only scalar
// #  state
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
 PATTERN : Single-pass greedy - running minimum as the buy pointer
 SOURCE  : YOUR OWN SOLUTION - your own annotation at c76939d
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  The answer rewrites as: max over every sell day j of prices[j] -
  min(prices[0..j-1]). Once you see that decomposition, one pass is forced. Each
  day needs exactly one number from its past - the cheapest price before it -
  not the whole prefix. So the entire prefix collapses into the single variable
  buyIndex, and there is nothing left to store.
BRUTE FORCE AND THE REDUNDANCY
  The obvious version is two nested loops over all pairs i < j, taking Math.Max
  over prices[j] - prices[i]. That is O(n^2), and the waste is specific: for
  each sell day it re-scans the same prefix to rediscover the same minimum. The
  prefix minimum is incrementally maintainable in O(1) per step, so the inner
  loop is pure repetition.
INVARIANT
  At the top of every iteration: prices[buyIndex] ==
  min(prices[0..sellIndex-1]), and maxProfit == the best profit over all pairs
  that end strictly before sellIndex.

  Base: sellIndex == 1, buyIndex == 0, and prices[0] is trivially the min of
  prices[0..0]; maxProfit == 0 with no pairs yet.

  Step: if prices[sellIndex] > prices[buyIndex], the prefix minimum is
  unchanged, so buyIndex correctly stays put and the pair (buyIndex, sellIndex)
  - the best pair ending at sellIndex - folds into maxProfit. Otherwise
  prices[sellIndex] <= prices[buyIndex], so prices[sellIndex] is the new prefix
  minimum and buyIndex = sellIndex restores it. Both branches re-establish the
  invariant, so at the end maxProfit is the max over all j, which is the answer.
WHY MOVING BUYINDEX IS SAFE
  This is the follow-up an interviewer actually asks: how do you know you are
  not throwing away a good buy day? The else branch is reached only when
  prices[sellIndex] <= prices[buyIndex]. So the pair (buyIndex, sellIndex) is
  worth at most 0 and is already covered by the 0 floor on maxProfit -
  abandoning the old buy day costs nothing today. And for every day after
  sellIndex, the new buy price is less than or equal to the old one, so it is at
  least as good. The move is never a gamble; it dominates.

  Same reason the day at sellIndex is not lost by being consumed as a buy: since
  its price is a new prefix minimum, no pair ending at it could have been
  profitable anyway.

  Equal prices also take the else branch and slide the pointer forward. Harmless
  - same price, later index, strictly fewer future days excluded is not even
  needed, the profit is identical.
WATCH OUT
  1. buyIndex at return time is NOT the buy day of the winning trade. It keeps
  moving after the best pair is found. If the variant asks you to report the
  actual days, snapshot bestBuy = buyIndex and bestSell = sellIndex inside the
  Math.Max update, comparing explicitly instead of using Math.Max.

  2. maxProfit = 0 encodes the rule "you may decline to trade." If a variant
  forces exactly one transaction, the answer can be negative and this
  initialization silently returns the wrong 0. Initialize to int.MinValue there.

  3. Empty and single-element input work by construction: the loop starts at 1
  and never runs, and prices[0] is never dereferenced outside the loop, so a
  zero-length array returns 0 rather than throwing.

  4. The if-guard prices[sellIndex] > prices[buyIndex] is not load-bearing for
  the Math.Max - a non-positive difference would lose to maxProfit anyway. It is
  load-bearing for the else, which is where the pointer update lives.
TRIGGER
  Ordered constraint (the buy must precede the sell) plus an objective that is a
  difference of two elements. That combination means each right endpoint depends
  on the min or max of everything to its left, which is a scalar you can carry.
  When you catch yourself writing a second loop only to recompute a prefix
  extreme, delete it and hoist the extreme into a variable.
FOLLOW-UPS
  Unlimited transactions: sum every positive prices[i] - prices[i-1]; the greedy
  pointer disappears entirely. At most k transactions, or a cooldown day, or a
  per-trade fee: none of these survive a single scalar - they become
  state-machine DP over (day, transactions used, holding or not).

  Worth knowing the equivalence: build the daily-delta array d[i] = prices[i] -
  prices[i-1]; this problem is the maximum subarray sum of d, and Kadane's
  running reset to 0 is exactly the buyIndex = sellIndex move seen from the
  other side.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
