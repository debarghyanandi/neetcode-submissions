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
 PATTERN : Kadane / DP over subarray end index
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Suboptimal
================================================================================
VARIABLES
  bestEndingAt   bestEndingAt[i] = largest sum of a subarray that ends exactly at index i
  maxSum         running best over all end positions seen so far
WHY THIS PATTERN
  The problem asks for the best contiguous block, and every block ends at
  exactly one index. So if you can answer "best block ending at i" for every i,
  the answer is the biggest of those. That subproblem has a one-line recurrence:
  bestEndingAt[i] is either nums[i] alone or nums[i] glued onto
  bestEndingAt[i-1], which is why one left-to-right pass is enough.
BETTER APPROACH
  The better version is the same recurrence with no array: keep one int current
  = nums[0] and one int maxSum, and inside the loop do current =
  Math.Max(nums[i], nums[i] + current) then maxSum = Math.Max(maxSum, current).
  That is O(1) extra space and one pass. This file loses because it materialises
  a whole int[] copy of nums it never needs again, and then walks it a second
  time in the foreach to take the maximum - the max could have been folded into
  the first loop.
INVARIANT
  After iteration i of the first loop, bestEndingAt[i] holds the true maximum
  sum over all subarrays whose last element is nums[i]. It holds because such a
  subarray is either just nums[i], or nums[i] plus a subarray ending at i-1, and
  the best of those is already stored in bestEndingAt[i-1]. Since every
  non-empty subarray ends somewhere, the maximum over the whole bestEndingAt
  array is the global answer.
ALL-NEGATIVE INPUT
  Seeding bestEndingAt with a copy of nums, and starting maxSum at
  bestEndingAt[0] rather than 0, is what makes the all-negative case correct.
  The recurrence can never drop below nums[i] itself, so for [-3,-1,-7] it
  returns -1, not 0. A version that starts maxSum at 0 would silently return the
  empty subarray.
WATCH OUT
  An empty nums throws IndexOutOfRangeException at maxSum = bestEndingAt[0],
  because the loop and the foreach are both safely skipped but that line is not
  guarded. The sum nums[i] + bestEndingAt[i - 1] is int arithmetic and will wrap
  around silently if the running sum overflows int; it is not checked. Also note
  nums.Clone() returns object, so the (int[]) cast is required - drop it and the
  file does not compile.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Return the start and end indices of the best subarray, not just the sum.
     Track a start pointer: when you choose nums[i] over nums[i] + previous, set
     curStart = i; when maxSum improves, record bestStart = curStart and bestEnd
     = i. Same time, a few more variables.
  2. What if the array is circular, so the subarray may wrap around the end?
     Answer is max(normal Kadane, totalSum - minimum subarray sum), computed
     with a second Kadane run on the minimum. Special case: if every number is
     negative the wrap formula gives 0, so fall back to the normal Kadane
     result.
  3. Many queries asking for the best subarray inside a given range [l, r].
     Kadane per query is too slow; build a segment tree where each node stores
     total sum, best prefix, best suffix and best inner sum, and merge those
     four values. Build is O(n), each query O(log n).
  4. The array does not fit in memory and arrives as a stream.
     The O(1)-space variant already handles it - you only ever need the previous
     value and the running maximum, so nothing has to be stored.
TRIGGER
  The problem asks for the best contiguous run in a sequence and you can define
  the answer "ending exactly at index i" from the answer at i-1.
C# NOTE
  (int[])nums.Clone() does a full array allocation and copy just to seed the DP;
  if you keep the array form, int[] bestEndingAt = new int[nums.Length] with
  bestEndingAt[0] = nums[0] is clearer, since every other slot is overwritten by
  the loop anyway. The final foreach could also be written as bestEndingAt.Max()
  with System.Linq, but that adds an enumerator over the array for no gain here.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
