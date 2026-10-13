// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(n) space
// --------------------------------------------------------------------------

public class Solution
{
    public int LargestRectangleArea(int[] heights)
    {

        Stack<int> stack = new Stack<int>();

        int maxArea = 0;

        int n = heights.Length;

        for (int i = 0; i < n; i++)
        {
            while (stack.Count > 0 && heights[stack.Peek()] > heights[i])
            {
                int element = heights[stack.Pop()];

                int nse = i;
                int pse = stack.Count == 0 ? -1 : stack.Peek();

                maxArea = Math.Max(maxArea, element * (nse - pse - 1));

            }
            stack.Push(i);

        }

        while (stack.Count > 0)
        {
            int nse = n;
            int currelement = heights[stack.Peek()];
            stack.Pop();
            int pse = stack.Count == 0 ? -1 : stack.Peek();
            maxArea = Math.Max(maxArea, currelement * (nse - pse - 1));

        }
        return maxArea;
    }
}

/*
================================================================================
 PROBLEM : You get an array heights. Each bar is 1 wide and stands side by
           side. Return the area of the largest rectangle that fits inside the
           histogram. Example: [2,1,5,6,2,3] -> 10 (bars 5 and 6, height 5,
           width 2).
 PATTERN : Monotonic Stack (increasing) + next/previous smaller
================================================================================
IDEA
  For each bar, the best rectangle with that bar as its height spans from
  its previous smaller bar (pse) to its next smaller bar (nse). The stack
  keeps indices with increasing heights. When heights[i] is smaller than
  the top, i is the nse of the top, and the index below it is its pse.
  Area = element * (nse - pse - 1). Bars left at the end use nse = n.
  Correct because every bar is popped once, and the max rectangle's
  height must equal some bar's height.
EXAMPLE
  [2,1,5,6,2,3]: i=1 pops 2 (pse=-1,nse=1) -> 2*1=2.
  i=4 pops 6 (pse=2) -> 6*1=6; pops 5 (pse=1) -> 5*2=10. Stack [1,4,5].
  End (nse=6): 3*1=3, 2*(6-1-1)=8, 1*6=6. Answer 10.
COMPLEXITY
  Time  O(n)  each index is pushed once and popped once (amortized O(1)).
  Space O(n)  the stack can hold all n indices (increasing heights).
PATH TO OPTIMAL
  Brute force: try every pair (l, r) with running min - O(n^2) - simple.
  Expand left/right from each bar - O(n^2) worst - same idea, no pairs.
  Monotonic stack (this file) - O(n) - finds pse/nse for all bars at once.
KEYWORDS
  monotonic stack, histogram, next smaller element, previous smaller element,
  maximal rectangle, amortized
WATCH OUT
  - Width is nse - pse - 1, not nse - pse. Use pse = -1 when stack is empty.
  - Forgetting the final while loop misses increasing runs like [1,2,3].
  - Push indices, not heights; you need positions to get the width.
  - element * width is int; very tall and wide inputs could overflow.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you do it in one pass without the second loop?
     -> Loop i to n and treat heights[n] as 0 (a sentinel). It forces all
        pops. Same O(n) time and space, shorter code.
  2. Largest rectangle of 1s in a binary matrix (Maximal Rectangle)?
     -> Build a heights row by row (+1 if cell is 1, else 0) and run this per
        row. O(rows*cols) time, O(cols) space.
  3. Why does strict > still work with equal heights?
     -> Equal bars stay on the stack. The leftmost one is popped last and gets
        the full width, so the max is still found.
  4. Any other technique?
     -> Divide and conquer on the min bar: O(n log n) average, O(n^2) worst on
        sorted input; a segment tree for range min gives O(n log n).
TRIGGER
  When each element needs its nearest smaller (or larger) neighbor on both
  sides to define a range, reach for a monotonic stack.
================================================================================
*/
