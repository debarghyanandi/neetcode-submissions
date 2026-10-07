public class Solution {
    public int MissingNumber(int[] nums) {
        
        int len = nums.Length ;
        int total = (len * (len + 1)) / 2;
        
        foreach (int num in nums)
        {
            total = total - num;
        }

        return total;
    }
}
