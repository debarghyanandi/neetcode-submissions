public class Solution {
    public int Jump(int[] nums) {

        //Same TC/Sc as the other sol.
        //Clean code but Intuitive.  

        int n = nums.Length;
        int jumps = 0;

        int currEnd = 0;
        int farthest = 0;
        
        for (int i = 0; i < n - 1; i++)
        {
            farthest = Math.Max(farthest, i + nums[i]);

            if (i == currEnd)
            {
                jumps++;
                currEnd = farthest;
            }
        }
        return jumps;
    }
}
