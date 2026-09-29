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
  maxSum      the best subarray sum seen so far; it starts at nums[0]
  currentSum  the sum of the best subarray that ends at the current number
WHY THIS PATTERN
  The problem asks for the largest sum of a contiguous subarray (a block of
  neighbours with no gaps). Every such subarray ends at some index. So it is
  enough to know, at each index, the best sum that ends exactly there. That
  value only depends on the value at the index before, so currentSum carries it
  forward and maxSum keeps the best one.
BRUTE FORCE
  Try every start index i. Then extend the end index j one step at a time, keep
  a running sum, and track the largest. This is O(n^2) time and O(1) space, and
  it is correct. It loses because it checks every pair (i, j) again, even though
  a start with a negative prefix sum can never beat starting later.
INVARIANT
  After each step of the loop, currentSum equals the best sum of a subarray that
  ends at number. This holds because the best subarray ending here is either
  number alone, or number added to the best subarray ending one step before. The
  code picks the first option exactly when the old currentSum is below 0. maxSum
  is the largest of all these values, so it covers every possible end index and
  is the answer.
RESET BEFORE ADD, MAX AFTER ADD
  The check "if (currentSum < 0) currentSum = 0" runs before number is added.
  Because of this, maxSum is always compared with a sum that includes at least
  one real element. This is why an all-negative input still returns its largest
  single element and not 0.
WATCH OUT
  An empty array throws IndexOutOfRangeException at nums[0]. The code never
  checks for this. The comment on currentSum is not true at the start: before
  the loop, 0 is not the sum of any subarray. It only becomes true after the
  first number is added. If you move the reset so it comes after the Math.Max
  line, all-negative input still works. But if you reset after adding and before
  Math.Max, the answer becomes 0, which is wrong.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How do you return the start and end indices of the best subarray, not just
  the sum?
     Use a for loop instead of foreach. Keep a tempStart variable and set it to
     i each time currentSum is reset. When maxSum improves, save tempStart and
     i. It is still O(1) extra space, with a few more variables.
  2. What if the array is circular, so a subarray can wrap from the end back to
  the start?
     The answer is the larger of two things: the normal Kadane result, or the
     total sum minus the minimum subarray sum. If every number is negative,
     return the normal result, because total minus minimum would mean an empty
     subarray.
  3. What about the largest-sum rectangle in a 2D matrix?
     Fix a pair of top and bottom rows. Add up each column between them into a
     1D array, then run this same loop on it. This costs O(rows^2 * cols) time.
  4. Can you solve it with divide and conquer?
     Yes. Split the array in half. The best subarray is in the left half, in the
     right half, or it crosses the middle. The crossing case is found by the
     best suffix sum of the left plus the best prefix sum of the right. This is
     O(n log n), which is slower, but each half can be solved in parallel.
TRIGGER
  Reach for this when a problem asks for the best contiguous subarray, and the
  best answer ending at index i can be built from the best answer ending at i-1.
C# NOTE
  In C#, int addition is unchecked by default. If currentSum += number goes past
  int.MaxValue, it wraps to a negative number with no error. Wrap the loop in a
  checked block to make it throw, or use long for both sums to hold larger
  totals.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
