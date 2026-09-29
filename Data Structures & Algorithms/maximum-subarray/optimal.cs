// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
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
 PROBLEM : Given an integer array nums, find the contiguous subarray (at least
           one element) with the largest sum and return that sum, not the
           indices. Numbers can be negative, so the answer can be negative
           too. Example: [-2,1,-3,4,-1,2,1,-5,4] -> 6 (subarray [4,-1,2,1]).
 PATTERN : Kadane's Algorithm (greedy running sum + running max)
================================================================================
IDEA
  Walk once through nums and keep currentSum, the best sum of a subarray that
  ends at the current number. If currentSum is negative, reset it to 0 so a
  fresh subarray starts here. Then add number and update maxSum.
  It is correct because a negative prefix only lowers every sum that follows.
  So the best subarray ending at i either extends the one ending at i-1 or
  starts at i.
EXAMPLE
  nums = [-2,1,-3,4,-1,2,1,-5,4], start maxSum=-2, currentSum=0
  cur: -2, (reset) 1, -2, (reset) 4, 3, 5, 6, 1, 5
  maxSum: -2, 1, 1, 4, 4, 5, 6, 6, 6 -> answer 6
  All-negative [-3,-1]: cur -3, (reset) -1 -> answer -1
COMPLEXITY
  Time  O(n)  one pass, constant work per number
  Space O(1)  only two int variables, currentSum and maxSum
PATH TO OPTIMAL
  Try every (i, j) and sum it - O(n^3) - the obvious starting point.
  Keep a running sum per start i - O(n^2) - no re-summing each range.
  DP array: dp[i] = best sum ending at i - O(n) time, O(n) space
  (suboptimal.cs).
  Kadane: only dp[i-1] is ever read, so one variable - O(1) space (this file).
KEYWORDS
  Kadane's algorithm, maximum subarray sum, dynamic programming, greedy,
  contiguous subarray, running sum
WATCH OUT
  - Starting maxSum at 0 returns 0 for all-negative input. Use nums[0].
  - An empty nums throws IndexOutOfRangeException at nums[0]. Ask about it.
  - Reset before you add, not after, or you drop a negative single answer.
  - currentSum is an int, so very large values can overflow; use long if so.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Return the start and end indices too.
     -> Save a temp start when currentSum resets. When maxSum improves, copy
        the temp start and set end = i. Still O(n) time, O(1) space.
  2. What if the array is circular?
     -> Answer is max(normal Kadane, total - minimum subarray). If all numbers
        are negative, return normal Kadane. O(n) time, O(1) space.
  3. Maximum product subarray instead of sum?
     -> Keep both the max and min product ending at i, because a negative
        number swaps them. O(n) time, O(1) space.
  4. Can you do it with divide and conquer?
     -> Best is in the left half, the right half, or crosses the middle. O(n
        log n) time, O(log n) stack. Slower, but it parallelizes and fits segment
        trees.
TRIGGER
  The problem asks for the best contiguous subarray, and each position only
  needs
  "extend the previous run or start fresh here".
================================================================================
*/
