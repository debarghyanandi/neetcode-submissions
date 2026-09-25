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
 PATTERN : Two Pointers - shrink from the shorter side
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-1.cs when it was first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  l          left scan index, walks right
  r          right scan index, walks left
  lMax       tallest bar seen in height[0..l]
  rMax       tallest bar seen in height[r..end]
  rainTotal  water collected so far, summed per column
WHY THIS PATTERN
  Water over one column is min(tallest to its left, tallest to its right) minus
  that column's own height. That needs two facts per index, but they come from
  opposite ends, so one pass from a single side cannot know both. Two pointers
  fix that: whenever height[l] <= height[r], the right side is known to hold at
  least height[r], so lMax alone decides the water at l, and the same logic
  mirrored decides the water at r.
BRUTE FORCE
  The first idea is: for each index i, scan left for the max and scan right for
  the max, add min of the two minus height[i]. That is correct and O(n^2) time,
  which loses on long inputs. The usual next step is two prefix arrays,
  leftMax[i] and rightMax[i], which is O(n) time but spends O(n) extra memory
  that lMax and rMax replace.
INVARIANT
  At the top of every loop pass, lMax is the maximum of height[0..l] and rMax is
  the maximum of height[r..end], and rainTotal is the exact water over every
  column already passed. The branch guarantee is the key: entering the first
  branch means height[l] <= height[r], so some bar at or right of r is at least
  as tall as any wall lMax could be, hence min(lMax, trueRightMax) == lMax and
  the water at l is lMax - height[l]. When the loop ends l == r, and that single
  column can hold nothing above the shorter of the two bounding walls, so
  nothing is missed.
WHY THE UPDATE AND THE ADD ARE EXCLUSIVE
  Each branch either adds water or raises the running max, never both. That is
  correct because if height[l] >= lMax then the column is itself the new wall
  and holds zero water, so adding (lMax - height[l]) would be zero or negative.
  Writing it as if/else avoids ever adding a negative amount, which is what a
  careless max-then-add order can do.
WATCH OUT
  lMax and rMax start at 0, which is safe only if no height is negative; a
  negative value would enter the else branch and set a max lower than the true
  wall. If height is null this throws on height.Length, and there is no explicit
  guard. An empty array gives r == -1 and the loop body never runs, returning 0,
  so that case is fine by accident rather than by design. The comparison must be
  height[l] <= height[r] or height[l] < height[r] with matching branches -
  flipping which side moves on a tie is fine, but the tie must move exactly one
  pointer, otherwise equal walls can spin or skip.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you do it in one forward pass with a stack instead?
     Yes - a monotonic decreasing stack of indices pops when a taller bar
     arrives and adds the rectangle between the popped bar and the new one.
     Still O(n) time but O(n) space in the worst case, and it computes water in
     horizontal slabs rather than per column.
  2. What if you must also return the index where the deepest water sits?
     Track a running best inside each branch: compare lMax - height[l] (or rMax
     - height[r]) against a stored maximum depth and record l or r when it wins.
     No extra pass and no extra memory beyond two ints.
  3. The input arrives as a stream you can read only once, forward?
     The two-pointer trick dies because it needs access to the right end. You
     would buffer the array, or make two passes if you can rewind: one to build
     rightMax, one to sum. That is back to O(n) space.
  4. How would you handle very large heights where the sum overflows int?
     rainTotal is an int here; change it and the return type to long. The
     per-column terms stay small, but the total is a sum over all columns and is
     the part that can overflow.
TRIGGER
  A value at each index depends on a max (or min) from both its left and its
  right, and you want to drop the two precomputed arrays.
C# NOTE
  height.Length is read once into r rather than checked in the loop condition,
  so the loop compares two locals only; and because int[] is used directly
  instead of IList<int> or a List, every access is a plain array index with no
  interface dispatch.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
