public class Solution {
    public int[] ProductExceptSelf(int[] nums) {
        var res = new int[nums.Length];
        Array.Fill(res, 1);

        int pre = 1, post = 1;

        for (int i = 0; i < nums.Length; i++){
            res[i] = pre;
            pre = nums[i]*pre;
        }

        for (int i = nums.Length-1; i >= 0; i--){
            res[i] = res[i]*post;
            post = nums[i]*post;
        }
        return res;
    }
}
