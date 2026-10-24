public class Solution {
    public void SortColors(int[] nums) {
        int i = 0; 
        int l = 0; int r = nums.Length - 1;

        while (i <= r)
        {
            if (nums[i] == 0)
            {
                Swap(nums, i, l);
                l++;
            }
            if (nums[i] == 2)
            {
                Swap(nums, i, r);
                r--;
                i--;
            }
            i++;
        }
    }

    private  void  Swap(int [] nums, int i, int j){
        int temp = nums [i];
        nums[i] = nums[j];
        nums[j] = temp;
    }
}