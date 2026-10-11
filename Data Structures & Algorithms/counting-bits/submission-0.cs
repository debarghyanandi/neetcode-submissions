public class Solution {
    public int[] CountBits(int n) {
        int [] dp = new int [n + 1];
        
        //Base case.
        dp[0] = 0;

        for (int i = 1; i <= n; i++){
            //i >> 1 creates a 1 right shift
            //so the ans is already built.
            //as we remove the last digit we need to add that back
            //i & 1 return 1 if last digit was 1 (odd)
            //and return 0 is the last digit was 0(even)
            dp[i] = dp[i >> 1] + (i & 1);
        }
        return dp;
    }
}
