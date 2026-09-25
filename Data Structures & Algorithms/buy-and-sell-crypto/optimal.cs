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
 PATTERN : Single-pass Greedy - running minimum, best sell today
 SOURCE  : YOUR OWN SOLUTION - your own annotation at c76939d
 STATUS  : Optimal
================================================================================
VARIABLES
  buyIndex    index of the cheapest price seen in prices[0..sellIndex-1]
  maxProfit   best prices[s] - prices[b] found so far, with b < s
  sellIndex   the day we try to sell on this iteration
WHY THIS PATTERN
  The problem asks for one buy and one later sell, so the buy day must come
  strictly before the sell day. That ordering means you never need to look
  ahead: for each sellIndex, the best partner is simply the smallest price to
  its left, which is exactly what buyIndex points at. So one left-to-right pass
  that keeps the running minimum and the running best difference answers the
  whole question.
BRUTE FORCE
  The first thing most people write is two nested loops: for every buy day b,
  try every sell day s > b and keep the largest prices[s] - prices[b]. That is
  correct but O(n^2) time. It loses because it recomputes the minimum of the
  prefix over and over, when one variable (buyIndex) can carry it forward for
  free.
INVARIANT
  At the top of each iteration, buyIndex is the index of the minimum value in
  prices[0 .. sellIndex-1], and maxProfit is the best profit over all pairs that
  end at or before sellIndex-1. The if branch extends the second half of the
  invariant by testing the one new pair (buyIndex, sellIndex), which is the best
  pair ending at sellIndex. The else branch restores the first half, since
  prices[sellIndex] is now no larger than any earlier price. maxProfit starts at
  0, so if prices only falls, the "do not trade" answer survives.
THE TWO BRANCHES ARE EXCLUSIVE AND THAT IS FINE
  When prices[sellIndex] is a new minimum, no profit can be recorded on that day
  anyway, because selling at a price at or below the current minimum gives at
  most 0, and maxProfit is already at least 0. So skipping the Math.Max call in
  the else branch loses nothing. The code is equivalent to the common version
  that updates the minimum first and then computes the profit unconditionally.
WATCH OUT
  The comment says "A new all-time low", but the else branch also runs when
  prices[sellIndex] equals prices[buyIndex], which is not lower. That is
  harmless here (the minimum value is unchanged, only the index moves), but the
  comment and the code do not match, and if you later change the code to also
  report the buy date you will get the latest tied cheap day, not the earliest.
  Also note the return type is int: on a very wide price range the subtraction
  prices[sellIndex] - prices[buyIndex] is an int subtraction and could overflow
  if prices ever held values near int.MaxValue and int.MinValue. Empty and
  single-element arrays are safe, since the loop starts at sellIndex = 1 and
  prices is never indexed outside it.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. What if you may buy and sell as many times as you like?
     Sum every positive step: add prices[i] - prices[i-1] whenever it is
     positive. Still one pass, and buyIndex is no longer needed at all.
  2. What if you must report the actual buy and sell days, not just the profit?
     Keep two extra ints, bestBuy and bestSell, and write them inside the if
     branch at the same moment maxProfit is updated. Cost is two more variables
     and nothing else.
  3. What if prices arrive as a stream and you cannot hold the array?
     The algorithm already works online. Replace buyIndex with an int minPrice,
     update it from each incoming value, and maxProfit is correct after every
     element.
  4. What if at most k transactions are allowed?
     The greedy no longer works; you need dynamic programming over (day,
     transactions used, holding or not), which costs O(n*k) time and O(k) space
     with rolling rows.
TRIGGER
  You need the best pair (i, j) with i < j under a difference or ratio, and the
  best left-hand partner for every j is just one running extreme.
C# NOTE
  Storing the value instead of the index (int minPrice) would cut each iteration
  from two array reads to one, since prices[buyIndex] is re-read every time; the
  index is only worth keeping if you later need the day number. Math.Max here
  resolves to the int overload, so there is no boxing or comparer call.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
