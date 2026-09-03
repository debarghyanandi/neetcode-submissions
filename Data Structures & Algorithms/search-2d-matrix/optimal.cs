// --------------------------------------------------------------------------
// -  optimal.cs            O(log(m*n)) time / O(1) space
// -  treat matrix as one flattened sorted array and binary search via index
// -  conversion
// -  [flattened-binary-search]
// -  ranks above suboptimal.cs (O(m log n) time / O(log n) space)
// -
// -  Reference solution - not one you solved yourself (from submission-1)
// -
// -  single binary search over the virtual 1D index space using div/mod to
// -  map back to row/col, done iteratively so no extra space
// --------------------------------------------------------------------------

public class Solution {
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
 PATTERN : Binary search over the flattened 2D index
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-1.cs when it was first processed
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  The problem's precondition is stronger than "every row is sorted": each row is
  sorted AND the first value of a row exceeds the last value of the row above
  it. That means reading the matrix left-to-right, top-to-bottom yields one
  fully sorted sequence of rows*cols values. Once you see that, there is no 2D
  structure left to exploit - it is an ordinary sorted array that happens to be
  stored in chunks of width cols, so one plain binary search over indices 0 ..
  rows*cols-1 is enough. No row-locating pass first, no second search inside the
  row.
ALGORITHM
  1. cols = matrix[0].Length is the stride; it is the only geometry the search
  needs.
  2. Search the virtual index range: left = 0, right = rows*cols - 1.
  3. mid = left + (right - left) / 2, then decode it: row = mid / cols, col =
  mid % cols.
  4. Compare target against matrix[row][col]: greater means the answer sits
  after mid, so left = mid + 1; smaller means before, so right = mid - 1; equal
  returns true.
  5. left > right means the range emptied without a hit - return false.
INVARIANT
  At the top of every iteration, if target is present in the matrix its virtual
  index lies in [left, right]. Each branch preserves this because the flattened
  sequence is sorted: everything at index <= mid is <= matrix[row][col], so when
  target is strictly greater it cannot be at or below mid, and symmetrically for
  the other branch. Termination is separate and just as important to state:
  every non-returning iteration either raises left or lowers right by at least
  one, since mid is always in [left, right], so the range strictly shrinks and
  the loop cannot spin.
WATCH OUT
  - Decode with cols, not rows. row = mid / cols and col = mid % cols; dividing
  by rows still passes on every square test matrix and silently fails on a 3x4.
  This is the single most common bug in this solution.
  - right = rows*cols - 1, not rows*cols. The loop condition is left <= right,
  so an inclusive upper bound is required; using rows*cols would let mid address
  one past the end.
  - matrix[0] is dereferenced before any bounds check, so a zero-row input
  throws rather than returning false. A zero-column input is survivable by
  accident: right becomes -1 and the loop body never executes.
  - mid = left + (right - left) / 2 rather than (left + right) / 2 is the
  habitual overflow-safe form. On the constraints here rows*cols is small, so
  this is discipline, not a fix for a live bug in this file.
TRIGGER
  Reach for the flatten-then-binary-search shape when a 2D array is globally
  sorted in row-major order and you need membership or position. The tell is the
  second half of the precondition - the cross-row guarantee. If the statement
  only promises sorted rows and sorted columns independently, this approach is
  wrong, not merely slower: the flattened sequence is no longer monotonic and
  the comparisons discard the wrong half.
FOLLOW-UP
  - "Rows and columns are each sorted, but a row can start below the previous
  row's end" (Search a 2D Matrix II): the flattening breaks. Walk a staircase
  from the top-right corner - move left when the value is too big, down when too
  small - for O(m + n) with no binary search at all.
  - "Return where it would be inserted instead of a bool": drop the equality
  return and run the loop to exhaustion; left is then the insertion index,
  decoded the same way with left / cols and left % cols.
  - "Why not binary search the first column for the row, then the row itself?":
  it is correct and the same asymptotics, but it is two loops and two sets of
  boundary conditions instead of one. The virtual index collapses them.
COMPLEXITY
  Time  : O(log(m*n))
  Space : O(1)
================================================================================
*/
