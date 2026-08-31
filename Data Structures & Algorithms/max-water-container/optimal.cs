// ##########################################################################
// #  optimal.cs            O(n) time / O(1) space
// #  two pointers, greedy elimination from ends   [two-pointer-elimination]
// #  the only solution in this folder
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  Starts at max width and discards the shorter wall each step since it
// #  can never yield a better area, giving a single O(n) sweep.
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
 PATTERN : Two Pointers - always discard the shorter wall
 SOURCE  : YOUR OWN SOLUTION - your own annotation at c76939d
 STATUS  : Optimal
================================================================================
BRUTE FORCE
  Every pair (i, j) with i < j, area = (j - i) * Math.Min(heights[i],
  heights[j]), keep the max. That is n*(n-1)/2 pairs. The two-pointer version
  visits only n - 1 of them, so the whole burden of the proof is showing that
  the pairs it never evaluates cannot beat the ones it does.
WHY THIS PATTERN
  Area has two factors that fight each other. Starting at left = 0, right =
  heights.Length - 1 pins width at its maximum, so every later candidate is
  strictly narrower. From that starting point width can only ever go down, which
  means a move is only worth making if it might raise the height cap - and that
  gives a deterministic rule for which pointer to move. No search over pairs is
  needed.
INVARIANT
  Invariant: the best pair over the whole array is either the pair currently at
  (left, right), or lies strictly inside the window [left, right]. It holds
  initially because that window is the whole array. It is preserved by the
  discard step, which is the argument below.
WHY DISCARDING IS SAFE
  Suppose heights[left] < heights[right] and the code does left++. Consider any
  pair (left, k) that is thereby thrown away, for left < k < right. Its width is
  k - left, strictly less than the current width right - left. Its height is
  Math.Min(heights[left], heights[k]) <= heights[left], which is exactly the
  current height cap since left is the shorter wall. Both factors are <= the
  ones just measured and the width is strictly smaller, so area(left, k) <
  area(left, right), and that value was already folded into maxArea. So no
  discarded pair could have been the answer. The mirrored argument covers
  right--.

  The short version to say out loud: the shorter wall is the binding constraint.
  Keeping it can never buy you height, and moving inward always costs you width.
THE TIE CASE
  When heights[left] == heights[right] the condition heights[left] <
  heights[right] is false, so the else branch runs and only right moves. This is
  correct rather than arbitrary: with equal heights, both walls impose the same
  cap h, so the argument above applies to either one and discarding just the
  right wall loses nothing. Moving only one pointer is also what keeps the loop
  total at n - 1 iterations with no case analysis. Do not be tempted to advance
  both pointers on a tie - it happens to be safe here for the same reason, but
  it is an extra claim to defend under questioning for zero gain.
ALGORITHM
  1. left = 0, right = heights.Length - 1, maxArea = 0.
  2. While left < right: width = right - left, height = Math.Min(heights[left],
  heights[right]), area = width * height; update maxArea if area is larger.
  3. Measure first, then move: if heights[left] < heights[right] do left++, else
  do right--.
  4. Return maxArea when the pointers meet.
WATCH OUT
  Math.Min, not Math.Max - the water level is set by the shorter wall, and using
  Max is the mistake that still passes small hand-tested cases.

  Move the pointer at the SHORTER wall. Moving the taller one keeps the same cap
  and a smaller width, so maxArea can never improve and the true answer gets
  walked past.

  The area must be computed before the pointer moves; swapping those two blocks
  skips the widest pair entirely.

  Loop guard is left < right, not left <= right. At left == right the width is
  0, so it contributes nothing, but the strict form is also what guarantees
  termination since exactly one pointer moves per iteration.

  maxArea = 0 is a safe seed only because areas are non-negative. An array of
  length 0 or 1 never enters the loop and returns 0, which is the right answer -
  no separate guard needed.
TRIGGER
  Reach for this shape when the objective is a function of a pair of indices
  that is monotone-decreasing in the index gap and capped by a min/max over the
  endpoints. That structure lets you start at the extreme width and prove, at
  each step, that one endpoint can be retired forever. Trapping Rain Water uses
  the same discard-the-shorter-side reasoning, but accumulates per-index water
  instead of tracking a single best pair.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
