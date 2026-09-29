// ##########################################################################
// #  suboptimal.cs         O(m log n) time / O(log n) space
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
 PROBLEM : Given an m x n integer matrix, return true if target is in it, else
           false. Each row is sorted ascending. The first value of each row is
           greater than the last value of the row above, so the whole grid is
           one sorted list. Example: [[1,3,5,7],[10,11,16,20],[23,30,34,60]],
           target 3 -> true.
 PATTERN : Binary Search (per row, recursive)
================================================================================
IDEA
  Loop over every row and run a normal binary search on it. BinarySearch
  takes left and right, checks nums[mid], and recurses into the half that
  can still hold target. It stops with false when left > right. It is
  correct because each row is sorted, so a search per row checks every
  cell. Unlike optimal.cs, it ignores that the rows chain into one list.
EXAMPLE
  matrix = [[1,3,5,7],[10,11,16,20],[23,30,34,60]], target = 16
  Row 0: mid values 3, 5, 7, all < 16, then left 4 > right 3 -> false.
  Row 1: mid 1 -> 11 < 16, mid 2 -> 16 == target -> return true.
  Row 2 is never searched. Answer: true.
COMPLEXITY
  Time  O(m log n)  m rows, each binary search halves n cells down to one
  Space O(log n)    recursion depth of BinarySearch is log n stack frames
WATCH OUT
  - It searches rows that cannot hold target. A cheap fix: skip a row when
    row[^1] < target, and break when row[0] > target.
  - Recursion uses log n stack frames. A while loop gives O(1) space, and
    an interviewer may ask for it.
  - Write right = row.Length - 1, not row.Length, or nums[mid] can read past
    the end. An empty row gives right = -1 and returns false safely.
  - Keep mid = left + (right - left) / 2. Writing (left + right) / 2 can
    overflow int on very large indices.
================================================================================
*/
