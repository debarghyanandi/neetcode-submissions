// --------------------------------------------------------------------------
// -  optimal.cs            O(log(m*n)) time / O(1) space
// -  binary search, flatten matrix to 1D   [binary-search-flattened-matrix]
// -  ranks above suboptimal.cs (O(m log n) time / O(log n) space)
// -
// -  Reference solution - not one you solved yourself
// -
// -  Single binary search on virtual 1D array of m*n elements halves search
// -  space each iteration.
// --------------------------------------------------------------------------

public class Solution
{
    public bool SearchMatrix(int[][] matrix, int target)
    {
        int rows = matrix.Length, cols = matrix[0].Length;

        int left = 0, right = rows * cols - 1;
        while (left <= right)
        {
            int mid = left + (right - left) / 2;
            int row = mid / cols; // convert virtual index back to row
            int col = mid % cols; // convert virtual index back to col

            if (target > matrix[row][col])
            {
                left = mid + 1;
            }
            else if (target < matrix[row][col])
            {
                right = mid - 1;
            }
            else
                return true;
        }
        return false;
    }
}

/*
================================================================================
 PATTERN : Binary Search on a flattened 2D matrix
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-1.cs when it was first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  rows    matrix.Length, number of rows
  cols    matrix[0].Length, row width, also the divisor for index math
  left    low end of the virtual 1D search range
  right   high end, starts at rows*cols - 1, the last flat index
  mid     current virtual 1D index being probed
  row     mid / cols, the real row of mid
  col     mid % cols, the real column of mid
WHY THIS PATTERN
  The matrix is sorted left to right in each row, and the first value of every
  row is larger than the last value of the row above. That means reading the
  matrix row by row gives one single sorted list. So we can pretend the whole
  grid is an array of length rows*cols and run a plain binary search on it,
  turning each mid back into matrix[row][col] with a division and a remainder.
BRUTE FORCE
  Scan every cell and compare to target: O(m*n) time. A middle step is to binary
  search the first column to pick the row, then binary search inside that row,
  which is O(log m + log n) - same order as this code but written twice. The
  full scan loses because it ignores the sorting completely.
INVARIANT
  At the top of every loop pass, if target exists in the matrix its flat index
  is inside [left, right]. Each comparison against matrix[row][col] drops the
  half that cannot hold it, since the flat order is non-decreasing. The range
  shrinks every pass, so the loop ends either by returning true or by left
  passing right, which proves target is absent.
MID WITHOUT OVERFLOW
  mid is computed as left + (right - left) / 2 instead of (left + right) / 2.
  With int indices over rows*cols, the second form can overflow when both ends
  are large; this form cannot, because right - left is never bigger than right.
WATCH OUT
  matrix[0].Length is read before anything checks that matrix has a row, so an
  empty matrix throws IndexOutOfRangeException. Ragged input also breaks it:
  cols is taken from row 0 only, and every later row is indexed with mid % cols,
  so a shorter row would throw. rows * cols is an int multiply - on a grid large
  enough it would overflow and silently break the search range. matrix[row][col]
  is evaluated twice per pass in the two comparisons; correct, but worth
  hoisting into a local.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. What changes if rows are sorted but a row's first value is NOT greater than
  the previous row's last value (the "Search a 2D Matrix II" variant)?
     The flattening is no longer sorted, so this code is wrong. Start at the
     top-right corner and move left when the value is too big, down when too
     small: O(m + n) time, still O(1) space.
  2. How would you return the position instead of a bool?
     Return the pair (row, col) already computed at the hit, or return mid and
     let the caller divide; no extra work, since the conversion is done every
     pass anyway.
  3. The matrix is huge and stored on disk, one row per block. How does that
  change the approach?
     Prefer the two-step search - binary search the column of first values to
     find the row, then binary search that row - because it touches O(log m)
     rows instead of one arbitrary row per probe, which keeps block reads down.
  4. What if duplicates exist and you must return the first occurrence?
     Drop the equality early return, keep moving right = mid - 1 on a match, and
     record mid; the loop then converges on the lowest flat index holding
     target.
TRIGGER
  A 2D grid whose rows are sorted and whose rows join end to end in sorted order
  - treat it as one array of length rows*cols.
C# NOTE
  int[][] is a jagged array, so each row is its own object and matrix[row][col]
  is two dereferences; a true 2D int[,] would let you index matrix[row, col] in
  one step and would also guarantee every row has the same width, removing the
  ragged-input risk.
COMPLEXITY
  Time  : O(log(m*n))
  Space : O(1)
================================================================================
*/
