// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
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
 PROBLEM : You get an array height of bar heights. Each bar is 1 wide. Return
           how many units of rain water are trapped between the bars. Water
           above index i is min(tallest bar left, tallest bar right) -
           height[i]. Example: [4,2,0,3,2,5] -> 9
 PATTERN : Two Pointers (converging) + running max
================================================================================
IDEA
  l starts at the left end, r at the right end. lMax and rMax keep the tallest
  bar seen so far on each side. Always move the side with the lower bar.
  If height[l] <= height[r], then some bar on the right is at least lMax.
  So lMax alone limits the water at l: add lMax - height[l], or raise lMax.
  The right side works the same way with rMax. Each cell is counted once.
EXAMPLE
  height = [2,0,3,0,1]
  r=4: 2>1, rMax=1 | r=3: add 1-0=1 | l=0: 2<=3, lMax=2 | l=1: add 2-0=2
  Loop stops at l=r=2 (the tallest bar is never visited). Answer: 3
COMPLEXITY
  Time  O(n)  each step moves l or r one index closer, so about n steps
  Space O(1)  only four ints (l, r, lMax, rMax) plus rainTotal
PATH TO OPTIMAL
  Brute force: for each i, scan left and right for maxes - O(n^2) time.
  Prefix/suffix max arrays - O(n) time, O(n) space - no rescans
  (suboptimal.cs).
  Two pointers with lMax/rMax - O(n) time, O(1) space - no extra arrays.
KEYWORDS
  two pointers, prefix max, suffix max, monotonic stack, water level, array
WATCH OUT
  - Compare height[l] with height[r], not lMax with rMax. Both work, but
    mixing the two rules in one loop gives wrong totals.
  - Raise lMax BEFORE you subtract, or use the else as here. Otherwise you
    can add a negative amount when height[l] > lMax.
  - Empty or 1-bar input: r = -1 or 0, the loop never runs, return 0. OK.
  - rainTotal is an int. A very long, tall input could overflow; use long.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Why is it safe to use only lMax when height[l] <= height[r]?
     -> height[r] is already at least height[l], so the right wall is at least
        lMax too. So min(leftMax, rightMax) = lMax, and we never need rightMax.
  2. Can you solve it with a stack?
     -> Use a monotonic decreasing stack of indices. When a taller bar comes,
        pop and add water layer by layer. O(n) time, O(n) space, fills by rows.
  3. What about a 2D grid (Trapping Rain Water II)?
     -> Push all border cells into a min-heap and pop the lowest wall first.
        BFS inward, fill neighbors to that level. O(mn log(mn)) time.
  4. What if the heights arrive as a stream?
     -> The right max is unknown, so you cannot finish a cell early. Keep a
        stack of open walls and add water when a taller bar closes a basin.
TRIGGER
  When the answer at each index depends on the max to its left and the max
  to its right, think prefix/suffix max, then shrink it to two pointers.
================================================================================
*/
