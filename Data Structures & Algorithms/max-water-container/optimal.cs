// ##########################################################################
// #  YOU SOLVED THIS YOURSELF  (submission-0, marked '//My Solution.')
// #  and it was already the optimal approach - nothing to improve
// ##########################################################################

public class Solution
{
    public int MaxArea(int[] heights)
    {
        int left = 0;
        int right = heights.Length - 1;
        int maxArea = 0;

        while (left < right)
        {
            // Width is the index gap; height is capped by the SHORTER wall.
            int width = right - left;
            int height = Math.Min(heights[left], heights[right]);
            int area = width * height;

            if (area > maxArea)
                maxArea = area;

            // Discard the shorter wall - it is the binding constraint and
            // cannot do better with any narrower pairing.
            if (heights[left] < heights[right])
                left++;
            else
                right--;
        }

        return maxArea;
    }
}

/*
================================================================================
 PATTERN : Two Pointers - Greedy Elimination from Both Ends
 SOURCE  : YOUR OWN SOLUTION (submission-0, marked '//My Solution.')
           - and it was already the optimal approach
 STATUS  : Optimal
================================================================================

WHY THIS PATTERN
  Area = width * min(left wall, right wall). Starting at the widest possible
  pair maximises the first factor, so every later move can only LOSE width -
  meaning it is only worth making if it can GAIN height. That asymmetry is
  what turns an O(n^2) search into a single sweep.

BRUTE FORCE (and why it fails)
  Try every pair: O(n^2). At n = 10^5 that is 5 * 10^9 area computations.

THE ELIMINATION ARGUMENT (this is the whole problem)
  Suppose heights[left] < heights[right]. Consider every container that still
  uses `left`. All of them are NARROWER than the current one, and none can be
  taller than heights[left], because the shorter wall caps the height no
  matter what it is paired with. So all of them are strictly worse than the
  area just measured. Therefore `left` can never appear in a better answer
  and is safe to discard forever.

  This is why moving the SHORTER pointer is correct and moving the taller one
  is not. If you cannot reconstruct this argument, you have memorised the
  solution rather than learned it - it is the standard follow-up question.

INVARIANT
  maxArea holds the best area over every pair already eliminated, and the
  optimal pair (if not yet found) still lies within [left, right].

TIE CASE
  When the walls are equal, either may be discarded - both are capped at the
  same height and every future pairing is narrower. The `else` branch here
  moves `right`; moving `left` is equally correct.

ALGORITHM (NeetCode: "Two Pointers")
  1. left at 0, right at the last index, maxArea at 0.
  2. Measure (right - left) * min(heights[left], heights[right]).
  3. Keep it if it beats the best so far.
  4. Move the pointer at the shorter wall inward.
  5. Repeat until the pointers meet.

COMPLEXITY
  Time  : O(n) - exactly one pointer moves each iteration, they start n apart.
  Space : O(1) - three ints.

TRIGGER
  "Maximise/minimise something over a PAIR of positions" where one end is
  clearly the limiting factor and can be argued away. Distinguish from
  trapping-rain-water, which asks for a SUM over all positions and uses the
  same two pointers with a different bookkeeping rule.

C# NOTES
  - Math.Min on ints compiles to a branchless CMOV - no reason to hand-roll it.
  - Naming `width`, `height`, `area` as separate locals costs nothing after
    JIT and makes the formula self-documenting. Worth it in an interview
    where the reader is judging your thinking, not your keystrokes.
  - width * height can overflow int for large synthetic inputs; the
    constraints here keep it safe, but `long` is the defensive choice.

WATCH OUT
  - Moving the TALLER pointer looks symmetric and is wrong - it can discard
    the optimal container. Test with [1, 8, 6, 2, 5, 4, 8, 3, 7] (answer 49).
  - Height is min, not max, and not the sum. The water spills over the
    shorter wall.
================================================================================
*/
