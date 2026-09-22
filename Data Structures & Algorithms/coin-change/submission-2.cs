public class Solution
{
    const int INF = 1_000_000_000;

    public int CoinChange(int[] coins, int amount)
    {
        int n = coins.Length;

        int[,] dp = new int[n, amount + 1];

        for (int i = 0; i < n; i++)
        {
            for (int target = 0; target <= amount; target++)
            {
                dp[i, target] = -1;
            }
        }

        int ans = F(n - 1, amount, coins, dp);

        return ans >= INF ? -1 : ans;
    }

    private int F(int i, int target, int[] coins, int[,] dp)
    {
        if (i == 0)
        {
            return target % coins[0] == 0
                ? target / coins[0]
                : INF;
        }

        if (dp[i, target] != -1)
            return dp[i, target];

        int notTake = F(i - 1, target, coins, dp);

        int take = INF;

        if (target >= coins[i])
        {
            take = 1 + F(i, target - coins[i], coins, dp);
        }

        return dp[i, target] = Math.Min(notTake, take);
    }
}