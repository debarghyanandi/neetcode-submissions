// ##########################################################################
// #  suboptimal.cs         O(m log n) time / O(log n) space
// #  recursive binary search per row   [binary-search-per-row-recursive]
// #  ranks below optimal.cs (O(log(m*n)) time / O(1) space)
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  Iterates m rows, each invoking recursive binary search at O(log n)
// #  depth on call stack.
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
 PATTERN : Binary search per row - misses the flattened single search
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-0.cs when it was
           first processed
 STATUS  : Suboptimal
================================================================================
VARIABLES
  found    true if target sits in the current row
  mid      middle index of the live range [left, right]
WHY THIS PATTERN
  Each row of the matrix is sorted left to right, so inside one row the classic
  halving search applies: compare target to nums[mid] and throw away the half
  that cannot contain it. SearchMatrix walks every row with foreach and calls
  BinarySearch(0, row.Length - 1, target, row), returning as soon as one row
  reports true. The pattern is correct because a sorted row gives you the
  ordering needed to decide which half to keep.
BETTER APPROACH
  The better approach uses the second guarantee of this problem: the first value
  of each row is larger than the last value of the previous row, so the whole
  matrix read row by row is one sorted sequence of m*n numbers. Search that
  sequence once with indices idx / n for the row and idx % n for the column,
  which gives O(log(m*n)). This file loses because it never looks at that
  cross-row ordering; it only assumes each row is sorted on its own, so it pays
  one full search for every row instead of one search total.
INVARIANT
  Inside BinarySearch, target can only be at an index in [left, right]; every
  recursive call preserves this by discarding the half proven too small
  (nums[mid] < target, so go mid + 1..right) or too large (left..mid - 1). When
  left > right the range is empty, so target is absent from that row. Since the
  outer loop tries every row, a false answer means target is in no row, and that
  is the right answer.
WATCH OUT
  row.Length - 1 is -1 for an empty row, which is safe here only because the
  left > right guard runs before any indexing - do not move that check below the
  mid computation. A null matrix throws in foreach, and a null row throws inside
  BinarySearch; nothing validates either. if (found == true) then return found
  is a roundabout way of writing if (found) return true, and the same shape
  invites the classic typo of writing = instead of ==. The code and the comment
  agree here: the comment already admits this is m log n and that log(m*n) is
  wanted, so do not present this file as the intended answer.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Remove the recursion - what changes?
     Turn BinarySearch into a while (left <= right) loop that reassigns left =
     mid + 1 or right = mid - 1. Same comparisons, but the call stack
     disappears, so the extra space becomes constant.
  2. What if only each row is sorted, with no relation between rows?
     Then this file is already the right shape - the flattened search is invalid
     because the full sequence is no longer sorted, and per-row searching is the
     best simple option.
  3. What if rows are sorted left to right AND columns sorted top to bottom, but
  rows do not chain (LeetCode 240)?
     Start at the top-right corner and step left when the value is too big, down
     when too small. That is O(m + n) and needs no binary search at all.
  4. The matrix is far too large to hold in memory and rows arrive one at a
  time?
     With the chaining guarantee you can compare target against a row's first
     and last element and skip the row in O(1), doing the inner search only for
     the single row whose range covers target.
TRIGGER
  A 2D grid described as sorted, where each row's first value exceeds the
  previous row's last value - that phrase means treat it as one sorted array,
  not m arrays.
C# NOTE
  Because matrix is int[][] (a jagged array of row references), foreach hands
  you each row with no copying and row.Length is that row's own width; a
  rectangular int[,] would force matrix.GetLength(1) and index pairs instead,
  and could not be iterated row by row like this.
COMPLEXITY
  Time  : O(m log n)
  Space : O(log n)
================================================================================
*/
