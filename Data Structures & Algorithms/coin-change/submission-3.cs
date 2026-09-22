public class Solution
{
    const int INF = 1_000_000_000;

    public int CoinChange(int[] coins, int amount)
    {
        int n = coins.Length;

        int[,] dp = new int[n, amount + 1];

        // Base case: using only coins[0]
        for (int target = 0; target <= amount; target++)
        {
            dp[0, target] =
                target % coins[0] == 0
                    ? target / coins[0]
                    : INF;
        }

        for (int i = 1; i < n; i++)
        {
            for (int target = 0; target <= amount; target++)
            {
                int notTake = dp[i - 1, target];

                int take = INF;

                if (target >= coins[i])
                {
                    take = 1 + dp[i, target - coins[i]];
                }

                dp[i, target] = Math.Min(notTake, take);
            }
        }

        int ans = dp[n - 1, amount];

        return ans >= INF ? -1 : ans;
    }
}