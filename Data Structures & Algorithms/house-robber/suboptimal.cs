// --------------------------------------------------------------------------
// -  suboptimal.cs         O(n) time / O(n) space
// -  Dynamic programming with DP table   [house-robber-dp]
// -  ranks below optimal.cs (O(n) time / O(1) space)
// -
// -  Reference solution - not one you solved yourself (from submission-0)
// -
// -  Single pass builds DP table where each entry stores the maximum money
// -  robable up to that house index.
// --------------------------------------------------------------------------

public class Solution {
    public int Rob(int[] nums) {
        
        int n = nums.Length;
        
        if(n == 1)
        return nums[0];
  
        int [] dp = new int [n+1];
        
        //Recurrence relation
        //pick = f(indx) + f(indx - 2);
        //not pick = 0 + f(index - 1);
        dp[0] = nums[0];
        dp[1] = Math.Max(nums[0], nums[1]);

        for(int i = 2; i < n; i++){
            int pick = nums[i] + dp[i - 2];
            int notPick = 0 + dp[i-1];
            dp[i] = Math.Max(pick, notPick);
        }
        return dp[n-1];
    }
}

/*
================================================================================
 PATTERN : Linear DP - rob or skip each house, keep best so far
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Suboptimal
================================================================================
WHY THIS PATTERN
  The problem asks for the largest sum you can take from nums when no two chosen
  items sit next to each other. That "no two adjacent" rule means the choice at
  index i depends only on what was already decided at i-1 and i-2, never on
  anything later. So a one-dimensional table works: dp[i] is the best total for
  the prefix ending at i, built from pick = nums[i] + dp[i-2] and notPick =
  dp[i-1].
BETTER APPROACH
  The better version drops the array. The loop only ever reads dp[i-1] and
  dp[i-2], so two int locals (say prev1 and prev2) carry everything needed;
  after computing the new value you shift prev2 = prev1, prev1 = cur. Same
  number of steps, but O(1) extra space instead of an n+1 element allocation.
  This file loses purely on memory - it keeps the whole history when only the
  last two entries are ever read again.
INVARIANT
  At the top of each iteration, dp[i-1] and dp[i-2] are the correct best totals
  for the prefixes ending exactly at those indices, under the no-adjacent rule.
  Given that, the only two legal ways to finish at i are take nums[i] (which
  forbids i-1, so add dp[i-2]) or skip it (carry dp[i-1] forward), and Math.Max
  picks the larger. Induction from the two base cases makes dp[n-1] the answer
  for the whole array.
WHY THERE ARE TWO BASE CASES
  dp[1] is Math.Max(nums[0], nums[1]), not nums[1], because "best ending at or
  before index 1" must allow skipping house 1 entirely. Filling both dp[0] and
  dp[1] before the loop is what lets the loop start at i = 2 and always read a
  valid i-2. The early return for n == 1 exists only so that nums[1] in the
  dp[1] line is safe to touch.
WATCH OUT
  An empty array breaks this: n == 0 skips the n == 1 guard, dp becomes new
  int[1], and dp[0] = nums[0] throws IndexOutOfRangeException. The array is
  allocated with size n+1 but the loop stops at n-1 and the return is dp[n-1],
  so the last slot is never written or read - a later reader may "correct" the
  return to dp[n] and silently get 0. The 0 + in notPick is dead arithmetic left
  over from the written recurrence. The comment block states the relation in
  recursive f(index) form while the code is bottom-up over an array, so the
  indices in the comment do not line up with the loop as written.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. The houses are in a circle, so the first and last are adjacent. How does
  this change?
     Run the same scan twice, once over nums[0..n-2] and once over nums[1..n-1],
     and take the max of the two results. You pay two passes, and n == 1 must
     still be handled separately because one of the two ranges is empty.
  2. Return which houses were robbed, not just the total.
     Keep the dp array (you now need the history this file already stores) and
     walk backwards from n-1: if dp[i] == dp[i-1] the house was skipped,
     otherwise it was taken and you jump to i-2. That is why the O(n) space
     version is not always the wrong choice.
  3. No two robbed houses may be within k of each other instead of 2.
     The recurrence becomes dp[i] = Math.Max(dp[i-1], nums[i] + dp[i-k]), with
     the first k entries as base cases. Rolling variables no longer suffice -
     you need a window of the last k values, so space goes back up to O(k).
TRIGGER
  A linear sequence where choosing an element blocks its immediate neighbour and
  you want the best total - think dp[i] from dp[i-1] and dp[i-2].
C# NOTE
  new int[n+1] is zero-initialized by the CLR, which is why the unused trailing
  slot is harmless but also why a wrong dp[n] return would look like a valid 0
  instead of crashing. Math.Max here resolves to the int overload, so there is
  no boxing and no cast back.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
