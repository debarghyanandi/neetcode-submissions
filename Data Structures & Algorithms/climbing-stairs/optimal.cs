// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
// -  Space-optimized Fibonacci iteration   [fibonacci-space-optimized]
// -  ranks above suboptimal.cs (O(n) time / O(n) space)
// -
// -  Reference solution - not one you solved yourself
// -
// -  Rolling variables track only the last two values, eliminating array
// -  storage.
// --------------------------------------------------------------------------

public class Solution
{
    public int ClimbStairs(int n)
    {
        if (n <= 2)
            return n;

        int prev2 = 1;
        int prev1 = 2;
        int curr = 0;

        for (int i = 3; i <= n; i++)
        {
            curr = prev1 + prev2;
            prev2 = prev1;
            prev1 = curr;
        }

        return prev1;
    }
}

/*
================================================================================
 PATTERN : 1D DP / Fibonacci - keep only the last two values
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-1.cs when it was first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  prev2    ways to reach step i-2 (starts at 1, the count for step 1)
  prev1    ways to reach step i-1 (starts at 2, the count for step 2)
  curr     ways to reach step i, built as prev1 + prev2
WHY THIS PATTERN
  You can climb 1 or 2 steps at a time. So the last move onto step i came from
  step i-1 or from step i-2. That means ways(i) = ways(i-1) + ways(i-2), which
  is the Fibonacci rule. Each new value needs only the two values before it, so
  prev1 and prev2 can stand in for a full dp array.
BRUTE FORCE
  The first idea is plain recursion: ways(n) = ways(n-1) + ways(n-2), stopping
  at n <= 2. It gives the right answer, but it solves the same smaller steps
  again and again, so it takes about O(2^n) time. Adding memoization (saving
  each answer the first time you compute it) or a dp array brings it down to
  O(n) time, but it still uses O(n) extra memory. This file keeps the O(n) time
  and drops the array.
INVARIANT
  At the start of the loop body for index i, prev1 = ways(i-1) and prev2 =
  ways(i-2). The body sets curr = ways(i), then moves the pair one step forward:
  prev2 becomes the old prev1 and prev1 becomes curr. This is true before the
  first pass (i = 3: prev1 = 2, prev2 = 1). So when the loop ends after i = n,
  prev1 holds ways(n).
WATCH OUT
  The order of the two updates matters. If you write prev1 = curr before prev2 =
  prev1, the old prev1 is lost and both variables end up equal. The early return
  uses n itself as the answer. That is right for n = 1 and n = 2, but it returns
  0 for n = 0 and a negative number for negative n. Whether ways(0) should be 1
  or 0 depends on the problem, so check it. The count grows like Fibonacci, so
  int overflows once n goes past about 45.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. What if you can take 1, 2, or 3 steps at a time?
     ways(i) = ways(i-1) + ways(i-2) + ways(i-3). Keep three rolling variables
     instead of two. Space is still O(1).
  2. What if the allowed step sizes are a given list, like {1, 3, 5}?
     Use a dp array where dp[i] = sum of dp[i-s] for each step s in the list.
     The rolling-variable trick only works when the largest step is a small
     fixed number. Otherwise the array costs O(n) space and the time is O(n *
     number of steps).
  3. Can you do better than O(n) time for very large n, with the answer taken
  modulo some number?
     Yes. Raise the 2x2 matrix [[1,1],[1,0]] to a power by repeated squaring.
     That takes O(log n) time. It is harder to write, and you need the modulo so
     the numbers stay small.
  4. What if some steps are broken and you cannot stand on them?
     Set ways(i) = 0 for a broken step and use the same rule for the rest. The
     rolling variables still work.
TRIGGER
  When the answer for size n is built from a fixed, small number of answers for
  smaller sizes, use bottom-up DP and keep only those few past values.
C# NOTE
  The variable curr is declared outside the loop and set to 0, but it is only
  used inside the loop body. You could write "int curr = prev1 + prev2;" inside
  the loop, or swap without it using a tuple: (prev2, prev1) = (prev1, prev1 +
  prev2);
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
