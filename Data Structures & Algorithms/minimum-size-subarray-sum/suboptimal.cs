// ##########################################################################
// #  suboptimal.cs         O(n) time / O(n) space
// #  sliding window, shrink while valid, prefix-sum lookup
// #  [sliding-window-prefix-sum]
// #  ranks below optimal.cs (O(n) time / O(1) space)
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  same two-pointer shrink logic but fetches each window sum via an
// #  O(n)-sized prefix array instead of tracking a running total, costing
// #  linear auxiliary space
// ##########################################################################

public class Solution
{
    public int MinSubArrayLen(int target, int[] nums)
    {
        // prefix[i] = sum of the first i elements, so the sum of nums[l..r]
        // is prefix[r + 1] - prefix[l] in O(1).
        int[] prefix = new int[nums.Length + 1];

        for (int i = 0; i < nums.Length; i++)
        {
            prefix[i + 1] = prefix[i] + nums[i];
        }

        int left = 0;
        int result = int.MaxValue;

        for (int right = 0; right < nums.Length; right++)
        {
            // Shrink while the window is still big enough to qualify.
            while (prefix[right + 1] - prefix[left] >= target)
            {
                result = Math.Min(result, right - left + 1);
                left++;
            }
        }

        return result == int.MaxValue ? 0 : result;
    }
}

/*
================================================================================
 PATTERN : Sliding window over a prefix-sum array (array redundant)
 SOURCE  : YOUR OWN SOLUTION - your own annotation at c76939d
 STATUS  : Suboptimal
================================================================================
WHY THIS PATTERN
  Every nums[i] is positive, so prefix is strictly increasing and the window sum
  prefix[right+1] - prefix[left] moves in a known direction with each pointer:
  it grows when right advances, shrinks when left advances. That monotonicity is
  the entire license for a two-pointer scan. Without it you would have to
  consider all O(n^2) (left, right) pairs, because a longer window would no
  longer be guaranteed to have a larger sum.
INVARIANT
  Two things hold at the top of every right iteration:
  1. sum(nums[left..right-1]) < target - the previous iteration's while loop ran
  until the window stopped qualifying, so left already sits past every start
  index that could pair with an earlier right.
  2. result is the minimum length over all qualifying subarrays that end at
  index right-1 or earlier.
  Also left <= right + 1 at all times. left == right + 1 means an empty window
  whose sum is 0, and the while condition 0 >= target is false for any target >=
  1, so left can never run past right + 1.
CORRECTNESS ARGUMENT
  The follow-up an interviewer will ask: why is it safe that left never rewinds?
  Suppose some l < left would qualify with a later right, i.e.
  sum(nums[l..right]) >= target. left only got past l because at some earlier
  index r <= right the window [l..r] already hit target, and at that moment the
  code recorded length r - l + 1. Since r <= right, that recorded length is <=
  right - l + 1. So the skipped candidate can never be shorter than something
  result already absorbed. Nothing is lost by discarding it, and because left
  only ever increments it advances at most nums.Length times across the whole
  run even though the while sits inside the for.
WHY THIS LOSES
  The prefix array is dead weight here. The window sum is only ever read as
  prefix[right+1] - prefix[left], and both endpoints move forward one step at a
  time, so a single running int would track it exactly: add nums[right] at the
  top of the for, subtract nums[left] just before left++. That is the same
  algorithm in one pass with O(1) extra space instead of a full n+1 array plus a
  separate build loop.

  A prefix array is the right data structure for the other solution to this
  problem - for each right, binary search the monotone prefix for the largest
  left with prefix[left] <= prefix[right+1] - target, giving O(n log n) time.
  This file pays that solution's memory cost while running the two-pointer scan
  that already beats it, so it gets no return on the array.
WATCH OUT
  - The recording happens before left++, inside the loop, not after it. Each
  pass records the current window and then shrinks, so every qualifying window
  ending at right is measured; Math.Min keeps the last and shortest. Moving the
  Math.Min after the while would measure a window that no longer qualifies.
  - int.MaxValue is the "never qualified" sentinel and must be mapped back to 0
  on return. Returning result raw is the classic wrong answer when no subarray
  reaches target.
  - prefix stays inside int only because n * max(nums) is about 1e5 * 1e4 = 1e9,
  just under int.MaxValue. Widen the constraint at all and prefix needs long.
  The running-sum version has the same ceiling but only over the live window, so
  it is harder to overflow.
  - Zeros are harmless (prefix stays non-decreasing), but a single negative
  value kills the shrink logic: the sum is no longer monotone in left, so
  dropping a prefix can raise the window sum back over target. That variant (LC
  862) needs a monotonic deque over the prefix array - which is where an
  explicit prefix array actually earns its space.
TRIGGER
  Reach for the two-pointer form when the ask is a shortest or longest
  contiguous run under a threshold AND the per-element contribution has one
  sign, so extending helps and shrinking hurts in a fixed direction. Keep only a
  running sum. Reach for a materialized prefix array instead when you need
  random access to arbitrary (l, r) pairs, an exact-sum count via a hash map, or
  a monotonic deque because signs are mixed.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
