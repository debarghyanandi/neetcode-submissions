// --------------------------------------------------------------------------
// -  suboptimal.cs         O(n) time / O(n) space
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
 PROBLEM : Given an integer array nums, find the contiguous subarray (at least
           one element) with the largest sum and return that sum, not the
           indices. Example: [-2,1,-3,4,-1,2,1,-5,4] -> 6 (subarray
           [4,-1,2,1]).
 PATTERN : 1D DP (Kadane's recurrence, stored as a full table)
================================================================================
IDEA
  bestEndingAt[i] is the best sum of a subarray that must end exactly at i.
  For each i, either start fresh with nums[i] or extend the run ending at i-1.
  A second pass takes the max of the table into maxSum.
  It is correct because every subarray ends somewhere, so the true answer is
  one of the bestEndingAt values. optimal.cs keeps only the previous value.
EXAMPLE
  nums = [-2, 1,-3, 4,-1, 2, 1,-5, 4]
  bestEndingAt= [-2, 1,-2, 4, 3, 5, 6, 1, 5] (at 4: max(4, 4+-2) = 4, fresh)
  maxSum = 6. All negative [-3,-1,-2] -> table [-3,-1,-2], answer -1.
COMPLEXITY
  Time  O(n)  one pass to fill the table, one pass to scan it
  Space O(n)  the bestEndingAt array holds one value per index
WATCH OUT
  - Empty nums throws at bestEndingAt[0]; ask if empty input can happen.
  - Seeding maxSum with 0 instead of bestEndingAt[0] returns 0 for an
    all-negative array; the answer must come from a real subarray.
  - Do not write Math.Max(bestEndingAt[i-1], ...): that mixes "best so far"
    with "best ending here" and allows gaps between elements.
  - Sums are int; a very long run of large values can overflow silently.
================================================================================
*/
