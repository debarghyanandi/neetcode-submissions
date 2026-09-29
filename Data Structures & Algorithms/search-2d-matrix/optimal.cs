// --------------------------------------------------------------------------
// -  optimal.cs            O(log(m*n)) time / O(1) space
// -  binary search, treat 2D as 1D   [virtual-1d-binary-search]
// -  ranks above suboptimal.cs (O(m log n) time / O(log n) space)
// -
// -  Reference solution - not one you solved yourself
// -
// -  Single binary search using virtual indexing converts between 1D
// -  positions and 2D coordinates, traversing log(m*n) comparisons.
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
 PATTERN : Binary Search - treat the 2D matrix as one sorted array
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-1.cs when it was first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  rows    number of rows, matrix.Length
  cols    number of columns, matrix[0].Length
  left    lowest virtual index still possible (starts at 0)
  right   highest virtual index still possible (starts at rows * cols - 1)
  mid     virtual index into the flattened matrix
  row     mid / cols, the real row of mid
  col     mid % cols, the real column of mid
WHY THIS PATTERN
  Each row is sorted, and the first value of a row is larger than the last value
  of the row before it. So if you read the matrix row by row, you get one long
  sorted list of rows * cols values. Searching a sorted list points to binary
  search. The code never builds that list. It keeps a virtual index mid and maps
  it back to matrix[row][col] with division and remainder.
BRUTE FORCE
  The simplest correct approach scans every cell and compares it with target.
  That is O(m*n) time and O(1) space. It ignores the sorted order completely. A
  better first attempt is two binary searches: one over the rows to find the row
  whose range could hold target, then one inside that row. That is also O(log m
  + log n) = O(log(m*n)), but it needs two loops and more edge cases, so the
  single flattened search is cleaner.
INVARIANT
  If target is in the matrix, its virtual index is always inside [left, right].
  Each step compares target with matrix[row][col]. If target is larger, every
  index up to mid is too small, so left = mid + 1. If target is smaller, every
  index from mid up is too large, so right = mid - 1. The range gets smaller
  every step. If it becomes empty (left > right), target cannot be anywhere, so
  returning false is correct.
INDEX MAPPING USES COLS, NOT ROWS
  Row-major order means the virtual index is row * cols + col. So row = mid /
  cols and col = mid % cols. Both must divide by cols, the row length. If you
  use rows here by mistake, it still works on square matrices and fails on
  non-square ones. That makes the bug easy to miss in tests.
WATCH OUT
  matrix[0].Length throws if matrix is empty (no rows). Add a guard like rows ==
  0 if the input can be empty. rows * cols is computed in int. For a very large
  matrix it could overflow before the minus 1. mid = left + (right - left) / 2
  avoids overflow in the midpoint, but it does not protect that product.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. What if each row is sorted and each column is sorted, but a row does not
  have to start above the end of the previous row (Search a 2D Matrix II)?
     The flattened list is no longer sorted. Start at the top-right corner. If
     the value is bigger than target, move left. If it is smaller, move down.
     This takes O(m + n) time and O(1) space, which is slower than O(log(m*n)).
  2. How would you return the position, or the insert position, instead of a
  bool?
     Return (mid / cols, mid % cols) on a match. For an insert position, run the
     loop until it ends and use left as the virtual index where target would go.
     Then map left back with the same / and %.
  3. What if the rows have different lengths?
     The simple / and % mapping breaks. Build a prefix-sum array of row lengths,
     which costs O(m) extra space. Then binary search that array to turn a
     virtual index into a row. Or do the two-step search: first find the row,
     then search inside it.
TRIGGER
  A 2D grid where reading it row by row gives one fully sorted sequence, and you
  need to find a value in it.
C# NOTE
  int[][] is a jagged array, so each row can have its own length. This code
  assumes every row has matrix[0].Length columns. It would crash with
  IndexOutOfRangeException on a shorter row. A rectangular int[,] would enforce
  equal row lengths through its type.
COMPLEXITY
  Time  : O(log(m*n))
  Space : O(1)
================================================================================
*/
