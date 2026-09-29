// ##########################################################################
// #  suboptimal.cs         O(n * m) time / O(n * m) space
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
 PATTERN : 2D DP / Memoized recursion on two string prefixes
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-0.cs when it was
           first processed
 STATUS  : Suboptimal
================================================================================
VARIABLES
  dp     dp[i, j] = LCS length of s[0..i] and t[0..j], or -1 if not computed yet
  s, t   text1 and text2, passed down under shorter names
  i, j   the last index of the current prefix of s and of t
WHY THIS PATTERN
  The problem asks for the best result over two sequences, and a subsequence
  keeps the original order. So each step only asks about the last character of
  each prefix: use both, drop one from s, or drop one from t. The same (i, j)
  pair comes up again and again through different paths. The dp table stores
  each answer, so Lcs does the real work only once per (i, j).
BETTER APPROACH
  The better approach is bottom-up tabulation. It fills the table in a loop with
  no recursion, and keeps only two rows (or one row plus a saved diagonal
  value), each of length min(n, m) + 1. That cuts extra space to O(min(n, m)).
  This file loses because it keeps the full n x m table, which is only needed to
  rebuild the actual subsequence. It also uses a call stack that can grow to
  about n + m frames, and it spends one extra pass setting every cell to -1.
INVARIANT
  When dp[i, j] is set, it equals the LCS length of s[0..i] and t[0..j]. If s[i]
  == t[j], you can always put that shared last character at the end of some best
  LCS. So 1 + Lcs(i - 1, j - 1) is correct, and the code does not need to try
  the two "skip" options. If they differ, at least one of s[i] or t[j] is not in
  the LCS, so the Math.Max of the two smaller cases covers every option. The
  base case i < 0 or j < 0 (an empty prefix) returns 0. So by induction the top
  call Lcs(n - 1, m - 1) is correct.
WATCH OUT
  The recursion can go about n + m levels deep, for example when no characters
  match and it walks one index at a time. On long strings this can throw
  StackOverflowException, which C# cannot catch. The -1 sentinel (a special
  value meaning "not computed") is safe only because a real LCS length is never
  negative. Empty input works: n or m is 0, the table is empty, and Lcs(-1, ...)
  returns 0 before it touches dp.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How do you return the subsequence itself, not just its length?
     Keep the full table and walk back from (n - 1, m - 1). On a match, take the
     character and move diagonally. Otherwise, move toward the larger neighbor.
     This takes O(n + m) extra time, but you can no longer use the two-row
     memory saving.
  2. How do you rebuild the subsequence when the strings are too long for an n x
  m table?
     Use Hirschberg's algorithm. It is divide and conquer: split s in half and
     run linear-space DP forward on one half and backward on the other to find
     where to split t. Then recurse on both halves. Memory is linear, and time
     is still O(n * m), about twice the constant.
  3. What changes for the LCS of three strings?
     Make the state (i, j, k). A match needs all three characters equal, and
     otherwise you take the max over dropping one of the three. Time and space
     become O(n * m * p).
  4. How does this connect to Shortest Common Supersequence or delete-only edit
  distance?
     Both come straight from the LCS length. SCS length = n + m - LCS.
     Delete-only distance = n + m - 2 * LCS.
TRIGGER
  Two strings or sequences, and a question about the best way to match or align
  them in order: think of a dp[i, j] over prefix pairs.
C# NOTE
  new int[n, m] already fills the table with 0. If you store LCS + 1 in each
  cell, then 0 can mean "not computed", and you can delete the nested -1 fill
  loop.
COMPLEXITY
  Time  : O(n * m)
  Space : O(n * m)
================================================================================
*/
