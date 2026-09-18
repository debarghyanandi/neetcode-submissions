// --------------------------------------------------------------------------
// -  suboptimal.cs         O(n) time / O(n) space
// -  Linear DP with full DP table   [house-robber-dp]
// -  ranks below optimal.cs (O(n) time / O(1) space)
// -
// -  Reference solution - not one you solved yourself
// -
// -  Entire DP table retained in memory; only last two entries are ever
// -  consulted again.
// --------------------------------------------------------------------------

public class Solution
{
    public int Rob(int[] nums)
    {

        int n = nums.Length;

        if (n == 1)
            return nums[0];

        int[] dp = new int[n + 1];

        //Recurrence relation
        //pick = f(indx) + f(indx - 2);
        //notPick = 0 + f(index - 1);
        dp[0] = nums[0];
        dp[1] = Math.Max(nums[0], nums[1]);

        for (int i = 2; i < n; i++)
        {
            int pick = nums[i] + dp[i - 2];
            int notPick = 0 + dp[i - 1];
            dp[i] = Math.Max(pick, notPick);
        }
        return dp[n - 1];
    }
}


/*
================================================================================
 PATTERN : Linear DP - max sum with no two adjacent picks
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Suboptimal
================================================================================
WHY THIS PATTERN
  The problem says you cannot rob two houses next to each other, so the decision
  at house i depends only on whether you took house i-1. That is a
  one-dimensional state: dp[i] is the best total using only houses 0..i.
  Each step is a two-way choice, pick = nums[i] + dp[i-2] or
  notPick = dp[i-1], and the larger one wins. There is no need to
  remember which houses were picked, only the best total, so a single array over
  i is enough.
BETTER APPROACH
  The better version keeps the same recurrence but drops the array. dp[i]
  only ever reads i-1 and i-2, so two int variables (say prev and prevPrev)
  carry all the state and the answer comes out in O(1) extra space. This file
  allocates a full int[n + 1] and never looks back further than two
  slots, so the whole array is dead weight after each step. Same time, more
  memory - that is the only gap.
INVARIANT
  After the loop body for index i, dp[i] holds the maximum money
  obtainable from houses 0..i with no two adjacent houses chosen. The base cases
  set this up: dp[0] = nums[0] is forced, and dp[1] =
  Math.Max(nums[0], nums[1]) because the two are adjacent and only one can be
  taken. The step is correct because any valid plan ending at or before i either
  takes house i, and then cannot touch i-1, leaving the best of 0..i-2, or skips
  it, leaving the best of 0..i-1. So dp[n - 1] is the answer for
  the whole street.
WATCH OUT
  An empty array crashes: n == 1 is checked, but n == 0 falls
  through to dp[0] = nums[0] and throws IndexOutOfRangeException. The
  array is sized n + 1 while the loop only writes up to n - 1,
  so the last slot is allocated and never used - harmless, but it hides the fact
  that the real size needed is n. The comment writes the recurrence as
  f(indx) + f(indx - 2), but f is used for two different things there: the first
  term is nums[i], the second is dp[i-2]; read it carefully or it looks
  self-referential. The 0 + in notPick is pure noise and can be dropped.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Rewrite it with O(1) extra space.
     Keep two ints, prevPrev = nums[0] and prev = Math.Max(nums[0], nums[1]),
     then loop cur = Math.Max(nums[i] + prevPrev, prev), shift prevPrev = prev,
     prev = cur, and return prev. Same arithmetic, no allocation; you lose the
     ability to inspect intermediate bests afterwards.
  2. What if the houses are in a circle, so house 0 and the last house are
  neighbours?
     Run this same routine twice, once on nums[0..n-2] and once on nums[1..n-1],
     and take the larger result; the circle is broken by forcing one of the two
     endpoints out. Handle n == 1 separately since both slices would be empty.
  3. The interviewer wants the actual list of robbed houses, not just the total.
     Then the array is worth keeping - walk backwards from n - 1 and at
     each i check whether dp[i] equals dp[i-1]; if not, house i
     was taken, so record it and jump to i-2. This is exactly where the
     O(1)-space version cannot follow you.
  4. Could you write this top-down instead?
     A recursive helper with a memo array gives the same values, but recursion
     depth grows with the number of houses and risks a stack overflow on a long
     street, so the bottom-up loop here is the safer shape.
TRIGGER
  A line of items where choosing one forbids its immediate neighbour and you
  want the best total - think i-1 versus i-2.
C# NOTE
  Math.Max on two ints is the right call here, but note new int[n + 1]
  zero-initialises every slot before the loop overwrites them; if you keep the
  array at all, size it n so the allocation matches what you actually
  index.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
