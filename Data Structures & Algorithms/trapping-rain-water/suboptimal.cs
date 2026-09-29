// --------------------------------------------------------------------------
// -  suboptimal.cs         O(n) time / O(n) space
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
 PATTERN : Prefix Max / Suffix Max - water = min of both walls
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Suboptimal
================================================================================
VARIABLES
  lMax       lMax[i] = tallest bar in height[0..i], including i
  rMax       rMax[i] = tallest bar in height[i..n-1], including i
  rainTotal  running sum of water trapped over every bar
WHY THIS PATTERN
  Water above bar i is set by the tallest wall on its left and the tallest wall
  on its right. It fills up to the lower of the two walls. Both "tallest so far"
  values can be computed once for every index: lMax in one pass from the left,
  and rMax in one pass from the right. After that, each bar costs one Math.Min
  and one subtraction.
BETTER APPROACH
  The better approach uses two pointers, left and right, and keeps a running
  leftMax and rightMax. It always moves the side with the smaller max, because
  that smaller max is already the limit for the bar there. It is still one pass
  in O(n) time, but it uses O(1) extra space. This file loses because it
  allocates two full arrays, lMax and rMax, of size n, when each index only
  needs the one smaller wall.
INVARIANT
  After the two fill loops, lMax[i] >= height[i] and rMax[i] >= height[i] for
  every i, because both ranges include i itself. So Math.Min(lMax[i], rMax[i]) -
  height[i] is the exact water depth above bar i, and it is never negative.
  Adding these depths for every i gives the total, because the water above each
  bar is counted once and only once.
WATCH OUT
  If height is empty, n is 0, so lMax[0] = height[0] throws
  IndexOutOfRangeException. You need a guard like if (n == 0) return 0.
  rainTotal is an int, so a very large total could overflow and wrap around
  without any error. Use long if the input can be large.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you solve it with a stack instead?
     Yes. Keep a monotonic stack of indices with decreasing heights. When you
     meet a taller bar, pop the bottom bar and add water in horizontal layers:
     (min(left wall, right wall) - bottom) * width. This is also O(n), but the
     logic is harder to get right than the prefix/suffix arrays.
  2. What if the heights are a 2D grid (Trapping Rain Water II)?
     A bar's limit is no longer just its left and right walls. It is the lowest
     point on any path out to the border. Put all border cells in a min-heap,
     pop the lowest cell, and flood into its neighbors, keeping a running max
     boundary. This takes O(mn log(mn)) time.
  3. What if the heights arrive as a stream and you cannot store them?
     rMax needs the future, so you cannot finalize the water at a bar until a
     taller bar arrives. A stack still works, but in the worst case (heights
     that keep decreasing) it holds everything. So O(1) memory is not possible
     in general.
TRIGGER
  Reach for this when the answer at each index depends on the maximum (or
  minimum) of everything to its left and everything to its right.
C# NOTE
  In C#, a line starting with /// is an XML documentation comment, not a normal
  comment. The commented-out if should use // so tools and warnings do not read
  it as broken doc markup.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
