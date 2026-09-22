public class Solution {
    public int LengthOfLIS(int[] nums) {
        //My solution
        //Memoization
        int n = nums.Length;
        int [,] dp = new int [n,n+1];
        //i ---> currentIndex
        //j ---> prevIndex
        for(int i = 0; i < n; i++){
            for(int j = 0; j <= n; j++){
                dp[i, j] = -1;
            }
        }
        
        return F(n - 1, n, nums, dp);
    }

    private int F(int i, int prevIndex, int[] nums, int [,] dp)
    {
        if (i < 0)
        return 0;

        if(dp[i, prevIndex] != -1)
        return dp[i, prevIndex];
        // Recurrence:
        // pick    = 1 + F(i - 1, nums, nums[i])   if nums[i] < prev
        // notPick = F(i - 1, nums, prev)
        // answer  = Math.Max(pick, notPick)

        int pick = 0;
        if(prevIndex == nums.Length || nums[i] < nums[prevIndex])
        {
            pick = 1 + F(i - 1, i, nums, dp);
        }
        
        int notPick = F(i - 1, prevIndex, nums, dp);

        return dp[i, prevIndex] = Math.Max(pick, notPick);
    }
}
