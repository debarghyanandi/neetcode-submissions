// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
// -  Dynamic Programming Space-Optimized   [dp-space-optimized]
// -  ranks above suboptimal.cs (O(n) time / O(n) space)
// -
// -  Reference solution - not one you solved yourself (from submission-1)
// -
// -  Uses rolling variables to track only the last two Fibonacci values
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
 PATTERN : Bottom-up DP on Fibonacci - two rolling variables
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-1.cs when it was first processed
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  To reach step n you must arrive from step n-1 (taking one step) or from step
  n-2 (taking two), and those two route sets never overlap, so ways(n) =
  ways(n-1) + ways(n-2). That recurrence only ever looks back two positions, so
  a full DP table is waste: prev2 and prev1 are enough state. The
  loop from i = 3 to n walks forward and rebuilds that pair each time.
BRUTE FORCE
  The first thing most people write is plain recursion: return ClimbStairs(n-1)
  + ClimbStairs(n-2) with base cases at 1 and 2. It is correct but the call tree
  branches twice at every level and recomputes the same subproblems, so it costs
  about O(2^n) time and O(n) stack depth. Adding a memo array fixes the time but
  still holds n entries; this file keeps the same forward order and drops the
  array.
INVARIANT
  At the top of each iteration for index i, prev1 holds the number of ways
  to reach step i-1 and prev2 holds the number of ways to reach step i-2.
  The body computes current for step i, then shifts the window so the invariant
  holds again for i+1. The seeds 1 and 2 are the true counts for steps 1 and 2,
  so by induction prev1 is the count for step n when the loop ends, which
  is what gets returned.
WATCH OUT
  The guard if (n <= 2) return n means n = 0 returns 0 and any negative n is
  returned unchanged; if the caller expects one way to climb zero stairs, that
  is wrong. The values are int and Fibonacci grows fast, so a large n silently
  overflows and wraps to a wrong or negative result - there is no checked block
  here. Also note current is declared outside the loop and initialized to 0 but
  never read after the loop; the return uses prev1, so the 0 is harmless
  but misleading to a reader.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. What if you can take 1, 2 or 3 steps at a time?
     Keep three rolling variables and sum all three each iteration, with seeds
     for n = 1, 2, 3. Space stays O(1); for a general k the window becomes an
     array of size k and time becomes O(n*k).
  2. n is huge and you must beat linear time.
     Use 2x2 matrix exponentiation (or fast doubling) on the Fibonacci
     recurrence for O(log n) multiplications. Trade-off: more code, and you need
     BigInteger or a modulus because the true answer stops fitting in any
     fixed-width integer long before that.
  3. Each step has a cost and you want the cheapest climb instead of the count.
     Same two-variable shape, but current becomes cost[i] +
     Math.Min(prev1, prev2) - min replaces sum. The rolling-window
     trick survives because the recurrence still looks back only two positions.
  4. Return the actual list of step sequences, not the count.
     You must backtrack and emit each path, so the output alone is exponential
     in n; rolling variables no longer help and O(1) space is impossible.
TRIGGER
  A counting or optimization question where the answer at position i depends
  only on a fixed number of earlier positions - collapse the DP table to that
  many variables.
C# NOTE
  C# tuple assignment would remove the current variable entirely: (prev2,
  prev1) = (prev1, prev1 + prev2) evaluates the right
  side first, so no temporary is needed. If overflow matters, change the return
  type to long or wrap the addition in checked so it throws instead of wrapping
  quietly.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
