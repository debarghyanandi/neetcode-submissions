// ##########################################################################
// #  suboptimal.cs         O(m log n) time / O(log n) space
// #  Recursive binary search per row   [row-wise-binary-search]
// #  ranks below optimal.cs (O(log(m*n)) time / O(1) space)
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  Searches each row independently with recursive binary search; call
// #  stack accumulates O(log n) depth per row, ignoring cross-row ordering.
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
 PATTERN : Row-wise Binary Search - one sorted nums per row
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-0.cs when it was
           first processed
 STATUS  : Suboptimal
================================================================================
WHY THIS PATTERN
  The problem gives a matrix whose rows are sorted left to right, so each `row`
  is a sorted nums and binary search applies directly to it. The outer
  `foreach` walks every row and calls `BinarySearch(0, row.Length - 1, target,
  row)`; the first `true` is returned immediately. This is the obvious reading
  of "sorted rows" and it is correct, but it uses only half of what the
  statement promises.
BETTER APPROACH
  The better approach uses the second guarantee: the first value of each row is
  greater than the last value of the previous row, so the whole matrix is one
  sorted sequence in row-major order. Binary search over the virtual index range
  0 .. m*n-1 and map `mid` back with `row = mid / n`, `col = mid % n`, giving
  O(log(m*n)) time. This file loses because it pays a separate O(log n) search
  for every one of the m rows instead of spending O(log m) to find the single
  candidate row first.
INVARIANT
  Inside `BinarySearch`, the target can only exist in `nums[left..right]`;
  every recursive call preserves that by discarding the half that cannot contain
  it, since `nums[mid] < target` rules out everything at or below `mid`. The
  outer loop keeps a weaker invariant: after finishing row k, the target is not
  in rows 0..k. Because the loop ends only after testing all rows, returning
  `false` at the end means the target is absent from all of them.
IT NEVER USES THE CROSS-ROW ORDERING
  This code only needs each row to be sorted on its own. It stays correct on a
  matrix where row 3 starts below where row 2 ended, which the flat-index binary
  search would get wrong. That is the honest trade: this version is more general
  than the problem requires, and pays m times for generality it does not need.
WATCH OUT
  There is no cheap pre-filter: adding `if (target < row[0] || target >
  row[row.Length - 1]) continue;` before the call would skip most rows for a few
  comparisons each, and it costs one line. `BinarySearch` recurses instead of
  looping, so it holds stack frames while a simple `while (left <= right)` would
  not. A row of length 0 gives `right = -1`, and `left > right` catches it on
  the first call, so that case is safe, but a null `matrix` or a null row
  throws. `found == true` is redundant; `if (found) return true;` says the same
  thing.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Write the O(log(m*n)) version without flattening the matrix into a new
  nums.
     Binary search `lo = 0`, `hi = m*n - 1` and read `matrix[mid / n][mid % n]`
     where `n = matrix[0].Length`. No extra memory, but it now requires every
     row to have the same length and requires the cross-row ordering to hold.
  2. What if rows are sorted and columns are sorted, but a row can start below
  where the previous ended?
     The flat search breaks. Start at the top-right corner: move left when the
     value is too big, move down when it is too small. That is O(m + n) time and
     O(1) space.
  3. Remove the recursion here.
     Replace the helper with a `while (left <= right)` loop updating `left = mid
     + 1` or `right = mid - 1`. Same comparisons, but constant stack instead of
     one frame per halving.
  4. The matrix is far too large to hold in memory and rows arrive from disk one
  at a time.
     Row-wise search is actually the better fit then, since it touches one row
     at a time; add the `row[0]` / last-element range check so most rows are
     rejected after two reads.
TRIGGER
  Sorted order that continues across row boundaries, not just inside a row -
  that is the signal to index the grid as one flat sorted nums.
C# NOTE
  The whole `BinarySearch` helper duplicates `Array.BinarySearch(row, target)`,
  which returns a non-negative index on a hit; `if (Array.BinarySearch(row,
  target) >= 0) return true;` is the same algorithm in one line. Note also that
  `int[][]` is a jagged nums, so `row.Length` can differ per row - the flat
  `mid / n` mapping silently assumes it does not.
COMPLEXITY
  Time  : O(m log n)
  Space : O(log n)
================================================================================
*/
