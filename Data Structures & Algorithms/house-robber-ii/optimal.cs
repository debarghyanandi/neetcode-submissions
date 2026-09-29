// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
// --------------------------------------------------------------------------

public class Solution
{
    public int Rob(int[] nums)
    {
        int n = nums.Length;

        if (n == 1)
            return nums[0];

        if (n == 2)
            return Math.Max(nums[0], nums[1]);

        int first = RobLinear(nums, 0, n - 2);
        int second = RobLinear(nums, 1, n - 1);

        return Math.Max(first, second);
    }


    private int RobLinear(int[] nums, int start, int end)
    {
        //Dp space Optimized

        int prev2 = nums[start];
        int prev = Math.Max(nums[start], nums[start + 1]);
        int curr = start;

        for (int i = start + 2; i <= end; i++)
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
 PROBLEM : Houses stand in a circle, so the first and last house are
           neighbors. nums[i] is the money in house i. You cannot rob two
           adjacent houses. Return the most money you can rob. Example:
           [2,3,2] -> 3.
 PATTERN : 1D DP (space optimized), run twice on two ranges
================================================================================
IDEA
  First and last house can never both be robbed, so split into two lines.
  RobLinear(nums, 0, n-2) skips the last house. RobLinear(nums, 1, n-1)
  skips the first. Each run keeps prev2 (best up to i-2) and prev (best up
  to i-1), then curr = max(nums[i] + prev2, prev). Every valid plan leaves
  out the first or the last house, so it is covered by one of the two runs.
EXAMPLE
  nums=[2,7,9,3,1]; the linear answer 2+9+1=12 is illegal (0 and 4 touch)
  first 0..3: prev2=2,prev=7 -> i=2: 11 -> i=3: max(3+7,11)=11
  second 1..4: prev2=7,prev=9 -> i=3: 10 -> i=4: max(1+9,10)=10
  answer = max(11,10) = 11
COMPLEXITY
  Time  O(n)  two linear passes over the array, O(1) work per index
  Space O(1)  only prev2, prev and curr, no dp array
PATH TO OPTIMAL
  Try every subset of houses - O(2^n) - correct but far too slow.
  Recursion with memo on (index, robbedFirst) - O(n) time, O(n) space.
  Two linear dp arrays - O(n)/O(n) - simpler than tracking the first-house
  flag.
  Two passes with rolling variables - O(n)/O(1) - this file, optimal.cs.
KEYWORDS
  dynamic programming, circular array, house robber, pick or skip, rolling
  variables
WATCH OUT
  - Robbing the whole array at once lets both ends in: [2,7,9,3,1] gives 12.
  - n==1 must return nums[0]: both ranges would be empty and start+1
    overflows.
  - An empty array throws on nums[0]; guard it if the interviewer allows n=0.
  - "int curr = start" stores an index, not money; it is unused, set it to 0.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Houses form a binary tree (House Robber III)?
     -> DFS returns a pair (rob this node, skip this node) for each subtree.
        O(n) time, O(h) stack space; the plain array DP no longer applies.
  2. Why is the two-range split correct?
     -> No valid plan uses both house 0 and house n-1, so every plan lies
        fully in [0, n-2] or in [1, n-1]. The best of the two runs is the answer.
  3. Can you also return which houses were robbed?
     -> Keep a full dp array and walk back from the end: a house was robbed if
        dp[i] came from the pick branch. This costs O(n) space.
TRIGGER
  A pick-or-skip DP with a no-adjacent rule on a circular array: break the
  circle by solving the same line problem twice, once without each end.
================================================================================
*/
