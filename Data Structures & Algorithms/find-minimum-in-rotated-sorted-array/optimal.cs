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
 PATTERN : Binary Search on the rotation point - compare mid to right
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-0.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  l    left end of the range still being searched (inclusive)
  r    right end of that range (inclusive), and the value nums[mid] is compared against
WHY THIS PATTERN
  The array was sorted and then rotated, so it is two ascending runs and every
  element in the left run is bigger than every element in the right run. That
  split gives a yes/no test that can be answered at one index: if nums[mid] >
  nums[r], then mid sits in the left run and the minimum must be after it, so l
  = mid + 1. Otherwise mid is already in the run that contains the minimum, so r
  = mid keeps mid as a candidate. Each test throws away half the range, which is
  what makes this a binary search rather than a scan.
BRUTE FORCE
  The first thing most people write is a single pass that keeps the smallest
  value seen, or just a scan for the one place where nums[i] > nums[i+1]. Both
  are correct and O(n) time. They lose because they read every element even
  though the two-run structure lets one comparison rule out half the array.
INVARIANT
  At the top of every loop iteration, the minimum element lies somewhere in
  nums[l..r] inclusive. Both updates preserve it: l = mid + 1 only runs when
  nums[mid] > nums[r], which proves nums[mid] is not the minimum and neither is
  anything before it in that run; r = mid keeps mid itself inside the range. The
  range shrinks by at least one each time, so the loop ends with l == r, and by
  the invariant that single position holds the answer.
COMPARE AGAINST NUMS[R], NOT NUMS[L]
  The pivot of the comparison must be the right end. Comparing nums[mid] to
  nums[l] does not separate the two cases: on an array that is not rotated at
  all, nums[mid] > nums[l] is true and would send the search the wrong way.
  Against nums[r] the not-rotated case falls into the else branch and r walks
  down to 0 correctly.
MIDPOINT AND LOOP BOUND
  mid = l + (r - l) / 2 avoids computing l + r, which could overflow a 32-bit
  int. The condition is l < r, not l <= r, and there is no early return: the
  loop is designed to converge to one index rather than to find an exact match,
  so l <= r would loop forever once l == r and the else branch sets r = mid = l.
WATCH OUT
  An empty array breaks this: nums.Length - 1 makes r = -1, the condition 0 < -1
  is false, and return nums[l] throws on index 0. There is no null check either.
  The strict > also means duplicates are not handled: on something like
  [3,3,1,3] the else branch can move r past the minimum, so this exact code only
  answers the distinct-values version of the problem.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How does it change when the array may contain duplicates?
     Add a third case: when nums[mid] == nums[r], do r-- instead of r = mid. It
     stays correct but the worst case degrades to O(n), for example an array of
     all equal values.
  2. The interviewer wants the index of the minimum, or the number of times the
  array was rotated.
     Return l instead of nums[l]; that index is also the rotation count, since
     the original element 0 moved to position l.
  3. Search for an arbitrary target in this rotated array, not just the minimum.
     Run this loop first to get l, then binary search normally on the rotated
     range by mapping a logical index to (l + idx) % nums.Length. Still O(log n)
     and no extra memory.
  4. Could this be written recursively?
     Yes, with (nums, l, r) parameters and tail recursion, but the loop is
     simpler and uses no call stack; C# does not guarantee tail-call
     elimination, so the iterative form is the safer choice.
TRIGGER
  A sorted array that has been shifted or cut and re-joined, where a single
  comparison against a fixed endpoint tells you which half the answer is in.
C# NOTE
  nums[^1] (index-from-end) would read the same value as nums[r] at the start
  and reads more clearly, but r must stay a real int because it is updated.
  Resist nums.Min() from LINQ here - it is correct but O(n) and allocates an
  enumerator, throwing away the whole point of this loop.
COMPLEXITY
  Time  : O(log n)
  Space : O(1)
================================================================================
*/
