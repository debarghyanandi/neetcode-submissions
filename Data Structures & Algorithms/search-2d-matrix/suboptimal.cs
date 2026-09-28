// ##########################################################################
// #  suboptimal.cs         O(m log n) time / O(log n) space
// #  recursive binary search per row   [per-row-recursive-search]
// #  ranks below optimal.cs (O(log(m*n)) time / O(1) space)
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  Iterates m rows and recursively binary searches each row; recursion
// #  depth is log n on row length.
// ##########################################################################

public class Solution
{
    public bool SearchMatrix(int[][] matrix, int target)
    {
        // My solution
        // this is good but mLogn
        // we need log(m*n)
        foreach (int[] row in matrix)
        {
            bool found = BinarySearch(0, row.Length - 1, target, row);
            if (found == true)
                return found;
        }
        return false;
    }

    private bool BinarySearch(int left, int right, int target, int[] nums)
    {
        if (left > right)
            return false;

        int mid = left + (right - left) / 2;

        if (nums[mid] == target)
            return true;

        if (nums[mid] < target)
            return BinarySearch(mid + 1, right, target, nums);

        return BinarySearch(left, mid - 1, target, nums);
    }
}

/*
================================================================================
 PATTERN : Binary Search - one search per row
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-0.cs when it was
           first processed
 STATUS  : Suboptimal
================================================================================
VARIABLES
  found    true if the current row holds target
  mid      middle index of nums between left and right
WHY THIS PATTERN
  Each row is sorted, so a value can be found in one row by halving the range.
  SearchMatrix walks every row with foreach and calls BinarySearch on it.
  BinarySearch compares nums[mid] with target and keeps only the half that can
  still hold it. The loop stops as soon as found is true.
BETTER APPROACH
  The better approach uses the fact that each row starts after the previous row
  ends. That makes the whole matrix one sorted list of m*n values. Run one
  binary search over index idx from 0 to m*n - 1, and read matrix[idx / n][idx %
  n], where n is the row length. That costs O(log(m*n)) time. This file loses
  because it runs a full search on all m rows. It never uses the order between
  rows, so it also searches rows whose first value is already larger than
  target.
INVARIANT
  In every BinarySearch call, if target is in nums, it lies inside
  nums[left..right]. When nums[mid] < target, everything at or left of mid is
  too small, so mid + 1 becomes the new left. When nums[mid] > target, the same
  logic moves right to mid - 1. When left > right, the range is empty, so target
  is not in this row. Since every row is checked, returning false after the loop
  is correct.
WATCH OUT
  The mid formula left + (right - left) / 2 cannot overflow. If you change it to
  (left + right) / 2, the sum can overflow on very large arrays. The comment
  "this is good but mLogn" is correct: the loop has no early exit for rows that
  start above target, and no early exit for rows that end below it. An empty row
  gives right = -1, so the call returns false at once. That case is safe, but
  the code only handles it by luck of the left > right check.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you remove the recursion?
     Yes. Use a while (left <= right) loop that moves left or right. The logic
     is the same, and the call stack no longer grows, so extra space becomes
     O(1).
  2. How do you do it without the flat index math?
     Use two binary searches. First search the first column to find the last row
     whose first value is <= target. Then search only that row. The time is
     O(log m + log n), which is the same as O(log(m*n)).
  3. What if each row and each column is sorted, but a row does not start after
  the previous row ends?
     The flat view no longer works. Start at the top-right corner. If the value
     is too big, move left. If it is too small, move down. This takes O(m + n)
     steps.
TRIGGER
  A 2D grid where the values read in sorted order row by row should make you
  think of one binary search over a flattened index.
C# NOTE
  Array.BinarySearch(row, target) >= 0 does the same job as the hand-written
  helper. It returns a negative number when the value is missing. The check
  found == true can also be written as just found.
COMPLEXITY
  Time  : O(m log n)
  Space : O(log n)
================================================================================
*/
