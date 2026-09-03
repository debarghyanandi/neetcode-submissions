// ##########################################################################
// #  optimal.cs            O(n) time / O(1) space
// #  one-pass running minimum, greedy   [running-min-greedy]
// #  the only solution in this folder
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  single forward scan tracks minimum price so far and updates max profit
// #  against it in O(1) per step
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
 PATTERN : Running minimum - fix the sell day, greedy buy day
 SOURCE  : YOUR OWN SOLUTION - your own annotation at c76939d
 STATUS  : Optimal
================================================================================
CORE IDEA
  Do not search over pairs. Fix the sell day sellIndex and ask: what is the best
  buy day for it? Answer: the cheapest day strictly before it. So the answer is
  max over every sellIndex of (prices[sellIndex] - min of
  prices[0..sellIndex-1]). The loop is exhaustive over sell days and greedy over
  buy days, which is why one pass suffices - buyIndex carries the whole prefix
  minimum in a single int.
INVARIANT
  At the top of every iteration: (1) buyIndex is the index of a minimum of
  prices[0..sellIndex-1], and (2) buyIndex < sellIndex.

  (1) holds on entry because prices[0..0] has minimum at index 0. It is
  preserved by the two branches: if prices[sellIndex] > prices[buyIndex] the new
  element cannot lower the minimum, so keeping buyIndex is right; otherwise
  prices[sellIndex] <= prices[buyIndex] and sellIndex is a valid new minimum
  index.

  (2) holds because buyIndex is only ever assigned the current sellIndex, and
  sellIndex increments before the next read. That is the entire proof that this
  never sells before it buys - there is no separate ordering check anywhere in
  the code.
WHY THE IF AND ELSE ARE EXHAUSTIVE
  The two branches are not 'profit case' and 'unrelated case'. They are the
  complement of each other on the same comparison: either prices[sellIndex]
  beats the running minimum (record profit) or it does not, and 'does not' is
  exactly the definition of a new minimum. That is why nothing is lost by
  skipping the Math.Max in the else branch - the difference there is <= 0, and
  maxProfit already starts at 0 and only grows.

  Equivalent branchless form, useful if an interviewer asks you to restructure
  it: always take maxProfit = Max(maxProfit, prices[sellIndex] -
  prices[buyIndex]), then separately update buyIndex when prices[sellIndex] <
  prices[buyIndex].
WATCH OUT
  1. The Math.Max is load-bearing. Assigning maxProfit = prices[sellIndex] -
  prices[buyIndex] instead breaks on [1,10,2,3]: the pair (1,10) gives 9, then
  (2,3) would overwrite it with 1. The running minimum can move forward while
  the best profit stays behind it.

  2. The buy day moves on a new all-time low, not on a local dip. The comparison
  is against prices[buyIndex], never against prices[sellIndex - 1]. Comparing to
  the previous day is the classic wrong rewrite: on [5,1,4,3,9] it would abandon
  the buy at 1 when the price falls from 4 to 3 and miss the 8.

  3. The else branch also fires on ties (prices[sellIndex] == prices[buyIndex]).
  Harmless - it swaps one minimum index for a later one of equal value, and
  later is never worse since it leaves more room to the right.
EDGE CASES
  Empty or single-element prices: sellIndex starts at 1, so the loop body never
  runs and 0 is returned. buyIndex = 0 is never dereferenced, so prices.Length
  == 0 does not throw - there is no explicit guard because the loop bound is the
  guard.

  Strictly decreasing prices: the if never fires, buyIndex walks to the last
  index, and maxProfit stays at its initial 0. That initial 0 is a real answer,
  not a sentinel - it encodes the always-legal choice of never trading, which is
  why the code never needs to handle 'no profitable pair' separately.
TRIGGER
  Reach for this shape when the answer is a max or min over pairs (i, j) with i
  < j, and the best partner for a fixed j depends only on a single aggregate of
  the prefix before j. Collapse the prefix into one variable and sweep j once.
  Here the aggregate is a minimum; the same skeleton with a running maximum
  solves 'largest drop', and with a running max of prices[i] - somethingElse it
  extends to the multi-transaction variants.
FOLLOW-UPS TO EXPECT
  Which days? buyIndex holds an index rather than a value, so this is one step
  from returning the trade: record buyIndex and sellIndex alongside maxProfit at
  the moment the Math.Max actually increases it. Note the live buyIndex at
  return time is the running minimum, not necessarily the winning buy day - it
  can have moved past it.

  Relation to Kadane: build d[i] = prices[i] - prices[i-1]; the profit of buying
  at i and selling at j is the sum of d over (i, j], so this problem is maximum
  subarray on d, and the running minimum here is the mirror of Kadane's 'reset
  when the prefix goes negative'.

  Unlimited transactions: sum every positive d[i] - the greedy changes
  completely because the buy day is no longer unique. At most k transactions or
  a cooldown day: the single-variable trick dies and you need DP over (day,
  transactions used, holding).

  Streaming input: nothing in the loop reads backwards, so this works verbatim
  on prices arriving one at a time with only buyIndex and maxProfit retained.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
