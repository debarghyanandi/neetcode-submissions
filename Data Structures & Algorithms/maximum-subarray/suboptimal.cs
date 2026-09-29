// --------------------------------------------------------------------------
// -  suboptimal.cs         O(n) time / O(n) space
// -  Kadane's algorithm with explicit DP table   [kadane-explicit-dp]
// -  ranks below optimal.cs (O(n) time / O(1) space)
// -
// -  Reference solution - not one you solved yourself
// -
// -  DP table stores best subarray sum ending at each position; trade space
// -  for clarity.
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
 PATTERN : 1D DP (Kadane) - best subarray sum ending at each index
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Suboptimal
================================================================================
VARIABLES
  bestEndingAt  bestEndingAt[i] = largest sum of any subarray that ends exactly at i
  maxSum        the best value seen so far across all bestEndingAt entries
  sum           one bestEndingAt entry, read in the second pass
WHY THIS PATTERN
  The problem asks for the best contiguous subarray. Every subarray ends at some
  index. So if you know the best sum ending at each index, the answer is the
  largest of those. The best sum ending at i depends only on the best sum ending
  at i-1. That is a one-step recurrence, so a simple DP works: bestEndingAt[i] =
  max(nums[i], nums[i] + bestEndingAt[i - 1]).
BETTER APPROACH
  The better approach is classic Kadane with two scalars: a running "current"
  and a running "best". That uses O(1) extra space and one pass. This file loses
  on two points. It allocates the full bestEndingAt array, but each step only
  reads bestEndingAt[i - 1]. It also makes a second foreach pass to find the
  max, when maxSum could be updated inside the first loop.
INVARIANT
  After step i, bestEndingAt[i] holds the true maximum sum over all subarrays
  that end at i. This holds because such a subarray is either just [nums[i]], or
  it extends a subarray ending at i-1. In that second case, the best choice is
  the best one ending at i-1. The base case bestEndingAt[0] = nums[0] comes from
  the Clone. Every subarray ends at some index, so the max over all of
  bestEndingAt is the answer.
DROP A NEGATIVE PREFIX
  Math.Max(nums[i], nums[i] + bestEndingAt[i - 1]) picks "start fresh" exactly
  when bestEndingAt[i - 1] < 0. A negative prefix can only make the sum smaller,
  so you cut it. Seeding maxSum from bestEndingAt[0] and not from 0 is what
  makes an all-negative input return its largest element, not 0.
WATCH OUT
  An empty nums throws IndexOutOfRangeException at bestEndingAt[0], because the
  loop is skipped but the read is not. The expression nums[i] + bestEndingAt[i -
  1] is unchecked int math, so it can silently overflow and wrap to a negative
  number when values are large. The foreach loop compares bestEndingAt[0] with
  itself once. That is harmless, but it is wasted work.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you return the start and end indices of the best subarray, not only its
  sum?
     Keep a "start" index and reset it to i whenever you choose nums[i] alone.
     When maxSum improves, record start and i. The time is the same, and you add
     two ints of state.
  2. What if the array is circular (the subarray can wrap around the end)?
     The answer is max(normal Kadane, total sum - minimum subarray sum). Run a
     second Kadane for the minimum. If every element is negative, return the
     normal Kadane result, because total - min would describe an empty subarray.
  3. Can you solve it with divide and conquer?
     Split at mid. The answer is the best of the left half, the right half, or
     the best subarray crossing mid. This takes O(n log n) time, so it is
     slower. But each half can be solved on its own, and it is the base of the
     segment-tree version that answers range queries.
  4. What if the input comes as a stream you can only read once?
     Kadane already needs only the previous value. Keep current and best as
     scalars and update them per element. There is no array and no second pass.
TRIGGER
  When a problem asks for the best contiguous subarray and extending or
  restarting depends only on the previous element's result, reach for "best
  ending here" DP (Kadane).
C# NOTE
  (int[])nums.Clone() returns object, so it needs a cast. It makes a shallow
  copy (only the top-level array is copied), which is fine for ints. You could
  drop the copy completely by writing into nums in place, if the caller allows
  it to change, or by using the two-scalar form.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
