public class Solution {
    public void MoveZeroes(int[] nums) {
        //my solution.
        int slow = 0;
        int fast = 0;

        while(nums[slow]!=0){
            slow++;
            if(slow >= nums.Length-1)
            return;
        }
        fast = slow;
        while(nums[fast]==0){
            fast++;
            if(fast >= nums.Length)
            return;
        }

        while(fast < nums.Length){
            if(nums[slow] == 0 && nums[fast] != 0){
                var temp = nums[slow];
                nums[slow] = nums[fast];
                nums[fast] = temp;
                slow++;
                fast++;
            }
            else
            fast++;
            
        }
    }
}