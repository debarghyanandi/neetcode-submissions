// --------------------------------------------------------------------------
// -  optimal-variant.cs    O(n) time / O(1) space
// --------------------------------------------------------------------------

public class Solution
{
    public int Jump(int[] nums)
    {

        //Same TC/Sc as the other sol.
        //Clean code but Not Intuitive.  

        int n = nums.Length;
        int jumps = 0;

        int currEnd = 0;
        int farthest = 0;

        for (int i = 0; i < n - 1; i++)
        {
            farthest = Math.Max(farthest, i + nums[i]);

            if (i == currEnd)
            {
                jumps++;
                currEnd = farthest;
            }
        }
        return jumps;
    }
}

/*
================================================================================
 PROBLEM : You get an int array nums. You start at index 0. From index i you
           may jump forward up to nums[i] steps. Return the minimum number of
           jumps to reach the last index. The last index is guaranteed to be
           reachable. Example: [2,3,1,1,4] -> 2 (0 -> 1 -> 4).
 PATTERN : Greedy (implicit BFS levels, one pass)
================================================================================
IDEA
  Think of jumps as BFS levels. currEnd is the last index you can reach with
  the current number of jumps. farthest is the furthest index that any index
  seen so far can reach. When i reaches currEnd, the current level is used up.
  So you must take one more jump, and the new level ends at farthest.
  Unlike optimal.cs with its explicit window, this is one index loop.
EXAMPLE
  nums = [2,3,1,1,4], loop i = 0..3
  i=0: farthest=2, i==currEnd -> jumps=1, currEnd=2
  i=1: farthest=4; i=2: farthest=4, i==currEnd -> jumps=2, currEnd=4
  i=3: farthest=4. Return 2. Edge case: [0] skips the loop and returns 0.
COMPLEXITY
  Time  O(n)  one pass of i over the array, O(1) work per step
  Space O(1)  only jumps, currEnd and farthest
WATCH OUT
  - The loop must be i < n - 1. With i < n, [1,1] gives 2, not 1. A jump
    would be counted when you are already standing on the last index.
  - Update farthest BEFORE the i == currEnd check. Otherwise the new
    currEnd misses the reach of index i itself.
  - The code assumes the end is reachable. [0,1] returns 1, not "impossible".
    To detect this, check whether farthest <= i when i == currEnd.
================================================================================
*/
