// ##########################################################################
// #  optimal.cs            O(n) time / O(1) space
// #  two-pointer, shrink shorter   [two-pointer-shrink]
// #  the only solution in this folder
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  Each pointer moves at most n times total as they converge from
// #  opposite ends; each iteration is O(1).
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
 PATTERN : Two Pointers - shrink from the shorter wall
 SOURCE  : YOUR OWN SOLUTION - your own annotation at c76939d
 STATUS  : Optimal
================================================================================
VARIABLES
  left      index of the left wall, starts at 0
  right     index of the right wall, starts at the last index
  maxArea   best water area seen so far
  width     right - left, the index gap for the current pair
  height    Math.Min of the two walls, the water level the pair can hold
WHY THIS PATTERN
  The problem asks for the best pair of lines, and the area of a pair is (right
  - left) * min of the two heights. Starting with left = 0 and right =
  heights.Length - 1 gives the widest possible pair, so width can only go down
  from there. That means the only way a later pair can win is by having a taller
  minimum, which tells you exactly which side to move: the shorter one. Each
  step throws away one wall, so the two pointers meet after scanning every
  element once.
BRUTE FORCE
  The first thing most people write is two nested loops over every pair (i, j)
  and take the max of (j - i) * Math.Min(heights[i], heights[j]). That is
  correct but O(n^2) time, which is too slow on a long array. It loses because
  it re-checks pairs that can never beat what is already found - once the wider
  pair with the same short wall has been measured, every narrower pair sharing
  that wall is provably worse.
INVARIANT
  At the top of every loop pass, maxArea is the best area over all pairs that
  contain at least one of the discarded walls, plus everything measured so far.
  When heights[left] < heights[right] we advance left: any pair (left, j) with j
  < right has width smaller than right - left and height at most heights[left],
  so its area cannot exceed the area just computed. Discarding that wall
  therefore loses no candidate, and when left meets right every pair has either
  been measured or been proven not better.
WATCH OUT
  If heights is null this throws a NullReferenceException on heights.Length, and
  if the array has 0 or 1 elements right becomes -1 or 0, the loop never runs,
  and the method returns 0 - check whether 0 is the answer you want for those
  inputs. The tie case heights[left] == heights[right] falls into the else and
  moves right; that is safe here because with equal walls both sides are equally
  binding, but be ready to explain why moving only one side is enough. width *
  height is int arithmetic, so a very long array of very tall walls can overflow
  silently into a negative number - a long accumulator would remove that risk.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you prove the greedy move more formally, or would you rather show it by
  contradiction?
     Assume the optimal pair is (a, b) with left <= a < b <= right, and that we
     are about to discard left because heights[left] < heights[right]. If a ==
     left, then the optimal pair has height at most heights[left] and width at
     most right - left, so the pair we just measured is at least as good - no
     loss. Otherwise a > left and the optimal pair survives the move.
  2. What changes if you must also return the two indices, not just the area?
     Keep two extra fields, bestLeft and bestRight, and assign them inside the
     same if (area > maxArea) block. No change to time or the pointer logic; you
     just carry two more ints.
  3. What if the input arrives as a stream and you cannot index from the right?
     The two-pointer move needs random access from both ends, so a one-pass
     stream breaks it. You would buffer the values into a list first, or fall
     back to a different formulation, which costs the extra space.
  4. What if the walls had width too - each bar holds water between itself and
  its neighbours?
     That is the trapping-rain-water problem, not this one. The answer is a sum
     over every index of min(maxLeft, maxRight) - height[i], still solvable with
     two pointers but carrying running maxima instead of a single best area.
TRIGGER
  The value of a pair depends on a span (distance) times a min or max over its
  endpoints, and you start at the extreme span - move the endpoint that is
  limiting the value.
C# NOTE
  Math.Min on two ints is the clear idiom here and keeps the "shorter wall" rule
  visible in the code. Taking int[] rather than IEnumerable<int> matters: the
  algorithm needs O(1) indexing at both ends, which IEnumerable cannot give.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
