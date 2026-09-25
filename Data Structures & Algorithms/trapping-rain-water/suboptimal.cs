// --------------------------------------------------------------------------
// -  suboptimal.cs         O(n) time / O(n) space
// -  Prefix and suffix maximum arrays   [prefix-suffix-max]
// -  ranks below optimal.cs (O(n) time / O(1) space)
// -
// -  Reference solution - not one you solved yourself (from submission-0)
// -
// -  Three linear passes: build left maxima, build right maxima, compute
// -  trapped water per position.
// --------------------------------------------------------------------------

public class Solution
{
    public int Trap(int[] height)
    {
        int n = height.Length;
        int rainTotal = 0;

        //prefixMax
        int[] lMax = new int[n];
        lMax[0] = height[0];
        for (int i = 1; i < n; i++)
        {
            lMax[i] = Math.Max(lMax[i - 1], height[i]);
        }

        //suffixMax
        int[] rMax = new int[n];
        rMax[n - 1] = height[n - 1];
        for (int i = n - 2; i >= 0; i--)
        {
            rMax[i] = Math.Max(rMax[i + 1], height[i]);
        }

        for (int i = 0; i < n; i++)
        {
            ///if(height[i] < lMax[i] && height[i] < rMax[i])

            // No if needed because lMax[i] and rMax[i] include height[i] itself.
            // So min(lMax[i], rMax[i]) is always >= height[i].
            // Therefore trapped water is never negative.
            rainTotal += Math.Min(lMax[i], rMax[i]) - height[i];

        }
        return rainTotal;
    }
}

/*
================================================================================
 PATTERN : Prefix/Suffix Max Arrays - water = min(left,right) - h
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Suboptimal
================================================================================
VARIABLES
  n          height.Length, reused for both array sizes
  rainTotal  running sum of trapped water over all columns
  lMax       lMax[i] = tallest bar in height[0..i], inclusive
  rMax       rMax[i] = tallest bar in height[i..n-1], inclusive
WHY THIS PATTERN
  The water sitting on top of column i depends only on the tallest bar to its
  left and the tallest bar to its right; the level is min of those two, and the
  column itself takes up height[i] of it. That is a per-index question whose
  answer needs information from both directions, which is exactly what a forward
  scan (lMax) plus a backward scan (rMax) gives you in one pass each. Once both
  arrays exist, the third loop is a plain independent sum into rainTotal, with
  no ordering worries.
BETTER APPROACH
  The better version is the two-pointer sweep: keep left and right indices plus
  leftMax and rightMax scalars, always move the side whose max is smaller, and
  add that side's max minus its height. It gets the same answer in O(1) extra
  space instead of the two int[n] arrays this file allocates. This file loses
  purely on memory; the time is the same, and it costs three passes instead of
  one.
INVARIANT
  After the first loop, lMax[i] is the max of height[0..i] for every i; after
  the second, rMax[i] is the max of height[i..n-1]. Both ranges are closed and
  both include position i itself, so min(lMax[i], rMax[i]) >= height[i] always.
  That makes every term added to rainTotal zero or positive, and each term is
  exactly the water above column i, so the sum is the total.
WATCH OUT
  An empty array breaks this immediately: n is 0, so lMax[0] = height[0] throws
  IndexOutOfRangeException, and rMax[n-1] indexes -1. A null height throws on
  height.Length before that. The long comment block is correct and the
  commented-out if on the line above it is genuinely redundant, but note the
  reason is the inclusive endpoints - if you ever switch lMax to "strictly left
  of i" (exclusive), the subtraction can go negative and you must put the guard
  back. Both helper arrays live until the method returns, so peak memory is 2n
  ints even though the last loop reads each slot once.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Cut the extra space to O(1).
     Two pointers from both ends with leftMax and rightMax as plain ints.
     Advance whichever side has the smaller max, because that side's answer is
     already decided by its own max. Same answer, one pass, no arrays - but the
     logic is harder to read than these three loops.
  2. Drop one array but keep the other.
     Keep rMax as the suffix array, then replace lMax with a single running int
     updated inside the final loop. Halves the allocation with almost no code
     change, and is an easy middle step to offer before the full two-pointer
     version.
  3. Many queries of trapped water on subranges [l, r] of the same fixed height
  array.
     Prefix/suffix maxima no longer serve, because the walls change per query.
     Use a sparse table or segment tree for range max and answer each query by
     walking or by precomputing prefix sums of water only if the ranges are
     nested.
  4. Heights are 2D (a grid of cells) instead of a row.
     min of left and right no longer works; water level at a cell is set by the
     lowest point on the cheapest escape path. Use a min-heap seeded with the
     border cells and flood inward, which is O(mn log(mn)).
TRIGGER
  Each index needs a value that depends on the max (or min, or sum) of
  everything strictly before it and everything strictly after it.
C# NOTE
  new int[n] in C# zero-fills both arrays before the loops overwrite every slot,
  so you pay one write per element twice; a stackalloc int[n] span would avoid
  the heap allocation for small n, but the two-pointer rewrite removes the
  question entirely.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
