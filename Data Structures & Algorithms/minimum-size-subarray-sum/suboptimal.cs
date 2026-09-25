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
 PATTERN : Sliding Window over a prefix-sum array - shrink from the left
 SOURCE  : YOUR OWN SOLUTION - your own annotation at c76939d
 STATUS  : Suboptimal
================================================================================
VARIABLES
  prefix   prefix[i] = sum of nums[0..i-1], so sum of nums[left..right] = prefix[right+1] - prefix[left]
  left     start of the current window; only ever moves forward
  right    end of the current window, driven by the outer loop
  result   shortest qualifying window length seen so far, int.MaxValue = none found yet
WHY THIS PATTERN
  The task asks for the shortest contiguous run whose sum reaches target. With
  non-negative numbers, extending the window at right can only raise the sum and
  moving left forward can only lower it, so the sum is monotonic in both ends.
  That lets one pass with two pointers find, for each right, the largest left
  that still qualifies, instead of testing every pair. The prefix array is only
  there to read any window sum in constant time.
BETTER APPROACH
  The better version of this same idea keeps one running int windowSum: add
  nums[right] on entry, subtract nums[left] before left++. That is the same time
  but O(1) extra space, so this file loses purely on the prefix array of
  nums.Length + 1 ints. The prefix array buys nothing here, because the window
  sum is always read at the current left and right and never at an arbitrary
  earlier index.
INVARIANT
  At the top of each outer iteration, left is the smallest index such that
  prefix[right+1] - prefix[left] is still below target for the previous right,
  meaning every window ending earlier that qualified has already been measured
  into result. The while loop records the length before each left++, so for
  every right the shortest qualifying window ending at right is compared. Since
  left never moves backward, the total work across both loops is linear, and
  result ends as the global minimum.
MEASURING INSIDE THE SHRINK LOOP
  result is updated on every iteration of the while loop, not once after it.
  That is harmless but does extra Math.Min calls: only the last window before
  the loop exits is the shortest for this right. Moving the update to just after
  the while loop, using right - left + 1 with the new left, gives the same
  answer with one comparison per right.
WATCH OUT
  prefix is int[], so a long array of large values can overflow the running sum
  silently and make comparisons against target wrong; long[] prefix would fix
  it. The whole monotonic argument dies if nums may contain negative numbers -
  shrinking could then raise the sum, and left would skip valid windows. Also
  note left can reach right + 1 (when a single element alone reaches target);
  the difference prefix[right+1] - prefix[right+1] is 0, so the loop exits
  safely, but there is no guard if someone later assumes left <= right. Empty
  nums returns 0 through the sentinel branch, which is correct but only by
  accident of result never being set.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Drop the extra array entirely.
     Keep int windowSum, add nums[right] each outer step and subtract nums[left]
     inside the shrink loop; same linear time, constant space, and no overflow
     risk beyond the single running sum.
  2. What if nums can hold negative values?
     Two pointers no longer work. Build the prefix array (now it earns its
     keep), and for each right find the largest left with prefix[left] <=
     prefix[right+1] - target using a monotonic increasing deque of indices,
     still linear; a simpler O(n log n) option is a sorted structure over prefix
     values.
  3. Return the subarray itself, not its length.
     Store bestLeft and bestRight whenever result is improved, then slice nums
     with that range; cost is two extra ints.
  4. Longest subarray with sum at most target instead?
     Same two pointers, but shrink while the sum exceeds target and record right
     - left + 1 after the shrink, taking Math.Max; the monotonicity requirement
     on non-negative values is unchanged.
TRIGGER
  Shortest or longest contiguous run meeting a sum threshold, with all values
  non-negative, so growing and shrinking the window move the sum in opposite
  directions.
C# NOTE
  new int[nums.Length + 1] is zero-filled by the runtime, which is why prefix[0]
  is never assigned explicitly - worth remembering if the code is ported to a
  language without that guarantee. The int.MaxValue sentinel plus the final
  ternary avoids a nullable int and keeps Math.Min working on plain ints.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
