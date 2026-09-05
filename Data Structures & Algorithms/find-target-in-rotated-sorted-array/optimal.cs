// ##########################################################################
// #  optimal.cs            O(log n) time / O(1) space
// #  single-pass modified binary search, sorted-half detection
// #  [rotated-binary-search-single-pass]
// #  ranks above optimal-variant.cs (O(log n) time / O(1) space)
// #
// #  YOU SOLVED THIS YOURSELF (from submission-0)
// #
// #  each iteration determines which half is sorted and narrows the range
// #  accordingly in one binary search pass
// ##########################################################################

public class Solution {
    public int Search(int[] nums, int target) {
        // my solution
        int left = 0;
        int right = nums.Length - 1;
        while (left <= right)
        {
            int mid = left + (right - left) / 2;
            if (target == nums[mid])
            {
                return mid;
            }
            //which part is sorted.
            if (nums[mid] > nums[right])
            {
                //left is sorted
                if (nums[left] <= target && target < nums[mid])
                    right = mid - 1;
                else
                    left = mid + 1;
            }

            else
            {
                // right half is sorted
                if (nums[mid] < target && target <= nums[right])
                    left = mid + 1;
                else
                    right = mid - 1;
            }

        }
        return -1;
    }
}

/*
================================================================================
 PATTERN : Modified binary search - find the sorted half each step
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-0.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  A single rotation cuts the array into two ascending runs, so target <
  nums[mid] no longer tells you which side to keep. One property does survive
  the rotation: whatever the window [left..right], cutting at mid leaves at
  least one side fully ascending. A fully ascending side is described completely
  by its two endpoints, so membership in it is a single two-sided comparison.
  Identify that side, test the range, discard one half. That is all this loop
  does.
INVARIANT
  At the top of every iteration: if target exists in nums, its index lies in
  [left, right]. Every branch discards a half only after proving target cannot
  be there - either target falls inside the sorted side's endpoint range (keep
  that side) or it does not (so it can only be in the other side, or nowhere).
WHY MID IS COMPARED TO RIGHT
  nums[mid] > nums[right] means the drop-off (the pivot) sits strictly between
  mid and right, so nums[left..mid] is one unbroken ascending run. Otherwise the
  pivot is at or before mid, so nums[mid..right] is the run. The else branch
  also absorbs mid == right, which happens when left == right since the mid
  formula floors toward left: there nums[mid] == nums[right], the code calls the
  right half sorted, and that half is the single cell already compared against
  target at the top of the loop. Its range test nums[mid] < target && target <=
  nums[right] cannot pass, so right = mid - 1 and the loop exits with -1. That
  is why the equal case needs no arm of its own.
THE RANGE TESTS ARE TWO-SIDED ON PURPOSE
  Left-sorted: nums[left] <= target && target < nums[mid]. Right-sorted:
  nums[mid] < target && target <= nums[right]. Both exclude nums[mid] because
  equality already returned mid at the top, and both are inclusive at the far
  endpoint because nums[left] and nums[right] are real candidates still in the
  window. Dropping either bound breaks it: testing only target < nums[mid] in
  the left-sorted case would send a very small target into the left run, when a
  value below nums[left] can only live in the rotated right run.
TERMINATION
  mid = left + (right - left) / 2 floors, so left <= mid <= right always, and
  mid < right whenever left < right. Each non-returning branch sets left = mid +
  1 or right = mid - 1, so the window strictly shrinks every pass and left <=
  right must eventually fail. The subtract-then-halve form is also what keeps
  left + right from overflowing int on a very large array.
WATCH OUT
  1. Distinct values are assumed. With duplicates (the LeetCode 81 variant)
  nums[mid] > nums[right] no longer identifies the sorted half - in [1,0,1,1,1]
  the comparison is 1 > 1 = false, and the right half is not sorted. The repair
  is a separate right-- arm when nums[mid] == nums[right], which gives up the
  halving in the worst case.
  2. The other common pivot test, nums[left] <= nums[mid], is equally valid but
  has a different small-window case analysis (left == mid makes it trivially
  true). If you swap the pivot test, re-derive both range tests rather than
  pasting the ones here.
TRIGGER
  Reach for this whenever a global sort order is broken by a bounded amount but
  any cut still leaves one provably ordered side - rotated arrays being the
  canonical case. The alternative shape, binary-search the pivot first and then
  run a plain binary search on the correct run, is also correct, but it is two
  passes and two off-by-one surfaces to get right; this single loop is the
  version worth having in muscle memory.
COMPLEXITY
  Time  : O(log n)
  Space : O(1)
================================================================================
*/
