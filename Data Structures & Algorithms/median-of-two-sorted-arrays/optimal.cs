// --------------------------------------------------------------------------
// -  optimal.cs            O(log(min(m, n))) time / O(1) space
// -  binary search on partition   [binary-search-partition]
// -  ranks above suboptimal.cs (O(n + m) time / O(1) space)
// -
// -  Reference solution - not one you solved yourself (from submission-1)
// -
// -  Binary search on the smaller array determines valid partition point in
// -  logarithmic iterations.
// --------------------------------------------------------------------------

public class Solution
{
    public double FindMedianSortedArrays(int[] nums1, int[] nums2)
    {
        //Hard problem Have to Understand again
        int n1 = nums1.Length;
        int n2 = nums2.Length;

        // Always binary search the smaller array
        if (n1 > n2)
            return FindMedianSortedArrays(nums2, nums1);

        int low = 0;
        int high = n1;

        int left = (n1 + n2 + 1) / 2;
        int n = n1 + n2;

        while (low <= high)
        {
            int mid1 = (low + high) / 2;
            int mid2 = left - mid1;

            int l1 = int.MinValue;
            int l2 = int.MinValue;

            int r1 = int.MaxValue;
            int r2 = int.MaxValue;

            if (mid1 < n1)
                r1 = nums1[mid1];

            if (mid2 < n2)
                r2 = nums2[mid2];

            if (mid1 - 1 >= 0)
                l1 = nums1[mid1 - 1];

            if (mid2 - 1 >= 0)
                l2 = nums2[mid2 - 1];

            // Correct partition
            if (l1 <= r2 && l2 <= r1)
            {
                // Odd total length
                if (n % 2 == 1)
                    return Math.Max(l1, l2);

                // Even total length
                return ((double)Math.Max(l1, l2)
                        + Math.Min(r1, r2)) / 2.0;
            }

            // Took too many elements from nums1
            if (l1 > r2)
            {
                high = mid1 - 1;
            }
            else
            {
                // Took too few elements from nums1
                low = mid1 + 1;
            }
        }

        return 0;
    }
}

/*
================================================================================
 PATTERN : Binary Search on Partition - median of two sorted arrays
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-1.cs when it was first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  n         total length n1 + n2, used only for the odd/even test
  left      how many elements must land in the left half: (n1 + n2 + 1) / 2
  low/high  search range for how many elements to take from nums1 (0..n1)
  mid1      count taken from nums1 (also the index of the first right-side element)
  mid2      count taken from nums2, forced to left - mid1
  l1, l2    last element on the left side from nums1 / nums2
  r1, r2    first element on the right side from nums1 / nums2
WHY THIS PATTERN
  Both inputs are already sorted and we only need the middle value, not the
  merged array. So the real question is "where do I cut each array so that every
  left element is <= every right element", and once mid1 is chosen, mid2 = left
  - mid1 is forced. That turns a two-dimensional choice into a one-dimensional
  search over mid1 in [0, n1], and the condition l1 <= r2 && l2 <= r1 is
  monotone: too big a mid1 makes l1 > r2, too small makes l2 > r1, so binary
  search finds the cut.
BRUTE FORCE
  Merge the two arrays with two pointers into one sorted list, then read the
  middle element (or average the two middle ones). That is O(n1 + n2) time and
  O(n1 + n2) extra space, and it is easy to get right. It loses because it
  builds the whole array when only one or two values near the center are ever
  needed.
INVARIANT
  At every iteration, mid1 + mid2 == left, so the left side always holds exactly
  the number of elements the median needs, whatever mid1 is. The loop keeps the
  correct cut inside [low, high]: when l1 > r2 the cut is too far right in nums1
  so high = mid1 - 1, otherwise it is too far left so low = mid1 + 1. When both
  cross-checks pass, every left element is <= every right element, so
  Math.Max(l1, l2) is the element at position left - 1 of the merged array -
  exactly the median for odd n, and the lower of the two middles for even n.
SENTINELS REPLACE EMPTY-SIDE CHECKS
  mid1 can be 0 (nothing taken from nums1) or n1 (everything taken), and the
  same for mid2. Instead of special-casing those, l1/l2 start at int.MinValue
  and r1/r2 at int.MaxValue, so a missing neighbour never blocks the comparison
  l1 <= r2 && l2 <= r1. This is why the code works when one array is entirely on
  one side of the cut, including when nums1 is empty and the whole answer comes
  from nums2.
THE (DOUBLE) CAST GUARDS THE SUM
  In the even branch the cast is on Math.Max(l1, l2), before the addition, so
  Math.Min(r1, r2) is widened too and the sum happens in floating point. Writing
  (Math.Max(l1, l2) + Math.Min(r1, r2)) / 2.0 instead would add two ints first
  and can overflow when both middles are large positive values.
WATCH OUT
  If both arrays are empty the loop still runs once with mid1 = mid2 = 0, all
  four sentinels stay in place, n % 2 == 0, and the method returns (int.MinValue
  + int.MaxValue) / 2.0 = -0.5 instead of failing loudly. The trailing return 0
  is dead code for any real input, so a wrong answer from it would be silent
  rather than an exception. Also note the comment "Took too many elements from
  nums1" sits on the l1 > r2 branch, which is right, but the else branch is
  entered for both "too few" and the impossible case, so do not read it as a
  proof that l2 > r1 held.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Turn this into "kth smallest of two sorted arrays".
     Replace left = (n1 + n2 + 1) / 2 with left = k and return Math.Max(l1, l2)
     once the cut is valid; drop the even/odd branch entirely. Same search range
     and same complexity.
  2. What about the median of k sorted arrays?
     The partition trick does not extend, because with k cuts the choice is no
     longer one-dimensional. Use a min-heap merge that stops at the middle, O(N
     log k) time, or binary search on the value instead of the index and count
     elements <= it in O(k log(maxLen)) per step.
  3. The numbers arrive as a stream and I must report the median after each
  insert.
     Different problem - keep a max-heap for the lower half and a min-heap for
     the upper half, rebalance so their sizes differ by at most one, O(log n)
     per insert and O(n) space.
  4. Can you drop the recursive call?
     Yes - assign the smaller array to a local variable pair and swap them once,
     then run the same loop. It removes the extra stack frame and the second
     entry into the public method; the recursion here is only ever one level
     deep, so it is a readability choice.
TRIGGER
  Two (or few) already sorted inputs plus a request for an order statistic - a
  median or a kth element - with a complexity target better than merging them.
C# NOTE
  The final return 0 exists only because C# requires every code path to return a
  value and the compiler cannot prove the while loop always exits through a
  return; throwing new ArgumentException there would document the intent better
  than a silent 0.
COMPLEXITY
  Time  : O(log(min(m, n)))
  Space : O(1)
================================================================================
*/
