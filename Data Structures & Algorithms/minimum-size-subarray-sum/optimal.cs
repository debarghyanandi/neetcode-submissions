// ##########################################################################
// #  optimal.cs            O(n) time / O(1) space
// ##########################################################################

public class Solution
{
    public int MinSubArrayLen(int target, int[] nums)
    {
        int sum = 0;
        int left = 0;
        int result = int.MaxValue;

        for (int right = 0; right < nums.Length; right++)
        {
            sum = sum + nums[right];   // admit the new right-hand element

            // Shrink while the window still qualifies - the first failure
            // means every shorter window ending here fails too.
            while (sum >= target)
            {
                result = Math.Min(result, right - left + 1);
                sum = sum - nums[left];  // evict, THEN move the edge
                left++;
            }
        }

        return result == int.MaxValue ? 0 : result;
    }
}

/*
================================================================================
 PROBLEM : Given an array of positive integers nums and an integer target,
           return the length of the shortest contiguous subarray whose sum is
           >= target. Return 0 if no such subarray exists. Example: target=7,
           nums=[2,3,1,2,4,3] -> 2 (subarray [4,3]).
 PATTERN : Sliding Window (variable size, shrink while valid)
================================================================================
IDEA
  Move right across nums and add each value to sum. While sum >= target,
  the window [left..right] is valid: record right-left+1 in result, then drop
  nums[left] and move left forward. All values are positive, so shrinking can
  only lower sum. Once it fails, every shorter window ending at right fails
  too.
EXAMPLE
  target=7, nums=[2,3,1,2,4,3]
  r=3 sum=8: result=4, drop 2 -> left=1 | r=4 sum=10: result=4, drop 3 ->
  sum=7: result=3, left=3 | r=5 sum=9: result=3, drop 2 -> sum=7: result=2
  Answer: 2 (window [4,3], left=4..right=5)
COMPLEXITY
  Time  O(n)  right moves n times, left moves at most n times in total
              (amortized)
  Space O(1)  only sum, left, right and result
PATH TO OPTIMAL
  Brute force: try every start and extend - O(n^2) - simple, no extra memory.
  Prefix sums + binary search per start - O(n log n) - no inner linear scan.
  Prefix sum array + two pointers - O(n)/O(n) - linear time (suboptimal.cs).
  Running sum window - O(n)/O(1) - no prefix array needed (this file).
KEYWORDS
  sliding window, two pointers, variable window, minimum length, prefix sum
WATCH OUT
  - Using "if" instead of "while" shrinks only one step per right, so it
    misses shorter windows, e.g. [1,1,1,7] with target 7 must give 1.
  - Negative numbers or zeros break the "shrinking lowers sum" logic.
  - Forgetting to map int.MaxValue to 0 returns a huge number when no
    window reaches target.
  - Update result BEFORE evicting nums[left]; the reverse order is off by one.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. What if nums can contain negative numbers?
     -> The window is no longer monotonic. Use prefix sums with a monotonic
        deque of increasing prefix values (LeetCode 862), O(n) time and O(n)
        space.
  2. Why is this O(n) with a nested while loop?
     -> left only moves forward and never passes n. So the inner loop runs at
        most n times over the whole run. That is amortized O(1) per right.
  3. Can you do it in O(n log n) instead, and why would you?
     -> Build prefix sums, then binary search for each start the first end
        with prefix[end]-prefix[start] >= target. It is slower, but useful as a
        step.
  4. What if the data arrives as a stream?
     -> Keep a queue of the window's values and the running sum, and evict
        from the front while it is valid. Memory is the current window size.
TRIGGER
  Positive numbers, a contiguous subarray, and "shortest/longest with sum >=
  K"
  means a variable sliding window that grows at right and shrinks at left.
================================================================================
*/
