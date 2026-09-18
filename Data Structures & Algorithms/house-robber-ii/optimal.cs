// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
// -  Dynamic programming, space-optimized, circular constraint split
// -  [dp-space-optimized-circular]
// -  the only solution in this folder
// -
// -  Reference solution - not one you solved yourself (from submission-0)
// -
// -  Two linear DP passes handle the circular constraint; each pass uses
// -  rolling variables instead of a table.
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
 PATTERN : House Robber II - circular array split into two linear DP runs
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  first     best loot from houses 0..n-2 (house n-1 never robbed)
  second    best loot from houses 1..n-1 (house 0 never robbed)
  prev2     best loot up to two houses back inside the current window
  prev      best loot up to the previous house inside the current window
  curr      best loot up to house i; becomes the new prev each step
  pick      take nums[i], so add it to prev2
  notPick   skip nums[i], so keep prev
WHY THIS PATTERN
  The houses sit in a circle, so house 0 and house n-1 are neighbours and cannot
  both be robbed. That single extra rule is the only difference from the plain
  line version, so you remove it by force: either house n-1 is off the table, or
  house 0 is. RobLinear solves each straight range with the usual take-or-skip
  recurrence, and the answer is Math.Max(first, second). Inside each run only
  prev2 and prev matter, because the choice at house i depends on nothing older
  than two steps.
BRUTE FORCE
  The first thing most people write is recursion over "rob house i or skip it"
  with a flag for whether house 0 was taken. That explores every valid subset
  and costs about O(2^n) time. Adding memoization on (i, flag) drops it to O(n)
  time but keeps an O(n) table and the recursion stack; this file reaches the
  same result with two scalars per run.
INVARIANT
  At the top of each iteration of the loop in RobLinear, prev holds the best
  loot from nums[start..i-1] and prev2 holds the best loot from
  nums[start..i-2]. Both are the true optimum for those prefixes, so curr =
  Math.Max(nums[i] + prev2, prev) is the true optimum for nums[start..i], since
  robbing i forbids i-1 and nothing else. The shift prev2 = prev; prev = curr
  restores the invariant for i+1, so the returned prev is the optimum for the
  whole window.
WATCH OUT
  An empty array crashes: n == 0 passes both guards and RobLinear reads
  nums[start] with start = 0. The variable curr is initialised to start, which
  is an index, not a loot value - it is dead because the method returns prev,
  but the name and the type make it easy to later return curr by mistake when
  start > end. RobLinear also reads nums[start + 1] unconditionally, which is
  safe only because every window here has at least two elements once n >= 3;
  reusing this helper on a one-element range would throw. The write int notPick
  = 0 + prev; is just prev with noise added.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Return which houses were robbed, not only the total.
     The rolling pair loses the path. Keep a full dp array plus a choice flag
     per index, or walk backwards comparing dp[i] against dp[i-1], and do that
     for both runs before taking the max - O(n) space again.
  2. Can you do it in one pass instead of calling RobLinear twice?
     Yes, carry two independent (prev2, prev) pairs in the same loop, one for
     the range excluding the last house and one excluding the first. Same O(n)
     time, one traversal, but the code gets harder to read than two clean calls.
  3. The rule changes to "no two robbed houses within 3 of each other".
     The recurrence becomes nums[i] + best up to i-3, so keep three rolling
     values instead of prev2 and prev. The circular fix also grows: you now need
     to block several first and last combinations, which is usually done with a
     small set of forced-exclusion runs.
  4. Houses form a binary tree instead of a circle.
     Switch to post-order DFS returning a pair (best with this node robbed, best
     without). No wrap-around case exists, so the two-run split disappears.
TRIGGER
  A linear DP you already know, but the array wraps around so the first and last
  elements conflict - run the linear DP twice on the two open ranges.
C# NOTE
  Passing start and end indices forces the helper to do index arithmetic like
  nums[start + 1]; taking a ReadOnlySpan<int> and calling
  RobLinear(nums.AsSpan(0, n - 1)) would slice without copying and let the loop
  just run from 2 to the span length.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
