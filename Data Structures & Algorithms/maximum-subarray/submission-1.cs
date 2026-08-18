public class Solution {
    public int MaxSubArray(int[] nums) {
        int[] dp = (int[])nums.Clone();
        for (int i = 1; i < nums.Length; i++) {
            dp[i] = Math.Max(nums[i], nums[i] + dp[i - 1]);
        }
        int maxSum = dp[0];
        foreach (int sum in dp) {
            maxSum = Math.Max(maxSum, sum);
        }
        return maxSum;
    }
}