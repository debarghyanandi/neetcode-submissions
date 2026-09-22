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
 PATTERN : Bottom-up DP - Fibonacci with two rolling variables
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-1.cs when it was first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  prev2    ways to reach step i-2
  prev1    ways to reach step i-1
  curr     ways to reach step i = prev1 + prev2
WHY THIS PATTERN
  You reach step n either from step n-1 (one step) or from step n-2 (two steps),
  and those two route sets never overlap. So ways(n) = ways(n-1) + ways(n-2), a
  recurrence that only looks two positions back. Because only two previous
  values matter, prev2 and prev1 replace a whole array, and curr is just the
  value being formed this round.
BRUTE FORCE
  The first thing most people write is plain recursion: return ClimbStairs(n-1)
  + ClimbStairs(n-2) with base cases at 1 and 2. That is correct but it
  recomputes the same subcalls over and over, giving exponential time and a call
  stack of depth n. Memoizing it with a dictionary or array fixes the time but
  still holds n entries and, for recursion, can overflow the stack.
INVARIANT
  At the top of each loop pass for index i, prev1 holds the number of ways to
  climb i-1 stairs and prev2 holds the number of ways to climb i-2 stairs. The
  body sets curr to their sum, which is the count for i, then shifts the window
  so the invariant holds again for i+1. The loop starts with the invariant true
  for i = 3 (prev1 = 2 ways for 2 stairs, prev2 = 1 way for 1 stair), so when
  the loop ends after i = n, prev1 is the answer for n.
WATCH OUT
  The early return covers n <= 2 by returning n itself, which silently treats n
  = 0 as 0 ways and any negative n as that negative number - if the problem
  allows n = 0, the usual answer is 1 way (climb nothing), so this is wrong
  there. curr is declared outside the loop but is never read after it; the
  function returns prev1, so if someone later "simplifies" the return to curr it
  breaks for n <= 2 where the loop never runs and curr stays 0. Also, the counts
  grow like Fibonacci, so for a large enough n the int addition prev1 + prev2
  overflows quietly with no exception.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. What if you may climb 1, 2 or 3 steps at a time?
     Keep three rolling variables instead of two and sum all three each round;
     space stays constant, time stays linear, but the base cases now need n = 0,
     1, 2 set up before the loop.
  2. What if the allowed step sizes are an arbitrary set, say steps = {1, 3, 5}?
     You need a dp array of length n+1 and, for each i, sum dp[i - s] over every
     valid s. That costs O(n * steps.Length) time and O(n) space, because the
     window you look back over is no longer just two wide.
  3. n is huge and you need the exact count fast.
     Use fast matrix exponentiation of [[1,1],[1,0]] or the fast-doubling
     Fibonacci identities for O(log n) time, and switch the accumulator to
     BigInteger since the true value stops fitting in 64 bits well before that.
  4. Some steps are broken and cannot be stepped on.
     Keep the same two-variable shift but set curr to 0 when step i is broken,
     so no path is counted through it; the rest of the recurrence is unchanged.
TRIGGER
  When the count of ways to reach state n depends only on a fixed number of
  earlier states, drop the array and roll a few variables forward.
C# NOTE
  prev2, prev1 and curr are plain int value types on the stack, so there is no
  allocation at all here; if you later move to BigInteger for overflow safety,
  each addition allocates a new object and the loop stops being allocation-free.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
