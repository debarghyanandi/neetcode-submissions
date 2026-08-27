// --------------------------------------------------------------------------
//  Reference solution - from NeetCode / other resource (submission-0)
//  Not one you solved yourself.
// --------------------------------------------------------------------------

public class Solution
{
    public int MaxSubArray(int[] nums)
    {
        int maxSum = nums[0];      // must be a real element: handles all-negative input
        int currentSum = 0;        // best sum of a subarray ending at the current index

        foreach (int number in nums)
        {
            // A negative running total can only hurt whatever comes next,
            // so drop it and start a fresh subarray here.
            if (currentSum < 0)
                currentSum = 0;

            currentSum += number;
            maxSum = Math.Max(maxSum, currentSum);
        }

        return maxSum;
    }
}

/*
================================================================================
 PATTERN : Kadane's Algorithm (1D DP collapsed to O(1) space)
 SOURCE  : NeetCode / other resource (submission-0)
 STATUS  : Optimal
================================================================================

WHY THIS PATTERN
  Exactly the DP in suboptimal.cs, with the table thrown away. The
  recurrence only ever looks one step back, so a single rolling variable
  replaces the whole array:

      bestEndingAt[i] = max(nums[i], nums[i] + bestEndingAt[i-1])
              becomes
      currentSum = max(number, currentSum + number)

  and `if (currentSum < 0) currentSum = 0;` is that same max written as a
  reset: if the carried prefix is negative, starting fresh is strictly better.

BRUTE FORCE (and why it fails)
  All subarrays with a running sum: O(n^2). Kadane makes ONE binary decision
  per element instead of re-summing every range.

THE GREEDY INTUITION WORTH REMEMBERING
  A prefix with a negative sum is a liability, never an asset. Whatever comes
  after it is better off without it - so the moment the running total dips
  below zero, abandon it. That single sentence reconstructs the algorithm
  from scratch if you ever blank on the code.

INVARIANT
  After processing index i:
    currentSum = the largest sum of any subarray ending at i (floored at 0
                 before the addition, which is the "start fresh" branch)
    maxSum     = the largest sum of any subarray within nums[0..i]

ALGORITHM (NeetCode: "Kadane's Algorithm")
  1. maxSum = nums[0], currentSum = 0.
  2. For each element: reset currentSum to 0 if it is negative.
  3. Add the element to currentSum.
  4. maxSum = max(maxSum, currentSum).

COMPLEXITY
  Time  : O(n) - single pass.
  Space : O(1) - two ints. This is the improvement over the DP table.

TRIGGER
  "Maximum sum of a contiguous subarray", or any running accumulation where a
  negative carry can be discarded. Variants that reuse the skeleton:
  maximum PRODUCT subarray (track min and max - a negative can flip to best),
  circular maximum subarray (answer = max(Kadane, total - minKadane)).

C# NOTES
  - foreach over int[] compiles to an indexed loop with bounds-check
    elimination; no performance reason to write for(;;) here.
  - Overflow is silent by default in C#. With int.MaxValue-scale inputs, use
    long or a `checked` block so corruption throws instead of hiding.
  - Returning the INDICES as well needs two more variables (a tentative start
    that resets whenever currentSum does, and a recorded start/end pair
    written when maxSum improves). That is the standard follow-up.

WATCH OUT
  - maxSum MUST be seeded with nums[0], not 0. Seeded with 0, the input
    [-3,-1,-2] returns 0 instead of -1. Every wrong Kadane fails on this.
  - The reset happens BEFORE adding the current element, not after.
  - Empty input would throw on nums[0]; the problem guarantees n >= 1.
================================================================================
*/
