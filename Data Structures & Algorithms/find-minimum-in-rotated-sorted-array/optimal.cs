// ##########################################################################
// #  optimal.cs            O(log n) time / O(1) space
// #  binary search comparing mid to right   [binary-search-rotated-min]
// #  the only solution in this folder
// #
// #  YOU SOLVED THIS YOURSELF (from submission-0)
// #
// #  halves search space each iteration by comparing nums[mid] to
// #  nums[right] to determine which side contains the minimum
// ##########################################################################

public class Solution
{
    public int FindMin(int[] nums)
    {
        //My Solution
        int left = 0;
        int right = nums.Length - 1;
        while (left < right)
        {
            int mid = left + (right - left) / 2;
            if (nums[mid] > nums[right])
            {
                left = mid + 1;
            }
            else
                right = mid;
        }
        return nums[left];
    }
}

/*
================================================================================
 PATTERN : Binary Search on the Break - compare mid against right
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-0.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  A rotated sorted array is two sorted runs glued together, and every element of
  the left run is strictly greater than every element of the right run. The
  minimum is the first element of the right run - the single point where the
  order breaks. That break is detectable from one comparison, so you never need
  to see the whole array: you only need to know which half of [left, right]
  still contains the break, which is exactly what binary search gives you.
BRUTE FORCE
  Scan all of nums and keep a running minimum, or scan for the one index i with
  nums[i] > nums[i+1] and return nums[i+1]. Both are linear and both throw away
  the fact that the input is sorted-but-for-one-break. The linear scan is the
  right answer only if duplicates are allowed and the worst case collapses
  anyway - see WATCH OUT.
INVARIANT
  The minimum is always inside the closed window [left, right]. It holds at
  entry (window is the whole array) and each branch is chosen to preserve it.
  The loop condition is left < right, not left <= right, because the window is
  never allowed to become empty - it shrinks to exactly one index, and that
  index is the answer. That is why the return is nums[left] with no post-loop
  comparison and no separate answer variable being tracked.
THE TWO BRANCHES
  1. nums[mid] > nums[right]: mid sits in the left (high) run while right sits
  in the right (low) run, so the break is strictly to the right of mid. mid
  itself cannot be the minimum - nums[right] already beats it - so left = mid +
  1 discards mid safely.
  2. nums[mid] <= nums[right]: the stretch mid..right has no break in it, so it
  is plainly sorted and its smallest element is nums[mid]. mid is still a
  candidate, so right = mid and NOT mid - 1. Everything strictly past mid is
  eliminated, which is enough progress.
WHY NUMS[RIGHT] AND NOT NUMS[LEFT]
  Comparing nums[mid] against nums[left] is ambiguous: nums[mid] > nums[left]
  happens both when the array is not rotated at all and when mid is in the left
  run, and those demand opposite moves. Anchoring on nums[right] has no such
  collision, because right is always the tail of the current window. It also
  handles the unrotated input for free: if nums is already sorted, every mid
  satisfies nums[mid] <= nums[right], right walks down to 0, and nums[0] comes
  back with no special case written for it.
TERMINATION
  mid = left + (right - left) / 2 floors, and the loop only runs when left <
  right, so mid < right always. That makes right = mid a strict decrease, and
  left = mid + 1 a strict increase. Neither branch can leave the window the same
  size, so no infinite loop. The same expression also avoids the left + right
  overflow, though with the loop bounds here the anti-overflow property matters
  less than the floor - if mid could equal right, the second branch would spin
  forever.
WATCH OUT
  - right = mid - 1 in the second branch loses the answer whenever mid IS the
  minimum. The asymmetry between mid + 1 and mid is the whole trick; do not tidy
  it into symmetry.
  - Flipping the test to nums[mid] >= nums[right] is still correct here only
  because the problem promises distinct values. With duplicates, nums[mid] ==
  nums[right] tells you nothing about which side the break is on, and the
  standard patch is right-- , which degrades to linear on an input like
  [1,1,1,1,0,1]. That is the Find Minimum in Rotated Sorted Array II variant,
  and this code does not solve it.
  - nums[left] is never read inside the loop, so there is no hidden dependence
  on the left endpoint's value.
TRIGGER
  When a problem hands you a sorted array with one rotation and asks for the
  minimum, the pivot index, the rotation count, or a target lookup, reach for a
  comparison against a fixed endpoint that you know lives on the low side. Pivot
  index is this exact code returning left instead of nums[left]; rotation count
  is the same left. For target search, this loop first, then a normal binary
  search in the correct run.
COMPLEXITY
  Time  : O(log n)
  Space : O(1)
================================================================================
*/
