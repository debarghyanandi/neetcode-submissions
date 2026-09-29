// ##########################################################################
// #  optimal.cs            O(log n) time / O(1) space
// #  Binary search on rotated array   [binary-search-rotated-array]
// #  the only solution in this folder
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  Binary search repeatedly halves the search space by comparing the
// #  middle element with the right boundary.
// ##########################################################################

public class Solution
{
    public int FindMin(int[] nums)
    {
        //My Solution
        int l = 0;
        int r = nums.Length - 1;
        while (l < r)
        {
            int mid = l + (r - l) / 2;
            if (nums[mid] > nums[r])
            {
                l = mid + 1;
            }
            else
                r = mid;
        }
        return nums[l];
    }
}

/*
================================================================================
 PATTERN : Binary Search on Rotated Array - compare mid with right end
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-0.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  l      left edge of the window that still holds the minimum
  r      right edge of that window; the loop compares against nums[r]
  mid    middle index of the window; after the check it becomes the new l - 1 or the new r
WHY THIS PATTERN
  The array was sorted and then rotated, so it is made of two sorted runs. The
  minimum is the first value of the second run. One comparison, nums[mid]
  against nums[r], tells you which run mid is in. That lets you throw away half
  of the window on every step. A problem that says "sorted" and asks for better
  than linear time points straight at binary search.
BRUTE FORCE
  Scan the whole array and keep the smallest value. Or scan until you find the
  first i where nums[i] < nums[i-1]. Both are correct and take O(n) time. They
  lose because they never use the sorted order, so they read every element when
  a logarithmic number of reads is enough.
INVARIANT
  The minimum is always inside nums[l..r]. If nums[mid] > nums[r], the drop
  point is to the right of mid, so mid cannot be the minimum and l = mid + 1 is
  safe. Otherwise nums[mid..r] is sorted, so the minimum is at mid or to its
  left, and r = mid keeps it in the window. The window gets smaller each step
  because mid < r, and when l == r the window holds one element, which must be
  the minimum.
COMPARE WITH NUMS[R], NOT NUMS[L]
  Comparing with the right end works both when the array is rotated and when it
  is not. If the array is not rotated, nums[mid] <= nums[r] is always true, so r
  moves left until it reaches index 0. If you compare with nums[l], the check
  "nums[mid] >= nums[l]" is also true for a sorted array and sends you right,
  away from the minimum.
WATCH OUT
  Use r = mid, not r = mid - 1. In the else branch mid may be the minimum
  itself, and mid - 1 would drop it. For an empty array, r starts at -1, the
  loop is skipped, and nums[0] throws IndexOutOfRangeException. The code quietly
  assumes that the input has at least one element. The loop condition must stay
  l < r. With l <= r and r = mid, the loop never ends once l == r.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. What if the array can contain duplicates?
     When nums[mid] == nums[r], you cannot tell which side the drop is on, so do
     r--. This is still correct, but the worst case becomes O(n), for example
     when all values are equal except one.
  2. How would you return the rotation count instead of the minimum value?
     Return l instead of nums[l]. The index of the minimum is the number of
     positions the array was rotated.
  3. How would you search for a target in the rotated array?
     Find the pivot with this loop, then run a normal binary search on the one
     sorted run that can hold the target. Or do it in one pass: check which half
     is sorted and whether the target is inside that half. Two passes are easier
     to get right. One pass reads fewer elements.
TRIGGER
  A sorted array that was rotated or shifted, and you must find a boundary or an
  element faster than a linear scan.
C# NOTE
  l + (r - l) / 2 avoids int overflow when l + r is larger than int.MaxValue.
  Since C# 11 you can also write (l + r) >>> 1. The unsigned right shift reads
  the overflowed sum as unsigned, so it still gives the correct midpoint for
  non-negative indices.
COMPLEXITY
  Time  : O(log n)
  Space : O(1)
================================================================================
*/
