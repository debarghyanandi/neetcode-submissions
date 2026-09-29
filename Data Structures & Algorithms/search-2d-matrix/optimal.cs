// --------------------------------------------------------------------------
// -  optimal.cs            O(log(m*n)) time / O(1) space
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
 PROBLEM : You get an m x n int matrix. Each row is sorted, and the first
           value of a row is greater than the last value of the row above it.
           Return true if target is in the matrix, else false. Example:
           [[1,3,5,7],[10,11,16,20],[23,30,34,60]], target 3 -> true
 PATTERN : Binary Search on a flattened (virtual) 1D array
================================================================================
IDEA
  Read row by row, the matrix is one sorted list of rows * cols values.
  So run a normal binary search on the virtual indexes 0 to rows*cols-1.
  Map each mid back with row = mid / cols and col = mid % cols.
  This is correct because the index-to-cell mapping keeps the sorted order.
EXAMPLE
  Same matrix, target 13, rows=3, cols=4, left=0, right=11
  mid=5 (1,1)=11 < 13 -> left=6; mid=8 (2,0)=23 > 13 -> right=7
  mid=6 (1,2)=16 > 13 -> right=5; left 6 > right 5 -> return false
COMPLEXITY
  Time  O(log(m*n))  each step halves the rows*cols search range
  Space O(1)         only a few int variables, no recursion
PATH TO OPTIMAL
  Brute force scan of every cell - O(m*n) - uses no ordering at all.
  Binary search each row (suboptimal.cs) - O(m log n) - uses row order.
  One binary search over all cells (this file) - O(log(m*n)) - also uses
  the order between rows, so the whole matrix is one sorted array.
KEYWORDS
  binary search, 2D matrix, flatten index, row-major order, mid / cols
WATCH OUT
  - Divide by cols, not rows: row = mid / cols, col = mid % cols. Using
    rows breaks every non-square matrix.
  - right must be rows * cols - 1. rows * cols alone reads out of bounds.
  - matrix[0].Length throws on an empty matrix. Guard rows == 0 if asked.
  - rows * cols can overflow int for huge inputs. Use long if that matters.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Rows and columns are sorted, but a row need not start above the last?
     -> Start at the top-right corner. Go left if the value is too big, go
        down if too small. O(m + n) time, O(1) space (Search a 2D Matrix II).
  2. Can you do two separate binary searches instead?
     -> Binary search the first column to find the row, then search that row.
        O(log m + log n), the same as log(m*n), and it avoids rows*cols overflow.
  3. Return the position, or where target would be inserted?
     -> Keep the same loop. On a hit return (row, col). On a miss, left is the
        insert index, so return (left / cols, left % cols). Cost stays the same.
TRIGGER
  A grid whose values keep increasing when read row by row is one sorted
  array in disguise, so binary search it with index = row * cols + col.
================================================================================
*/
