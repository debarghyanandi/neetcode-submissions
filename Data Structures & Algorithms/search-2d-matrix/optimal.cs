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
 PATTERN : Binary Search on a flattened 2D matrix
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-1.cs when it was first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  left    lowest virtual index still possible (0 .. rows*cols-1)
  right   highest virtual index still possible
  mid     virtual index, as if the matrix were one long sorted array
  row     mid / cols = the real row of mid
  col     mid % cols = the real column of mid
WHY THIS PATTERN
  Each row is sorted, and the first value of each row is bigger than the last
  value of the row before it. So if you read the rows one after another, you get
  one long sorted list of rows*cols values. A sorted list with a "find the
  target" question points straight to binary search. The code never builds that
  list. It searches the virtual index range [left, right] and turns each mid
  into (row, col) only when it needs to read a value.
BRUTE FORCE
  Check every cell with two nested loops and return true on a match. This is
  O(m*n) time. It is correct, but it ignores both sorted properties, so it reads
  every cell when a halving search reads only a few. A middle step is to scan
  each row's first and last value to find the right row, then binary search that
  row. This is O(m + log n), and it is still slower than one search over the
  whole range.
INVARIANT
  If target is in the matrix, its virtual index is always inside [left, right].
  When target > matrix[row][col], every index up to mid holds a value that is
  too small, so left = mid + 1 is safe. When target < matrix[row][col], every
  index from mid up is too big, so right = mid - 1 is safe. The range gets
  smaller on every step. When left > right, the range is empty, so returning
  false is correct.
DIVIDE BY COLS, NOT ROWS
  The mapping is row = mid / cols and col = mid % cols. Each full row holds cols
  values, so the row length is the number to divide by. Using rows here reads
  the wrong cell on any non-square matrix. On a square matrix it still looks
  correct, which makes this bug easy to miss in tests.
WATCH OUT
  matrix[0].Length throws if the matrix has no rows. It needs a guard like "if
  (matrix.Length == 0) return false". The code takes cols from row 0 only. If
  the jagged array has rows of different lengths, the index mapping is wrong and
  matrix[row][col] can go out of range. rows * cols is int math, so a very large
  matrix could overflow and give a negative right. The inline comments match
  what the code does.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. What if each row is sorted and each column is sorted, but a row's first
  value can be smaller than the previous row's last value?
     The flat list is no longer sorted, so this search fails. Start at the
     top-right corner. Move left if the value is too big, and move down if it is
     too small. That is O(m + n) time with O(1) space.
  2. Return the (row, col) position instead of a bool.
     Return new[] { row, col } where the code now returns true, and return { -1,
     -1 } at the end. The cost does not change.
  3. If the target is missing, where would it be inserted?
     Keep the same loop. When it ends, left is the first virtual index whose
     value is bigger than target. Map it with left / cols and left % cols. If
     left == rows*cols, the target goes after the last cell.
  4. Can you do it as two separate binary searches?
     Yes. First binary search on the first column to find the last row whose
     first value is <= target. Then binary search inside that row. The time is
     the same, and it avoids the rows*cols product entirely.
TRIGGER
  A 2D grid where reading the rows in order gives one fully sorted sequence, and
  the question is "does this value exist" or "where is it".
C# NOTE
  C# integer math is unchecked by default, so rows * cols wraps around silently
  instead of throwing. Writing checked(rows * cols), or doing the index math in
  long, turns a hidden wrong answer into a clear error.
COMPLEXITY
  Time  : O(log(m*n))
  Space : O(1)
================================================================================
*/
