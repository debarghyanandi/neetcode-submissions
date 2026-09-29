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
 PROBLEM : Given two sorted int arrays nums1 and nums2, return the median of
           all their values together, as a double. If the total count is even,
           the median is the average of the two middle values. Example: [1,3],
           [2] -> 2.0.
 PATTERN : Two Pointers (merge step of merge sort) + counter
================================================================================
IDEA
  Walk both arrays with i and j, as in the merge step, always taking the
  smaller value. cnt is the index of that value in the merged order. When cnt
  hits ind1 = n/2 - 1 or ind2 = n/2, save the value in ind1el or ind2el. This
  is correct because the merge visits values in fully sorted order. Unlike
  optimal.cs, it walks the arrays value by value instead of binary searching.
EXAMPLE
  nums1=[1,3], nums2=[2,4,5,6]: n=6, ind1=2, ind2=3.
  Main loop: 1(cnt 0), 2(cnt 1), 3(cnt 2) -> ind1el=3; nums1 is used up.
  Tail loop on nums2: 4(cnt 3) -> ind2el=4; 5 and 6 are still walked.
  n is even -> (3+4)/2 = 3.5.
COMPLEXITY
  Time  O(n + m)  every value of both arrays is visited once by i or j; no
                  early exit
  Space O(1)      only a few int counters and two saved values
WATCH OUT
  - Forgetting the two tail loops: here the median value 4 is set only in
    the nums2 tail loop, so skipping it gives a wrong answer.
  - For odd n return ind2el (index n/2), not ind1el. When n=1, ind1 is -1,
    so ind1el is never set, and that is fine.
  - Keep the (double) cast before the add. Adding two ints first can
    overflow, and dividing by 2 as int drops the .5.
  - If both arrays are empty, it returns -1.0 (the start values of ind1el
    and ind2el) and does not fail.
================================================================================
*/
