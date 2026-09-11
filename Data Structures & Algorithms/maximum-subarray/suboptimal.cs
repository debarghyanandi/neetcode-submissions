// --------------------------------------------------------------------------
// -  suboptimal.cs         O(n) time / O(n) space
// -  Kadane's recurrence materialized into a DP array   [kadane]
// -  ranks below optimal.cs (O(n) time / O(1) space)
// -
// -  Reference solution - not one you solved yourself
// -
// -  Same best-sum-ending-at-i recurrence as Kadane but stores every
// -  intermediate value in an array before scanning for the max.
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
 PATTERN : Kadane / DP on best subarray ending at i
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Suboptimal
================================================================================
WHY THIS PATTERN
  Every non-empty subarray has exactly one last index. So partition the whole
  search space by that last index: let bestEndingAt[i] be the largest sum among
  subarrays ending at i. The answer is the max over all i. That reframing is the
  entire trick - it turns a two-dimensional search (start, end) into one pass
  over the end index, because the best run ending at i is determined by the best
  run ending at i-1 alone.
THE RECURRENCE
  A subarray ending at i is either the single element [i], or some subarray
  ending at i-1 with nums[i] glued on. The best of the second kind is nums[i] +
  bestEndingAt[i-1], since nums[i] is a fixed additive term and maximizing the
  tail means maximizing the part that ends at i-1. Hence bestEndingAt[i] =
  Math.Max(nums[i], nums[i] + bestEndingAt[i-1]).

  Read greedily, the Max is just: if bestEndingAt[i-1] is negative, drop
  everything before i and start fresh; otherwise keep extending. Those two
  readings are the same line of code.
WHY THE SEEDING WORKS
  bestEndingAt is (int[])nums.Clone(), which pre-fills every cell with the
  single-element subarray [i]. Only cell 0 actually needs that seed - it is the
  base case, the one index with no i-1 to extend - and the loop from i = 1
  overwrites the rest. Clone also means nums is never mutated; writing the
  recurrence back into nums would work identically and use no extra array, but
  it destroys the caller's input.
THE TRAP
  maxSum starts at bestEndingAt[0], not 0. Initialize it to 0 and the
  all-negative case breaks: for nums = [-3, -1, -5] every bestEndingAt entry is
  negative, and a 0 seed returns 0, which corresponds to the empty subarray -
  not an allowed answer. Same trap applies to the alternative of seeding
  int.MinValue and then computing nums[i] + running, which can underflow. Seed
  from real data, index 0.
WHY THIS ONE LOSES
  bestEndingAt[i] reads exactly one earlier cell, bestEndingAt[i-1], and nothing
  else ever reads cell i again. So the array is pure waste: replace it with a
  single int (call it current), update current = Math.Max(nums[i], nums[i] +
  current) inside the loop, and fold maxSum = Math.Max(maxSum, current) into the
  same iteration. That is constant extra space and one pass instead of two. The
  stored table buys nothing here because there is no reconstruction step
  consuming it.
FOLLOW-UP TO EXPECT
  "Now return the subarray, not just the sum." The rolling-variable version
  handles it with three ints: when the Max picks nums[i] you are starting fresh,
  so set start = i; when maxSum improves, record (start, i). Notably the stored
  bestEndingAt array does NOT make this easier - you would still walk backward
  from the argmax while the running sum stays positive. The other expected
  follow-up is the divide-and-conquer solution (best-left, best-right,
  best-crossing-the-midpoint), which is the standard answer when asked for a
  different paradigm; it is strictly slower than this linear scan.
TRIGGER
  Reach for this shape when the ask is a single best contiguous run over a
  sequence and elements can be negative, so growing the window is not
  monotonically good. The signature is that a running accumulator becomes a
  liability once it goes negative and should be abandoned. Variants that rhyme:
  maximum product subarray (carry both max and min because a negative flips
  them), and best-time-to-buy-and-sell-stock (same scan, tracking a running
  minimum instead of a running sum).
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
