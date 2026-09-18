public class Solution {
    public int Rob(int[] nums) {
        int n = nums.Length;

        if(n == 1)
        return nums[0];

        if(n == 2)
        return Math.Max(nums[0], nums[1]);

        int first = RobLinear(nums, 0, n - 2);
        int second = RobLinear(nums, 1, n - 1);

        return Math.Max(first, second);
    }
    
    
    private int RobLinear(int[] nums, int start, int end) {
        //Dp space Optimized

        int prev2 = nums[start];
        int prev = Math.Max(nums[start], nums[start + 1]);
        int curr = start;

        for(int i = start + 2; i <= end; i++){
            
            int pick = nums[i] + prev2;
            int notPick = 0 + prev;
            curr = Math.Max(pick, notPick);

            prev2 = prev;
            prev = curr;

        }
        return prev;

    }
}
