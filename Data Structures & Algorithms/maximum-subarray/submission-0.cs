public class Solution {
    public int MaxSubArray(int[] nums) {
        int maxSum = nums[0];
        int currentSum = 0;
        foreach (int num in nums){
            if (currentSum < 0)
                currentSum = 0;
            currentSum = currentSum + num;
            maxSum = Math.Max(currentSum, maxSum);
        }
        return maxSum;
    }
}
