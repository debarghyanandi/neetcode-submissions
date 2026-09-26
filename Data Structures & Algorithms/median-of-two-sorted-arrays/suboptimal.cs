// --------------------------------------------------------------------------
// -  suboptimal.cs         O(n + m) time / O(1) space
// -  two-pointer merge   [two-pointer-merge]
// -  ranks below optimal.cs (O(log(min(m, n))) time / O(1) space)
// -
// -  Reference solution - not one you solved yourself (from submission-0)
// -
// -  Both arrays are walked exactly once with two pointers, merging in
// -  sorted order.
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
 PATTERN : Two-pointer merge - pick the two middle positions by count
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Suboptimal
================================================================================
VARIABLES
  n        n1 + n2, the total length of the virtual merged array
  ind2     upper middle index in the merged order, n / 2
  ind1     lower middle index, ind2 - 1
  cnt      how many merged elements have been consumed so far
  ind1el   value found at merged position ind1 (stays -1 if never hit)
  ind2el   value found at merged position ind2 (stays -1 if never hit)
WHY THIS PATTERN
  The median is defined by position in the sorted union of nums1 and nums2, and
  both inputs are already sorted. So you never need to build the union - you
  only need to walk it in order and stop at the one or two middle positions. The
  classic two-pointer merge does exactly that: i and j always sit on the
  smallest unused element of each array, cnt counts how many merged elements
  have passed, and ind1el / ind2el are captured when cnt reaches ind1 and ind2.
BETTER APPROACH
  The better approach is the binary search partition: binary search on how many
  elements of the shorter array go into the left half, check the four border
  values (maxLeft1, minRight1, maxLeft2, minRight2), and get the median in O(log
  min(n1, n2)) time. This file loses because it physically steps over every
  element up to the middle, so the work grows with the input size instead of
  with its logarithm. The two answers are identical; only the cost differs.
INVARIANT
  Before each comparison, everything already counted is exactly the first cnt
  elements of the true merged sorted order, and nums1[i] / nums2[j] are the two
  candidates for merged position cnt. Taking the smaller head keeps that true
  for cnt + 1, because both arrays are sorted so no smaller unused value exists
  elsewhere. Therefore the checks cnt == ind1 and cnt == ind2 fire on exactly
  the elements sitting at those merged positions, which is the definition of the
  median inputs.
ONE INDEX SCHEME FOR BOTH PARITIES
  The code always tracks two positions, ind2 = n / 2 and ind1 = ind2 - 1,
  instead of branching on parity during the walk. For odd n, ind2 is the single
  true middle and ind1el is captured but simply ignored by the final return. For
  even n, the two captured values are the pair that must be averaged. This keeps
  the three loop bodies identical and moves the parity decision to one line at
  the end.
THE LOOPS NEVER STOP EARLY
  Once cnt passes ind2, both target values are already stored, yet all three
  loops keep running to the end of both arrays. Adding a break when cnt > ind2
  would cut the constant factor roughly in half on average without changing the
  asymptotic cost. It does not affect correctness because ind1el and ind2el are
  only written on exact matches.
WATCH OUT
  If both arrays are empty, n is 0, ind2 is 0, ind1 is -1, no loop body runs,
  and the method returns the sentinel -1 as if it were a median. If n is 1, ind1
  is -1 and ind1el is never set, which is safe only because the odd branch
  returns ind2el alone - do not "fix" the odd case to average both. The sentinel
  -1 is also a legal input value, so never test "was it found" by comparing
  ind1el to -1. The equal case falls into the else branch and advances j, which
  is fine for the median but is not a stable merge if you later reuse this loop
  for something order sensitive.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How do you get to logarithmic time?
     Binary search the split point on the shorter array: choose cut1 in [0, n1],
     set cut2 = (n + 1) / 2 - cut1, then move the search window until maxLeft1
     <= minRight2 and maxLeft2 <= minRight1. Trade-off: many off-by-one and
     empty-side edge cases (use int.MinValue / int.MaxValue for missing borders)
     in exchange for far fewer steps.
  2. What if you needed the k-th smallest element instead of the median?
     The same walk works with a single target index k and one captured value;
     only ind1/ind2 collapse into one variable. The binary search version
     generalises too, by cutting so that cut1 + cut2 == k.
  3. What if there are K sorted arrays, not two?
     Replace the pairwise comparison with a min-heap of the K current heads
     (PriorityQueue in C#), pop and push until cnt reaches the middle positions.
     Cost becomes O(n log K) where n is the total length.
  4. What if the numbers arrive as a stream with no sorted order?
     Switch to the two-heap median technique - a max-heap for the lower half, a
     min-heap for the upper half, rebalanced on each insert. That gives O(log n)
     per insert but needs O(n) memory, unlike this file.
TRIGGER
  Two or more already sorted sequences and the question asks for a value at a
  specific position in their union.
C# NOTE
  The explicit (double) on ind1el is load-bearing: without it, ind1el + ind2el
  would be integer division by 2.0... actually it would be an int sum that can
  overflow before the division, so the cast both widens and protects the sum. In
  the odd branch, return ind2el needs no cast at all because C# implicitly
  converts int to double on return.
COMPLEXITY
  Time  : O(n + m)
  Space : O(1)
================================================================================
*/
