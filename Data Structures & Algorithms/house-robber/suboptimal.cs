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
 PATTERN : 1-D DP - House Robber, no two adjacent picks
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Suboptimal
================================================================================
VARIABLES
  n        number of houses, nums.Length
  dp       dp[i] = best loot robbing only houses 0..i
  pick     rob house i: nums[i] + dp[i-2]
  notPick  skip house i: dp[i-1]
WHY THIS PATTERN
  The problem asks for the maximum sum of a subset of nums where no two chosen
  indices are adjacent. That single "cannot take the neighbour" rule means the
  choice at house i only depends on what was already decided at i-1 and i-2, not
  on the whole history. So one linear scan filling dp works: at each i you
  compare pick against notPick and keep the larger. The answer is dp[n-1], the
  best over the whole street.
BETTER APPROACH
  The better version keeps only two numbers instead of the whole dp array, since
  the loop reads nothing older than dp[i-2]. Two rolling ints (say prev1 =
  dp[i-1], prev2 = dp[i-2]), shifted each step, give the same answer in constant
  extra space. This file loses on memory only: it allocates an int array of size
  n+1 to store values it never looks at again, and one slot, dp[n], is never
  even written or read.
INVARIANT
  After the iteration for index i, dp[i] holds the best total obtainable using
  only houses 0..i, with the adjacency rule respected. This holds at the start
  because dp[0] = nums[0] and dp[1] = Math.Max(nums[0], nums[1]) are the correct
  answers for those prefixes. Each step preserves it: any valid plan for 0..i
  either uses house i, and then cannot use i-1, giving nums[i] + dp[i-2], or
  does not, giving dp[i-1]; taking the max covers both cases. So dp[n-1] is the
  best over all houses.
WATCH OUT
  An empty array breaks this: with n == 0 the n == 1 guard does not fire, dp
  becomes new int[1], and dp[0] = nums[0] throws IndexOutOfRangeException. The
  comment block describes the recurrence as calls f(indx - 1) and f(indx - 2),
  which suggests recursion, but the code is a bottom-up loop with no function
  calls - read it as notation, not as what runs. The 0 + in notPick adds nothing
  and can be deleted. Also note the array is sized n + 1 while the loop stops at
  n - 1, so the extra slot is dead weight and returning dp[n] instead of dp[n-1]
  would silently give 0.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. The houses are in a circle, so the first and last are neighbours. What
  changes?
     Run the same linear scan twice, once on nums[0..n-2] and once on
     nums[1..n-1], and take the max of the two results; the two runs exclude the
     conflicting pair. Handle n == 1 separately as here.
  2. Return the actual houses robbed, not just the total.
     Keep the dp array, which this file already has, then walk backwards from i
     = n-1: if dp[i] == dp[i-1] the house was skipped, move to i-1; otherwise
     record i and move to i-2. That is why the full array is sometimes worth
     keeping.
  3. What if you must leave two houses between picks instead of one?
     The recurrence becomes dp[i] = Math.Max(nums[i] + dp[i-3], dp[i-1]), so you
     need three rolling variables and three base cases instead of two.
TRIGGER
  A linear sequence where choosing an element forbids its immediate neighbour
  and you want the best total.
C# NOTE
  Math.Max on two ints is the right call here, but note new int[n + 1] is
  zero-initialised by the runtime, so the untouched dp[n] silently reads as 0
  rather than as an error - a good reason to size the array exactly n.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
