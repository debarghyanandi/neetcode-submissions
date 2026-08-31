// ##########################################################################
// #  optimal.cs            O(n) time / O(1) space
// #  sliding window, shrink while valid, running sum   [sliding-window-running-sum]
// #  ranks above suboptimal.cs (O(n) time / O(n) space)
// #
// #  YOU SOLVED THIS YOURSELF - marked '//My solution'
// #
// #  maintains window sum incrementally by adding nums[right] and
// #  subtracting nums[left] as the two pointers advance, each index visited
// #  O(1) amortized times
// ##########################################################################

public class Solution
{
    public int MinSubArrayLen(int target, int[] nums)
    {
        int sum = 0;
        int left = 0;
        int result = int.MaxValue;

        for (int right = 0; right < nums.Length; right++)
        {
            sum = sum + nums[right];   // admit the new right-hand element

            // Shrink while the window still qualifies - the first failure
            // means every shorter window ending here fails too.
            while (sum >= target)
            {
                result = Math.Min(result, right - left + 1);
                sum = sum - nums[left];  // evict, THEN move the edge
                left++;
            }
        }

        return result == int.MaxValue ? 0 : result;
    }
}

/*
================================================================================
 PATTERN : Sliding Window - Shrinkable, with a RUNNING SUM
 SOURCE  : YOUR OWN SOLUTION (submission-1, marked '//My solution')
 STATUS  : Optimal - O(n) time, O(1) space
================================================================================

WHY THIS PATTERN
  suboptimal.cs stores every prefix so it can answer "sum of nums[l..r]" for
  any pair. But this algorithm never asks about any pair - it asks about ONE
  window that only ever moves forward. A quantity that changes by a known
  delta on each move does not need to be stored; it needs to be maintained.

      right++  ->  sum += nums[right]
      left++   ->  sum -= nums[left]

  That is the whole difference, and it is the single most reusable idea in
  the sliding-window family: keep the window's summary incrementally, never
  recompute it.

BRUTE FORCE (and why it fails)
  All O(n^2) subarrays, each summed in O(n): O(n^3), or O(n^2) with prefix
  sums. Both re-examine work the window already did. Because all values are
  non-negative, the sum is MONOTONIC in the window size - growing the window
  can only increase it - so once a window qualifies there is no reason ever
  to move `left` backward. That is what licenses the two pointers.

INVARIANT
  sum == the sum of nums[left..right] at every point where the loop condition
  is tested, and result == the shortest qualifying window seen so far.

ALGORITHM (NeetCode: "Sliding Window")
  1. sum = 0, left = 0, result = int.MaxValue.
  2. For right = 0..n-1:
       a. sum += nums[right].
       b. While sum >= target:
            - result = min(result, right - left + 1)
            - sum -= nums[left]; left++
  3. Return result == int.MaxValue ? 0 : result.

COMPLEXITY
  Time  : O(n). right advances n times; left advances at most n times over
          the ENTIRE run, not per iteration - that amortised argument is the
          thing to say out loud in an interview, because the nested while
          makes it look quadratic at a glance.
  Space : O(1) - three ints.

TRIGGER
  "Smallest / shortest window satisfying a threshold" where the window's
  measure only moves one way as the window grows. Non-negative values, or
  counts, or any monotone accumulation. If the measure can go both ways, the
  shrink step is unsound and this pattern does not apply.

C# NOTES
  - sum stays int here; on a 10^5 x 10^4 input it can reach 10^9, which fits,
    but long is the safer default the moment constraints are not in front of
    you.
  - Math.Min on ints is a JIT intrinsic - no branch, no method call.
  - Array.Empty<int>() / a length check first is worth adding if nums can be
    null; LeetCode guarantees it is not.

WATCH OUT
  - Subtract nums[left] BEFORE incrementing left. Reversing those two lines
    is the classic bug and it silently returns wrong answers rather than
    crashing.
  - `while`, not `if`. With target = 3 and nums = [4, 1, 1], the window at
    right = 0 qualifies and must shrink to empty before moving on.
  - Return 0, not int.MaxValue, when no window qualifies.
================================================================================
*/
