// --------------------------------------------------------------------------
// -  suboptimal-2.cs       O(n^2) time / O(n) space
// --------------------------------------------------------------------------

public class Solution
{
    public int LengthOfLIS(int[] nums)
    {
        int n = nums.Length;
        int[] dp = new int[n];
        Array.Fill(dp, 1); // Lowest Lis is 1

        int maximum = 1;
        for (int i = 0; i < n; i++)
        {
            for (int prev = 0; prev < i; prev++)
            {
                if (nums[prev] < nums[i])
                    dp[i] = Math.Max(dp[i], 1 + dp[prev]);
            }
            maximum = Math.Max(maximum, dp[i]);
        }
        return maximum;
    }

}

/*
================================================================================
 PROBLEM : Given an integer array nums, return the length of the longest
           strictly increasing subsequence. A subsequence keeps the original
           order but may skip elements. Equal values do not count as
           increasing. Example: [10,9,2,5,3,7,101,18] -> 4 (for example
           2,3,7,18).
 PATTERN : 1D Dynamic Programming (LIS ending at each index)
================================================================================
IDEA
  dp[i] is the length of the longest increasing subsequence that ends at i.
  Each dp[i] starts at 1, because nums[i] alone is a subsequence. For each
  i, look at every earlier prev. If nums[prev] < nums[i], nums[i] can extend
  that chain, so dp[i] = max(dp[i], 1 + dp[prev]). The answer is the largest
  dp value, kept in maximum. This is correct because every increasing
  subsequence ends at some i, and its second-to-last element is some prev.
  optimal.cs instead keeps sorted tails and uses binary search.
EXAMPLE
  nums = [3,1,4,4,2,5]
  dp: i0=1, i1=1 (no smaller before), i2=2 (from 3 or 1), i3=2 (the earlier
  4 is not < 4), i4=2 (from 1), i5=3 (from any dp of 2)
  Answer: maximum = 3 (for example 1,4,5 or 1,2,5)
COMPLEXITY
  Time  O(n^2)  the double loop checks every (prev, i) pair, about n^2/2
                checks
  Space O(n)    one dp array of length n
WATCH OUT
  - Return max over all dp, not dp[n-1]. The LIS may end early:
    for [1,2,0], dp[2] is 1 but the answer is 2.
  - Use strict <. With <=, the input [4,4] would wrongly give 2.
  - For an empty nums, maximum starts at 1, so this code returns 1, not 0.
  - Initialize dp to 1, not 0. Otherwise single elements never count.
================================================================================
*/
