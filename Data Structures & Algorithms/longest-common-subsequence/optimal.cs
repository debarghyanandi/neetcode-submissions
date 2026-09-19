// ##########################################################################
// #  optimal.cs            O(n * m) time / O(m) space
// #  Space-optimized tabulation, rolling array   [space-optimized-lcs]
// #  ranks above suboptimal.cs (O(n * m) time / O(n * m) space)
// #
// #  YOU SOLVED THIS YOURSELF (from submission-4)
// #
// #  Rolling window maintains only current and previous row, reducing
// #  auxiliary space from O(n*m) to O(m).
// ##########################################################################

public class Solution
{
    public int LongestCommonSubsequence(string text1, string text2)
    {
        //My solution
        //Space optimized Tabulation one right shift
        int n = text1.Length;
        int m = text2.Length;

        //int[,] dp = new int[n+1, m+1];
        int[] prev = new int[m + 1];
        int[] curr = new int[m + 1];

        for (int j = 0; j < m + 1; j++)
            prev[j] = 0;//optional as array already have 0

        for (int i = 1; i <= n; i++)
        {
            for (int j = 1; j <= m; j++)
            {
                if (text1[i - 1] == text2[j - 1])
                    curr[j] = 1 + prev[j - 1];
                else
                    curr[j] = Math.Max(prev[j], curr[j - 1]);
            }
            //I know swapping is not necessary
            //we only want prev to become curr
            //but also we need to preserve the array of curr
            //for next iteration.
            (prev, curr) = (curr, prev);
        }
        //as we are swapping curr and prev at end of loop
        //The Current value is in Prev
        return prev[m];
    }

}

/*
================================================================================
 PATTERN : 2-D DP over two strings, rolling two rows
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-4.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  n       length of text1, the row dimension
  m       length of text2, the column dimension
  prev    prev[j] = LCS length of text1[0..i-2] and text2[0..j-1] (previous row)
  curr    curr[j] = LCS length of text1[0..i-1] and text2[0..j-1] (row being built)
WHY THIS PATTERN
  The question asks for the longest subsequence common to both strings, so at
  every pair of positions you make one of two choices: the characters match and
  both advance, or they do not and you drop one character from one side. That is
  a grid of subproblems indexed by (i, j), which is exactly a 2-D table. Row i
  of that table only ever reads row i-1 and the cell to its left, so the whole
  table collapses to prev and curr.
BRUTE FORCE
  The first thing most people write is recursion on (i, j): if text1[i-1] ==
  text2[j-1] return 1 + f(i-1, j-1), else return max(f(i-1, j), f(i, j-1)).
  Without memoization that branches twice per call and is exponential, roughly
  O(2^(n+m)). It loses because the same (i, j) pair is recomputed a huge number
  of times; adding a memo table gives the same work as this loop but pays for
  recursion frames.
INVARIANT
  At the moment the inner loop is about to compute curr[j], prev holds the
  complete answer row for prefix text1[0..i-2] over all j, and curr[0..j-1]
  holds the finished part of row i. So 1 + prev[j-1] is the true diagonal value
  and Math.Max(prev[j], curr[j-1]) is the true "skip one character" value. Since
  every cell is filled left to right and every row from top to bottom, the
  invariant holds at every step, and after the last swap prev[m] is the
  full-string answer.
WHY CURR[0] AND PREV[0] STAY ZERO
  The base case of this DP is "one string is empty, so the common subsequence is
  empty". The j loop starts at 1, so index 0 of both arrays is never written
  after allocation and keeps its default 0 forever, even across swaps. That is
  what makes prev[j-1] and curr[j-1] safe when j == 1 without any extra guard.
WATCH OUT
  The comment "I know swapping is not necessary / we only want prev to become
  curr" is misleading: plain prev = curr would make both names point at the same
  array, and the next row would read the row it is writing. The swap is what
  keeps two distinct buffers alive. The explicit loop that sets prev[j] = 0 is
  dead code and the comment next to it admits this. Also, the row reuse is only
  safe because every index 1..m of curr is overwritten each row; if you ever
  added an early break or a partial inner range, old values from two rows back
  would leak through.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you do it with a single array instead of prev and curr?
     Yes. Keep one int[] dp and one scalar holding the old dp[j-1] (the
     diagonal) before you overwrite dp[j]. Same time, half the memory, but the
     code is harder to read and easy to get wrong.
  2. How do you return the actual subsequence, not just its length?
     Reconstruction needs the full table, so you go back to int[n+1, m+1] and
     walk from (n, m) backwards, which costs O(n*m) memory. If memory matters,
     Hirschberg's divide-and-conquer builds the string in O(min(n, m)) space at
     about twice the time.
  3. What if text2 is far longer than text1?
     Memory here is tied to m, so swap the arguments first so that m is the
     shorter length. The answer is symmetric, so the result is unchanged.
  4. What changes for longest common substring (contiguous)?
     Drop the else branch entirely: on a mismatch set curr[j] = 0, and track a
     running maximum over all cells instead of reading the last cell, because
     the best substring can end anywhere.
TRIGGER
  Two sequences compared position by position, where a match lets both advance
  and a mismatch means dropping one element from one side.
C# NOTE
  The tuple form (prev, curr) = (curr, prev) swaps two references in place, so
  no array contents are copied per row; writing Array.Copy(curr, prev, m + 1)
  instead would add real O(m) copying work on every row.
COMPLEXITY
  Time  : O(n * m)
  Space : O(m)
================================================================================
*/
