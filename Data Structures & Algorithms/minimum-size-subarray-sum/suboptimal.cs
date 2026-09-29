// ##########################################################################
// #  suboptimal.cs         O(n) time / O(n) space
// #  sliding window with prefix sum   [sliding-window-prefix-sum]
// #  ranks below optimal.cs (O(n) time / O(1) space)
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  Prefix array precomputation uses O(n) space to enable O(1) range sum
// #  queries, unlike the running sum approach.
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
 PATTERN : Sliding Window over Prefix Sums - shrink while sum >= target
 SOURCE  : YOUR OWN SOLUTION - your own annotation at c76939d
 STATUS  : Suboptimal
================================================================================
VARIABLES
  prefix   prefix[i] = sum of nums[0..i-1], so sum of nums[l..r] = prefix[r+1] - prefix[l]
  left     start index of the current window
  result   shortest qualifying window length seen so far; int.MaxValue means none found
WHY THIS PATTERN
  The problem asks for the shortest contiguous subarray whose sum is at least
  target. With positive numbers, a longer window never has a smaller sum. So
  when the window nums[left..right] qualifies, we can move left forward to look
  for a shorter answer. We never need to move left back. Each index enters the
  window once through right and leaves once through left. The prefix array gives
  each window sum as prefix[right + 1] - prefix[left].
BETTER APPROACH
  The better approach is the same window with a single running int sum: add
  nums[right] when the window grows, and subtract nums[left] before left++. That
  uses O(1) extra space. This file loses because it builds the whole prefix
  array of size nums.Length + 1. The window only ever needs the sum of its
  current range, so the array adds memory and gives nothing back. (A prefix
  array only pays off in the O(n log n) binary-search version, which this code
  does not use.)
INVARIANT
  When the while loop exits for a given right, prefix[right + 1] - prefix[left]
  < target. This means no window that ends at right and starts at left or later
  qualifies. Every left we skipped past was recorded in result before left++.
  That was already the shortest window for that start, because a later right
  would only make it longer. So every useful (left, right) pair is checked, and
  result ends as the true minimum.
WATCH OUT
  If target <= 0, the while condition stays true even when left passes right + 1
  (the window sum becomes 0 or negative but is still >= target). left keeps
  growing until prefix[left] reads past the end of the array and throws
  IndexOutOfRangeException. The window logic also needs every nums value to be
  positive. With negative values, moving left forward can raise the sum, so
  skipped windows might have qualified and the answer can be wrong. prefix is an
  int[], so large totals can overflow and wrap to negative numbers. Then the
  window-sum checks give wrong results.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. What if nums can contain negative numbers?
     The two-pointer idea breaks. Keep the prefix array and add a monotonic
     deque of indexes. A monotonic deque is a double-ended queue whose prefix
     values always increase from front to back. Pop from the front while
     prefix[right+1] - prefix[front] >= target, and pop from the back while
     prefix[back] >= prefix[right+1]. This is still O(n) time, and now the O(n)
     prefix array is really needed.
  2. Can you use the prefix array you already built in another way?
     Yes. The prefix values only increase, so for each start l you can binary
     search for the smallest r with prefix[r] >= prefix[l] + target. This is O(n
     log n), slower than the window, but each start is handled on its own.
  3. What if the numbers arrive as a stream and you cannot store them?
     Use the running-sum window, but you still have to keep the values inside
     the current window so you can subtract them. Keep them in a queue. Memory
     then grows with the longest window, not with the whole input.
TRIGGER
  The problem asks for the shortest or longest contiguous subarray under a sum
  limit, and all values are non-negative, so the window sum only goes up as the
  window grows.
C# NOTE
  C# int arithmetic is unchecked by default, so an overflow in prefix[i + 1] =
  prefix[i] + nums[i] wraps around without any error. Put that line in
  checked(...) while testing, or make prefix a long[], so an overflow fails with
  an error or cannot happen.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
