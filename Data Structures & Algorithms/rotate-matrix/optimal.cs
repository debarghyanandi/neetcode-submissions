// --------------------------------------------------------------------------
// -  optimal.cs            O(n^2) time / O(1) space
// --------------------------------------------------------------------------

public class Solution
{
    public void Rotate(int[][] matrix)
    {

        int n = matrix.Length;

        //transpose
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                if (j >= i)
                    continue;
                int temp = matrix[i][j];
                matrix[i][j] = matrix[j][i];
                matrix[j][i] = temp;
            }
        }

        //Refactor use ---->for (int j = i + 1; j < n; j++)
        // then we dont need if (j >= i)

        foreach (int[] arr in matrix)
        {
            Array.Reverse(arr);
        }

    }
}

/*
================================================================================
 PROBLEM : Given an n x n matrix of ints, rotate it 90 degrees clockwise in
           place. Return nothing; you must change the input matrix, not build
           a new one. Example: [[1,2],[3,4]] -> [[3,1],[4,2]].
 PATTERN : Matrix in place: Transpose + Reverse Rows
================================================================================
IDEA
  Clockwise rotation sends cell (i, j) to (j, n-1-i).
  Do it in two simple steps. First transpose: swap matrix[i][j] with
  matrix[j][i], which maps (i, j) to (j, i). Then Array.Reverse each row,
  which maps (j, i) to (j, n-1-i). Together they give the rotation.
  The "if (j >= i) continue" makes each pair swap only once.
EXAMPLE
  [[1,2,3],[4,5,6],[7,8,9]]
  transpose -> [[1,4,7],[2,5,8],[3,6,9]]
  reverse each row -> [[7,4,1],[8,5,2],[9,6,3]] (answer)
COMPLEXITY
  Time  O(n^2)  both passes touch each of the n*n cells a constant number of
                times
  Space O(1)    only temp for swaps; Array.Reverse works in place
PATH TO OPTIMAL
  Copy to new grid, res[j][n-1-i] = matrix[i][j] - O(n^2)/O(n^2) - simple.
  Transpose + reverse rows in place - O(n^2)/O(1) - no extra grid (this file).
  No sibling file; only optimal.cs exists in this folder.
KEYWORDS
  rotate image, matrix rotation, in-place, transpose, reverse rows, 2D array
WATCH OUT
  - Loop over all (i, j) with no skip and every pair swaps twice.
    The matrix comes back unchanged.
  - Reversing columns instead of rows gives a counterclockwise turn.
  - Non-square input breaks: n = matrix.Length, so matrix[j][i] can go out of
    range.
  - The comment's refactor (j = i + 1) is correct. It swaps the same pairs.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Rotate counterclockwise?
     -> Transpose, then reverse the columns (or reverse each row first, then
        transpose). Still O(n^2) time, O(1) space.
  2. Do it in one pass without a transpose?
     -> Go layer by layer and rotate 4 cells at once with one temp. Same
        O(n^2)/O(1), but the index math is harder to get right.
  3. Rotate 180 degrees?
     -> Reverse the order of the rows, then reverse each row. O(n^2), O(1).
  4. What if the matrix is m x n?
     -> The shape becomes n x m, so in place is not possible. Build a new grid
        with res[j][m-1-i] = matrix[i][j]. That costs O(mn) extra space.
TRIGGER
  When asked to rotate or flip a square grid in place, think of chaining
  transpose and reflections.
================================================================================
*/
