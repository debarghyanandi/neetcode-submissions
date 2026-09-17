// --------------------------------------------------------------------------
// -  optimal.cs            O(log(m*n)) time / O(1) space
// -  Binary search on flattened matrix   [flattened-binary-search]
// -  ranks above suboptimal.cs (O(m log n) time / O(log n) space)
// -
// -  Reference solution - not one you solved yourself
// -
// -  Single binary search over virtual 1D index space treats sorted 2D
// -  matrix as one array using div/mod conversion.
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
            else return true;
        }
        return false;
    }
}

/*
================================================================================
 PATTERN : Binary Search on a flattened sorted matrix
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-1.cs when it was first processed
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  The matrix rows are sorted and each row starts after the previous row ends, so
  reading it row by row gives one fully sorted sequence. That means the whole
  grid behaves like a single sorted array of length rows*cols, and binary search
  is the standard tool for a sorted array. The code never materializes that
  array: it searches the index range left=0 to right=rows*cols-1 and maps each
  mid back with row = mid / cols and col = mid % cols.
BRUTE FORCE
  Walk every cell with two nested loops and compare with target. That is O(m*n)
  time and O(1) space, and it is correct, but it throws away the ordering the
  problem hands you. A middle step is to binary search the first column to pick
  the row, then binary search that row - O(log m + log n), which is the same as
  O(log(m*n)) but takes two loops instead of one.
INVARIANT
  At the top of every iteration, if target exists anywhere in the matrix, its
  flattened index lies inside [left, right]. Each comparison against
  matrix[row][col] is a comparison against the sorted sequence value at index
  mid, so discarding mid and everything below it (left = mid + 1) or mid and
  everything above it (right = mid - 1) can never drop the target. The range
  shrinks by at least one each pass, so the loop ends; when left > right the
  range is empty and returning false is correct.
MID WITHOUT OVERFLOW
  mid = left + (right - left) / 2 is used instead of (left + right) / 2. With
  right = rows*cols - 1 the sum of two large indices could exceed int.MaxValue,
  and this form keeps every intermediate value inside the current range. It
  matters more here than in a normal array search because the search space is
  the product of two dimensions, not one.
WATCH OUT
  matrix[0].Length runs before any check, so an empty outer array (matrix.Length
  == 0) throws IndexOutOfRangeException rather than returning false. cols is
  read once from row 0, so a ragged input where some row is shorter would make
  matrix[row][col] throw - the code assumes a true rectangle. rows * cols itself
  is computed as int, so a grid large enough to overflow that product silently
  produces a wrong right bound. Also note matrix[row][col] is evaluated twice
  per iteration when the first comparison fails.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. The matrix is sorted left to right and top to bottom, but a row no longer
  starts after the previous row ends. What changes?
     The flatten trick dies, since the sequence is no longer sorted. Start at
     the top-right corner and move left when the value is too big, down when too
     small - O(m + n) time, still O(1) space.
  2. Return the coordinates of target instead of a bool.
     Replace "return true" with returning row and col, which are already
     computed, and return something like (-1, -1) on the miss. No extra cost.
  3. Return the index where target should be inserted if it is absent.
     Switch to a lower-bound loop: use while (left < right), never return early
     on equality, and after the loop left is the insertion index. The trade-off
     is you lose the early exit, so it always runs the full log number of steps.
  4. How would you handle a matrix so large that rows*cols does not fit in an
  int?
     Make left, right and mid long, and cast back to int for the row and col
     division. The comparison logic is unchanged; only the index arithmetic
     widens.
TRIGGER
  A grid whose rows are sorted and where each row's first value is larger than
  the previous row's last value - that is a sorted array wearing a 2D costume.
C# NOTE
  int[][] is a jagged array, so matrix[row][col] is two pointer hops plus two
  bounds checks; caching int val = matrix[row][col] once per iteration would
  remove the duplicate lookup in the else-if. A true rectangular int[,] would
  make the shape assumption explicit, at the cost of a different indexing syntax
  and losing matrix[0].Length in favour of GetLength(1).
COMPLEXITY
  Time  : O(log(m*n))
  Space : O(1)
================================================================================
*/
