// ##########################################################################
// #  optimal.cs            O(log n) time / O(log n) space
// #  recursive binary search   [binary-search-recursive]
// #  the only solution in this folder
// #
// #  YOU SOLVED THIS YOURSELF (from submission-1)
// #
// #  Halves the search range each call; recursion stack depth is O(log n)
// #  since it's not tail-call optimized in C#.
// ##########################################################################

public class Solution {
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
 PATTERN : Binary Search - recursive halving of a sorted range
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-1.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
THE INVARIANT
  If target is present in nums, its index lies inside [left, right]. Every
  recursive call is only allowed because it preserves that.

  When searchTarget < nums[mid]: nums is sorted ascending, so every index >= mid
  holds a value >= nums[mid] > searchTarget. Nothing at or right of mid can
  match, and dropping to [left, mid - 1] loses no candidate. The mirror argument
  covers searchTarget > nums[mid] and [mid + 1, right].

  So the only way to exit without a match is the range going empty, which is
  exactly what left > right tests. The -1 is not a guess - it is the invariant
  saying the candidate window is empty.
WHY IT TERMINATES
  mid is always in [left, right], and both recursive calls exclude mid. The
  window therefore shrinks by at least one element per call, so it cannot spin.

  Work the tightest case by hand, because that is where a broken binary search
  hangs. With left == right, (right - left) / 2 is 0 so mid == left. Either
  nums[mid] matches and it returns, or it recurses into (left, left - 1) or
  (left + 1, left). Both immediately trip left > right. No frame ever
  re-searches a range it already saw.
THE MID FORMULA
  left + (right - left) / 2 rather than (left + right) / 2. The two agree on
  every value for this problem's constraints, so this is not the source of a
  wrong answer here - it is the habit you want reflexively. (left + right)
  overflows int once both indices exceed roughly 1.07 billion, producing a
  negative mid and an IndexOutOfRangeException. The subtraction form never
  builds a sum larger than right.

  Because the division truncates, mid biases toward left. That is only a naming
  detail here, but it is the whole ballgame in the boundary variants below.
EDGE CASES ALREADY COVERED
  Empty array: nums.Length - 1 is -1, so the first call is Search(0, -1, ...)
  and left > right fires before any indexing. No length guard is needed at the
  public entry point.

  Single element: handled by the left == right trace above.

  Target outside the range entirely: the window walks to one end and collapses.
  No special casing for target < nums[0] or target > nums[^1] is required, and
  adding it buys nothing.
THE OFF-BY-ONE TRAP
  Two mutations of this file both compile and both silently break it.

  Writing Search(left, mid, ...) instead of Search(left, mid - 1, ...) keeps mid
  in the window. With left == right - 1, mid == left, and the call repeats the
  identical range forever - a stack overflow, not a wrong answer.

  Changing the base case to left >= right drops the single-element window
  without inspecting it, so any target sitting at a position that is reached
  only when left == right returns -1. That miss is data-dependent and will pass
  a small hand-written test while failing on some larger array.
THE FOLLOW-UP TO REHEARSE
  Both recursive calls sit in return position with nothing after them, so the
  loop rewrite is mechanical: keep left and right as locals, loop while left <=
  right, and replace Search(left, mid - 1, ...) with right = mid - 1 and
  Search(mid + 1, right, ...) with left = mid + 1. Same comparisons in the same
  order; the stack frames just stop existing. Have that conversion ready - it is
  the standard ask after this file.

  The other likely probe is duplicates. This code returns on the first equality
  it finds, so with repeated values it returns an arbitrary matching index. To
  get the leftmost occurrence you stop returning early: on nums[mid] ==
  searchTarget record mid and keep going left with right = mid - 1. Rightmost is
  the same with left = mid + 1. That variant is the base for lower_bound /
  upper_bound style questions.
COMPLEXITY
  Time  : O(log n)
  Space : O(log n)
================================================================================
*/
