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
 PATTERN : 1D Dynamic Programming - Fibonacci-style recurrence
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Suboptimal
================================================================================
VARIABLES
  dp  dp[i] = number of distinct ways to reach step i using moves of 1 or 2 steps
WHY THIS PATTERN
  Each move is 1 or 2 steps, so the last move onto step i comes from step i-1 or
  from step i-2. So the count for step i depends only on two smaller answers
  that overlap. Dynamic programming means saving each smaller answer once and
  reusing it. The code fills dp from the bottom up with dp[i] = dp[i - 1] + dp[i
  - 2].
BETTER APPROACH
  A better version keeps only two int variables, the last two answers, and
  slides them forward. It does the same additions but needs constant memory.
  This file loses because it keeps the whole dp array, and it only ever reads
  dp[i - 1] and dp[i - 2]. Every older entry is stored and never read again.
INVARIANT
  When the loop starts step i, dp[1..i-1] already hold the correct counts. Every
  path to step i ends with a 1-step move from i-1 or a 2-step move from i-2.
  These two groups do not overlap, and together they cover every path. So their
  sum is exactly dp[i]. By induction from dp[1] = 1 and dp[2] = 2, dp[n] is
  correct.
EARLY RETURN PROTECTS THE ARRAY
  The check if (n <= 2) return n is not only a shortcut. When n = 1 the array is
  new int[2], so the line dp[2] = 2 would throw IndexOutOfRangeException. The
  early return makes sure the array has at least 3 slots before the base cases
  are written. Also, dp[0] is allocated but never used.
WATCH OUT
  The result is dp[n] = Fib(n+1), and int overflows at n = 46. There dp[46] =
  2971215073, which is larger than int.MaxValue. C# arithmetic is unchecked by
  default, so this becomes a negative number with no error. For n = 0 the code
  returns 0, but many definitions count 1 way (do nothing). For negative n it
  returns n itself, which is a negative count.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. What if you can climb 1, 2, ..., k steps at a time?
     dp[i] = sum of dp[i-1] through dp[i-k]. Keep a running window sum: add
     dp[i-1] and subtract dp[i-k-1]. Each step is O(1) work, but you must keep
     the last k values.
  2. What if n is huge and the answer is asked modulo some M?
     Use matrix exponentiation on [[1,1],[1,0]]. Repeated squaring gives O(log
     n) time. It is harder to write and only pays off for very large n.
  3. What if some steps are broken and you cannot land on them?
     Set dp[i] = 0 for a broken step and keep the same recurrence. The paths
     through that step are then removed automatically.
  4. How would you write it top-down?
     Use recursion plus a memo array (a cache of answers already found). It is
     easy to read, but the recursion depth grows with n and it uses stack
     memory.
TRIGGER
  Reach for this when the number of ways to reach state i is a sum over a few
  fixed earlier states, such as "you may take 1 or 2 steps".
C# NOTE
  The two-variable version is short in C# with a tuple swap: (a, b) = (b, a +
  b); inside the loop. It updates both values in one line with no temp variable.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
