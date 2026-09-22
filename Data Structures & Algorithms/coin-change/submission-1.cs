public class Solution {
    const int INF = 1_000_000_000;
    
    public int CoinChange(int[] coins, int amount)
    {
        //My solution
        //Memoization
        int n = coins.Length;
        
        int [,] dp = new int [n, amount + 1];
        
        for (int i = 0; i < n; i++){
            for (int j = 0; j <= amount; j++){
                dp[i, j] = -1;
            }
        }
        int ans = f(n-1, amount, coins, dp);
        
        return ans >= INF ? -1 : ans;
    }

    private int f(int index, int amount, int [] coins, int[,] dp)
    {
        //base case
        if(index == 0)
            return amount % coins[0] == 0
                ? amount / coins[0]
                : INF;
        
        if(dp[index, amount] != -1)
            return dp[index, amount];
        
        int notTake = f(index-1, amount, coins, dp);
        int take = INF;
        
        if(amount >= coins[index]){
            take = 1 + f(index, amount - coins[index], coins, dp);
        }
        dp[index, amount] = Math.Min(notTake, take);
        return dp[index, amount];
    }
    
}
