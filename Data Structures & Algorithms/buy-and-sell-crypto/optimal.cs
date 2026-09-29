// ##########################################################################
// #  optimal.cs            O(n) time / O(1) space
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
 PROBLEM : prices[i] is the price of one coin on day i. Buy on one day and
           sell on a LATER day, at most once. Return the max profit, or 0 if
           no trade gains. Example: [7,1,5,3,6,4] -> 5 (buy at 1, sell at 6).
 PATTERN : Greedy one pass: running min (or two pointers)
================================================================================
IDEA
  buyIndex always points to the cheapest day seen so far. sellIndex walks
  forward. If today is higher than the buy price, record the profit in
  maxProfit. If not, today is the new cheapest day, so buyIndex jumps here.
  It is correct because the best sale on any day uses the lowest price before
  that day, and buyIndex holds exactly that price.
EXAMPLE
  [7,1,5,3,6,0,2]: s=1 buy->1(price 1); s=2 +4; s=3 +2; s=4 +5 max=5;
  s=5 price 0 is lower, buy->5; s=6 2-0=2, max stays 5.
  Answer 5. A new low after the peak does not erase the best profit.
COMPLEXITY
  Time  O(n)  sellIndex visits each day once, O(1) work per day
  Space O(1)  only buyIndex and maxProfit, no extra arrays
PATH TO OPTIMAL
  Brute force: try every (buy, sell) pair - O(n^2) - simple but slow.
  Running min, this file - O(n) / O(1) - keep the best buy day so far,
  instead of testing every earlier day again for each sell day.
KEYWORDS
  stock buy sell, running minimum, greedy, one pass, Kadane, two pointers
WATCH OUT
  - Do not use global max minus global min. The max can come BEFORE the min:
    [5,1] would give 4, but the answer is 0.
  - Start maxProfit at 0, not int.MinValue. Falling prices [5,4,3] must give
    0.
  - The comment says "a new all-time low", but the else branch also runs on an
    equal price. That is harmless: same price, so same future profits.
  - Empty or single-day input: the loop never runs, so 0 is returned. Good.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Unlimited transactions (Stock II)?
     -> Add every positive step prices[i]-prices[i-1]. O(n) time, O(1) space.
        It works because a long rise is the sum of its small daily rises.
  2. At most 2 trades, or at most k trades?
     -> DP with states buy1, sell1, buy2, sell2 updated per day, O(n) / O(1).
        For k trades use arrays of size k: O(nk) time, O(k) space.
  3. Cooldown after a sale, or a fee per trade?
     -> State machine DP (hold, sold, rest) per day, O(n) / O(1). Subtract the
        fee when you sell. Same one pass, just more states.
  4. How is this like Kadane's algorithm?
     -> Turn prices into daily differences. The answer is the max subarray sum
        of those differences, floored at 0. Same O(n) / O(1).
TRIGGER
  When you need the best pair i < j scoring on a[j] - a[i], keep a running
  min.
================================================================================
*/
