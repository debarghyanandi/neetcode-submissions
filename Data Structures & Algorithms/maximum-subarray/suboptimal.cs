// --------------------------------------------------------------------------
//  Reference solution - from NeetCode / other resource (submission-1)
//  Not one you solved yourself.
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
 PATTERN : Dynamic Programming - 1D table, "best ending at index i"
 SOURCE  : NeetCode / other resource (submission-1)
 STATUS  : Sub-optimal on SPACE (O(n) table where O(1) suffices)
================================================================================

WHY THIS PATTERN
  The obstacle is that a subarray can start anywhere. Fix that by defining
  the state around its END instead: "the best subarray ending exactly at i."
  Now every index has one well-defined answer, and each depends only on its
  neighbour - which is the definition of a 1D DP.

  Recurrence:
      bestEndingAt[i] = max( nums[i], nums[i] + bestEndingAt[i-1] )
  In words: at each element you make one binary decision - START A NEW
  SUBARRAY HERE, or EXTEND THE ONE BEFORE. That is the whole problem.

  The global answer is max over all i, because the optimal subarray must end
  somewhere.

BRUTE FORCE (and why it fails)
  All O(n^2) subarrays, summing each: O(n^3), or O(n^2) with a running sum.
  The DP notices that the best run ending at i is one step from the best run
  ending at i-1 - so the recomputation is pure waste.

WHY THIS IS SUB-OPTIMAL
  The table is never read further back than one index. Storing all n values
  and then scanning them again is O(n) space and two passes for information
  that fits in two ints. Collapsing it gives optimal.cs (Kadane).
  Keep this file: writing the explicit table FIRST and then collapsing it is
  the reliable way to derive Kadane rather than memorise it - and the same
  collapse trick applies to climbing-stairs, house-robber, and most 1D DP.

ALGORITHM (NeetCode: "Dynamic Programming")
  1. Copy nums into bestEndingAt (each element alone is a valid subarray).
  2. For i from 1: bestEndingAt[i] = max(nums[i], nums[i] + bestEndingAt[i-1]).
  3. Return the maximum entry of the table.

COMPLEXITY
  Time  : O(n) - one pass to fill, one to scan. Could be fused into one.
  Space : O(n) - the table.

TRIGGER
  "Maximum/minimum over all CONTIGUOUS subarrays." Whenever a subarray's
  start is unconstrained, re-anchor the state on its end.

C# NOTES
  - (int[])nums.Clone() is a shallow copy; the cast is needed because
    Array.Clone() returns object. nums.ToArray() (LINQ) or Array.Copy do the
    same - Clone is the allocation-cheapest of the three.
  - Cloning also PROTECTS THE INPUT: writing into nums directly would mutate
    the caller's array, an invisible side effect and a real interview ding.
  - The final scan could be folded into the fill loop, saving a pass.

WATCH OUT
  - Seed maxSum from bestEndingAt[0], never from 0 - an all-negative array
    like [-3,-1,-2] must return -1, not 0. This is THE test case for this
    problem.
================================================================================
*/
