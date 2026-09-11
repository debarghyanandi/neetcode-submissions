// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
// -  Kadane's algorithm, rolling max   [kadane]
// -  ranks above suboptimal.cs (O(n) time / O(n) space)
// -
// -  Reference solution - not one you solved yourself
// -
// -  Single pass keeping a running sum reset to 0 when negative, tracking
// -  the max seen with O(1) extra state.
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
 PATTERN : Kadane - reset the running sum when it goes negative
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Optimal
================================================================================
THE RECURRENCE
  Define best(i) = the largest sum of a subarray that ENDS exactly at index i.
  Any such subarray either extends the one ending at i-1 or starts fresh at i,
  so best(i) = max(nums[i], best(i-1) + nums[i]) = max(0, best(i-1)) + nums[i].
  Those two forms are identical, and the second is what the loop body computes:
  the guard turns currentSum into max(0, best(i-1)), then currentSum += number
  finishes it. Every nonempty subarray ends at some index, so the answer is max
  over i of best(i) - which is exactly what maxSum accumulates.
INVARIANT
  At the top of each iteration, currentSum holds best(i-1) (and 0 on the first
  iteration, which is the correct identity since there is no previous element).
  After the two statements that follow, currentSum holds best(i) and maxSum
  holds max(nums[0], best(0..i)). Nothing else carries state across iterations -
  no index, no length, no start pointer - which is why a single int of history
  is enough.
WHY DROPPING A NEGATIVE PREFIX IS SAFE
  This is the whole trick and the thing an interviewer will push on. If
  best(i-1) < 0, then for any subarray ending at i that includes index i-1,
  deleting that entire prefix gives a subarray ending at i with a strictly
  larger sum. So no optimal subarray ending at i can carry a negative running
  total into i. The comment in the code states this; the exchange argument above
  is the proof. Note the test is on the accumulated sum, not on the element - a
  negative number in the middle of a strong run is kept, because currentSum
  stays positive through it.
WHY MAXSUM STARTS AT NUMS[0]
  currentSum is clamped up to 0 before each add, so it can never be used to
  report a negative answer directly - but on an all-negative input every best(i)
  equals nums[i], and the true answer is the largest (least negative) element.
  Seeding maxSum with 0 would return 0 there, which is a subarray of length zero
  and not allowed by this problem. Seeding with nums[0] guarantees the returned
  value is always a real element's contribution. That same line is the unguarded
  read that assumes nums has at least one element; the problem's constraints
  promise it, and an empty array throws rather than silently returning 0.
ORDER OF OPERATIONS TRAP
  The clamp happens BEFORE the add and the maxSum update happens AFTER it. Both
  placements matter. Clamp after adding and you would record a sum that was
  already discarded; update maxSum before adding and you would record best(i-1)
  twice and never see best(n-1). The three statements - clamp, add, record -
  only produce best(i) in this order.
TRIGGER
  Contiguous, one dimension, asked for an extremum of a running aggregate, and
  the elements can go negative so a longer window is not automatically better.
  That combination is Kadane. If the values were all nonnegative the answer
  would trivially be the whole array; if the subarray had to be non-contiguous
  it would be a selection problem, not this one.
FOLLOW-UPS TO EXPECT
  1) Return the indices, not the sum: track a start that resets to the current
  index whenever the clamp fires, and copy start/current into begin/end whenever
  maxSum is updated. 2) Allow the empty subarray: seed maxSum at 0 instead of
  nums[0] and the rest is unchanged. 3) Maximum sum circular subarray: answer is
  max(Kadane, totalSum - minKadane), with the all-negative case handled
  separately since totalSum - minKadane would pick the empty subarray. 4) Why
  not divide and conquer: it works (recurse on halves, plus a best-crossing
  scan) but is strictly worse here and harder to get right.
BRUTE FORCE FOR CONTRAST
  Fix a left endpoint, extend right while keeping a running total, and take the
  max - quadratic, no extra memory, and easy to write under pressure as a
  correctness oracle against this version. The optimal version is the
  observation that the inner loop only ever needs one number from the previous
  outer iteration.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
