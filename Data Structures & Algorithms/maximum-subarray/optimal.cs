// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
// -  Kadane's algorithm, rolling max   [kadane-rolling]
// -  ranks above suboptimal.cs (O(n) time / O(n) space)
// -
// -  Reference solution - not one you solved yourself
// -
// -  Single pass keeping a running sum reset to 0 when negative, tracking
// -  the max seen, so only O(1) extra state is needed.
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
 PATTERN : Kadane - drop the running sum once it turns negative
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  The answer is a max over quadratically many contiguous ranges, but every range
  is pinned by its right endpoint. So replace the global question with a local
  one asked at each index: what is the best subarray that ENDS here. That local
  answer depends only on the previous local answer, which collapses the whole
  search into one left-to-right pass. currentSum carries the local answer;
  maxSum carries the running global best.
BRUTE FORCE
  Two nested loops: fix a start, extend the end, accumulate. Quadratic, and it
  recomputes the same prefixes over and over. Kadane keeps the identical "extend
  the end" motion and deletes the outer loop, because the only start worth
  keeping is determined by the sign of currentSum - no search over starts is
  needed.
INVARIANT
  At the point just after the reset check, currentSum is the best sum of a
  subarray ending at the PREVIOUS index if that sum was positive, and 0 (the
  empty tail) otherwise. After currentSum += number, currentSum is exactly the
  max sum over subarrays ending at this number, and it is nonempty since
  currentSum >= number. maxSum is therefore the max over all nonempty subarrays
  ending at or before the current index; at the end of the loop that is the
  answer.
WHY THE RESET IS CORRECT
  The best subarray ending at index i is either nums[i] alone or (best ending at
  i-1) + nums[i]. The max of those two equals max(prev, 0) + nums[i], which is
  precisely what the reset plus the add compute. Intuition for the drop: a
  prefix with a negative total lowers every extension of it by that amount, so
  restarting at i beats carrying it by |prev|. Order matters - the reset runs
  BEFORE the add, so the value compared against maxSum always contains at least
  the current element.
THE ALL-NEGATIVE CASE
  maxSum is seeded with nums[0], not 0. Seed it with 0 and [-3,-1,-2] returns 0,
  i.e. the empty subarray, which the problem forbids. With this seed every
  currentSum ends up equal to its own element (each previous total is negative
  and gets dropped) and maxSum settles on the largest element, -1. Note the loop
  also touches nums[0]: currentSum is 0 there, so it becomes nums[0] and maxSum
  = Math.Max(nums[0], nums[0]). No double counting, no special case for the
  first index.
WATCH OUT
  1. nums[0] throws on an empty array - the code leans on the constraint that
  nums has at least one element; call that out before the interviewer does.
  2. Do not move the clamp to after the add-and-compare. Zeroing a negative
  currentSum before taking Math.Max lets 0 win on an all-negative input, which
  is the empty-subarray answer again.
  3. currentSum is int. If the constraints widen, n * max|value| can overflow;
  long is the fix.
  4. Math.Max(maxSum, currentSum) must run every iteration, not only when
  currentSum is positive - the best answer may be negative.
FOLLOW-UPS
  Return the indices, not the sum: keep a start variable set to the current
  index whenever currentSum is zeroed, and snapshot (start, i) whenever maxSum
  actually improves.
  Circular array: answer is max(kadane(nums), totalSum - minKadane(nums)), with
  a guard for the all-negative case where the second term is the empty subarray.
  Max subarray over arbitrary query ranges: Kadane does not compose, so switch
  to divide and conquer / a segment tree node storing best-prefix, best-suffix,
  total, and best.
  Fixed-length window is a different problem - that one is a plain sliding
  window or prefix sums, not this.
TRIGGER
  A one-dimensional array, an unconstrained-length contiguous span, and a
  running quantity that can be summarized by a single number carried left to
  right where a negative carry is strictly worse than restarting. Same skeleton
  as best-time-to-buy-and-sell-stock (which carries a running minimum instead of
  resetting) and gas station (which resets the tank at the same signal).
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
