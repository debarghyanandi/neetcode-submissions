// --------------------------------------------------------------------------
// -  suboptimal.cs         O(n) time / O(n) space
// -  Dynamic programming tabulation   [fibonacci-dp]
// -  ranks below optimal.cs (O(n) time / O(1) space)
// -
// -  Reference solution - not one you solved yourself
// -
// -  Each position computed once using previously stored values in array.
// --------------------------------------------------------------------------

public class Solution
{
    public int ClimbStairs(int n)
    {
        if (n <= 2)
        {
            return n;
        }

        int[] dp = new int[n + 1];
        dp[1] = 1;
        dp[2] = 2;

        for (int i = 3; i <= n; i++)
        {
            dp[i] = dp[i - 1] + dp[i - 2];
        }

        return dp[n];
    }
}

/*
================================================================================
 PATTERN : Bottom-up DP (Fibonacci recurrence) over a full table
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Suboptimal
================================================================================
VARIABLES
  dp    dp[i] = number of distinct ways to reach step i from the ground
WHY THIS PATTERN
  The last move onto step i is either a 1-step from i-1 or a 2-step from i-2,
  and those two sets of paths never overlap. So the count for i is exactly the
  sum of the counts for i-1 and i-2, which is a recurrence with overlapping
  subproblems - the definition of dynamic programming. Filling dp from 3 upward
  means both dp[i-1] and dp[i-2] are already final when dp[i] is written, so no
  recursion and no memo lookups are needed.
BETTER APPROACH
  The better version keeps only two numbers, say prev1 = dp[i-1] and prev2 =
  dp[i-2], and rolls them forward inside the same loop. It returns the same
  value with the same number of additions but uses constant extra space instead
  of an n+1 array. This file loses because every dp[i] below the last one is
  read twice and then never needed again, yet all of them stay alive until the
  method returns.
INVARIANT
  Before each iteration with index i, every entry dp[1..i-1] already holds the
  true number of ways to reach that step. The base cases dp[1] = 1 and dp[2] = 2
  are true by inspection, and the assignment dp[i] = dp[i-1] + dp[i-2] only
  reads indices strictly below i, so the invariant is preserved. When the loop
  ends, i has passed n, so dp[n] is final and correct.
WATCH OUT
  The early return for n <= 2 is load-bearing, not just a shortcut: without it,
  n = 1 would allocate an array of length 2 and then dp[2] = 2 would throw
  IndexOutOfRangeException. A negative n makes new int[n + 1] throw
  OverflowException before any logic runs. dp[0] is never assigned, so it stays
  0 and is silently unused - if you ever start the loop at 2 instead of 3 you
  get the wrong answer, because the correct base is dp[0] = 1, not 0. The values
  are Fibonacci numbers, so int overflows silently once n grows past the mid-40s
  and the result becomes negative garbage.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Reduce the space to O(1).
     Replace the array with two ints seeded to 1 and 2, then in the loop compute
     cur = a + b, a = b, b = cur, and return b. Same time, no allocation; you
     lose the ability to query any intermediate step afterwards.
  2. What if a step can be 1, 2, or 3 stairs?
     The recurrence becomes dp[i] = dp[i-1] + dp[i-2] + dp[i-3] with three base
     cases; for an arbitrary set of allowed step sizes you sum dp[i - s] over
     every s in the set, which costs O(n * |set|) time.
  3. n is huge, say a billion, with the answer modulo 1e9+7.
     Use 2x2 matrix exponentiation or fast doubling on the Fibonacci recurrence
     for O(log n) time; the loop here would do a billion additions and the array
     would not fit comfortably in memory.
  4. Now each step has a cost and you want the cheapest way up.
     Same table shape, but dp[i] = cost[i] + Min(dp[i-1], dp[i-2]) instead of a
     sum - the transition switches from counting to minimising while the scan
     order stays identical.
TRIGGER
  The answer at position i depends only on a fixed number of earlier positions,
  and you are counting disjoint ways to arrive rather than searching paths.
C# NOTE
  new int[n + 1] is zero-initialised by the runtime, which is why dp[0] can be
  skipped without a compile error - but it also means the whole array is
  heap-allocated and touched by the GC for a result that only ever needs two
  ints on the stack.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
