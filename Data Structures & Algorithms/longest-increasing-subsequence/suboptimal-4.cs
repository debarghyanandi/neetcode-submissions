// --------------------------------------------------------------------------
// -  suboptimal-4.cs       O(n^2) time / O(n^2) space
// --------------------------------------------------------------------------

public class Solution
{
    public int LengthOfLIS(int[] nums)
    {
        int n = nums.Length;

        // +1 shift for prevIndex
        // prevIndex = -1 -> column 0
        // prevIndex =  0 -> column 1
        // prevIndex =  1 -> column 2
        int[,] dp = new int[n + 1, n + 1];

        // Base case:
        // dp[n, *] = 0
        // Already 0 by default in C#

        for (int i = n - 1; i >= 0; i--)
        {
            for (int prevIndex = i - 1; prevIndex >= -1; prevIndex--)
            {
                // Not pick current
                int notPick = dp[i + 1, prevIndex + 1];

                int pick = 0;

                // Pick current
                if (prevIndex == -1 || nums[i] > nums[prevIndex])
                {
                    pick = 1 + dp[i + 1, i + 1];
                }

                dp[i, prevIndex + 1] = Math.Max(pick, notPick);
            }
        }

        return dp[0, 0];
    }
}

/*
================================================================================
 PROBLEM : Given an integer array nums, return the length of the longest
           strictly increasing subsequence. A subsequence keeps the original
           order but may skip elements. Example: [10,9,2,5,3,7,101,18] -> 4
           (for example 2,3,7,18).
 PATTERN : Bottom-up DP (take / skip with previous index)
================================================================================
IDEA
  dp[i, prevIndex + 1] is the best LIS length using nums[i..n-1], when the
  last element we took is at prevIndex (-1 means nothing taken yet). At each
  i we either skip (notPick) or take nums[i] if it is bigger than
  nums[prevIndex] (pick), and keep the max. Rows are filled from i = n-1 down,
  so row i+1 is always ready. This is the memoized recursion turned into a
  table; optimal.cs uses binary search on tails instead.
EXAMPLE
  nums = [3,1,2] (the greedy first pick 3 is a trap)
  i=2: dp[2,0]=1, dp[2,1]=0 (2>3 fails), dp[2,2]=1
  i=1: dp[1,1]=0 (1>3 fails); dp[1,0]=max(1+dp[2,2]=2, dp[2,0]=1)=2
  i=0: dp[0,0]=max(pick 1+dp[1,1]=1, skip dp[1,0]=2) = 2 -> answer 2
COMPLEXITY
  Time  O(n^2)  each (i, prevIndex) pair with prevIndex < i is filled once in
                O(1)
  Space O(n^2)  the full (n+1) x (n+1) dp table
WATCH OUT
  - Forgetting the +1 column shift: prevIndex = -1 would index column -1.
    Every read and write must use prevIndex + 1, and pick uses dp[i+1, i+1].
  - Use > not >=. With >=, [2,2] would wrongly return 2 instead of 1.
  - Loop order matters: i must go from n-1 down, because row i reads row i+1.
  - Only row i+1 is ever read, so two 1D rows are enough (O(n) space).
================================================================================
*/
