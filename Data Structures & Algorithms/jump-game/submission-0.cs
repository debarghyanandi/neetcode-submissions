public class Solution {
    public bool CanJump(int[] nums) {
        //Greedy
        int n = nums.Length;
        int maxLength = 0;
        
        for(int i = 0; i < n; i++){
            
            // you cant reached this index. MaxLength is less.
            if(i > maxLength)
                return false;
              
            maxLength = Math.Max(maxLength, i + nums[i]);
        }
        return true;
    }
}
