// ##########################################################################
// #  suboptimal.cs         O(m log n) time / O(log n) space
// #  binary search each row independently   [row-wise-binary-search]
// #  ranks below optimal.cs (O(log(m*n)) time / O(1) space)
// #
// #  YOU SOLVED THIS YOURSELF (from submission-0)
// #
// #  runs a full binary search on every row, and the recursive BinarySearch
// #  adds O(log n) call-stack depth since C# does not guarantee tail-call
// #  optimization
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
 PATTERN : Binary search per row - rows searched independently
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-0.cs when it was
           first processed
 STATUS  : Suboptimal
================================================================================
WHAT IT ACTUALLY USES
  The problem gives two guarantees: (1) every row is sorted left to right, and
  (2) the first element of a row is greater than the last element of the row
  above it. This code uses only (1). The foreach over matrix treats each row as
  an unrelated sorted array and pays for a fresh BinarySearch on every one of
  them. Guarantee (2) - the one that makes the whole matrix a single sorted
  sequence - is never touched. That single sentence is both the correctness
  story and the reason this is marked suboptimal.
INVARIANT
  BinarySearch(left, right, target, nums) holds: if target occurs anywhere in
  nums, its index lies in [left, right]. Every recursive call preserves it.
  nums[mid] < target means target cannot sit at mid or below, so the window
  becomes [mid+1, right]; nums[mid] > target means it cannot sit at mid or
  above, so the window becomes [left, mid-1]. Both branches strictly shrink the
  window, so left > right is reached in finite steps, and at that point the
  window is empty - the invariant then says target is not in nums at all, which
  is exactly what the false return claims.
CORRECTNESS OF THE OUTER LOOP
  The rows partition the matrix: every cell belongs to exactly one row. So
  target is in matrix if and only if it is in some row. The loop returns true
  the moment any row reports found, and returns false only after every row has
  been searched and rejected - complete on one side, sound on the other. Note
  this argument needs no ordering between rows, which is precisely the point
  made above.
WHY THIS LOSES
  Use guarantee (2) and the matrix is one sorted array of length m*n that
  happens to be stored in chunks. Take n = matrix[0].Length, run a single binary
  search over lo = 0, hi = m*n - 1, and decode each probe as matrix[mid / n][mid
  % n]. One search, not m of them. The equivalent two-step version is easier to
  get right under pressure: binary search the column of row-first (or row-last)
  elements to pick the one row that could contain target, then binary search
  that row. Same asymptotics, no div/mod indexing to fumble.
  The honest trade this file makes: because it reads row.Length per row and
  never assumes rows line up, it works on a ragged matrix. The flattened search
  hard-requires a rectangle, since mid / n and mid % n assume a uniform row
  width.
THE FOLLOW-UP
  "Rows are sorted, but drop the guarantee that each row starts after the
  previous one ends - what now?" Then this file is the right answer, not a
  fallback. An adversary can place target in any row you did not look at, so any
  correct algorithm must inspect at least one element of every row; the per-row
  binary search is the natural way to pay that. Being able to name which
  precondition each solution consumes is the whole point of the question.
WATCH OUT
  1. An empty row is already safe: right = row.Length - 1 = -1, so the first
  call hits left(0) > right(-1) and returns false without indexing. An empty
  matrix skips the loop and returns false. No guard needed for either.
  2. mid = left + (right - left) / 2 rather than (left + right) / 2 - the
  overflow-proof form. Keep the habit even when the bounds are small.
  3. Both recursive calls are in tail position, so converting BinarySearch to a
  while loop that reassigns left and right is a mechanical edit and removes the
  call frames entirely.
  4. if (found == true) return found; is just if (found) return true; - the
  comparison against a bool adds nothing.
COMPLEXITY
  Time  : O(m log n)
  Space : O(log n)
================================================================================
*/
