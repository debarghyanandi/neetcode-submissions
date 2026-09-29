// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
// -  Two-pointer converging from ends   [two-pointer-trap]
// -  ranks above suboptimal.cs (O(n) time / O(n) space)
// -
// -  Reference solution - not one you solved yourself (from submission-1)
// -
// -  Each element visited once as pointers move inward; maxima tracked in
// -  scalars as convergence proceeds.
// --------------------------------------------------------------------------

public class Solution
{
    public int Trap(int[] height)
    {
        int l = 0;
        int r = height.Length - 1;

        int lMax = 0;
        int rMax = 0;
        int rainTotal = 0;

        while (l < r)
        {

            if (height[l] <= height[r])
            {
                if (height[l] < lMax)
                    rainTotal += (lMax - height[l]);
                else
                    lMax = height[l];

                l++;
            }
            else
            {
                if (height[r] < rMax)
                    rainTotal += (rMax - height[r]);
                else
                    rMax = height[r];

                r--;
            }

        }
        return rainTotal;
    }
}

/*
================================================================================
 PATTERN : Two Pointers - move the lower side, keep a max per side
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-1.cs when it was first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  l          left pointer; bars left of l are already counted
  r          right pointer; bars right of r are already counted
  lMax       tallest bar seen so far in height[0..l-1]
  rMax       tallest bar seen so far in height[r+1..end]
  rainTotal  sum of water counted at every bar already passed
WHY THIS PATTERN
  The water above one bar is min(tallest bar on its left, tallest bar on its
  right) minus its own height. You do not need both exact maximums. You only
  need the smaller one. The two pointers let the code always work on the side
  whose limit is already known, so one pass with lMax and rMax is enough.
BRUTE FORCE
  For each index, scan left and right to find the tallest bar on each side. Then
  add min(leftMax, rightMax) - height[i]. This is correct but takes O(n^2) time,
  because every bar rescans the whole array. A middle step is to fill prefix-max
  and suffix-max arrays first. That gives O(n) time but costs two extra arrays
  of size n, and this file removes them.
INVARIANT
  When height[l] <= height[r], some bar on the right, height[r], is at least as
  tall as height[l]. Also, every earlier value of lMax was set while a right bar
  at least that tall existed. So the true right maximum for index l is >= lMax,
  and the water at l is exactly lMax - height[l] (or 0 when height[l] is the new
  lMax). The right side follows the same argument with rMax. Every bar is
  settled once, at the moment its pointer passes it, so rainTotal ends as the
  exact total.
WATCH OUT
  rainTotal is an int. If the array is very long and the bars are tall, the sum
  can overflow without any error, so use long if the limits allow large totals.
  The tie height[l] == height[r] goes to the left branch. That is safe, but if
  you change <= to <, you must check the argument again for equal heights. The
  bar where l and r meet is never processed. That is correct, because it is the
  tallest bar seen and holds no water, but it is easy to "fix" by mistake with l
  <= r.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you solve it with a stack instead?
     Yes. Keep a monotonic decreasing stack of indexes, meaning the heights on
     the stack only go down from bottom to top. When a taller bar arrives, pop
     the bottom of the pit and add water in horizontal layers: width times
     (min(left wall, current) - pit height). It is also O(n) time, but it uses
     O(n) space in the worst case.
  2. What if the map is 2D (Trapping Rain Water II)?
     Two pointers no longer work. Put all border cells in a min-heap and pop the
     lowest wall each time. For each neighbor, add max(0, wall - neighbor) as
     water, and push the neighbor with height max(wall, neighbor). This takes
     O(mn log(mn)) time.
  3. What if the bars arrive as a stream and you cannot go back?
     You cannot use the right pointer, because you never see the end. Use the
     stack method, which only looks backward, and add water as each bar arrives.
TRIGGER
  The value at each index depends on the maximum on its left and the maximum on
  its right, and you only need the smaller of the two.
C# NOTE
  You can replace each if/else with lMax = Math.Max(lMax, height[l]); rainTotal
  += lMax - height[l]; (the same for rMax on the right side). The result is the
  same, because the added amount is 0 when the bar is the new maximum, and each
  branch becomes two short lines.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
