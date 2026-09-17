public class Solution {
    public int Rob(int[] nums) {
        
        int n = nums.Length;
        
        if(n == 1)
        return nums[0];
  
        int [] dp = new int [n+1];
        
        //Recurrence relation
        //pick = f(indx) + f(indx - 2);
        //not pick = 0 + f(index - 1);
        dp[0] = nums[0];
        dp[1] = Math.Max(nums[0], nums[1]);

        for(int i = 2; i < n; i++){
            int pick = nums[i] + dp[i - 2];
            int notPick = 0 + dp[i-1];
            dp[i] = Math.Max(pick, notPick);
        }
        return dp[n-1];
    }
}





