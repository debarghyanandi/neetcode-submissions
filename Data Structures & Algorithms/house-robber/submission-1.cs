public class Solution {
    public int Rob(int[] nums) {
        //Dp space Optimized
        int n = nums.Length;

        if(n == 1)
        return nums[0];

        int prev2 = nums[0];
        int prev = Math.Max(nums[0], nums[1]);
        int curr = 0;

        for(int i = 2; i < n; i++){
            
            int pick = nums[i] + prev2;
            int notPick = 0 + prev;
            curr = Math.Max(pick, notPick);

            prev2 = prev;
            prev = curr;

        }
        return prev;

    }
}