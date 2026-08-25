public class Solution {
    public int MaxProfit(int[] prices) {
        //mY solution.
        //optimal O(n) but we are doing more steps.
        int left = 0;
        int maxProfit = 0;
        for(int right = 0; right < prices.Length; right++){
            while(prices[left]>prices[right]){
                left++;
            }
            maxProfit = Math.Max(maxProfit, prices[right]-prices[left]);
        }
        return maxProfit;
    }
}
