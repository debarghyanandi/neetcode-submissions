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
 PATTERN : Two Pointers / Greedy - drop the shorter wall
 SOURCE  : YOUR OWN SOLUTION - your own annotation at c76939d
 STATUS  : Optimal
================================================================================
VARIABLES
  left     index of the left wall in the current pair
  right    index of the right wall in the current pair
  height   the water level for this pair = the shorter of the two walls
WHY THIS PATTERN
  The problem asks for the best pair of walls, and a pair's value depends on two
  things: the distance between the walls and the shorter wall. We start with
  left and right at the two ends, which gives the widest pair. After that, every
  move makes the width smaller. So the only way to get a bigger area is to find
  a taller shorter wall. That means we should always move the pointer on the
  shorter side.
BRUTE FORCE
  Try every pair with two nested loops, i < j. Compute (j - i) *
  Math.Min(heights[i], heights[j]) and keep the largest value. This is correct
  and simple, but it takes O(n^2) time because it checks about n^2 / 2 pairs. It
  loses because it never uses the fact that a shorter wall limits every pair it
  is part of.
INVARIANT
  Before each loop step, the best pair is either already counted in maxArea or
  it lies fully inside [left, right]. Say heights[left] < heights[right]. Pair
  left with any inner wall: the width is smaller, and the height is still at
  most heights[left]. So none of those pairs can beat the area we just recorded,
  and removing left loses nothing. The loop stops when left meets right, and at
  that point every pair has been counted or safely ruled out.
WATCH OUT
  width * height uses int. If the heights and the array length are both large,
  the product can overflow and wrap to a negative number. If that can happen,
  compute area as a long. When heights[left] == heights[right], the else branch
  moves right. This is correct, because both walls cap every inner pair at the
  same level, but do not "fix" it with a third branch that moves nothing, or the
  loop never ends. The comment on the discard step matches the code.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How would you return the two indices instead of the area?
     When area > maxArea, also save left and right into two extra variables. The
     time stays the same, and you only need two more ints.
  2. Can you skip some steps?
     After you drop a wall of height h, keep moving that pointer past every wall
     with height <= h. None of those walls can do better, because the width is
     smaller and the height is no higher. This saves area calculations, but the
     worst case is still linear.
  3. How is this different from Trapping Rain Water?
     Here only two walls hold the water, and the walls in between do not matter.
     In Trapping Rain Water, every bar holds its own water, so you must track
     leftMax and rightMax and add up the water at each index. It is still two
     pointers, but it builds a running total instead of keeping one best pair.
TRIGGER
  When you must pick the best pair (i, j) in an array and the score is limited
  by the smaller of the two ends, start pointers at both ends and move the
  weaker side inward.
C# NOTE
  The if (area > maxArea) block can be written as maxArea = Math.Max(maxArea,
  area). The result is the same, and the one-line form is the usual C# idiom and
  is easier to read quickly in an interview.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
