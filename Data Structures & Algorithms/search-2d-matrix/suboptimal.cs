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
 PATTERN : Binary Search - one search per row
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-0.cs when it was
           first processed
 STATUS  : Suboptimal
================================================================================
VARIABLES
  left, right  inclusive bounds of the part of nums still being searched
  mid          middle index, written as left + (right - left) / 2
  found        true if the current row holds target
WHY THIS PATTERN
  Each row is sorted, so a binary search can find target in one row in log time.
  SearchMatrix runs BinarySearch on every row until one of them returns true.
  This works, but it only uses the fact that each row is sorted. It ignores the
  stronger rule that the first value of each row is bigger than the last value
  of the row before it.
BETTER APPROACH
  The better approach is one binary search over the whole matrix, read as a
  single sorted list of m*n values. Map a flat index mid to matrix[mid / n][mid
  % n], where n is the row length. That takes O(log(m*n)) time. This file loses
  because it searches every row. The rows are also sorted relative to each
  other, so it could skip any row whose first value is greater than target or
  whose last value is less than target.
INVARIANT
  In BinarySearch: if target is anywhere in nums, it is inside
  nums[left..right]. Each step compares nums[mid] with target and removes the
  half that cannot hold it, so the range gets smaller every call. When left >
  right, the range is empty, and target is not in the row. In the foreach loop:
  every row already checked does not contain target, so returning false after
  the loop is correct.
WATCH OUT
  The comment "this is good but mLogn" is right about the cost, but "good" hides
  the real miss: a row whose values are all larger than target is still
  searched. Because the rows are ordered, once row[0] > target no later row can
  match, and the loop could stop there. A row of length 0 is handled safely:
  right is -1, so left > right and the method returns false at once. The code
  does not guard against a null row, though.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you remove the recursion?
     Yes. Use a while (left <= right) loop that moves left or right. This gives
     O(1) extra space instead of O(log n) for the call stack. The logic stays
     the same.
  2. What if only each row and each column are sorted, and one row does not have
  to start after the previous row ends?
     A single flat binary search no longer works. Start at the top-right corner.
     If the value is bigger than target, move left. If it is smaller, move down.
     This takes O(m + n) time and O(1) space.
  3. How would you do it as two searches instead of one flat search?
     First, binary search on the first value of each row (matrix[r][0]) to find
     the last row that starts at or below target. Then binary search inside that
     row. This is O(log m + log n), the same as O(log(m*n)), and you never need
     the index math with / and %.
  4. In the flat version, what happens if m*n is very large?
     m*n can overflow int. Compute the high bound and mid as long, then cast
     back to int when you index the row and the column.
TRIGGER
  If a 2D grid, read row by row, forms one sorted sequence, treat it as a flat
  sorted array and run a single binary search.
C# NOTE
  Array.BinarySearch(row, target) >= 0 does the same job as the hand-written
  helper, and it is iterative. Also, "if (found == true) return found;" can be
  written as "if (found) return true;".
COMPLEXITY
  Time  : O(m log n)
  Space : O(log n)
================================================================================
*/
