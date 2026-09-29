// ##########################################################################
// #  optimal.cs            O(log n) time / O(1) space
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
 PROBLEM : You get an ascending array of distinct numbers that was rotated
           some unknown number of times, so a sorted tail may now sit in
           front. Return the smallest value, not its index. Example:
           [4,5,6,7,0,1,2] -> 0.
 PATTERN : Binary Search on rotation point (compare mid with right)
================================================================================
IDEA
  The array is two sorted runs. The minimum is the first value of the right
  run.
  Compare nums[mid] with nums[r]. If nums[mid] > nums[r], mid is in the left
  run, so the minimum is after mid: l = mid + 1. Otherwise mid is in the right
  run and may be the minimum itself: r = mid. The minimum always stays inside
  [l, r], and the range shrinks every step, so l == r lands on it.
EXAMPLE
  [4,5,6,7,0,1,2]: l=0,r=6,mid=3: 7>2 -> l=4; l=4,r=6,mid=5: 1<=2 -> r=5;
  l=4,r=5,mid=4: 0<=1 -> r=4; l==r -> return nums[4] = 0.
  Tricky [3,1,2]: mid=1: 1<=2 -> r=1; mid=0: 3>1 -> l=1; return 1.
COMPLEXITY
  Time  O(log n)  each loop step halves the range [l, r]
  Space O(1)      only l, r, mid are stored
PATH TO OPTIMAL
  Linear scan for the min - O(n) - simple, but ignores the sorted order.
  Binary search on nums[mid] vs nums[r] - O(log n) - drops half the range
  per step (this file, optimal.cs).
KEYWORDS
  binary search, rotated sorted array, pivot, inflection point, min element
WATCH OUT
  - Comparing with nums[l] instead of nums[r] fails on an unrotated array:
    [1,2,3] has no "left run", so you move the wrong way.
  - Writing r = mid - 1 can skip the minimum. Pairing r = mid with l <= r
    loops forever.
  - Duplicates break it: [3,3,1,3] returns 3, but the answer is 1.
  - An empty array throws: r = -1, then nums[0] is read out of range.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. What if duplicates are allowed?
     -> When nums[mid] == nums[r], you cannot tell the side, so do r--. It
        stays correct, but the worst case becomes O(n), for example with all
        values equal.
  2. How many times was the array rotated?
     -> Return l instead of nums[l]. The minimum's index is the rotation
        count. Still O(log n) time and O(1) space.
  3. Search for a target in the rotated array?
     -> In each step one half of [l, r] is sorted. Check if the target is
        inside that half's range, then go there or to the other half. O(log n).
  4. Why compare with nums[r] and not nums[l]?
     -> nums[r] is always in the right run, so the test is never ambiguous.
        nums[l] can't tell a no-rotation array from a mid in the left run.
TRIGGER
  A sorted array that was shifted or rotated, where the task asks for a pivot
  or target in less than O(n), means binary search on which half is sorted.
================================================================================
*/
