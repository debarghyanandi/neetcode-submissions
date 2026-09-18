// --------------------------------------------------------------------------
// -  suboptimal.cs         O(n) time / O(n) space
// -  Dynamic Programming Tabulation   [dp-tabulation]
// -  ranks below optimal.cs (O(n) time / O(1) space)
// -
// -  Reference solution - not one you solved yourself (from submission-0)
// -
// -  Builds and fills a DP array bottom-up with Fibonacci recurrence
// -  relation
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
 PATTERN : Bottom-up DP - Fibonacci recurrence on a 1-D table
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Suboptimal
================================================================================
WHY THIS PATTERN
  The problem asks for the number of distinct ways to reach step n when each
  move is 1 or 2 steps. The last move into step i is either a 1-step from i-1 or
  a 2-step from i-2, and those two sets of paths never overlap, so dp[i]
  = dp[i-1] + dp[i-2]. Counting plus a recurrence that depends
  only on smaller values is the signature of bottom-up dynamic programming: fill
  dp from 3 upward and read dp[n].
BETTER APPROACH
  The better version keeps the same loop but drops the array: hold two ints,
  prev = 1 and curr = 2, and in each iteration set next = prev + curr, prev =
  curr, curr = next, then return curr. That is O(1) space instead of an int[n +
  1] that is allocated, zeroed, and then read only at the last index. This file
  loses only on memory - the time is identical - so the array is pure waste for
  every i below n - 1.
INVARIANT
  After the iteration for index i, dp[i] holds the exact count of
  distinct 1/2-step sequences that end on step i. The base cases seed this truth
  for i = 1 and i = 2, and the loop only ever reads i-1 and i-2, which the
  previous iterations already finished. Because every path into i must arrive by
  a final 1-step or a final 2-step and never both, the sum is a complete,
  non-overlapping split, so the invariant carries to n.
WATCH OUT
  The early return "return n" is doing double duty: for n = 1 and n = 2 it is
  the real answer, but for n = 0 it returns 0 and for negative n it returns the
  negative number itself. Many versions of this problem treat n = 0 as 1 way
  (the empty path), so check the expected value for 0 before reusing this. The
  guard is also load-bearing for safety, not just speed: without it, n = 1 would
  make the array length 2 and the write to dp[2] would throw
  IndexOutOfRangeException. Finally, the values are Fibonacci numbers and grow
  fast - dp[46] is 2971215073, past int.MaxValue, so the addition
  silently overflows to a negative number for n >= 46.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. The input n is huge and you need the count modulo 1e9+7. What changes?
     Take the modulus on every addition, dp[i] = (dp[i-1] +
     dp[i-2]) % MOD, which also removes the overflow problem. If n is so
     large that an O(n) loop is too slow, switch to 2x2 matrix power or fast
     doubling for O(log n) time.
  2. Steps of 1, 2 or 3 are allowed. What changes?
     The recurrence becomes dp[i] = dp[i-1] + dp[i-2] +
     dp[i-3], with three base cases and the loop starting at i = 4. The
     rolling-variable version then needs three variables instead of two.
  3. Some steps are broken and cannot be stepped on. What changes?
     Keep the array - you now need a per-index flag, so write dp[i] = 0
     when step i is broken and otherwise apply the same sum. This is a case
     where the O(n) table is justified, since the answer depends on positional
     data, not just the last two counts.
  4. An interviewer asks for the recursive form instead. What is the cost?
     Plain recursion on n-1 and n-2 is exponential; adding a memo array makes it
     O(n) time but adds O(n) recursion stack depth, which can overflow the stack
     for large n. The loop here avoids that entirely.
TRIGGER
  Count the number of ways to reach a state, where each state is reached from a
  fixed, small set of earlier states - write the recurrence and fill it
  bottom-up.
C# NOTE
  "new int[n + 1]" zero-initializes the whole block, which is why index 0 is
  never assigned yet never wrong; if you keep the array, that free zeroing is
  the only thing it buys you. The two-int rolling version needs no allocation at
  all and so puts no pressure on the garbage collector.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
