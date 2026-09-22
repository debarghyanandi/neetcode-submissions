// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
// -  Kadane's algorithm, space-optimized   [kadane-constant-space]
// -  ranks above suboptimal.cs (O(n) time / O(n) space)
// -
// -  Reference solution - not one you solved yourself
// -
// -  Single pass tracks running sum and resets when negative; updates
// -  maximum in one pass.
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
 PATTERN : Kadane's Algorithm - drop a negative running sum
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Optimal
================================================================================
VARIABLES
  maxSum      best subarray sum seen so far over the whole array
  currentSum  best sum of a subarray that ends at the element just added
WHY THIS PATTERN
  The problem asks for the largest sum over contiguous elements, so every
  candidate subarray is fixed by its right end. That lets you scan once and keep
  only one number per right end: currentSum, the best sum ending here. The
  recurrence is local - the best subarray ending at this element is either this
  element alone or this element glued to the best one ending just before it - so
  no array of past states is needed, only maxSum as the running answer.
BRUTE FORCE
  The first thing most people write is two nested loops: for every start index,
  extend the end index and keep a running total, taking the max. That is O(n^2)
  time and correct, but it recomputes the same prefix sums again and again.
  Kadane's keeps the single fact those loops rediscover - that a negative prefix
  is never worth carrying - and collapses the inner loop away.
INVARIANT
  After the body runs for element number, currentSum equals the maximum sum of
  any subarray whose last element is number, and maxSum equals the maximum over
  all subarrays that end at or before it. The reset `if (currentSum < 0)
  currentSum = 0;` preserves the first half: if the best sum ending before this
  element was negative, starting fresh at this element beats extending. Since
  every subarray has exactly one last element, and maxSum is updated at every
  element, the final maxSum has seen every candidate.
WHY MAXSUM STARTS AT NUMS[0] AND NOT 0
  Initialising maxSum to 0 would silently allow the empty subarray and return 0
  for input like [-3, -1, -5], where the true answer is -1. Seeding with nums[0]
  guarantees the answer is the sum of at least one real element. The reset only
  zeroes currentSum, never maxSum, so an all-negative array still ends with the
  largest single element: each iteration sets currentSum to that element alone,
  and maxSum keeps the biggest.
WATCH OUT
  The first line reads nums[0] with no guard, so an empty array throws
  IndexOutOfRangeException and a null nums throws NullReferenceException.
  currentSum and maxSum are int, so a long array of large values can overflow
  and wrap to a negative sum without any error - a long accumulator would be the
  safe change. The reset sits before the add, which is what makes the invariant
  hold at the top of each step; moving it after `currentSum += number` still
  works only because maxSum is taken before the reset, so do not reorder these
  three lines casually.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Return the start and end indices of the best subarray, not just the sum.
     Track a candidate start that is set to the current index whenever
     currentSum is reset to 0, and copy it plus the current index into
     bestStart/bestEnd only when maxSum is updated. That needs the index, so the
     foreach becomes a plain for loop.
  2. What if the array is circular, so a subarray may wrap from the end to the
  front?
     Run this scan twice - once for the maximum sum, once for the minimum sum -
     and compare maxSum against totalSum - minSum. Special case: if every
     element is negative, minSum equals the total and the wrap answer is an
     empty subarray, so return maxSum.
  3. The array does not fit in memory and arrives as a stream.
     This code already works unchanged, because it keeps only two ints and
     touches each element once; you just need the first element to seed maxSum
     before the loop starts.
  4. What changes for maximum product of a contiguous subarray?
     A single running value is not enough, because a negative times a negative
     can become the largest product. You carry both the running max and the
     running min ending here and swap them when the element is negative.
TRIGGER
  A question asks for the best contiguous run in a one-dimensional array and the
  quantity is additive, so each element only needs to know the best run ending
  at its neighbour.
C# NOTE
  Math.Max(int, int) is a plain non-generic overload here, so there is no
  comparer or boxing involved; the idiomatic hardening is a guard such as `if
  (nums is null or { Length: 0 }) throw new ArgumentException(nameof(nums));`
  before the nums[0] read, using C# pattern matching instead of two separate if
  statements.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
