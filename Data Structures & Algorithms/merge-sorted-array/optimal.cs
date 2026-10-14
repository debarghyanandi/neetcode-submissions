// --------------------------------------------------------------------------
// -  optimal.cs            O(m + n) time / O(1) space
// --------------------------------------------------------------------------

public class Solution
{
    public void Merge(int[] nums1, int m, int[] nums2, int n)
    {

        int last = m + n - 1;
        int i = m - 1;
        int j = n - 1;

        while (j >= 0)
        {
            if (i >= 0 && nums1[i] > nums2[j])
            {
                nums1[last] = nums1[i];
                last--;
                i--;
            }
            else
            {
                nums1[last] = nums2[j];
                last--;
                j--;
            }
        }
    }
}

/*
================================================================================
 PROBLEM : nums1 has length m + n: m sorted values, then n zeros as free
           space. nums2 holds n sorted values. Merge nums2 into nums1 in place
           so nums1 is fully sorted. Return nothing. Example:
           nums1=[1,2,3,0,0,0], m=3, nums2=[2,5,6], n=3 -> [1,2,2,3,5,6]
 PATTERN : Two Pointers (from the back, writing into free space)
================================================================================
IDEA
  Fill nums1 from the end. i points at the last real value of nums1, j at
  the last value of nums2, and last at the next slot to write. Each step
  copies the larger of nums1[i] and nums2[j] into nums1[last] and moves
  that pointer left. It is safe because last >= i always, so a write never
  overwrites a nums1 value we still need.
EXAMPLE
  nums1=[1,2,3,0,0,0], m=3, nums2=[2,5,6], n=3
  3 vs 6 -> write 6 at [5]; 3 vs 5 -> 5 at [4]; 3 vs 2 -> 3 at [3]
  2 vs 2 -> tie, take nums2: 2 at [2]; j=-1, loop ends
  Answer: [1,2,2,3,5,6] (the 1 at [0] never moved)
COMPLEXITY
  Time  O(m + n)  every write lowers last by one, so at most m + n steps
  Space O(1)      only three index variables, merged inside nums1
PATH TO OPTIMAL
  Copy nums2 into the tail, then sort nums1 - O((m+n) log(m+n)) - simple.
  Merge front to back into a new array - O(m+n) time, O(m+n) space - no sort.
  Merge back to front in place (this file) - O(m+n) time, O(1) space.
KEYWORDS
  merge sorted arrays, two pointers, in-place, reverse iteration, merge sort
WATCH OUT
  - Loop on j >= 0, not i >= 0. When nums2 runs out, the rest of nums1 is
    already in place. Looping on i would leave nums2 values unwritten.
  - Keep the i >= 0 check before nums1[i]: with m=0, i=-1 and you crash.
  - Merging from the front overwrites nums1 values you have not read yet.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Why does the loop not need to copy leftover nums1 values?
     -> They are already at the front of nums1 in sorted order. Only nums2
        values must be moved, so we stop when j < 0.
  2. Merge k sorted arrays?
     -> Use a min-heap holding the head of each array. O(N log k) time, O(k)
        extra space, versus O(N k) for pairwise scans.
  3. What if nums1 had no free space at the end?
     -> Then in-place O(1) is hard; use a new array of size m+n for O(m+n)
        time and space, or the gap method (shell sort style) in O(1) space.
  4. Is the merge stable?
     -> Not as written: ties pick nums2 first. Use >= in the check to place
        nums1 ties last, so they keep their order ahead of nums2.
TRIGGER
  Two sorted sequences must merge into a buffer with free space at its end.
================================================================================
*/
