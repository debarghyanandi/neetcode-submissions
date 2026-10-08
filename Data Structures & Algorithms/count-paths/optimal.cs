// --------------------------------------------------------------------------
// -  optimal.cs            O(m * n) time / O(n) space
// --------------------------------------------------------------------------

public class Solution
{
    public int UniquePaths(int m, int n)
    {
        //space optimized tabulation
        int[] curr = new int[n + 1];
        int[] next = new int[n + 1];

        next[n - 1] = 1;

        for (int i = m - 1; i >= 0; i--)
        {
            for (int j = n - 1; j >= 0; j--)
            {
                curr[j] = next[j] + curr[j + 1];
            }
            (curr, next) = (next, curr);
        }
        return next[0];
    }
}

/*
================================================================================
 PROBLEM : A robot starts at the top-left cell of an m x n grid. It may only
           move right or down. Return how many different paths reach the
           bottom-right cell. Example: m = 3, n = 7 -> 28. Also m = 3, n = 3
           -> 6.
 PATTERN : 2D DP (bottom-up tabulation), rolling two rows
================================================================================
IDEA
  The paths from a cell = paths from the cell below + paths from the cell
  to the right. next holds the row below, and curr is the row being filled.
  We fill curr from j = n-1 down to 0, using curr[j+1] as the right
  neighbour. The extra slot curr[n] stays 0, so it acts as the wall.
  next[n-1] = 1 seeds the goal cell. Correct because every path makes
  exactly one first move, down or right, so the two counts never overlap.
EXAMPLE
  m = 3, n = 3. Seed next = [0,0,1,0].
  i=2: curr = [1,1,1] -> swap; i=1: curr = [3,2,1] -> swap
  i=0: curr = [6,3,1] -> swap; return next[0] = 6
COMPLEXITY
  Time  O(m * n)  each of the m*n cells is computed once with O(1) work
  Space O(n)      only two arrays of n+1 ints, reused by swapping
PATH TO OPTIMAL
  Plain recursion, try down and right - O(2^(m+n)) - simple but recomputes.
  Memoization or full 2D table (suboptimal.cs, suboptimal-2.cs) - O(m*n)
  space - each cell is solved once.
  Two rolling rows (this file) - O(n) space - a row needs only the row below.
KEYWORDS
  grid DP, unique paths, tabulation, rolling array, space optimization,
  combinatorics
WATCH OUT
  - After the last swap the answer is in next, not curr. Returning curr[0]
    gives a stale row.
  - The j loop must go right to left. curr still holds old values from two
    rows down, so curr[j+1] must be written before it is read.
  - int overflows on big grids. Use long if the answer can exceed 2^31-1.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Some cells are blocked (Unique Paths II)?
     -> Set curr[j] = 0 for a blocked cell, and keep the same recurrence.
        Still O(m*n) time and O(n) space.
  2. Can you use one array?
     -> Yes. dp[j] += dp[j+1], because dp[j] still holds the value from below.
        Same time, half the memory.
  3. Faster than O(m*n)?
     -> Every path is m-1 downs and n-1 rights, so the answer is C(m+n-2,
        m-1). Compute it with a running product in O(min(m,n)) time and O(1)
        space. Watch overflow and divide at each step.
  4. Minimum path sum instead of a count?
     -> Same grid DP, but use min(down, right) + cell cost instead of a sum.
TRIGGER
  Counting or optimizing paths on a grid with only right/down moves means
  each cell depends on two neighbours, so use grid DP with rolling rows.
================================================================================
*/
