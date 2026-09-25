// ##########################################################################
// #  optimal.cs            O(log n) time / O(log n) space
// #  Binary search, recursive   [binary-search-recursive]
// #  the only solution in this folder
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  Each recursive call halves the search space; the call stack depth
// #  reaches logarithmic maximum.
// ##########################################################################

public class Solution
{
    public int Search(int[] nums, int target)
    {
        // My Solution
        return Search(0, nums.Length - 1, target, nums);
    }

    private int Search(int left, int right, int searchTarget, int[] nums)
    {
        if (left > right)
            return -1;

        int mid = left + (right - left) / 2;

        if (nums[mid] == searchTarget)
            return mid;

        if (searchTarget < nums[mid])
            return Search(left, mid - 1, searchTarget, nums);

        return Search(mid + 1, right, searchTarget, nums);
    }
}

/*
================================================================================
 PATTERN : Binary Search on a sorted array - recursive halving
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-1.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  left           lowest index still possible, inclusive
  right          highest index still possible, inclusive
  searchTarget   the value being looked for, renamed from target in the helper
  mid            midpoint of [left, right], computed overflow-safe
WHY THIS PATTERN
  The input nums is sorted, so one comparison against nums[mid] tells you which
  half can still hold searchTarget. Each call throws away the other half, so the
  range [left, right] shrinks by about half every step. The public Search just
  seeds the range with 0 and nums.Length - 1 and lets the private overload do
  the work.
BRUTE FORCE
  The first thing most people write is a linear scan: loop i from 0 to
  nums.Length - 1 and return i when nums[i] == target. That is correct and O(n)
  time with O(1) space. It loses because it ignores the sorted order - it still
  touches every element for a value that is not present, while this code touches
  about log2(n) of them.
INVARIANT
  At every call, if searchTarget exists in nums, its index lies inside [left,
  right]. The two recursive calls keep that true: when searchTarget < nums[mid]
  every index from mid upward holds a value that is too big, so [left, mid - 1]
  is safe; otherwise every index up to mid is too small, so [mid + 1, right] is
  safe. The range strictly shrinks each time, so the recursion ends either on a
  hit at nums[mid] or with left > right, which means the value was never in the
  array.
MIDPOINT WITHOUT OVERFLOW
  mid is computed as left + (right - left) / 2 instead of (left + right) / 2.
  With int indices, left + right can exceed int.MaxValue on a very large array
  and wrap to a negative number, which would make nums[mid] throw. The
  subtraction form never builds a value larger than right, so it is always in
  range.
WATCH OUT
  Correctness depends on nums being sorted in ascending order; the code never
  checks this, and on unsorted input it silently returns -1 or a wrong index. If
  nums holds duplicates of searchTarget, the returned index is whichever one mid
  happens to land on, not the first or last. The empty array is handled by luck,
  not by an explicit guard: nums.Length - 1 gives right = -1, and the left >
  right check catches it before any indexing. The comment "// My Solution" says
  nothing about the algorithm and should be dropped.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you remove the recursion?
     Replace the calls with a while (left <= right) loop that reassigns right =
     mid - 1 or left = mid + 1. Same time, and space drops to O(1) because there
     are no stack frames to hold.
  2. What if the array can contain duplicates and you must return the first
  occurrence?
     Do not return on the first equality. When nums[mid] == searchTarget, record
     mid as a candidate and keep searching [left, mid - 1]; the last candidate
     recorded is the leftmost index. Still O(log n), but it always runs the full
     log2(n) steps instead of stopping early.
  3. What if the array is sorted but rotated at an unknown pivot?
     Still binary search, but at each step decide which side of mid is the
     normally sorted half, then check whether searchTarget lies inside that
     half's value range to pick the direction. Complexity is unchanged; the
     branch logic gets longer.
  4. The data is too large to fit in memory and lives on disk or behind an API.
     The same index math works, but each nums[mid] becomes a fetch, so the cost
     model changes from comparisons to I/O. You would switch to a B-tree style
     layout or fetch blocks around mid so that one read serves several
     comparisons.
TRIGGER
  The input is already sorted (or the answer is monotonic: false, false, then
  true) and you are asked for a single position or value.
C# NOTE
  The int[] nums is passed as the last argument on every recursive call, which
  copies only the reference, not the elements. If you want the library version,
  Array.BinarySearch(nums, target) does the same job but returns the bitwise
  complement of the insertion point when the value is missing, not -1, so the
  caller must translate the result.
COMPLEXITY
  Time  : O(log n)
  Space : O(log n)
================================================================================
*/
