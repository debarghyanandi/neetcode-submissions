// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
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
 PROBLEM : You climb a staircase with n steps. Each move is 1 step or 2 steps.
           Return how many distinct ways reach the top. Order matters: 1+2 and
           2+1 are two different ways. Example: n = 3 -> 3.
 PATTERN : 1-D Dynamic Programming (Fibonacci, rolling variables)
================================================================================
IDEA
  The last move onto step i is either 1 step (from i-1) or 2 steps (from i-2).
  So ways(i) = ways(i-1) + ways(i-2), the Fibonacci rule.
  prev1 holds ways(i-1) and prev2 holds ways(i-2). Each loop computes curr,
  then shifts both values forward by one step. The two cases never overlap
  and cover every path, so adding them counts each way exactly once.
EXAMPLE
  n = 5, start prev2 = 1, prev1 = 2
  i=3: curr=3 -> prev2=2, prev1=3 | i=4: curr=5 -> prev2=3, prev1=5
  i=5: curr=8 -> prev2=5, prev1=8
  return prev1 = 8
COMPLEXITY
  Time  O(n)  one loop from 3 to n, O(1) work per step
  Space O(1)  only prev2, prev1, curr, whatever n is
PATH TO OPTIMAL
  Plain recursion ways(n-1)+ways(n-2) - O(2^n) - base, repeats subproblems.
  Memoization or dp array - O(n) / O(n) - each step solved once
  (suboptimal.cs).
  Two rolling variables - O(n) / O(1) - dp[i] needs only the last two values.
KEYWORDS
  dynamic programming, Fibonacci, recurrence, memoization, bottom-up,
  tabulation
WATCH OUT
  - Update order: set prev2 = prev1 BEFORE prev1 = curr, or you lose a value.
  - int overflows: n = 46 gives 2971215073 > int.MaxValue. Use long or
    BigInteger.
  - n <= 2 returns n, so n = 0 returns 0 and negative n returns negative.
    Some define ways(0) = 1. Say your base case out loud.
  - Return prev1, not curr: curr is 0 whenever the loop body never runs.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. What if you may climb 1, 2, ..., k steps at a time?
     -> ways(i) = sum of the last k values. Keep a sliding window sum over a
        size-k buffer: O(n) time, O(k) space.
  2. What if some steps are broken and cannot be stepped on?
     -> Set ways(i) = 0 for a broken step and keep the same recurrence. Still
        O(n) time, O(1) space.
  3. n is huge, like 10^18, answer modulo a prime?
     -> Use matrix power of [[1,1],[1,0]] with fast exponentiation. O(log n)
        time, O(1) space; more code, only worth it for huge n.
  4. Why does this work?
     -> The final move splits all paths into two disjoint groups, ending with
        a 1-step or a 2-step, so the counts add.
TRIGGER
  Count the ways to reach a state when each step depends only on a fixed
  number of previous states.
================================================================================
*/
