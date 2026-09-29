// --------------------------------------------------------------------------
// -  suboptimal.cs         O(n) time / O(n) space
// -  Dynamic programming tabulation   [dp-tabulation]
// -  ranks below optimal.cs (O(n) time / O(1) space)
// -
// -  Reference solution - not one you solved yourself
// -
// -  Fills a 1-D DP table in a single pass, storing the maximum value
// -  robbed at each house index.
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
 PATTERN : 1D Dynamic Programming - pick or skip each house
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Suboptimal
================================================================================
VARIABLES
  dp        dp[i] = most money you can rob from houses 0..i
  pick      nums[i] + dp[i - 2], the best total if you rob house i
  notPick   dp[i - 1], the best total if you skip house i
WHY THIS PATTERN
  You cannot rob two houses next to each other, and you want the largest total.
  So at each house there are only two choices: rob it or skip it. Each choice
  depends only on answers you already have for shorter streets. That is
  overlapping subproblems with optimal substructure, which means the best answer
  is built from best answers to smaller parts. So dp[i] can be filled from left
  to right using pick and notPick.
BETTER APPROACH
  A better version keeps only two numbers, the answer one house back and the
  answer two houses back, and rolls them forward. That uses O(1) extra space
  instead of this file's dp array. This file loses only on memory. The loop
  reads just dp[i - 1] and dp[i - 2], so the rest of the array is never used
  again.
INVARIANT
  When the loop starts step i, dp[0..i-1] holds the true best total for each
  prefix of houses. If the best plan for 0..i robs house i, it must skip house
  i-1, so its value is nums[i] + dp[i - 2]. If the plan skips house i, its value
  is dp[i - 1]. Taking the max of these two covers every legal plan, so dp[i] is
  correct, and dp[n - 1] is the answer.
BASE CASE DP[1] IS A MAX
  dp[1] is Math.Max(nums[0], nums[1]), not just nums[1]. With two houses you can
  rob only one, so you take the bigger one. If you set dp[1] = nums[1], a case
  like [5, 1] gives the wrong answer.
WATCH OUT
  An empty nums array throws an error: n == 1 is the only guard, so dp[0] =
  nums[0] runs with no element there. The comment "pick = f(indx) + f(indx - 2)"
  does not match the code. The code adds nums[i], the money in the house, not
  f(i), the best total so far. dp is sized n + 1, but index n is never used.
  That does not break anything, but it suggests an off-by-one mix-up between
  0-based and 1-based dp. The "0 +" in notPick does nothing and can be removed.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. What if the houses are in a circle, so the first and last are neighbors?
     Run this same logic twice: once on houses 0..n-2 and once on houses 1..n-1.
     Return the larger result. That is still linear time. Handle n == 1 on its
     own.
  2. What if the houses form a binary tree and you cannot rob a parent and its
  child together?
     Do a post-order DFS (children first, then the parent). Each node returns a
     pair: the best total with this node robbed and the best without it. Space
     becomes O(tree height) for the recursion stack.
  3. How would you return which houses were robbed, not just the total?
     Keep the full dp array, then walk back from n - 1. If dp[i] != dp[i - 1],
     house i was robbed, so jump to i - 2. Otherwise move to i - 1. This is one
     reason to keep the array instead of two rolling variables.
TRIGGER
  You make a yes/no choice for each item in a line, a choice blocks nearby
  items, and you want the best total.
C# NOTE
  When you roll two variables, tuple deconstruction does the shift in one line:
  (prev2, prev1) = (prev1, Math.Max(prev1, prev2 + nums[i])). You do not need a
  temp variable.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
