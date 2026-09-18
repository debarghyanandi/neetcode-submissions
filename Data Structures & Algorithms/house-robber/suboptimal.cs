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

        int numsLength = nums.Length;

        if (numsLength == 1)
            return nums[0];

        int[] maxAmount = new int[numsLength + 1];

        //Recurrence relation
        //robCurrent = f(indx) + f(indx - 2);
        //skipCurrent = 0 + f(index - 1);
        maxAmount[0] = nums[0];
        maxAmount[1] = Math.Max(nums[0], nums[1]);

        for (int i = 2; i < numsLength; i++)
        {
            int robCurrent = nums[i] + maxAmount[i - 2];
            int skipCurrent = 0 + maxAmount[i - 1];
            maxAmount[i] = Math.Max(robCurrent, skipCurrent);
        }
        return maxAmount[numsLength - 1];
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
  one-dimensional state: maxAmount[i] is the best total using only houses 0..i.
  Each step is a two-way choice, robCurrent = nums[i] + maxAmount[i-2] or
  skipCurrent = maxAmount[i-1], and the larger one wins. There is no need to
  remember which houses were picked, only the best total, so a single array over
  i is enough.
BETTER APPROACH
  The better version keeps the same recurrence but drops the array. maxAmount[i]
  only ever reads i-1 and i-2, so two int variables (say prev and prevPrev)
  carry all the state and the answer comes out in O(1) extra space. This file
  allocates a full int[numsLength + 1] and never looks back further than two
  slots, so the whole array is dead weight after each step. Same time, more
  memory - that is the only gap.
INVARIANT
  After the loop body for index i, maxAmount[i] holds the maximum money
  obtainable from houses 0..i with no two adjacent houses chosen. The base cases
  set this up: maxAmount[0] = nums[0] is forced, and maxAmount[1] =
  Math.Max(nums[0], nums[1]) because the two are adjacent and only one can be
  taken. The step is correct because any valid plan ending at or before i either
  takes house i, and then cannot touch i-1, leaving the best of 0..i-2, or skips
  it, leaving the best of 0..i-1. So maxAmount[numsLength - 1] is the answer for
  the whole street.
WATCH OUT
  An empty array crashes: numsLength == 1 is checked, but numsLength == 0 falls
  through to maxAmount[0] = nums[0] and throws IndexOutOfRangeException. The
  array is sized numsLength + 1 while the loop only writes up to numsLength - 1,
  so the last slot is allocated and never used - harmless, but it hides the fact
  that the real size needed is numsLength. The comment writes the recurrence as
  f(indx) + f(indx - 2), but f is used for two different things there: the first
  term is nums[i], the second is maxAmount[i-2]; read it carefully or it looks
  self-referential. The 0 + in skipCurrent is pure noise and can be dropped.
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
     Then the array is worth keeping - walk backwards from numsLength - 1 and at
     each i check whether maxAmount[i] equals maxAmount[i-1]; if not, house i
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
  Math.Max on two ints is the right call here, but note new int[numsLength + 1]
  zero-initialises every slot before the loop overwrites them; if you keep the
  array at all, size it numsLength so the allocation matches what you actually
  index.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
