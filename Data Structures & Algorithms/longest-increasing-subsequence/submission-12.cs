public class Solution {
    public int LengthOfLIS(int[] nums) {
        List<int> dp = new List<int>();
        dp.Add(nums[0]);

        int LIS = 1;
        for (int i = 1; i < nums.Length; i++) {
            if (dp[dp.Count - 1] < nums[i]) {
                dp.Add(nums[i]);
                LIS++;
                continue;
            }

            int idx = dp.BinarySearch(nums[i]);
            if (idx < 0) idx = ~idx;
            dp[idx] = nums[i];
        }

        return LIS;
    }
}