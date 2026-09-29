// ##########################################################################
// #  suboptimal.cs         O(n) time / O(n) space
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
 PROBLEM : Given a positive integer target and an array of positive integers
           nums, return the length of the shortest contiguous subarray whose
           sum is >= target. Return 0 if no such subarray exists. Example:
           target = 7, nums = [2,3,1,2,4,3] -> 2 (the subarray [4,3]).
 PATTERN : Sliding Window (variable size) + prefix sum
================================================================================
IDEA
  First build prefix, where prefix[i] is the sum of the first i numbers.
  Then the window sum nums[left..right] is prefix[right+1] - prefix[left].
  Move right forward one step at a time. While the window sum is >= target,
  record right - left + 1 in result and move left forward.
  All numbers are positive, so moving left can only lower the sum. Each
  shortest valid window is therefore seen before left passes it. Unlike
  optimal.cs, this code reads sums from the prefix array instead of a
  running total.
EXAMPLE
  target=7, nums=[2,3,1,2,4,3], prefix=[0,2,5,6,8,12,15]
  r=3: sum 8 -> result=4, left=1. r=4: sum 10 -> 4, then sum 7 -> 3, left=3
  r=5: sum 9 -> 3, then sum 7 -> result=2, left=5, sum 3 stops. Answer: 2
COMPLEXITY
  Time  O(n)  right and left each move at most n times, so the work is
              amortized O(1)
  Space O(n)  the prefix array holds n + 1 sums
WATCH OUT
  - If target <= 0, the while loop never stops. left passes right+1, and
    prefix[left] then goes out of range.
  - The sliding window needs positive numbers. With negative values,
    shrinking can raise the sum again, and the window misses answers.
  - prefix is int[], so large sums can overflow. Use long if values are big.
  - Do not forget the final check: return 0 when result is int.MaxValue.
================================================================================
*/
