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
 PATTERN : Binary search twice - find rotation pivot, then search halves
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-1.cs when it was first processed
 STATUS  : Optimal variant - ties the best complexity by another route
================================================================================
VARIABLES
  l        left bound of the range that still contains the smallest element
  r        right bound of that range; when l == r it is the smallest element
  m        midpoint of the pivot hunt, compared against nums[r], not nums[l]
  pivot    index of the smallest element, so the start of the second sorted run
  result   -1 or the index found in the left run, used to decide if the right run is searched
WHY THIS PATTERN
  The input is a sorted array that was cut once and the two pieces swapped, so
  it is not sorted as a whole but it is made of exactly two sorted runs. A plain
  binary search fails on the whole array, but it works on each run. So this file
  first spends one binary search to find where the cut is (pivot), then runs the
  ordinary BinarySearch on 0..pivot-1 and, if that misses, on
  pivot..nums.Length-1.
BRUTE FORCE
  The first thing most people write is a for loop over nums comparing each
  element to target and returning the index. That is O(n) time and always
  correct, including with duplicates. It loses because it ignores the fact that
  both halves are still sorted, which is the only structure the problem gives
  you.
INVARIANT
  Through the first loop, the smallest element's index always lies inside [l,
  r]. If nums[m] > nums[r], then the run containing m keeps rising past the end,
  so the cut is strictly after m and l = m + 1 is safe. Otherwise nums[m] <=
  nums[r] means m is already inside the final rising run, so the smallest
  element is at m or before it, and r = m keeps it in range. The window shrinks
  every step, so it ends at l == r == pivot, and from there each of the two
  ranges passed to BinarySearch is plain sorted.
WHY COMPARE WITH NUMS[R] AND NOT NUMS[L]
  Comparing the midpoint to the right end also handles the not-rotated case for
  free. If nums is already fully sorted, nums[m] <= nums[r] holds every time, so
  r walks down to 0, pivot becomes 0, the first BinarySearch gets the empty
  range 0..-1 and returns -1, and the second one searches the whole array.
  Comparing against nums[l] instead would need a separate check for that case.
WATCH OUT
  The pivot loop is the one place where r must move to m and not m - 1; m itself
  can be the smallest element, and m - 1 would skip it and loop forever or land
  wrong. The loop condition is l < r, not l <= r, on purpose - with l <= r the
  window would never close. Duplicate values break the pivot hunt: with
  something like [3,3,1,3] the test nums[m] > nums[r] gives no information and
  the found pivot can be wrong; this code assumes distinct values. Also note (l
  + r) / 2 and (left + right) / 2 add before dividing, so they would overflow on
  an array long enough for the two indices to sum past int.MaxValue; left +
  (right - left) / 2 avoids that.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you do it in one pass instead of finding the pivot first?
     Yes. In a single loop, check whether nums[left] <= nums[mid] to learn which
     side is the sorted one, then test whether target falls inside that sorted
     side and keep it, else keep the other side. Same complexity, one loop
     instead of up to three, but the branch logic is harder to get right than
     this file's two clean phases.
  2. What if the array may contain duplicates?
     When nums[m], nums[l] and nums[r] are equal you cannot tell which side is
     sorted, so you shrink the ends by one and the worst case degrades to O(n),
     for example an array of all equal values with one different.
  3. The question only asks for the number of times the array was rotated - what
  changes?
     Delete both BinarySearch calls and return pivot; the first while loop alone
     is the whole answer.
  4. Why not make BinarySearch recursive?
     It would read the same but add O(log n) stack frames; the while loop here
     keeps space flat, which is why the recursive form is the version to avoid
     if the interviewer asks for constant space.
TRIGGER
  An array that is sorted except for one wrap-around point, with a lookup or
  minimum asked for in better than linear time.
C# NOTE
  BinarySearch is declared public and takes nums as a parameter, so it leaks as
  part of the Solution API - make it private static, since it touches no
  instance state. Note also that Array.BinarySearch from the base library cannot
  be used on nums directly here, because it requires the whole array to be
  sorted; it would only work on the two sub-ranges after pivot is known.
COMPLEXITY
  Time  : O(log n)
  Space : O(1)
================================================================================
*/
