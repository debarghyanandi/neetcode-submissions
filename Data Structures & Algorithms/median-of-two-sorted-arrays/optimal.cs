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
 PROBLEM : You get two sorted int arrays, nums1 and nums2, of any lengths.
           Return the median of all their numbers together, as a double. For
           an even total, the median is the average of the two middle values.
           nums1 = [1,3], nums2 = [2,4] -> 2.5
 PATTERN : Binary Search on the partition (smaller array)
================================================================================
IDEA
  Cut both arrays so the left parts hold exactly left = (n1+n2+1)/2 items.
  Binary search mid1, the cut in nums1; then mid2 = left - mid1 is forced.
  The cut is right when l1 <= r2 and l2 <= r1. Then the left side has the
  smallest half, so the median is max(l1,l2), or its average with min(r1,r2).
  If l1 > r2 we took too many from nums1 (high = mid1-1), else too few.
EXAMPLE
  nums1=[1,2,3,4,5,6], nums2=[7,8] -> swap: nums1=[7,8], nums2=[1..6], left=4
  mid1=1,mid2=3: l1=7 > r2=4 -> high=0
  mid1=0,mid2=4: l1=MinValue, l2=4, r1=7, r2=5 -> valid cut
  n=8 even -> (max(MinValue,4) + min(7,5)) / 2 = 4.5
COMPLEXITY
  Time  O(log(min(m, n)))  each loop halves [low, high], a range of size n1+1
                           (smaller array)
  Space O(1)               only a few int variables; the swap recursion runs
                           just once
PATH TO OPTIMAL
  Merge into a new array, sort or merge, take middle - O(n+m) space.
  Two pointers walk to the middle without storing - O(n+m) time, O(1) space;
  this is suboptimal.cs.
  Binary search the cut in the smaller array - O(log(min(m,n))), this file.
KEYWORDS
  binary search, partition, median, two sorted arrays, kth element, sentinel
WATCH OUT
  - Skip the swap and mid2 = left - mid1 can go negative or past n2.
  - high starts at n1, not n1-1: taking all of nums1 is a valid cut.
  - Odd total returns max(l1,l2) only because left rounds up with +1.
  - Both arrays empty returns -0.5 from (MinValue+MaxValue)/2, not an error.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Find the kth smallest of two sorted arrays instead?
     -> Use the same search with left = k and return max(l1,l2). Or drop k/2
        items from one array each step. Both are O(log) time and O(1) space.
  2. Why does l1 <= r2 && l2 <= r1 prove the cut is right?
     -> Each array is sorted, so l1 <= r1 and l2 <= r2 already hold. The two
        cross checks then put every left value <= every right value.
  3. Numbers arrive as a stream and you need the median at any time?
     -> Keep a max-heap for the low half and a min-heap for the high half.
        Insert is O(log n), median is O(1), and it uses O(n) memory.
  4. What about k sorted arrays?
     -> Binary search on the value range and count items <= x in each array
        with its own binary search. That is O(k log n log range) time.
TRIGGER
  You need a middle or kth value across sorted arrays in sub-linear time.
================================================================================
*/
