// --------------------------------------------------------------------------
// -  suboptimal.cs         O(n) time / O(n) space
// --------------------------------------------------------------------------

public class Solution
{
    public int Rob(int[] nums)
    {

        int n = nums.Length;

        if (n == 1)
            return nums[0];

        int[] dp = new int[n + 1];

        //Recurrence relation
        //pick = f(indx) + f(indx - 2);
        //notPick = 0 + f(index - 1);
        dp[0] = nums[0];
        dp[1] = Math.Max(nums[0], nums[1]);

        for (int i = 2; i < n; i++)
        {
            int pick = nums[i] + dp[i - 2];
            int notPick = 0 + dp[i - 1];
            dp[i] = Math.Max(pick, notPick);
        }
        return dp[n - 1];
    }
}

/*
================================================================================
 PROBLEM : Houses stand in a row and nums[i] is the money in house i. You
           cannot rob two adjacent houses, or the alarm goes off. Return the
           largest total you can rob. Example: [1,2,3,1] -> 4 (rob houses 0
           and 2).
 PATTERN : 1D Dynamic Programming (take / skip, bottom-up)
================================================================================
IDEA
  dp[i] is the best total using only houses 0..i. At house i you either
  rob it (pick = nums[i] + dp[i - 2]) or skip it (notPick = dp[i - 1]).
  dp[i] keeps the larger. This is correct because any best plan for 0..i
  either ends with house i robbed (so i-1 is not) or leaves house i alone.
  optimal.cs keeps only the last two dp values, not the whole array.
EXAMPLE
  nums = [2,7,9,3,1]; dp[0]=2, dp[1]=max(2,7)=7
  i=2: pick 9+2=11, skip 7 -> 11; i=3: pick 3+7=10, skip 11 -> 11
  i=4: pick 1+11=12, skip 11 -> 12; answer dp[4] = 12 (houses 0,2,4)
COMPLEXITY
  Time  O(n)  one loop, each index i does O(1) work
  Space O(n)  the dp array holds one value per house
WATCH OUT
  - dp[1] must be max(nums[0], nums[1]), not nums[1]. With [5,1,1] the
    wrong base gives 5+1=6 at i=2 instead of the right answer 6 vs 5 logic.
  - An empty nums throws: n == 1 is the only guard, so dp[0] = nums[0] fails.
  - The comment says pick = f(indx) + f(indx - 2); the code correctly uses
    nums[i], not dp[i]. Say "value of house i" when explaining it.
  - dp is sized n + 1 but dp[n] is never used; return dp[n - 1], not dp[n].
================================================================================
*/
