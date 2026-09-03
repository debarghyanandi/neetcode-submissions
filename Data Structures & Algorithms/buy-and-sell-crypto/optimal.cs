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
 PATTERN : Sliding Window / Greedy - track the running minimum
 SOURCE  : YOUR OWN SOLUTION - your own annotation at c76939d
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  Every candidate answer is a pair (buy, sell) with buy < sell. Fix the sell day
  and the search collapses: the best profit ending at sellIndex is
  prices[sellIndex] minus the smallest price anywhere strictly before it.
  Nothing else about the prefix matters - not the order, not the second
  smallest, not where the peaks were. So the whole left side of the array
  compresses into one fact, the minimum so far, and that fact is maintained for
  free as the loop walks right. buyIndex is that fact.
BRUTE FORCE
  Two nested loops over every i < j, taking the max of prices[j] - prices[i].
  The waste is that the inner loop recomputes the same prefix minimum from
  scratch for every j. buyIndex caches that prefix minimum across iterations,
  and that single caching move is the entire difference between the two
  solutions - the outer loop over sell days is unchanged.
INVARIANT
  At the top of the iteration for sellIndex:
  1. prices[buyIndex] is the minimum of prices[0 .. sellIndex-1].
  2. maxProfit is the best profit over all pairs contained in prices[0 ..
  sellIndex-1].

  The body restores both. The if branch extends (2) to pairs ending at
  sellIndex; the else branch extends (1) to include sellIndex. On exit sellIndex
  == prices.Length, so (2) is the answer over the whole array and gets returned
  directly - there is no post-loop fixup.
WHY THE BRANCHES ARE COMPLEMENTS
  The guard prices[sellIndex] > prices[buyIndex] and its else are exact
  complements, so exactly one fires per day and no day is skipped. A day can
  never be both a profitable sell and a new low, which is why nothing is lost by
  making them exclusive.

  In the if branch, Math.Max is still required: a positive profit is not
  automatically the best profit seen so far. Note also that buyIndex is
  deliberately NOT advanced here - no cheaper day can have appeared, and a
  higher sell day may still be coming, so the same buy day is reused.

  In the else branch, prices[sellIndex] - prices[buyIndex] is zero or negative,
  so there is nothing worth recording, and every future sell day strictly
  prefers buying at sellIndex over buyIndex. That is why the assignment jumps
  straight to sellIndex in ONE step: no backward scan, no list of old buy days,
  because an old buy day can never become useful again.
WHY AN INDEX, NOT A PRICE
  buyIndex stores a day rather than a price, so every comparison re-reads
  prices[buyIndex]. Storing an int minPrice instead would be an equivalent
  algorithm with the same invariant. Keeping the index is the version that can
  answer the natural follow-up - which day do I buy, and which day do I sell -
  by recording sellIndex alongside the maxProfit update.
WATCH OUT
  - buy-before-sell is enforced by the loop shape, not by a check. buyIndex =
  sellIndex runs at the end of day sellIndex, and the loop increments before
  buyIndex is read again, so buyIndex < sellIndex always holds.
  - Ties fall into the else branch, since the guard is a strict >. Moving
  buyIndex onto an equally cheap later day changes no future profit, so this is
  harmless either way, but state it out loud rather than leaving it to the
  interviewer to ask.
  - Length 0 or 1 never enters the loop and returns 0. An empty array is safe
  because buyIndex = 0 is only an assignment - prices[0] is never evaluated.
  - maxProfit starting at 0 rather than int.MinValue is a claim about the
  problem, not a convenience: not trading is legal. On a strictly decreasing
  array the if branch never fires and 0 comes back. If a trade were mandatory
  and losses had to be reported, that initialization would be the bug.
FOLLOW-UPS
  - "Isn't this Kadane's?" Yes, in disguise. maxProfit equals the maximum
  subarray sum of the consecutive differences prices[i+1] - prices[i]. The
  prefix sums of those differences are prices[j] - prices[0], and
  max-subarray-via-prefix-sums is exactly max over j of P[j] - min of P[i] for i
  < j - which is this loop, with buyIndex playing the running-minimum role.
  - Unlimited transactions: the answer becomes the sum of every positive
  consecutive difference. Still one pass, but a different state - the running
  minimum stops being the right thing to carry.
  - At most k transactions, or a cooldown day: greedy breaks and you move to DP
  over (day, transactions used, holding or not).
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
