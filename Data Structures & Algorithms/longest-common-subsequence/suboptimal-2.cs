// ##########################################################################
// #  suboptimal-2.cs       O(n * m) time / O(n * m) space
// #  Tabulation, bottom-up DP   [tabulation-lcs]
// #  ranks below optimal.cs (O(n * m) time / O(m) space)
// #
// #  YOU SOLVED THIS YOURSELF (from submission-2)
// #
// #  Two nested loops fill the complete 2D table row by row, each cell
// #  computed exactly once.
// ##########################################################################

public class Solution
{
    public int LongestCommonSubsequence(string text1, string text2)
    {
        //My solution
        //Tabulation one right shift
        int n = text1.Length;
        int m = text2.Length;

        int[,] dp = new int[n + 1, m + 1];
        /*for (int i = 0; i < n+1; i++){
            for (int j =0; j < m+1; j++){
                dp[i,j] = -1;
            }
        }*/
        //base cases
        for (int j = 0; j < m + 1; j++)
            dp[0, j] = 0;
        for (int i = 0; i < n + 1; i++)
            dp[i, 0] = 0;

        for (int i = 1; i < n + 1; i++)// (or <= n)
        {
            for (int j = 1; j < m + 1; j++) //(or <= m)
            {
                if (text1[i - 1] == text2[j - 1])
                    dp[i, j] = 1 + dp[i - 1, j - 1];
                else
                    dp[i, j] = Math.Max(dp[i - 1, j], dp[i, j - 1]);
            }
        }

        return dp[n, m];
    }

}

/*
================================================================================
 PATTERN : 2D DP on two prefixes - match takes the diagonal
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-2.cs when it was
           first processed
 STATUS  : Suboptimal
================================================================================
VARIABLES
  dp  dp[i,j] = LCS length of text1[0..i-1] and text2[0..j-1]
  n   length of text1, the number of real rows (row 0 is the empty prefix)
  m   length of text2, the number of real columns (column 0 is the empty prefix)
WHY THIS PATTERN
  The problem asks for the best result over two sequences, and any common
  subsequence can be built one character at a time from the end. So the answer
  for two prefixes depends only on the answers for shorter prefixes. That is
  overlapping subproblems, and a table dp over every (i, j) pair of prefix
  lengths solves each one once. The final answer is dp[n, m].
BETTER APPROACH
  A better approach keeps the same recurrence but stores only two rows, or one
  row plus a saved diagonal value. That needs O(min(n, m)) space if the shorter
  string is used for the columns. This file loses because it keeps the whole
  (n+1) x (m+1) table. Each row i only reads row i-1 and the current row, so all
  older rows are never read again.
INVARIANT
  When the loop writes dp[i, j], it is already correct for every cell above it
  and to its left, because the loops fill row by row, left to right. If
  text1[i-1] == text2[j-1], an optimal LCS can end with that shared character,
  which gives 1 + dp[i-1, j-1]. If they differ, at least one of the two
  characters is not in the LCS, so the answer is the larger of dp[i-1, j] and
  dp[i, j-1]. Row 0 and column 0 are 0 because an empty prefix has no common
  subsequence. By induction, dp[n, m] is correct.
MATCH DOES NOT NEED THE MAX
  On a match the code uses only 1 + dp[i-1, j-1] and does not compare it with
  dp[i-1, j] or dp[i, j-1]. This is safe because removing one character lowers
  the LCS by at most 1, so dp[i-1, j] and dp[i, j-1] are never bigger than 1 +
  dp[i-1, j-1]. If an interviewer asks why there is no third option, this is the
  proof.
WATCH OUT
  A null text1 or text2 throws a NullReferenceException at .Length. Empty
  strings are fine and return 0. The commented-out block that fills dp with -1
  is left over from a memoization version, where -1 means "not computed yet".
  Tabulation does not need it, and if you uncommented it without the base-case
  loops after it, row 0 and column 0 would hold -1 and the answers would be
  wrong. The comment "Tabulation one right shift" is accurate: dp index i stands
  for character i-1. Mixing up the two indexes is the most common bug when you
  rewrite this.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How do you return the actual subsequence, not just its length?
     Start at dp[n, m] and walk back. On a match, add the character and move
     diagonally. Otherwise, move toward the larger of the up and left neighbors.
     This needs the full table, so it conflicts with the rolling-row memory
     saving.
  2. How does this change for longest common substring, where the characters
  must be next to each other?
     On a mismatch, set dp[i, j] = 0 instead of taking the max. Keep a running
     maximum over all cells, because the answer is no longer in dp[n, m].
  3. What is the shortest common supersequence length?
     n + m - dp[n, m]. Characters in the LCS are shared, so each one is counted
     only once.
  4. What about the LCS of three strings?
     Use a 3D table dp[i, j, k] with the same match or drop-one rule. On a
     mismatch, take the max over the three neighbors where one index is lowered.
     Time and space grow to O(n * m * p).
TRIGGER
  Two strings or arrays, and the question asks for the best matching, alignment,
  or common part while keeping order: build a table indexed by the two prefix
  lengths.
C# NOTE
  new int[n + 1, m + 1] already sets every cell to 0, so the two base-case loops
  change nothing. You can delete them and keep only a comment that row 0 and
  column 0 are the empty-prefix base cases.
COMPLEXITY
  Time  : O(n * m)
  Space : O(n * m)
================================================================================
*/
