// --------------------------------------------------------------------------
// -  optimal.cs            O(log(min(m, n))) time / O(1) space
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
 PATTERN : Binary Search on Partition - split both arrays at once
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-1.cs when it was first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  left     how many elements the combined left half must hold: (n1 + n2 + 1) / 2
  low      smallest cut position in nums1 still possible
  high     largest cut position in nums1 still possible
  mid1     how many elements of nums1 go to the left half
  mid2     how many elements of nums2 go to the left half, equal to left - mid1
  l1, l2   last element on the left side of the cut in nums1 / nums2 (int.MinValue if that side is empty)
  r1, r2   first element on the right side of the cut in nums1 / nums2 (int.MaxValue if that side is empty)
WHY THIS PATTERN
  Both arrays are already sorted, and the median only depends on where the "left
  half" ends. It does not depend on the full merged order. Once you pick mid1,
  the value of mid2 is fixed by left - mid1, so there is only one free choice.
  Moving that choice gives a monotonic answer: l1 > r2 means too many elements
  came from nums1, and l2 > r1 means too few. This makes binary search on mid1
  possible.
BRUTE FORCE
  Merge the two arrays with two pointers, like the merge step of merge sort.
  Stop at index n/2 and read the middle one or two values. This takes O(m + n)
  time and can use O(1) space if you only keep the last two values you saw. It
  is simple and correct, but it is linear, and the problem asks for logarithmic
  time.
INVARIANT
  The correct mid1 always stays inside [low, high]. When l1 > r2, every cut at
  mid1 or larger also takes too much from nums1, so high = mid1 - 1 is safe.
  When l2 > r1, every cut at mid1 or smaller takes too little, so low = mid1 + 1
  is safe. When both l1 <= r2 and l2 <= r1 hold, every left element is less than
  or equal to every right element. So max(l1, l2) and min(r1, r2) are exactly
  the values next to the median.
WHY LEFT ROUNDS UP
  Because left = (n1 + n2 + 1) / 2, the left half gets the extra element when
  the total length is odd. That is why the odd case returns only Math.Max(l1,
  l2) and never looks at r1 or r2. If you round down instead, the odd case must
  return Math.Min(r1, r2).
SEARCH THE SMALLER ARRAY
  The swap when n1 > n2 does more than save time. It guarantees that mid2 = left
  - mid1 stays in the range [0, n2] for every mid1 in [0, n1]. If you search the
  larger array, mid2 can become negative or go past n2, and the index checks
  break.
WATCH OUT
  If both arrays are empty, the loop runs once with all four sentinels (the
  int.MinValue / int.MaxValue stand-ins). It then returns (int.MinValue +
  int.MaxValue) / 2.0 = -0.5 instead of failing clearly. The final return 0 can
  only run if the input is not sorted, so bad input gives a silent wrong answer,
  not an error. The sentinels assume real data never needs a value outside the
  int range. If you change the element type to long, you must also change the
  sentinels to long.MinValue and long.MaxValue.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How do you find the k-th smallest element of the two arrays, not only the
  median?
     Keep the same partition search, but use k in place of left. Clamp the range
     to low = max(0, k - n2) and high = min(k, n1). The answer is max(l1, l2).
     It is still logarithmic, but the bounds need more care.
  2. What if the numbers arrive as a stream and you need the median after each
  one?
     Use two heaps. A max-heap holds the lower half and a min-heap holds the
     upper half, and you keep their sizes within one of each other. Each insert
     costs O(log n), and reading the median costs O(1). You lose the benefit of
     sorted input, but you can handle data that is never fully stored in order.
  3. How would you find the median of k sorted arrays?
     The partition trick does not extend well to k arrays. Binary search on the
     answer value instead. For each candidate value, count the elements less
     than or equal to it with one binary search per array. This costs about O(k
     log n log range). The other option is a k-way merge with a heap up to the
     middle position, which costs O(total/2 * log k).
  4. Can you remove the recursive swap call?
     Yes. Swap the references in local variables (for example with (nums1,
     nums2) = (nums2, nums1)) and swap n1 and n2 too. It is a small readability
     choice. The recursion is only one level deep either way.
TRIGGER
  Two sorted arrays and a question about an order statistic (median or k-th
  element) that must be answered faster than a merge.
C# NOTE
  The (double) cast on Math.Max(l1, l2) happens before the + Math.Min(r1, r2).
  Because of this, the addition is done in double and cannot overflow int. If
  you write (Math.Max(l1, l2) + Math.Min(r1, r2)) / 2.0, the int sum can wrap
  around for large values.
COMPLEXITY
  Time  : O(log(min(m, n)))
  Space : O(1)
================================================================================
*/
