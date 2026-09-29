// --------------------------------------------------------------------------
// -  suboptimal-3.cs       O(n^2) time / O(n^2) space
// --------------------------------------------------------------------------

public class Solution
{
    public int LengthOfLIS(int[] nums)
    {
        int n = nums.Length;

        int[,] dp = new int[n, n + 1];

        for (int i = 0; i < n; i++)
        {
            for (int prevIndex = 0; prevIndex <= n; prevIndex++)
            {
                dp[i, prevIndex] = -1;
            }
        }

        return F(0, -1, nums, dp);
    }

    private int F(int i, int prevIndex, int[] nums, int[,] dp)
    {
        if (i == nums.Length)
            return 0;

        if (dp[i, prevIndex + 1] != -1)
            return dp[i, prevIndex + 1];

        // Recurrence:
        // notPick = F(i + 1, prevIndex)
        // pick    = 1 + F(i + 1, i)
        //           only if prevIndex == -1 || nums[i] > nums[prevIndex]
        // answer  = max(pick, notPick)

        int notPick = F(i + 1, prevIndex, nums, dp);

        int pick = 0;

        if (prevIndex == -1 || nums[i] > nums[prevIndex])
        {
            pick = 1 + F(i + 1, i, nums, dp);
        }

        return dp[i, prevIndex + 1] = Math.Max(pick, notPick);
    }
}

/*
================================================================================
 PROBLEM : Given an int array nums, return the length of the longest strictly
           increasing subsequence. A subsequence keeps the original order but
           may skip elements. Equal values do not count as increasing.
           Example: [10,9,2,5,3,7,101,18] -> 4 (for example 2,3,7,101).
 PATTERN : DP pick / not-pick (top-down memoization)
================================================================================
IDEA
  F(i, prevIndex) is the best length we can still add from index i onward,
  when the last picked element is nums[prevIndex] (-1 means nothing picked).
  At each i we either skip it (notPick) or take it (pick), and we may take it
  only if nums[i] > nums[prevIndex]. Trying both choices covers every
  subsequence, so the max is correct. dp[i, prevIndex + 1] caches each state.
  The column is shifted by 1 so that prevIndex = -1 fits.
EXAMPLE
  nums = [3,1,2]. F(0,-1): pick 3 -> F(1,0): 1 and 2 are not > 3, so 0;
  pick=1.
  notPick -> F(1,-1): pick 1 -> F(2,1): 2 > 1 so 1; pick=2. notPick gives 1.
  F(1,-1)=2, so F(0,-1) = max(1, 2) = 2 (subsequence 1,2).
COMPLEXITY
  Time  O(n^2)  n*(n+1) states (i, prevIndex), each solved once with O(1) work
  Space O(n^2)  the n x (n+1) dp table, plus a recursion stack up to n deep
WATCH OUT
  - Forgetting the +1 shift: dp[i, prevIndex] with prevIndex = -1 throws
    IndexOutOfRangeException.
  - Using >= instead of > counts duplicates: [2,2] must return 1, not 2.
  - Recursion depth reaches n, so a very long array can overflow the stack.
    The bottom-up loops in the other files avoid this.
  - The -1 sentinel works only because every real answer is >= 0. Do not
    reuse this sentinel for DP values that can be negative.
================================================================================
*/
