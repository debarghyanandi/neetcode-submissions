// ##########################################################################
// #  suboptimal.cs         O(n * m) time / O(n * m) space
// #  Memoization with recursion   [memoization-lcs]
// #  ranks below optimal.cs (O(n * m) time / O(m) space)
// #
// #  YOU SOLVED THIS YOURSELF (from submission-0)
// #
// #  Memoization table caches each subproblem result to avoid
// #  recomputation; call stack depth is at most O(n+m).
// ##########################################################################

public class Solution
{
    public int LongestCommonSubsequence(string text1, string text2)
    {
        //My solution
        //Memoization
        int n = text1.Length;
        int m = text2.Length;

        int[,] dp = new int[n, m];
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < m; j++)
            {
                dp[i, j] = -1;
            }
        }
        return Lcs(n - 1, m - 1, text1, text2, dp);
    }

    private int Lcs(int i, int j, string s, string t, int[,] dp)
    {
        if (i < 0 || j < 0)
            return 0;

        if (dp[i, j] != -1)
            return dp[i, j];

        if (s[i] == t[j])
            return dp[i, j] = 1 + Lcs(i - 1, j - 1, s, t, dp);

        return dp[i, j] = Math.Max(Lcs(i - 1, j, s, t, dp), Lcs(i, j - 1, s, t, dp));
    }
}

/*
================================================================================
 PATTERN : Top-down DP on two string indices (LCS memoization)
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-0.cs when it was
           first processed
 STATUS  : Suboptimal
================================================================================
VARIABLES
  n     text1.Length, the starting row index is n - 1
  m     text2.Length, the starting column index is m - 1
  dp    dp[i, j] = length of the LCS of text1[0..i] and text2[0..j], -1 means not computed
  i, j  in Lcs: the last index still in play in s and t
WHY THIS PATTERN
  The question asks for the longest subsequence common to both strings, and a
  subsequence lets you skip characters freely. So at each pair of ends (i, j)
  there are only two moves: if s[i] == t[j] both characters must pair up,
  otherwise you drop one of them and try both drops. That branching repeats the
  same (i, j) pairs many times, so dp caches each pair once and the recursion
  becomes linear in the number of pairs.
BETTER APPROACH
  The better version is the bottom-up table: fill dp[i, j] with two nested loops
  from the small indices up, no recursion at all. It does the same work but has
  no call overhead and, more importantly, cannot blow the stack. Even better,
  the bottom-up form only ever reads row i - 1 and row i, so it can be reduced
  to two int arrays of length m and drop the space to O(m). This file keeps the
  whole n by m table and a recursion depth of up to n + m.
INVARIANT
  Every time Lcs(i, j) returns, dp[i, j] holds the true LCS length of the two
  prefixes ending at i and j, and it is never overwritten with a different
  value. This holds because each call either hits the base case (one prefix is
  empty, answer 0), reads a cell already proven correct, or builds its answer
  from strictly smaller subproblems: (i-1, j-1) on a match, (i-1, j) and (i,
  j-1) otherwise. The match branch is safe because if the last characters are
  equal there is always an optimal LCS that pairs them, so nothing is lost by
  not also trying the two drops.
WATCH OUT
  If either string is empty, n or m is 0, and new int[0, 0] is fine, but the
  first call is Lcs(-1, -1, ...) which hits the i < 0 guard and returns 0 before
  touching dp - correct, only by one line. The comment says "Memoization" but
  the initialisation loop is a full n * m pass that runs even for inputs that
  barely recurse. The base check must stay i < 0 || j < 0 and not i == 0 || j ==
  0; with the latter you would silently ignore the first character of both
  strings. Deep recursion is the real risk here: two long strings with few
  matches push the call chain toward n + m frames and can throw
  StackOverflowException, which .NET does not let you catch.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How do you return the actual subsequence, not just its length?
     Keep the filled dp and walk back from (n-1, m-1): on s[i] == t[j] prepend
     the character and move to (i-1, j-1), otherwise step to whichever of
     dp[i-1, j] or dp[i, j-1] is larger. That is O(n + m) extra time and the
     string buffer as extra space, but it forces you to keep the full table, so
     it is incompatible with the two-row trick.
  2. What if the problem changed to longest common substring (contiguous)?
     The recurrence loses the two-drop branch: on a mismatch the value is 0, on
     a match it is 1 + dp[i-1, j-1], and the answer is the maximum cell instead
     of dp[n-1, m-1]. Same table size, but you must track the running max
     separately.
  3. How would you remove the recursion without changing the answer?
     Rewrite as two loops over i and j ascending, with an extra leading row and
     column of zeros so that the i - 1 and j - 1 reads need no bounds test. That
     removes the stack risk and the -1 sentinel fill entirely.
  4. What if only the length matters and memory is tight?
     Roll the table to two int rows of length m + 1, swapping them each outer
     iteration, since dp[i, j] only depends on the previous row and the current
     row to the left.
TRIGGER
  Two sequences compared position by position where you may skip elements in
  either one - that pairs-of-indices shape is the signal for a two-dimensional
  DP table.
C# NOTE
  int[,] is a true rectangular array, so dp[i, j] is one bounds-checked access
  and the whole block is one allocation; it is the right choice over int[][]
  here. The manual -1 fill exists only because new int[n, m] zero-fills, and 0
  is a legal LCS answer - if you instead stored "length + 1" you could use 0 as
  the sentinel and delete the loop.
COMPLEXITY
  Time  : O(n * m)
  Space : O(n * m)
================================================================================
*/
