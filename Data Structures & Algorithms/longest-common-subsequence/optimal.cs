// ##########################################################################
// #  optimal.cs            O(n * m) time / O(m) space
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
 PROBLEM : Given two strings text1 and text2, return the length of their
           longest common subsequence. A subsequence keeps the order of
           characters but may skip some; it does not need to be contiguous.
           Return 0 if none. Example: "abcde", "ace" -> 3 ("ace").
 PATTERN : 2D DP (grid over two strings), rolled into two rows
================================================================================
IDEA
  dp[i][j] is the LCS of the first i chars of text1 and the first j of text2.
  If text1[i-1] == text2[j-1], extend the diagonal: 1 + prev[j-1]. If not,
  drop one char: Math.Max(prev[j], curr[j-1]). Row i needs only row i-1,
  so we keep only prev and curr and swap them after each row.
  It is correct because every common subsequence either ends with a match
  or skips the last char of one string.
EXAMPLE
  text1="abcde", text2="ace". Rows are prev[0..3] after each i (after swap):
  a:[0,1,1,1] b:[0,1,1,1] c:[0,1,2,2] (c==c: 1+prev[1])
  d:[0,1,2,2] e:[0,1,2,3] (e==e: 1+prev[2]=1+2) -> return prev[3] = 3
COMPLEXITY
  Time  O(n * m)  each (i, j) cell is filled once with O(1) work
  Space O(m)      only two rows of size m+1, prev and curr
PATH TO OPTIMAL
  Brute recursion on (i, j) - O(2^(n+m)) - baseline, repeats subproblems.
  Memoization or full n x m table (suboptimal.cs, suboptimal-2.cs) -
  O(n*m) / O(n*m) - each state is solved only once.
  Two rolling rows (this file) - O(n*m) / O(m) - a row needs only the last.
KEYWORDS
  LCS, dynamic programming, tabulation, memoization, rolling array, strings
WATCH OUT
  - After the final swap the last row is in prev, so return prev[m].
    Returning curr[m] gives the row before the last one.
  - Swap arrays, do not assign prev = curr. Both would then be the same
    array, and the next row would overwrite the values it still reads.
  - curr keeps stale values from two rows back. It is safe only because
    j runs from 1 to m and writes curr[j] before curr[j-1] is read again.
  - With one array, save the old dp[j-1] in a diag variable first.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Return the subsequence itself, not only its length?
     -> Keep the full n x m table and walk back from (n, m): on a match take
        the char and go diagonal, else move toward the larger neighbor. O(n*m)
        space, because the rolled rows lose the path.
  2. Use even less memory?
     -> Put the shorter string on the inner loop, so space is O(min(n, m)).
        One array plus a diag variable also works. Time stays O(n*m).
  3. Longest common substring (contiguous) instead?
     -> Same grid, but set the cell to 0 on a mismatch and track the max over
        all cells. Still O(n*m) time.
  4. Related problems?
     -> Edit Distance and Distinct Subsequences use the same two-string grid,
        with different match and mismatch rules.
TRIGGER
  Two strings or arrays, a question about matching them in order with
  skips allowed: build a dp grid on (i, j) prefixes.
================================================================================
*/
