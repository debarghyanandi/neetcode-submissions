public class Solution {
    public int LengthOfLIS(int[] nums) {
        int n = nums.Length;
        int [] dp = new int[n];
        Array.Fill(dp, 1); // Lowest Lis is 1

        int maximum = 1;
        for(int i = 0; i < n; i++){
            for(int prev = 0; prev < i; prev++){
                if(nums[prev] < nums[i])
                    dp[i] = Math.Max(dp[i], 1 + dp[prev]);
            }
            maximum = Math.Max(maximum, dp[i]);
        }
        return maximum ;
    }
    
}
