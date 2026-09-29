// --------------------------------------------------------------------------
// -  suboptimal.cs         O(n + m) time / O(1) space
// --------------------------------------------------------------------------

public class Solution
{
    public double FindMedianSortedArrays(int[] nums1, int[] nums2)
    {
        int n1 = nums1.Length;
        int n2 = nums2.Length;

        int i = 0;
        int j = 0;

        int n = n1 + n2;

        int ind2 = n / 2;
        int ind1 = ind2 - 1;

        int cnt = 0;

        int ind1el = -1;
        int ind2el = -1;

        while (i < n1 && j < n2)
        {
            if (nums1[i] < nums2[j])
            {
                if (cnt == ind1)
                    ind1el = nums1[i];

                if (cnt == ind2)
                    ind2el = nums1[i];

                cnt++;
                i++;
            }
            else
            {
                if (cnt == ind1)
                    ind1el = nums2[j];

                if (cnt == ind2)
                    ind2el = nums2[j];

                cnt++;
                j++;
            }
        }

        while (i < n1)
        {
            if (cnt == ind1)
                ind1el = nums1[i];

            if (cnt == ind2)
                ind2el = nums1[i];

            cnt++;
            i++;
        }

        while (j < n2)
        {
            if (cnt == ind1)
                ind1el = nums2[j];

            if (cnt == ind2)
                ind2el = nums2[j];

            cnt++;
            j++;
        }

        if (n % 2 == 1)
            return ind2el;

        return ((double)ind1el + ind2el) / 2.0;
    }
}

/*
================================================================================
 PATTERN : Two Pointers Merge - count to the middle index
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Suboptimal
================================================================================
VARIABLES
  ind2     index of the upper middle in the merged order, n / 2
  ind1     index of the lower middle, ind2 - 1 (it is -1 when n is 1)
  cnt      how many elements were taken from the merge so far, which is the merged index of the next element
  ind1el   value found at merged index ind1
  ind2el   value found at merged index ind2
WHY THIS PATTERN
  Both arrays are already sorted, and the median depends only on the elements at
  one or two fixed positions of the merged order. With two pointers, i and j, we
  can walk the merged order without building it. At each step we take the
  smaller of nums1[i] and nums2[j]. The counter cnt tells us when we reach ind1
  or ind2, and we store those values in ind1el and ind2el.
BETTER APPROACH
  The better approach is binary search on a partition of the smaller array. You
  pick a cut in nums1 and a matching cut in nums2 so that the left halves
  together hold (n + 1) / 2 elements. Then you move the cut until the largest
  value on the left is at most the smallest value on the right. That takes
  O(log(min(n1, n2))) time. This file loses because it steps through elements
  one by one. It also keeps going to the end of both arrays after it has already
  passed ind2.
INVARIANT
  Before each step, cnt equals i + j. The elements already taken are exactly the
  cnt smallest values of the combined input, in sorted order. So the element
  taken when cnt == ind1 really is the value at merged index ind1, and the same
  holds for ind2. The two tail loops keep this true once one array runs out,
  because the other array is already sorted.
CAST BEFORE ADDING
  The even case computes ((double)ind1el + ind2el). The cast happens before the
  addition, so the sum is done in double. If you wrote (ind1el + ind2el) / 2.0,
  the int sum could overflow when both values are near int.MaxValue.
WATCH OUT
  If both arrays are empty, n is 0, so ind1 is -1 and ind2 is 0. Neither index
  is ever reached, and the method returns (-1 + -1) / 2 = -1 with no error. That
  -1 comes from the starting values of ind1el and ind2el, and it looks like a
  real answer. When n is 1, ind1 is -1 and is never matched. This is safe only
  because the odd branch returns ind2el and never reads ind1el.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How can you make this merge faster without changing the method?
     Break out of the loops as soon as cnt > ind2. This cuts the work to about n
     / 2 steps, but the time is still linear. The code also gets more complex,
     because all three loops need the exit.
  2. How do you find the k-th smallest element of the two arrays?
     Use the same merge and stop when cnt == k. For log time, compare nums1[k/2
     - 1] with nums2[k/2 - 1] and drop the smaller half. Then k shrinks by about
     half each round, which gives O(log k) time.
  3. What if the numbers arrive as a stream and you need the median after each
  one?
     Keep a max-heap for the lower half and a min-heap for the upper half. Keep
     their sizes equal or off by one. Each insert costs O(log n), and reading
     the median costs O(1). You pay O(n) memory for the heaps.
TRIGGER
  Reach for this pattern when you need a value at a fixed rank (a median, or the
  k-th smallest) from two sorted arrays and you do not need the whole merged
  result.
C# NOTE
  The same four-line body (check ind1, check ind2, cnt++, advance) appears three
  times. A local function such as void Take(int v) would remove the copies, so a
  fix only has to be made in one place.
COMPLEXITY
  Time  : O(n + m)
  Space : O(1)
================================================================================
*/
