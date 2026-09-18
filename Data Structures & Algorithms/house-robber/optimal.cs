// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
// -  Space-optimized linear DP, rolling variables   [house-robber-dp]
// -  ranks above suboptimal.cs (O(n) time / O(n) space)
// -
// -  Reference solution - not one you solved yourself
// -
// -  Only the two most recent DP states are retained in variables; no table
// -  allocated.
// --------------------------------------------------------------------------

public class Solution
{
    public int Rob(int[] nums)
    {
        //Dp space Optimized
        int length = nums.Length;

        if (length == 1)
            return nums[0];

        int prevPrev = nums[0];
        int prev = Math.Max(nums[0], nums[1]);
        int curr = 0;

        for (int i = 2; i < length; i++)
        {
            int pick = nums[i] + prevPrev;
            int notPick = 0 + prev;
            curr = Math.Max(pick, notPick);

            prevPrev = prev;
            prev = curr;
        }

        return prev;
    }
}

/*
================================================================================
 PATTERN : Linear DP with rolling variables - two-state house robber
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-1.cs when it was first processed
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  The problem asks for the maximum sum of a subset of nums where no two chosen
  indexes are adjacent. That gives a clean recurrence: the best answer at house
  i is either nums[i] plus the best answer two houses back, or the best answer
  one house back. Because each step only reads the two previous answers, the
  whole DP table collapses into prevPrev and prev, which slide forward once per
  index.
BRUTE FORCE
  The first thing most people write is recursion: rob(i) = max(nums[i] +
  rob(i-2), rob(i-1)), with no memo. That explores both branches at every index
  and costs O(2^n) time. Adding a memo array or a full dp[] table fixes the time
  to O(n) but keeps an O(n) array; this file drops that array because only two
  cells are ever read.
INVARIANT
  At the top of each iteration for index i, prev holds the best loot from houses
  0..i-1 and prevPrev holds the best loot from houses 0..i-2. Both are "best
  over the whole prefix", not "best ending exactly at that house", which is why
  max(pick, notPick) is a valid choice - notPick simply carries the prefix
  answer forward unchanged. The seeds prevPrev = nums[0] and prev = max(nums[0],
  nums[1]) satisfy the invariant for i = 2, so it holds for every later index
  and prev at the end is the answer for the whole array.
WATCH OUT
  An empty array crashes: length == 1 is checked, but length == 0 falls through
  to nums[0] and throws IndexOutOfRangeException. The method returns prev, not
  curr - that is deliberate and must stay that way, because with length == 2 the
  loop body never runs and curr is still 0. The notPick line is written as 0 +
  prev; the 0 is dead weight left from the "skip this house" idea and adds
  nothing.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. The houses are in a circle, so the first and last are adjacent. What
  changes?
     Run this same routine twice, once on nums[0..n-2] and once on nums[1..n-1],
     and take the larger result; the single-element case must be returned before
     that split. Cost is still O(n) time, just two passes.
  2. Return which houses were robbed, not only the total.
     Rolling variables are no longer enough - you need an O(n) array of the
     per-index choice, or store the picked index sets, then walk backwards from
     the end choosing pick whenever dp[i] != dp[i-1]. You trade the O(1) space
     for the ability to reconstruct.
  3. The rule becomes "no two robbed houses within k of each other".
     The recurrence turns into max(nums[i] + best(i-k-1), best(i-1)), so you
     keep a window of the last k+1 answers in a small ring buffer instead of two
     scalars. Time stays O(n), space becomes O(k).
  4. The houses form a binary tree instead of a line.
     Do a post-order traversal returning a pair (best with this node robbed,
     best without); the parent combines children the same way max(pick, notPick)
     does here. Time is O(nodes), space is the recursion depth.
TRIGGER
  A linear sequence where choosing an element forbids its immediate neighbour,
  and the recurrence only ever looks one or two steps back.
C# NOTE
  curr is declared before the loop and set to 0 only so it survives the loop
  scope, but nothing after the loop reads it - moving int curr inside the loop
  body would compile the same and keep the variable's meaning local.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
