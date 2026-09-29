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
 PATTERN : 2D DP / LCS - tabulation with two rolling rows
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-4.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  n     length of text1 (the number of DP rows)
  m     length of text2 (the number of DP columns)
  prev  prev[j] = LCS of text1[0..i-2] and text2[0..j-1] (the row above)
  curr  curr[j] = LCS of text1[0..i-1] and text2[0..j-1] (the row being built)
WHY THIS PATTERN
  The problem asks for the best result over two sequences, where you can skip
  characters but must keep their order. That points at a 2D prefix DP: the
  answer for prefixes i and j depends only on smaller prefixes. If text1[i-1] ==
  text2[j-1], that character extends the diagonal answer prev[j-1]. If not, you
  drop one character from one of the strings, so you take Math.Max(prev[j],
  curr[j-1]).
BRUTE FORCE
  The first correct idea is plain recursion. Compare the last characters. If
  they match, add 1 and recurse on both shorter strings. If not, take the max of
  dropping from text1 or dropping from text2. This recomputes the same (i, j)
  pairs again and again, so it is exponential, up to O(2^(n+m)). Memoizing it
  gives the same O(n * m) time, but it keeps a full table and uses deep
  recursion.
INVARIANT
  At the start of row i, prev holds the full DP row i-1. Index 0 of both arrays
  is always 0, because the empty prefix has LCS 0 and the code never writes to
  it. Inside the row, curr[1..j-1] is already final for row i, and prev is
  untouched. So the three cells each cell reads (diagonal, up, left) are exactly
  the values the recurrence needs. After the last row, the swap moves row n into
  prev, so prev[m] is the answer.
WHY A STALE CURR IS SAFE
  After the swap, curr holds the old row i-2, which is stale data. This is safe
  because every curr[j] for j >= 1 is written before it is read as curr[j-1] in
  the same row. The only cell read without a write first is curr[0], and that
  cell is always 0.
WATCH OUT
  The comment "I know swapping is not necessary" is misleading. A plain prev =
  curr would make both names point to one array. The next row would then read
  cells it had already overwritten, and the answer would be wrong. The swap, or
  a copy, is required. The loop that sets prev[j] = 0 is dead code, as its own
  comment says, because new int[] is already zero-filled. If you move the return
  to read curr[m], you get the stale row, not the answer. The swap has already
  moved the last row into prev, as the final comment explains.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you use even less memory?
     Yes. Use one array plus one saved int for the diagonal. Before you
     overwrite dp[j], save its old value, because it becomes the diagonal for
     j+1. You can also make the shorter string the column string, which gives
     O(min(n, m)) space. The cost is code that is harder to read.
  2. How do you return the subsequence itself, not only its length?
     Keep the full (n+1) x (m+1) table. Walk back from (n, m): on a match, take
     the character and move diagonally. Otherwise, move toward the larger
     neighbor. This needs O(n * m) space, or Hirschberg's divide-and-conquer if
     you need linear space.
  3. How does this change for edit distance or shortest common supersequence?
     The grid and the rolling rows stay the same. Only the cell rule and the row
     0 / column 0 base values change. For example, in edit distance, dp[0][j] =
     j instead of 0.
TRIGGER
  Two strings or arrays, order must be kept, characters may be skipped, and you
  want a max or min over matchings: build a DP grid over prefix pairs.
C# NOTE
  The tuple swap (prev, curr) = (curr, prev) swaps only the two array
  references. No elements are copied, so each row costs O(1) extra work, while
  Array.Copy would cost O(m) per row.
COMPLEXITY
  Time  : O(n * m)
  Space : O(m)
================================================================================
*/
