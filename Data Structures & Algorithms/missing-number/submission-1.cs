public class Solution {
    public int MissingNumber(int[] nums) {
        int n = nums.Length;
        int xorr = n;
        for (int i = 0; i < n; i++) {
            xorr ^= i ^ nums[i];
        }
        return xorr;
    }
}