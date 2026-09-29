// ##########################################################################
// #  optimal.cs            O(n) time / O(1) space
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
 PROBLEM : Given an array heights, each value is a vertical wall at that
           index. Pick two walls. With the x-axis, they form a container that
           holds water. Return the max water: (j - i) * min(heights[i],
           heights[j]). Example: [1,8,6,2,5,4,8,3,7] -> 49 (walls at index 1
           and 8).
 PATTERN : Two Pointers (converging from both ends)
================================================================================
IDEA
  Start with left at 0 and right at the last index, the widest container.
  Each step computes area and keeps the best in maxArea.
  Then move the pointer at the SHORTER wall inward.
  This is correct because the shorter wall limits height. Any other pair
  using it is narrower and no taller, so it can never beat the current area.
EXAMPLE
  heights = [2,5,3,5,1] (tie case at 5,5)
  (0,4) w4 h1 a4 -> (0,3) w3 h2 a6 -> (1,3) w2 h5 a10, tie so right--
  -> (1,2) w1 h3 a3 -> left == right, stop. Answer: 10
COMPLEXITY
  Time  O(n)  each step moves left or right one index, so at most n-1 steps
  Space O(1)  only a few int variables
PATH TO OPTIMAL
  Brute force: try every pair i<j - O(n^2) time - simple but too slow.
  Two pointers (optimal.cs) - O(n) time, O(1) space - drops a whole wall
  per step instead of checking all its pairs.
KEYWORDS
  two pointers, greedy, container with most water, array, area, shorter wall
WATCH OUT
  - Moving the TALLER wall is the classic bug: it can only lose width and
    never gains height, so it can skip the answer.
  - width * height is int. If heights and length are big, it can overflow;
    use long if the interviewer says values are large.
  - Do not mix it up with Trapping Rain Water: here only two walls count,
    walls in between do not block anything.
  - Tie: heights[left] == heights[right] goes to right--. Either move is
    safe, since both walls are equally limiting.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Why is it safe to skip the shorter wall for good?
     -> Every other pair with it is narrower and its height is still at most
        that wall. So its best area is the one just computed.
  2. Return the two indices, not the area?
     -> Save left and right when area > maxArea. Still O(n) time, O(1) space.
  3. What if the water trapped over all bars is asked (Trapping Rain Water)?
     -> Use two pointers with leftMax and rightMax, add min side max minus
        height at each step. O(n) time, O(1) space.
  4. Can you skip useless steps?
     -> After moving, keep moving while the new wall is not taller than the
        old one. Same O(n) worst case, fewer area computations.
TRIGGER
  Pick a pair from an array where the score is width times the smaller value,
  so a greedy move from both ends can safely drop the limiting side.
================================================================================
*/
