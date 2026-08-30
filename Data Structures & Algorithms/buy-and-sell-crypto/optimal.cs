// ##########################################################################
// #  YOU SOLVED THIS YOURSELF  (submission-2, marked '//mY solution.')
// #  merged with submission-3 - same min-tracking logic, one-step jump
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
 PATTERN : Sliding Window / Greedy - track the running minimum
 SOURCE  : YOUR OWN SOLUTION (submission-2, marked '//mY solution.'), merged
           with submission-3 - your while-loop walked `left` forward one day
           at a time; jumping straight to the new low is the same idea in one
           move, with no inner loop to reason about
 STATUS  : Optimal
================================================================================

WHY THIS PATTERN
  Profit = sell price - buy price, with buy strictly BEFORE sell. Walking
  forward, the best sale on day d is always (price[d] - cheapest day so far).
  So only ONE number from the past matters - the minimum - and it can be
  maintained in O(1) as you go. Nothing else needs remembering.

BRUTE FORCE (and why it fails)
  Every (buy, sell) pair: O(n^2). The inner loop recomputes "what was the
  cheapest earlier day" from scratch every time - the classic sign that a
  running accumulator can replace it.

WHY THE GREEDY CHOICE IS SAFE
  If prices[sellIndex] <= prices[buyIndex], then for every day AFTER
  sellIndex, buying at sellIndex is at least as good as buying at buyIndex -
  same or lower cost, and still in the past. So the old buy day can be
  discarded permanently, with no risk of losing the optimum. That exchange
  argument is what makes a one-pass greedy provably correct here, and it is
  the follow-up question if you only describe the mechanics.

INVARIANT
  prices[buyIndex] is the minimum of prices[0..sellIndex-1], and maxProfit is
  the best profit achievable using any sell day up to sellIndex.

ALGORITHM (NeetCode: "Sliding Window")
  1. buyIndex = 0, maxProfit = 0.
  2. Walk sellIndex from 1 to the end.
  3. Higher than the buy price -> update maxProfit.
  4. Lower or equal -> that day becomes the new buy day.

COMPLEXITY
  Time  : O(n) - one pass, one comparison per day, no inner loop.
  Space : O(1) - two ints.

TRIGGER
  "Best pair (i, j) with i < j maximising f(j) - g(i)", where g is monotone
  in the sense that only the running best of the past matters.
  Sibling problems: maximum-subarray (running sum instead of running min) and
  the "best time to buy and sell II / with cooldown" DP variants, where the
  state stops being a single number and this trick stops working.

C# NOTES
  - Tracking prices[buyIndex] as a plain `int minPrice` is equally valid and
    slightly faster - the index is kept here because it makes the "which days
    did I trade" question answerable, which interviewers do ask.
  - Math.Max on ints is branchless after JIT; no reason to hand-roll an if.
  - Start sellIndex at 1: day 0 cannot be both buy and sell.

WATCH OUT
  - Return 0, not a negative number, when prices only fall. maxProfit
    starting at 0 encodes "no trade is always allowed" - do not initialise it
    to int.MinValue.
  - The `else` must reassign buyIndex, not just skip. Forgetting it silently
    anchors every comparison to day 0.
================================================================================
*/
