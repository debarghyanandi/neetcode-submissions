// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
// -  Dynamic programming space optimization   [dp-space-optimized]
// -  ranks above suboptimal.cs (O(n) time / O(n) space)
// -
// -  Reference solution - not one you solved yourself
// -
// -  Single pass through array with constant rolling variables tracking
// -  only the last two DP values.
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
 PATTERN : Linear DP, rolling variables - pick vs skip
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-1.cs when it was first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  n        number of houses, nums.Length
  prev2    best loot using houses 0..i-2
  prev     best loot using houses 0..i-1
  curr     best loot using houses 0..i, rebuilt each step
  pick     take house i, so add nums[i] to prev2
  notPick  skip house i, so keep prev unchanged
WHY THIS PATTERN
  The problem says you cannot rob two adjacent houses, so the choice at house i
  depends only on whether you took house i-1. That is a one-step dependency,
  which makes the answer at i a function of the answers at i-1 and i-2 only. So
  a full dp array is waste: prev and prev2 carry all the history the recurrence
  needs, and they shift forward once per house.
BRUTE FORCE
  The first thing most people write is recursion: rob(i) = max(nums[i] +
  rob(i-2), rob(i-1)), no memo. That explores both branches at every house and
  costs about O(2^n) time. Adding a memo table or a dp array of size n fixes the
  time but still holds n integers; this file keeps the same recurrence and drops
  the array down to two variables.
INVARIANT
  At the top of each loop pass for index i, prev holds the best loot from houses
  0..i-1 and prev2 holds the best from houses 0..i-2. curr = max(nums[i] +
  prev2, prev) is then correct, because any valid plan that includes house i
  cannot include house i-1 and so is capped by prev2. The two shifts at the end
  restore the invariant for i+1, and after the last pass prev is the best over
  the whole array, which is what is returned.
WATCH OUT
  An empty array breaks this: n == 0 skips the n == 1 guard and nums[0] throws
  IndexOutOfRangeException. The loop starts at 2, so for n == 2 it never runs
  and the return value comes straight from the initial prev = Math.Max(nums[0],
  nums[1]) - that is correct, but it means the return must be prev and not curr,
  since curr would still be 0. The comment says "Dp space Optimized" and the
  code matches, but curr is declared outside the loop for no reason and is dead
  after the final assignment. notPick = 0 + prev is just prev; the 0 is
  decoration from the recursive version.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. The houses are in a circle, so house 0 and house n-1 are neighbours. What
  changes?
     Run this same scan twice, once on nums[0..n-2] and once on nums[1..n-1],
     and take the larger result. Robbing both ends is then impossible by
     construction; cost stays linear, but you need a separate n == 1 guard
     because one of the two ranges is empty.
  2. Return which houses were robbed, not just the total.
     Rolling variables are not enough - you need to know the decision at each i.
     Keep an n-length array of booleans (or the full dp array) and walk
     backwards from the end, which pushes space back to O(n).
  3. The rule becomes "no two houses within k of each other".
     The recurrence turns into max(nums[i] + best(i-k-1), best(i-1)), so two
     variables no longer suffice. Keep a rolling buffer of the last k+1 answers,
     or a running maximum, giving O(n) time and O(k) space.
  4. What if the loot values can be negative?
     Then notPick must also allow taking nothing at all; here pick = nums[i] +
     prev2 can drag the total down. Clamp with max(0, ...) or seed prev2 and
     prev from 0 instead of nums[0].
TRIGGER
  A sequence where each element is take-or-skip and taking one blocks its
  immediate neighbour - that is prev/prev2 in two variables.
C# NOTE
  curr belongs inside the loop body - declaring it outside only to satisfy
  definite assignment costs nothing but widens its scope past its last real use.
  Math.Max(int, int) is the right call here; there is no need for LINQ or any
  collection, since the method never allocates beyond the input array.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
