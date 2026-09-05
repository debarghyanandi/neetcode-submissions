// --------------------------------------------------------------------------
// -  optimal-variant.cs    O(log n) time / O(1) space
// -  find pivot via binary search, then binary search correct half
// -  [rotated-binary-search-find-pivot-then-search]
// -  ties with optimal.cs on O(log n) time / O(1) space
// -
// -  Reference solution - not one you solved yourself (from submission-1)
// -
// -  two sequential binary searches: one to locate the rotation pivot,
// -  another to search within the appropriate sorted subarray
// --------------------------------------------------------------------------

public class Solution
{
    public int Search(int[] nums, int target)
    {
        int low = 0, high = nums.Length - 1;

        while (low < high)
        {
            int center = (low + high) / 2;
            if (nums[center] > nums[high])
            {
                low = center + 1;
            }
            else
            {
                high = center;
            }
        }

        int pivot = low;

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
 PATTERN : Find rotation pivot, then binary search each sorted run
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-1.cs when it was first processed
 STATUS  : Optimal variant - ties the best complexity by another route
================================================================================
THE SPLIT
  A rotated array of distinct values is two ascending runs: nums[0..pivot-1] and
  nums[pivot..n-1], where pivot is the index of the minimum. Every value in the
  first run exceeds every value in the second. Once pivot is known, both runs
  are ordinary sorted arrays and the helper BinarySearch needs no rotation
  awareness at all - it is the textbook version, unmodified.

  One detail that saves a bug: BinarySearch takes absolute left/right bounds
  into nums rather than a sliced copy, so the index it returns is already the
  answer. No offset correction on the second call.
PIVOT LOOP INVARIANT
  Invariant: the minimum always lies inside nums[low..high].

  If nums[center] > nums[high], then center sits in the high-valued first run
  while high sits in the second, so the minimum is strictly right of center -
  low = center + 1 discards center safely.

  Otherwise nums[center] <= nums[high] puts center and high in the same run, so
  the minimum is at center or left of it - high = center keeps center as a live
  candidate.

  When low == high the range holds exactly one element, and it is the minimum.
  pivot = low.
WHY NUMS[HIGH] AND NOT NUMS[LOW]
  Anchoring the comparison to the right end is what makes the zero-rotation case
  work for free.

  Take [1,2,3]. Against nums[low]: center = 1, nums[1] = 2 > nums[0] = 1, so low
  = 2 and pivot = 2 - wrong, the minimum is at 0. Against nums[high]: 2 > 3 is
  false so high = 1, then center = 0, 1 > 2 is false so high = 0, pivot = 0 -
  correct.

  The nums[low] variant needs an explicit "array is already sorted" pre-check.
  This one does not.
TERMINATION AND THE OFF BY ONE
  Two choices are coupled and must move together.

  high = center, not center - 1, because center may itself be the minimum.

  while (low < high), not <=, because with high = center the state low == high
  == center would set center back to itself forever.

  Progress still holds: with low < high, center = (low + high) / 2 satisfies low
  <= center < high, so low = center + 1 strictly raises low and high = center
  strictly lowers high. The window shrinks every iteration.
DEGENERATE CASES
  Non-rotated input gives pivot = 0. The first call becomes BinarySearch(nums,
  target, 0, -1), whose while (left <= right) is false on entry, so it returns
  -1 immediately and the second call sweeps the whole array. Costs one wasted
  comparison, not a special case.

  Empty nums: high starts at -1, the pivot loop never runs, pivot = 0, and both
  calls are (0, -1). Search returns -1. No length guard is needed anywhere.
COST OF THE TWO-PASS SHAPE
  The interviewer follow-up is "can you do this in a single pass?" Yes - decide
  at each mid which side is sorted and whether target falls inside that side's
  range. This file instead pays up to three logarithmic passes (pivot, then run
  one, then run two) for two pieces that are individually hard to get wrong.

  Cheap middle ground if pressed: after computing pivot, choose the run in O(1)
  by comparing target against nums[0] - if target >= nums[0] search [0,
  pivot-1], else search [pivot, n-1]. That drops the worst case to two passes
  and removes the try-and-fall-through structure entirely.
WATCH OUT
  Duplicates break the pivot loop, and the problem statement is what protects
  you - it promises distinct values. Counterexample [2,2,2,0,2]: every
  nums[center] > nums[high] test is false, high walks down to 0, pivot = 0, and
  the second call runs a plain binary search over [2,2,2,0,2], which is not
  sorted. It returns -1 while 0 sits at index 3. Supporting duplicates requires
  a high-- fallback on ties, which surrenders the logarithmic worst case.

  (low + high) / 2 overflows int once low + high passes int.MaxValue, as does
  (left + right) / 2 in the helper. Unreachable at these constraints, but low +
  (high - low) / 2 is the expected answer when asked.
TRIGGER
  Reach for pivot-then-search whenever a sorted structure has been rotated once
  and you need a lookup: recover the rotation point, then reduce to the problem
  you already know. The same first loop, standalone, answers "minimum in a
  rotated sorted array" by returning nums[low] and "how many times was it
  rotated" by returning pivot.
COMPLEXITY
  Time  : O(log n)
  Space : O(1)
================================================================================
*/
