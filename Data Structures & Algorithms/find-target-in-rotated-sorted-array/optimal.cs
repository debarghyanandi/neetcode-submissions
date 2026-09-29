// ##########################################################################
// #  optimal.cs            O(log n) time / O(1) space
// ##########################################################################

public class Solution
{
    public int Search(int[] nums, int target)
    {
        // my solution
        int l = 0;
        int r = nums.Length - 1;
        while (l <= r)
        {
            int mid = l + (r - l) / 2;
            if (target == nums[mid])
            {
                return mid;
            }
            //which part is sorted.
            if (nums[mid] > nums[r])
            {
                //left is sorted
                if (nums[l] <= target && target < nums[mid])
                    r = mid - 1;
                else
                    l = mid + 1;
            }

            else
            {
                // right half is sorted
                if (nums[mid] < target && target <= nums[r])
                    l = mid + 1;
                else
                    r = mid - 1;
            }

        }
        return -1;
    }
}

/*
================================================================================
 PROBLEM : You get an array nums of distinct integers. It was sorted
           ascending, then rotated at an unknown pivot. Return the index of
           target, or -1 if it is not there. You must do it in O(log n).
           Example: nums = [4,5,6,7,0,1,2], target = 0 -> 4
 PATTERN : Binary Search (modified, on rotated sorted array)
================================================================================
IDEA
  Do a normal binary search with l, r and mid. After a cut at mid, at least
  one half is fully sorted. If nums[mid] > nums[r], the drop is on the
  right, so [l..mid] is sorted. Otherwise [mid..r] is sorted. Check with a
  simple range test whether target lies in the sorted half. If yes, keep
  that half. If no, keep the other half. This is correct because a sorted
  half gives an exact yes/no answer, so we never drop the half with target.
EXAMPLE
  nums=[4,5,6,7,0,1,2], target=0
  l=0 r=6 mid=3 (7): 7>2, left sorted, 0 not in [4,7) -> l=4
  l=4 r=6 mid=5 (1): 1<=2, right sorted, 0 not in (1,2] -> r=4
  l=4 r=4 mid=4 (0): found -> return 4
COMPLEXITY
  Time  O(log n)  each loop step drops half of the range [l..r]
  Space O(1)      only l, r and mid are stored
PATH TO OPTIMAL
  Linear scan - O(n) - simple, but ignores the sorted order.
  Find pivot (the min) by binary search, then a normal binary search on the
  right part - O(log n) - uses the order; two passes.
  This file - O(log n) - one pass. optimal-variant.cs is another O(log n).
KEYWORDS
  binary search, rotated sorted array, pivot, sorted half, O(log n), search
WATCH OUT
  - Duplicates break the test nums[mid] > nums[r]. With [1,1,1,0,1] and
    target 0, it keeps [0..1] and returns -1, but the answer is 3.
  - Keep <= on the edges: nums[l] <= target and target <= nums[r]. Using <
    misses a target at l or r.
  - Keep the strict test nums[mid] > nums[r]. With >= and l == r, the code
    takes the wrong branch and may skip the answer.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. What if nums can have duplicates (Search in Rotated Sorted Array II)?
     -> If nums[mid] == nums[r], you cannot tell which half is sorted, so do
        r--. The worst case becomes O(n), for example when all values are equal.
  2. Find the minimum, or how many times the array was rotated.
     -> Binary search: if nums[mid] > nums[r], set l = mid + 1, else r = mid.
        Stop when l == r. The min index is also the rotation count. O(log n).
  3. Why is one half always sorted?
     -> There is only one drop point. It can sit in only one of the two
        halves, so the other half is sorted.
TRIGGER
  A sorted array was shifted or rotated and you must search in O(log n).
================================================================================
*/
