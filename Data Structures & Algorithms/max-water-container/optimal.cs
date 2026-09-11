// ##########################################################################
// #  optimal.cs            O(n) time / O(1) space
// #  two pointers, discard shorter wall   [two-pointer-elimination]
// #  the only solution in this folder
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  Single sweep from both ends inward, always discarding the shorter wall
// #  since it can never yield a larger area.
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
 PATTERN : Two Pointers - shrink inward from the widest pair
 SOURCE  : YOUR OWN SOLUTION - your own annotation at c76939d
 STATUS  : Optimal
================================================================================
BRUTE FORCE FIRST
  The honest baseline is every pair: for each i, for each j > i, area = (j - i)
  * Min(heights[i], heights[j]). That is n*(n-1)/2 pairs. The two-pointer
  version visits only n-1 of them, because each iteration proves a whole family
  of pairs can never beat what has already been measured. The starting state
  left = 0, right = heights.Length - 1 is not arbitrary - it is the single
  widest pair, the only pair whose width term cannot be improved.
INVARIANT
  At the top of every iteration, the best pair that has not yet been evaluated
  lies entirely inside the window [left, right], and maxArea already holds the
  best area over every pair that has been excluded from that window. The loop
  shrinks the window by exactly one index per pass (left++ or right--), so width
  = right - left strictly decreases and the loop runs at most heights.Length - 1
  times and always terminates.
CORRECTNESS ARGUMENT
  This is the part an interviewer will push on: why is discarding a wall safe?

  Suppose heights[left] < heights[right], so the branch does left++. Take any
  pair (left, j) with j <= right that this discards. Its area is (j - left) *
  Min(heights[left], heights[j]). Two bounds hold at once: j - left <= right -
  left, and Min(heights[left], heights[j]) <= heights[left] = Min(heights[left],
  heights[right]). Multiply them - every discarded pair is bounded above by
  (right - left) * heights[left], which is exactly the area just computed and
  folded into maxArea on this same iteration. Nothing is lost. The mirrored
  argument covers right-- when heights[right] <= heights[left].

  Note the order matters in the code: area is computed and compared BEFORE a
  pointer moves. The proof leans on that - the discarded family is dominated by
  the pair being retired, so the pair must be measured first.
WHY NOT MOVE THE TALLER WALL
  A weaker intuition says "move the taller wall, maybe you find something
  better." You do not. Moving right-- when heights[right] is the taller one
  gives a pair whose height is still capped at or below heights[left] and whose
  width is strictly smaller, so that single step can only shrink area. But that
  local observation is not the proof - it only rules out the next pair, not all
  of them. The real justification is the domination bound above: the SHORTER
  wall is the binding constraint, and it stays binding for every pairing it
  could still take, all of which are narrower than the current one.
ALGORITHM
  1. left = 0, right = heights.Length - 1, maxArea = 0.
  2. While left < right: width = right - left, height = Math.Min(heights[left],
  heights[right]), area = width * height.
  3. Keep area if it beats maxArea.
  4. If heights[left] < heights[right] then left++, else right--.
  5. Return maxArea.

  Degenerate inputs fall out for free: length 0 or 1 makes right equal -1 or 0,
  the while condition fails immediately, and the answer is 0.
WATCH OUT
  Math.Min, not Math.Max - water spills over the shorter wall. Writing Max here
  still passes a few hand-tested cases, which makes it a nasty bug to spot.

  The area sequence is NOT monotonic as the window shrinks, so maxArea must be
  updated every iteration and there is no valid early exit; the loop has to run
  all the way to left == right.

  The tie case heights[left] == heights[right] falls into the else and does
  right--. That is fine: when both walls are equal, each is binding, and the
  domination bound holds for either one, so discarding just one is safe
  (discarding both would also be safe, but is not needed for correctness).

  This is not Trapping Rain Water. The walls between left and right are
  irrelevant here - nothing blocks the container, and no index between the two
  pointers is ever read. The only reads are heights[left] and heights[right].
TRIGGER
  Reach for this shape when the answer is a pair (i, j) whose score factors into
  a term that improves monotonically in one direction (width, maximal at the
  extremes) and a term set by a min or max over the endpoints. Start at the
  extreme of the monotone term and repeatedly retire the endpoint that caps the
  other term. The same skeleton with a different score is how many "best pair
  over a sorted or index-ordered array" problems collapse from n^2 to n.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
