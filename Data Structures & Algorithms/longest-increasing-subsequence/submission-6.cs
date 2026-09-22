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