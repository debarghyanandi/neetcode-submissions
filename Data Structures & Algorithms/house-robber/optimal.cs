// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
// -  Dynamic programming space optimization   [dp-space-optimized]
// -  ranks above suboptimal.cs (O(n) time / O(n) space)
// -
// -  Reference solution - not one you solved yourself
// -
// -  Single pass through array with constant rolling variables tracking
// -  only the last two DP values.
// --------------------------------------------------------------------------

public class Solution
{
    public int Rob(int[] nums)
    {
        //Dp space Optimized
        int n = nums.Length;

        if (n == 1)
            return nums[0];

        int prev2 = nums[0];
        int prev = Math.Max(nums[0], nums[1]);
        int curr = 0;

        for (int i = 2; i < n; i++)
        {
            int pick = nums[i] + prev2;
            int notPick = 0 + prev;
            curr = Math.Max(pick, notPick);

            prev2 = prev;
            prev = curr;
        }

        return prev;
    }
}

/*
================================================================================
 PATTERN : 1-D DP / Space-Optimized - pick or skip each house
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-1.cs when it was first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  prev2    best loot from houses 0..i-2
  prev     best loot from houses 0..i-1
  pick     loot if you rob house i: nums[i] + prev2
  notPick  loot if you skip house i: same as prev
  curr     best loot from houses 0..i
WHY THIS PATTERN
  You cannot rob two houses that sit next to each other, and you want the
  largest total. So the choice at house i depends only on the best totals for
  the two prefixes before it. That is a DP (dynamic programming: build the
  answer from answers to smaller prefixes) with the recurrence best(i) =
  max(nums[i] + best(i-2), best(i-1)). Each step reads only two older values, so
  prev2 and prev can replace the whole table.
BRUTE FORCE
  The first correct idea is recursion: at each house, try both "rob it and jump
  to i+2" and "skip it and go to i+1", then return the larger result. It is
  correct, but it solves the same suffixes again and again, so it takes O(2^n)
  time. With memoization it drops to O(n) time, but it still needs O(n) memory
  for the memo array and the call stack. This file keeps the same recurrence and
  removes both costs.
INVARIANT
  When the loop starts step i, prev2 holds the best total for houses 0..i-2 and
  prev holds the best total for houses 0..i-1. Any valid plan for 0..i either
  robs house i, which means house i-1 is not robbed and the rest is best(i-2),
  or it skips house i, which gives best(i-1). curr takes the larger of the two,
  so it is exactly best(i). The shift prev2 = prev, prev = curr brings the
  invariant forward to step i+1. When the loop ends, prev is best(n-1).
SECOND BASE CASE IS A MAX
  prev starts as Math.Max(nums[0], nums[1]), not nums[1]. With two houses you
  can rob only one of them, so the best total is the richer one. If you start
  prev at nums[1], then every later step is built on a wrong best(1).
WATCH OUT
  An empty array crashes: n == 0 passes the n == 1 check, and then nums[0]
  throws IndexOutOfRangeException. The sums use int, so a large total overflows
  quietly and gives a wrong answer. The code returns prev, not curr, and that is
  on purpose: when n == 2 the loop never runs and curr is still 0. If you change
  the return to curr, the n == 2 case breaks.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. What if the houses stand in a circle, so the first and last houses are
  neighbors?
     Run this same loop twice, once on houses 0..n-2 and once on 1..n-1, and
     return the larger result. The cost stays O(n) time and O(1) space. You must
     handle n == 1 on its own.
  2. Can you also return which houses to rob, not only the total?
     Keep the full dp array, then walk back from the end. If dp[i] == dp[i-1],
     house i was skipped. If not, house i was robbed, so jump to i-2. This needs
     O(n) space, because two rolling variables lose the history you need.
  3. What if the houses form a binary tree, and a parent and its child cannot
  both be robbed?
     Do a post-order DFS (visit the children before the parent). Each node
     returns a pair: (best if robbed, best if not robbed). The time is still
     O(n), but the recursion stack costs O(height) space.
TRIGGER
  Reach for this pattern when you must choose items along a line to get the
  biggest sum, and choosing one item bans its direct neighbor.
C# NOTE
  curr is declared outside the loop, but it is only used inside the loop. If you
  declare it as int curr = Math.Max(pick, notPick) inside the loop, the scope is
  smaller and nobody can mistake it for the return value. The "0 + prev" in
  notPick does nothing and can be written as just prev.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
