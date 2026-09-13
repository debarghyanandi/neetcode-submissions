// --------------------------------------------------------------------------
// -  suboptimal.cs         O(n) time / O(n) space
// -  Kadane's recurrence materialized into a DP array
// -  [kadane-max-subarray]
// -  ranks below optimal.cs (O(n) time / O(1) space)
// -
// -  Reference solution - not one you solved yourself
// -
// -  same best-ending-at-i recurrence as Kadane but stores every
// -  intermediate value in an array before scanning for the max
// --------------------------------------------------------------------------

public class Solution
{
    public int MaxSubArray(int[] nums)
    {
        // bestEndingAt[i] = the largest sum of any subarray that ENDS at i.
        // Seeded with nums itself: the single-element subarray [i].
        int[] bestEndingAt = (int[])nums.Clone();

        for (int i = 1; i < nums.Length; i++)
        {
            // Either start fresh at i, or extend the best run ending at i-1.
            bestEndingAt[i] = Math.Max(nums[i], nums[i] + bestEndingAt[i - 1]);
        }

        int maxSum = bestEndingAt[0];

        foreach (int sum in bestEndingAt)
        {
            maxSum = Math.Max(maxSum, sum);
        }

        return maxSum;
    }
}

/*
================================================================================
 PATTERN : Kadane DP - best subarray ending at each index
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Suboptimal
================================================================================
WHY THIS PATTERN
  The unknown is a subarray, which has two free endpoints - enumerating both is
  quadratic. Pin the right endpoint instead: every candidate subarray ends at
  exactly one index, so the n(n+1)/2 candidates partition into n classes keyed
  by end index. Solve each class, then take the best of the n class winners. The
  whole method rests on that class for i being derivable from the class for i-1
  in constant work.
INVARIANT
  After the loop body runs for i, bestEndingAt[i] is the maximum sum over all
  subarrays whose last element is nums[i] - not the best overall, which is why
  the final fold is needed at all.

  The clone establishes the base: every slot starts at nums[i], the one-element
  subarray [i]. bestEndingAt[0] is therefore already correct and the loop
  deliberately starts at i = 1 and never revisits it.
RECURRENCE ARGUMENT
  A best subarray ending at i is either exactly [i], or has length at least two.
  In the second case, deleting nums[i] leaves a subarray ending at i-1, and the
  total is maximized precisely when that remainder is the best subarray ending
  at i-1. So Math.Max(nums[i], nums[i] + bestEndingAt[i - 1]) covers both cases
  with no gap - the optimal substructure is that a suffix of an optimal run
  ending at i is an optimal run ending at i-1. Read the same line as a decision:
  extend the previous run only when bestEndingAt[i - 1] is positive, otherwise
  the carried prefix is dead weight and you restart.

  Worth noticing that Clone makes the read of nums[i] identical to the
  not-yet-overwritten bestEndingAt[i], so this is really an in-place rewrite of
  the copy.
WHY THIS LOSES
  bestEndingAt[i] is consumed by exactly one reader: iteration i+1. Nothing ever
  looks further back, and the closing foreach only needs a running max. So the
  entire array collapses to a single int, and maxSum can be updated inside the
  same loop - that is textbook Kadane at O(1) extra space in one pass over nums.

  This version instead touches n elements three times (Clone, forward loop,
  foreach) and allocates an array the size of the input. The correctness
  argument above is word-for-word the same either way; only the storage differs.
  Keeping the table is a choice you should be able to justify, and there is
  exactly one justification:
WHAT THE TABLE BUYS
  Reconstruction. If the follow-up is "return the subarray, not the sum," the
  array answers it: find the argmax index i, then walk j backward while
  bestEndingAt[j] != nums[j], which is the signature of an extend step; the
  first j where they are equal is the restart, hence the start of the run. (When
  bestEndingAt[j - 1] is exactly 0 the two branches tie and the reported start
  is ambiguous - harmless, since the sum is the same.) The O(1)-space version
  has to carry an explicit start pointer and reset it on every restart branch.
WATCH OUT
  1. maxSum is seeded from bestEndingAt[0], not 0. Seeding 0 is the classic bug:
  on an all-negative input like [-3, -1, -2] it returns 0, but the empty
  subarray is not a legal answer and the correct result is -1. This code is safe
  because the seed is a real element.

  2. nums.Length == 0 throws IndexOutOfRangeException at bestEndingAt[0] before
  the foreach ever runs. LeetCode 53 guarantees at least one element so no guard
  is written, but name the assumption if asked.

  3. int is wide enough here: 1e5 elements bounded by 1e4 in magnitude caps any
  sum at 1e9, under int.MaxValue. Loosen either bound and the accumulator has to
  be long.
TRIGGER
  Contiguous plus an objective that decomposes when you fix the right endpoint.
  Direct hits: maximum product subarray (carry both the best and the worst
  ending at i, since a negative flips them), maximum sum circular subarray
  (total minus the minimum subarray, with the all-negative case special-cased),
  best time to buy and sell stock (Kadane over consecutive price deltas). If the
  problem says subsequence rather than subarray, this recurrence does not apply
  - the deleted element no longer has to be nums[i].
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
