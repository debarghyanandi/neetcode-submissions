public class Solution {
    public int Search(int[] nums, int target) {
        // my solution
        int l = 0;
        int r = nums.Length - 1;
        while(l <= r)
        {
            int mid = l + (r-l)/2 ;
            if(target == nums[mid]){
                return mid;
            }
            //which part is sorted.
            if(nums[mid]  > nums[r]){
                //left is sorted
                if(nums[l] <= target && target < nums[mid])
                    r = mid - 1;
                else
                    l = mid + 1;
            }

            else{
            // right half is sorted
            if(nums[mid] < target && target <= nums[r])
                l = mid + 1;
            else
                r = mid - 1;
            }

        }
        return -1;
    }
}
