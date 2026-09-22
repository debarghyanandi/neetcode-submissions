public class Solution
{
    const int INF = 1_000_000_000;

    public int CoinChange(int[] coins, int amount)
    {
        //space optimization
        int n = coins.Length;

        int[] prev = new int[amount + 1];
        int[] curr = new int [amount + 1];

        // Base case: using only coins[0]
        for (int target = 0; target <= amount; target++)
        {
            prev[target] =
                target % coins[0] == 0
                    ? target / coins[0]
                    : INF;
        }

        for (int i = 1; i < n; i++)
        {
            for (int target = 0; target <= amount; target++)
            {
                int notTake = prev[target];

                int take = INF;

                if (target >= coins[i])
                {
                    take = 1 + curr[target - coins[i]];
                }

                curr[target] = Math.Min(notTake, take);
            }
            prev = curr;
        }

        int ans = prev[amount];

        return ans >= INF ? -1 : ans;
    }
}