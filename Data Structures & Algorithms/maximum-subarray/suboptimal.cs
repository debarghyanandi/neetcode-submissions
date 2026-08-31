// --------------------------------------------------------------------------
// -  suboptimal.cs         O(n) time / O(n) space
// -  1D DP table, best sum ending at i   [kadane-dp-table]
// -  ranks below optimal.cs (O(n) time / O(1) space)
// -
// -  Reference solution - not one you solved yourself
// -
// -  Same recurrence as Kadane but materializes the full array of per-index
// -  best sums before scanning it for the max, costing O(n) auxiliary
// -  space.
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
 PATTERN : Kadane / DP - best subarray ending at each index
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Suboptimal
================================================================================
WHY THIS PATTERN
  The brute force enumerates every (start, end) pair - quadratic, and it
  recomputes overlapping sums. The reframe that kills it: instead of asking
  "what is the best subarray?", ask "what is the best subarray that ENDS exactly
  at index i?" There are only n such questions, and the answer at i is
  determined entirely by the answer at i-1. That turns a search over pairs into
  a single left-to-right scan, which is exactly what bestEndingAt holds.
THE RECURRENCE AND WHY IT IS EXHAUSTIVE
  bestEndingAt[i] = Math.Max(nums[i], nums[i] + bestEndingAt[i - 1]).

  Any subarray ending at i is either the single element [i], or it also contains
  i-1 - there is no third case. In the second case it is some subarray ending at
  i-1 plus nums[i], and since nums[i] is a fixed additive constant, maximizing
  that sum means maximizing the part ending at i-1, which is bestEndingAt[i - 1]
  by the inductive hypothesis. So the two-way max covers every candidate. Read
  it as the greedy "drop the prefix if it is a net loss": you extend only while
  bestEndingAt[i - 1] is positive.
WHAT CLONE IS BUYING
  (int[])nums.Clone() seeds every cell with nums[i], the length-1 subarray,
  which is the base case of the recurrence and the correct value for
  bestEndingAt[0]. That is why the loop can start at i = 1 - index i-1 is always
  in range and always already final, because the scan writes cell i only after
  cell i-1 is settled. Clone also copies rather than aliases, so nums itself is
  never mutated; the caller's array survives.
THE NEGATIVE-NUMBERS TRAP
  maxSum is seeded with bestEndingAt[0], not 0. This is the single line most
  likely to be gotten wrong from memory. On all-negative input like [-3, -1, -2]
  the true answer is -1, the least-bad single element; seeding maxSum = 0 would
  return 0, an empty subarray, which the problem does not allow. Same reason the
  recurrence uses Math.Max(nums[i], ...) rather than Math.Max(0, ...) - the run
  is allowed to be negative, it is just not allowed to be empty.
WHY THIS LOSES TO THE ONE-PASS VERSION
  bestEndingAt[i] is read exactly once, by iteration i+1, and never again.
  Nothing reconstructs indices from the table, and the final foreach just takes
  a max over it. So the whole array collapses to one int: keep a rolling current
  = Math.Max(nums[i], nums[i] + current) and fold maxSum = Math.Max(maxSum,
  current) inside the same loop, both seeded from nums[0]. That is constant
  extra space and one traversal instead of two, with the identical recurrence.
  The array form is the useful teaching scaffold - it makes the DP table visible
  - but it stores a history the algorithm has no use for.
EDGE CASES THE CODE ASSUMES AWAY
  nums.Length == 0 throws IndexOutOfRangeException at maxSum = bestEndingAt[0];
  the method is written for a guaranteed non-empty input. Length 1 is fine: the
  loop body never runs and maxSum is nums[0]. Overflow on nums[i] +
  bestEndingAt[i - 1] is a real int hazard in principle, but under the standard
  constraints (n up to 1e5, |nums[i]| up to 1e4) the extreme total is 1e9,
  inside int range - worth saying out loud rather than leaving it looking
  unexamined.
THE FOLLOW-UP TO EXPECT
  "Return the subarray, not just the sum." Track start, end, and a candidate
  start: when Math.Max picks nums[i] over the extension, the run restarted, so
  set candidateStart = i; when the running value beats maxSum, commit start =
  candidateStart and end = i. Note that the current array-based shape does not
  give you this for free - you would still need those trackers, since
  bestEndingAt records sums, not boundaries. Related variants that reuse this
  recurrence: maximum product subarray (track both max and min ending at i,
  because a negative flips them) and maximum sum circular subarray (total minus
  the minimum subarray, with the all-negative case special-cased for the same
  reason as above).
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
