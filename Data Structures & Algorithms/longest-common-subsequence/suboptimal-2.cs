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
 PATTERN : 2D DP tabulation - LCS with 1-based shift
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-2.cs when it was
           first processed
 STATUS  : Suboptimal
================================================================================
VARIABLES
  n     length of text1
  m     length of text2
  dp    dp[i,j] = LCS length of text1's first i chars and text2's first j chars
WHY THIS PATTERN
  The problem asks for the longest subsequence common to both strings, and a
  subsequence lets you skip characters freely in either string. That means at
  every position pair you face a branching choice: match both characters, or
  drop one from text1, or drop one from text2. Those choices overlap heavily, so
  each state (i, j) is worth storing once in dp. The loop fills dp in increasing
  i and j so every value it reads, dp[i-1,j-1], dp[i-1,j] and dp[i,j-1], is
  already final.
BETTER APPROACH
  The better approach keeps only two rows, or even one row plus a saved diagonal
  value. dp[i, j] reads only from row i-1 and from the cell just left of it, so
  the rows above i-1 are dead weight the moment row i starts. A rolling pair of
  int[m+1] arrays gives the same answer with memory proportional to m instead of
  n*m. This file loses on space only; its running time is the same.
INVARIANT
  After the inner loop finishes for a given i, every dp[i, j] holds the true LCS
  length of text1[0..i-1] and text2[0..j-1]. The match branch is safe because if
  text1[i-1] equals text2[j-1], some optimal LCS can always be taken to pair
  those two last characters, so 1 + dp[i-1, j-1] is optimal. The mismatch branch
  is safe because the two characters cannot both be the last matched pair, so
  one of them must be dropped, and Math.Max covers both drops. Row 0 and column
  0 are 0 because an empty string shares nothing, which anchors the recursion.
THE OFF-BY-ONE SHIFT
  dp has n+1 rows and m+1 columns, but text1 and text2 are still 0-based. That
  is why the comparison is text1[i-1] == text2[j-1] while the write is dp[i, j].
  If you ever change the loops to start at 0 to "save a row", every index in the
  body has to shift too, and the empty-prefix base case has to be handled some
  other way.
WATCH OUT
  The commented-out block that fills dp with -1 belongs to a memoization
  solution, not this one; if you uncomment it here it destroys the zero base
  cases and the answer becomes garbage. The two explicit base-case loops are
  pure no-ops: new int[n+1, m+1] in C# already zeroes every cell, so those lines
  only cost time. Empty input is fine - if either string has length zero, the
  main loops never run and dp[n, m] is the zero the allocation gave you. The
  comment "one right shift" is misleading: nothing is shifted at runtime, it
  just describes the +1 index offset.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Reduce the space to one dimension?
     Keep one int[m+1] row and a single scalar holding the old dp[i-1, j-1]
     value before you overwrite the cell. You save memory, but you lose the
     ability to walk the table backwards.
  2. Return the actual subsequence string, not just its length?
     You need the full dp table this file already builds; start at dp[n, m] and
     walk back, stepping diagonally when text1[i-1] == text2[j-1] and otherwise
     toward the larger of dp[i-1,j] and dp[i,j-1], then reverse the collected
     characters. This is a real argument for keeping the O(n*m) table instead of
     rolling rows.
  3. What if the answer must be a common substring (contiguous) instead?
     The mismatch branch changes to dp[i, j] = 0 and you track a running maximum
     over all cells, because a break in contiguity kills the run rather than
     letting you skip a character.
  4. Which string should be the columns if one is much shorter?
     With the rolling-row version, make the shorter string the column dimension
     so the row array is the smaller of the two lengths; swap the arguments at
     the top if needed.
TRIGGER
  Two sequences, and the answer depends on a prefix of each one independently -
  that is a (i, j) table.
C# NOTE
  int[,] is a true rectangular array stored in one block, unlike int[][] jagged
  arrays which are an array of row references; here the rectangular form is the
  right pick since every row has the same length m+1.
COMPLEXITY
  Time  : O(n * m)
  Space : O(n * m)
================================================================================
*/
