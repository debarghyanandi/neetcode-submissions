// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
// -  Kadane's algorithm, rolling max   [kadane-max-subarray]
// -  ranks above suboptimal.cs (O(n) time / O(n) space)
// -
// -  Reference solution - not one you solved yourself
// -
// -  single pass keeping a running sum reset to 0 when negative, tracking
// -  the max with O(1) extra state
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
 PATTERN : Kadane - drop a negative running sum, keep the best
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  The subarray must be contiguous, so at every index there is exactly one
  decision: extend the subarray that ended at the previous index, or start a new
  one here. One decision bit means one number of carried state (currentSum), and
  the whole search collapses into a single pass. The moment a problem says
  "contiguous" and asks for a max/min over all such ranges, look for this shape
  before reaching for anything fancier.
BRUTE FORCE
  Fix each start i, walk j forward keeping a running total, record the best:
  O(n^2). Recomputing each sum from scratch makes it O(n^3).

  The prefix-sum reframing is worth remembering because it links this to a whole
  family: with P[k] = sum of the first k elements, the answer is max over j > i
  of P[j] - P[i], so you sweep j and keep the smallest prefix seen so far. That
  is the same algorithm as this file wearing different clothes - "smallest
  prefix so far" and "reset currentSum when it goes negative" are the same
  quantity. It is also literally the best-time-to-buy-and-sell-stock sweep.
INVARIANT
  Write best(i) for the largest sum of a subarray ending exactly at index i.

  After the line currentSum += number executes on the iteration for index i,
  currentSum == best(i). After the Math.Max line, maxSum == the best over all
  subarrays ending at any index <= i. The return is that statement at i = n-1.

  Note what the declaration comment on currentSum does and does not claim: it
  describes the post-add state. At declaration time the 0 is not a subarray sum
  at all, it is a stand-in for the empty prefix, which is why nothing reads it
  before the first += runs.
WHY DROPPING THE PREFIX IS SAFE
  The recurrence is best(i) = nums[i] + max(best(i-1), 0), and the clamp is a
  direct encoding of that max.

  The argument for the max: a subarray ending at i either is nums[i] alone or is
  some subarray ending at i-1 with nums[i] appended. If best(i-1) < 0, then
  every subarray ending at i-1 has negative sum, so appending any of them to
  nums[i] gives strictly less than nums[i] - no optimal subarray ending at i can
  contain index i-1, and discarding the whole running total loses nothing.

  The part people skip: discarding also cannot lose a global candidate, because
  maxSum already absorbed best(i-1) on the previous iteration, before the clamp
  had a chance to zero it. Every best(i) is compared into maxSum exactly once,
  in the iteration that computes it.
WATCH OUT
  maxSum is seeded with nums[0], not 0, and that is the entire correctness story
  for all-negative input. Seed it with 0 and [-3,-1,-2] returns 0 - the sum of
  the empty subarray, which the problem does not allow - instead of -1.
  currentSum can safely start at 0 because the clamp-then-add pair rebuilds it
  into a real value before any comparison happens.

  Reading nums[0] means an empty array throws IndexOutOfRangeException; the
  problem guarantees length >= 1, so this is a deliberate precondition, not an
  oversight. Say so if asked rather than adding a guard reflexively.

  nums[0] is touched twice - once as the seed, once on the first iteration -
  which is harmless only because Math.Max is idempotent.
FOLLOW-UPS AN INTERVIEWER WILL ASK
  1. Return the indices, not the sum. Keep a candidate start: when the clamp
  fires on index i, the next subarray begins at i. Commit start and i into the
  answer pair only when maxSum actually improves - not when currentSum improves.

  2. Circular array. Answer is max(Kadane(nums), total - MinKadane(nums)), with
  the special case that when every element is negative the wraparound branch
  corresponds to removing everything, so return the plain Kadane result.

  3. "Any other approach?" Divide and conquer: best in left half, best in right
  half, best crossing the midpoint, O(n log n). It is strictly worse here but is
  the expected answer to that prompt.

  4. Maximum product subarray. The clamp trick does not transfer - a negative
  factor swaps largest and smallest, so you must carry both a running max and a
  running min.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
