public class Solution {
    public int Search(int[] nums, int target) 
    {
        //My Solution
        return Search(0, nums.Length-1 , target, nums);
    }
    
    private int Search(int l, int r, int t, int[] nums)
    {
        if(l > r)
            return -1;
        
        int mid = l + (r - l) / 2;
        
        if(nums[mid] == t)
            return mid;
        
        if (t < nums[mid])
            return Search(0, mid-1, t, nums);
        
        return Search(mid + 1, r, t, nums);
    }
}
