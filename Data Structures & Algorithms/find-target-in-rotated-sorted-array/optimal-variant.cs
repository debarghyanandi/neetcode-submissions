// --------------------------------------------------------------------------
// -  optimal-variant.cs    O(log n) time / O(1) space
// -  Find pivot point, then binary search   [find-pivot-binary-search]
// -  ties with optimal.cs on O(log n) time / O(1) space
// -
// -  Reference solution - not one you solved yourself
// -
// -  First phase finds rotation pivot in O(log n), then performs standard
// -  binary search on the appropriate half in O(log n).
// --------------------------------------------------------------------------

public class Solution
{
    public int Search(int[] nums, int target)
    {
        int l = 0, r = nums.Length - 1;

        while (l < r)
        {
            int m = (l + r) / 2;
            if (nums[m] > nums[r])
            {
                l = m + 1;
            }
            else
            {
                r = m;
            }
        }

        int pivot = l;

        int result = BinarySearch(nums, target, 0, pivot - 1);
        if (result != -1)
        {
            return result;
        }

        return BinarySearch(nums, target, pivot, nums.Length - 1);
    }

    public int BinarySearch(int[] nums, int target, int left, int right)
    {
        while (left <= right)
        {
            int mid = (left + right) / 2;
            if (nums[mid] == target)
            {
                return mid;
            }
            else if (nums[mid] < target)
            {
                left = mid + 1;
            }
            else
            {
                right = mid - 1;
            }
        }
        return -1;
    }
}

/*
================================================================================
 PATTERN : Binary Search - find the pivot, then search one sorted half
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-1.cs when it was first processed
 STATUS  : Optimal variant - ties the best complexity by another route
================================================================================
VARIABLES
  l, r     search bounds while looking for the pivot. The loop ends when l == r.
  m        midpoint used to decide which side holds the minimum
  pivot    index of the smallest value, where the rotated array starts over
  result   index found in the left part [0, pivot-1], or -1
WHY THIS PATTERN
  The array was sorted and then rotated. So it is really two sorted runs joined
  together, and the problem asks for better than a linear scan. Binary search
  can find where the runs meet, which is the smallest value, stored in pivot.
  Each run is fully sorted, so a normal BinarySearch works on nums[0..pivot-1]
  and on nums[pivot..end].
BRUTE FORCE
  Scan every index and return i when nums[i] == target. This takes O(n) time. It
  is correct, but it ignores that the data is sorted inside each run, and every
  lookup reads the whole array.
INVARIANT
  In the first loop, the minimum always stays inside [l, r]. If nums[m] >
  nums[r], the drop happens after m, so l = m + 1 is safe. If not, nums[m..r] is
  sorted, so the minimum is at m or to its left, and r = m keeps it. When l ==
  r, pivot is the minimum. In BinarySearch, the target, if it is present, always
  stays inside [left, right].
COMPARE AGAINST NUMS[R], NOT NUMS[L]
  Comparing nums[m] with nums[r] also works when the array is not rotated at
  all. Then pivot ends at 0, and the first search gets the empty range [0, -1].
  If you compare with nums[l], a sorted array gives no drop to follow, and you
  need a special case.
WATCH OUT
  The pivot loop assumes all values are distinct. With duplicates, nums[m] ==
  nums[r] tells you nothing, and r = m can throw away the real minimum. The code
  often runs both searches: when the target is in the right part, the left
  search runs first and fails. This is still correct, but it is extra work. An
  empty array does not crash. r starts at -1, the loop is skipped, and both
  searches get empty ranges.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you search only one half?
     Yes. If target >= nums[pivot] and target <= nums[nums.Length - 1], search
     [pivot, end]. Otherwise search [0, pivot-1]. It needs one more comparison,
     but it never runs a search that will fail.
  2. Can you do it in a single binary search, without finding the pivot first?
     Yes. At each m, one of the two sides [l, m] or [m, r] is sorted. Check
     whether target falls inside the sorted side's range, and move toward it.
     This makes one pass instead of two, but there are more branches to get
     wrong.
  3. What if duplicates are allowed?
     When nums[m] == nums[r], you cannot tell which side the minimum is on, so
     you do r-- and continue. The worst case becomes O(n), for example an array
     of all equal values.
  4. What if you only need the minimum value?
     Stop after the first loop and return nums[l]. That loop is the whole
     solution to "find minimum in rotated sorted array".
TRIGGER
  A sorted array was rotated or shifted, and you must find something in less
  than linear time.
C# NOTE
  The helper BinarySearch can be replaced by Array.BinarySearch(nums, pivot,
  length, target). It returns a negative number when the target is not found, so
  check for idx < 0 rather than -1.
COMPLEXITY
  Time  : O(log n)
  Space : O(1)
================================================================================
*/
