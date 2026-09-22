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