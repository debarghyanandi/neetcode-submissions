// ##########################################################################
// #  optimal.cs            O(log n) time / O(log n) space
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
 PROBLEM : You get an array nums sorted in ascending order and an int target.
           Return the index of target in nums, or -1 if it is not there. You
           must do it in O(log n) time. Values are distinct. Example: nums =
           [-1,0,3,5,9,12], target = 9 -> 4.
 PATTERN : Binary Search (recursive, closed interval [left, right])
================================================================================
IDEA
  Keep a range [left, right] that must hold target if target exists.
  Look at mid. If nums[mid] equals target, return mid.
  If target is smaller, recurse on [left, mid - 1], else on [mid + 1, right].
  When left > right the range is empty, so return -1.
  It is correct because the array is sorted: each step drops only the
  half that cannot hold target, so target is never thrown away.
EXAMPLE
  nums = [-1,0,3,5,9,12], target = 9:
  (0,5) mid=2 nums=3 <9 -> (3,5) mid=4 nums=9 -> return 4.
  target = 2: (0,5) mid=2 nums=3 -> (0,1) mid=0 nums=-1 -> (1,1) mid=1
  nums=0 -> (2,1) left > right -> return -1.
COMPLEXITY
  Time  O(log n)  each call halves the range, so about log2(n) calls
  Space O(log n)  one stack frame per call, recursion depth is log2(n)
PATH TO OPTIMAL
  Linear scan - O(n) time, O(1) space - simple, but ignores the sorting.
  Recursive binary search (this file) - O(log n) time, O(log n) space -
  uses the sorting to drop half the range each step.
  Iterative binary search - O(log n) time, O(1) space - same steps, loop
  instead of recursion, so there is no stack.
KEYWORDS
  binary search, sorted array, divide and conquer, halving, O(log n), lower
  bound
WATCH OUT
  - Recursing on (left, mid) or (mid, right) instead of mid - 1 / mid + 1
    can loop forever when left == right: the range never shrinks.
  - Stopping at left >= right instead of left > right skips a 1-item range.
  - (left + right) / 2 can overflow for huge indices; the code avoids it
    with left + (right - left) / 2.
  - With duplicates this returns some matching index, not the first one.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you do it in O(1) space?
     -> Use a while (left <= right) loop that updates left and right. Same
        O(log n) time, no call stack, and no risk of stack overflow.
  2. The array has duplicates. Return the first index of target.
     -> On a match, save mid and keep searching left (right = mid - 1). Still
        O(log n); it is the lower bound search.
  3. The sorted array is rotated at an unknown point.
     -> At each mid, one half is sorted. Check if target is inside that half
        and go there, else go to the other half. Still O(log n) time.
  4. What if target is missing? Where would it go?
     -> Return left when the loop ends: it is the insert position, O(log n).
TRIGGER
  The input is sorted (or the answer is monotonic), and you must find a
  value faster than O(n).
================================================================================
*/
