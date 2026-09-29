// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
// --------------------------------------------------------------------------

public class Solution
{
    public int Rob(int[] nums)
    {
        //Dp space Optimized
        int n = nums.Length;

        if (n == 1)
            return nums[0];

        int prev2 = nums[0];
        int prev = Math.Max(nums[0], nums[1]);
        int curr = 0;

        for (int i = 2; i < n; i++)
        {
            int pick = nums[i] + prev2;
            int notPick = 0 + prev;
            curr = Math.Max(pick, notPick);

            prev2 = prev;
            prev = curr;
        }

        return prev;
    }
}

/*
================================================================================
 PROBLEM : Houses stand in a row. nums[i] is the money in house i. You cannot
           rob two adjacent houses, or the alarm goes off. Return the maximum
           money you can rob. Example: [2,7,9,3,1] -> 12 (rob 2, 9, 1).
 PATTERN : 1D Dynamic Programming (take / skip, rolling variables)
================================================================================
IDEA
  The best total up to house i is max(nums[i] + best up to i-2, best up to
  i-1). You need only the last two answers, so prev2 (up to i-2) and prev
  (up to i-1) replace the whole dp array. Each step computes pick and notPick
  and stores the larger in curr, then shifts: prev2 = prev, prev = curr. This
  is correct because the last house is either robbed (so i-1 is skipped) or
  not robbed, and both cases are covered.
EXAMPLE
  nums = [2,7,9,3,1]; start prev2=2, prev=max(2,7)=7
  i=2: pick=9+2=11, notPick=7 -> 11 | i=3: pick=3+7=10, notPick=11 -> 11
  i=4: pick=1+11=12, notPick=11 -> 12 (prev2=11, prev=12)
  Answer: 12
COMPLEXITY
  Time  O(n)  one pass over nums, O(1) work per index
  Space O(1)  only prev2, prev, curr are kept, no dp array
PATH TO OPTIMAL
  Brute recursion (rob or skip each house) - O(2^n) - tries all valid sets.
  Memoization on index - O(n)/O(n) - each subproblem is solved only once.
  Bottom-up dp array - O(n)/O(n) - no recursion stack (suboptimal.cs).
  Two rolling variables - O(n)/O(1) - dp[i] only reads dp[i-1] and dp[i-2].
KEYWORDS
  dynamic programming, house robber, no adjacent elements, take or skip,
  space optimization, rolling variables, max non-adjacent sum
WATCH OUT
  - Empty nums crashes: n==1 is checked, but n==0 still reads nums[0].
    Add "if (n == 0) return 0;" if an empty input can occur.
  - prev must be max(nums[0], nums[1]), not nums[1]: [5,1] needs 5.
  - Return prev, not curr: when n==2 the loop never runs and curr stays 0.
  - Shift in the right order: prev2 = prev first, then prev = curr.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Houses are in a circle (House Robber II)?
     -> First and last are adjacent. Run this code on nums[0..n-2] and on
        nums[1..n-1] and take the max. Still O(n) time, O(1) space.
  2. Houses form a binary tree (House Robber III)?
     -> DFS returns a pair (rob this node, skip this node) for each subtree.
        Parent robbed = val + both kids' skip. O(n) time, O(h) stack space.
  3. Also return which houses were robbed?
     -> Keep the full dp array, then walk back from the end. If dp[i] !=
        dp[i-1], house i was taken, so jump to i-2. This needs O(n) space.
  4. Why is greedy (always take the biggest house) wrong?
     -> [2,3,2]: greedy takes 3 and blocks both 2s, but 2+2=4 is better. DP
        compares both choices at every house, so it never gets stuck.
TRIGGER
  Maximize a sum over a sequence where picking one item bans its neighbor,
  so each state depends only on the previous one or two answers.
================================================================================
*/
