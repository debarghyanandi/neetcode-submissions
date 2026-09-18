// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
// -  Dynamic programming, space-optimized, circular split
// -  [dp-space-optimized-circular]
// -  the only solution in this folder
// -
// -  Reference solution - not one you solved yourself
// -
// -  Circular constraint split into two linear DP passes using rolling
// -  variables instead of a table.
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
 PATTERN : House Robber II - two linear DP runs on a broken circle
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  first     best loot from houses 0..n-2 (drops the last house)
  second    best loot from houses 1..n-1 (drops the first house)
  prev2     best loot up to the house two before i
  prev      best loot up to the house just before i
  curr      best loot up to house i inside the loop
  pick      take nums[i], so add it to prev2
  notPick   skip nums[i], so keep prev
WHY THIS PATTERN
  The houses sit in a circle, so house 0 and house n-1 are neighbours and cannot
  both be robbed. That single extra rule is the only difference from the plain
  line version, and it splits into two cases: either house 0 is left out, or
  house n-1 is left out. RobLinear solves each case as the ordinary "no two
  adjacent" problem, and Math.Max(first, second) picks the better case. Inside
  RobLinear the choice at each house depends only on the two answers behind it,
  so prev and prev2 are enough state.
BRUTE FORCE
  The first thing most people write is recursion: at each house, try robbing it
  and jumping two ahead, or skipping it and moving one ahead, then take the max.
  Without memoisation that branches twice per house and costs O(2^n) time.
  Adding a memo table drops it to O(n) time but keeps an O(n) array; this file
  keeps only prev and prev2, so the table disappears.
INVARIANT
  At the top of each iteration of the loop in RobLinear, prev holds the best
  loot for houses start..i-1 and prev2 holds the best loot for houses
  start..i-2. The choice at i is forced: either nums[i] plus the best that ends
  at or before i-2, or the best that ends at i-1. Both candidates respect the
  no-adjacent rule, and every valid plan for start..i ends in one of these two
  shapes, so curr is exact. After the loop prev is the answer for the whole
  range start..end.
WHY THE TWO CASES COVER EVERYTHING
  Neither run forbids robbing both endpoints of its own range; it just removes
  one house from the circle. That is enough, because in any valid circular plan
  at least one of house 0 and house n-1 is not robbed. If house 0 is skipped the
  plan is legal inside 1..n-1, and if house n-1 is skipped it is legal inside
  0..n-2. So the true optimum appears in at least one of first or second, and
  neither run can produce an illegal plan, since after cutting one end the range
  is a plain line.
WATCH OUT
  An empty array crashes: n == 0 passes both guards, then RobLinear reads
  nums[0] on a length-zero array. RobLinear also assumes the range has at least
  two houses because it reads nums[start + 1] before the loop; the n == 1 and n
  == 2 early returns are what keep that safe, so removing them breaks the
  helper. The line int curr = start; stores an index into a variable that is
  supposed to hold money - it is dead because the loop overwrites curr before
  any read, but it is confusing and would be wrong if the code ever returned
  curr instead of prev. The 0 + in notPick = 0 + prev adds nothing and only
  exists to mirror the pick line.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How would you return which houses were robbed, not just the total?
     The two-variable form has thrown that information away. Keep an O(n) dp
     array per run plus a parent or a boolean "took house i" flag, then walk
     backwards from the end. Memory goes back to O(n), time stays the same.
  2. What if no two robbed houses may be within k houses of each other, still in
  a circle?
     prev2 becomes prev_k, the best answer k+1 positions back, so you need a
     rolling window of k+1 values or a running max over that window. The
     circular fix also grows: instead of two cases you exclude each of the first
     k houses in turn, or run the line version on the ranges that break the
     wrap.
  3. The array is huge and arrives as a stream you can read only once.
     The recurrence already needs only the last two values, so one pass works
     for the line case. The circular case is the problem: it needs two passes
     over almost the same data, so either buffer the first and last element and
     run both recurrences in parallel in a single pass, or accept a second read.
TRIGGER
  A "no two adjacent" choice problem where the items are arranged in a ring
  instead of a line - cut the ring at one link and run the linear DP twice.
C# NOTE
  Passing start and end as int parameters avoids building sub-arrays; the LINQ
  or Array.Copy version (nums.Take(n-1).ToArray()) would allocate two new arrays
  and lose the O(1) space claim. If you wanted the slice style without the copy,
  ReadOnlySpan<int> with nums.AsSpan(0, n - 1) gives a view rather than a new
  array.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
