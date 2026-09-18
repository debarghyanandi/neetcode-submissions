// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
// -  Space-optimized dynamic programming   [house-robber-dp]
// -  ranks above suboptimal.cs (O(n) time / O(n) space)
// -
// -  Reference solution - not one you solved yourself (from submission-1)
// -
// -  Single pass through houses, tracking only the previous two DP states
// -  with rolling variables for constant space.
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
 PATTERN : Linear DP, space-optimized to two rolling variables
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-1.cs when it was first processed
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  The problem asks for the maximum sum of chosen numbers where no two chosen
  positions are next to each other. That "cannot take i and i-1" rule means the
  best answer at house i depends only on the best answers at i-1 and i-2, which
  is a one-dimensional recurrence. Since the recurrence looks back exactly two
  steps, the whole DP table collapses into prev2 (best up to i-2) and prev (best
  up to i-1), and the loop advances them one house at a time.
BRUTE FORCE
  The first thing most people write is recursion: rob(i) = max(nums[i] +
  rob(i-2), rob(i-1)), with no memo. That explores both branches at every index,
  so it costs about O(2^n) time and O(n) stack depth. Adding a memo array fixes
  the time to O(n) but still holds an O(n) table; this file keeps the same time
  and drops the table because only the last two entries are ever read.
INVARIANT
  At the top of each iteration for index i, prev holds the best total robbing
  only houses 0..i-1, and prev2 holds the best total for houses 0..i-2. Both
  choices at house i are covered: pick = nums[i] + prev2 is legal because prev2
  never includes house i-1, and notPick = prev keeps the previous best
  untouched. The two assignments at the end shift the window so the invariant
  holds again for i+1, and after the last iteration prev is the best over all
  houses, which is what is returned.
WATCH OUT
  An empty array breaks this: n == 0 skips the n == 1 guard and then nums[0]
  throws IndexOutOfRangeException. curr is declared outside the loop and
  initialized to 0, but nothing outside the loop reads it, so that 0 is dead and
  the return uses prev instead - if someone later changes the return to curr,
  the n == 2 case would wrongly return 0 because the loop never runs. The 0 + in
  notPick = 0 + prev is a leftover from writing the recurrence and adds nothing.
  Note also that prev is seeded with Math.Max(nums[0], nums[1]), which silently
  assumes n >= 2 at that line.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. What changes if the houses are in a circle, so the first and last are
  adjacent?
     Run this same loop twice, once on nums[0..n-2] and once on nums[1..n-1],
     and take the larger result; the cost is two passes instead of one, and n ==
     1 still needs its own guard.
  2. How do you report which houses were robbed, not just the total?
     You need the choice at each index, so keep an O(n) array of booleans (or
     the full DP table) and walk backwards from the end; that gives up the
     constant space this version was written for.
  3. What if the rule becomes "no two robbed houses within k of each other"?
     The recurrence becomes max(nums[i] + best[i-k-1], best[i-1]), so two
     variables are no longer enough - keep a rolling buffer of the last k+1
     values, which is O(k) space.
  4. What if the input arrives as a stream and you cannot index backwards?
     Nothing needs to change - the loop only ever reads nums[i] once and keeps
     two ints, so it already works as a single forward pass over a stream.
TRIGGER
  A maximize-the-sum problem with a "you cannot use two neighbours" or "must
  skip at least one" restriction on a line of items.
C# NOTE
  The shift at the end of the loop can be written as (prev2, prev) = (prev,
  Math.Max(nums[i] + prev2, prev)) using C# tuple assignment, which removes curr
  entirely and makes the rolling step one line with no temporary to keep in
  sync.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
