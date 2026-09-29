// ##########################################################################
// #  optimal.cs            O(log n) time / O(1) space
// #  Binary search with rotation awareness   [binary-search-rotated-direct]
// #  ties with optimal-variant.cs on O(log n) time / O(1) space
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  Each iteration determines which half is sorted and eliminates half the
// #  search space by comparing mid with right boundary.
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
 PATTERN : Modified Binary Search - find the sorted half each step
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-0.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  l      left edge of the window still being searched
  r      right edge of the window still being searched (inclusive)
  mid    middle index; nums[mid] is compared to target and to nums[r]
WHY THIS PATTERN
  The array is sorted but rotated, and we must find one value fast. A rotation
  cuts the array into two sorted runs. So any window [l..r] cut at mid has at
  least one half that is fully sorted. We can test in O(1) whether target lies
  inside that sorted half. That lets us throw away half of the window each step,
  just like normal binary search.
BRUTE FORCE
  Scan nums from left to right and return the first index where nums[i] ==
  target, else -1. It is always correct, but it is O(n) time. It ignores that
  the data is almost sorted. The interviewer expects the logarithmic version.
INVARIANT
  If target is in nums, its index is always inside [l..r]. Each step either
  returns mid or removes a part that cannot hold target. If nums[mid] > nums[r],
  then [l..mid] is sorted, and target is there only if nums[l] <= target <
  nums[mid]. Otherwise [mid..r] is sorted, and target is there only if nums[mid]
  < target <= nums[r]. The kept part still holds target, the window gets smaller
  every step, and when l > r the window is empty, so -1 is correct.
COMPARE MID WITH NUMS[R], NOT NUMS[L]
  The test nums[mid] > nums[r] is safe when l == mid, because it never compares
  an element with itself in the wrong way. If you compare with nums[l], you need
  nums[l] <= nums[mid] (with the equals sign). Without it, a two-element window
  like [3,1] picks the wrong half. The bounds are also uneven on purpose: target
  < nums[mid] is strict because mid was already checked. The far ends nums[l]
  and nums[r] are inclusive because they are not checked yet.
WATCH OUT
  This code assumes all values are distinct. With duplicates it can drop the
  wrong half. Example: nums = [1,1,1,0,1], target 0. At mid = 2, nums[mid] ==
  nums[r], so the code says "right half is sorted", sets r = 1, and returns -1
  even though 0 is at index 3. An empty array is fine: r starts at -1, the loop
  never runs, and the code returns -1.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How do you find the minimum, which is the rotation point, instead of a
  target?
     Use the same loop without the target checks. If nums[mid] > nums[r], set l
     = mid + 1. Otherwise set r = mid, and stop when l == r. That index is the
     smallest value. It also costs O(log n).
  2. Can you do it in two plain binary searches instead of one tricky one?
     First find the pivot as above. Then run a normal binary search on the one
     sorted run that can hold target. The code is easier to prove correct. The
     cost is a second pass, but it is still O(log n).
  3. What if you need every index of target, not just one?
     In a rotated array of distinct values, there is only one. If duplicates are
     allowed, find the pivot first. Then, on each sorted run, use lower-bound
     and upper-bound searches to get the range of equal values.
TRIGGER
  A sorted array that was shifted or rotated, and you must search it faster than
  a linear scan.
C# NOTE
  In the two-pass version, you do not need to write the second search yourself.
  Array.BinarySearch(nums, start, length, target) searches just one sorted run.
  It returns a negative number when the value is missing, so map any negative
  result to -1.
COMPLEXITY
  Time  : O(log n)
  Space : O(1)
================================================================================
*/
