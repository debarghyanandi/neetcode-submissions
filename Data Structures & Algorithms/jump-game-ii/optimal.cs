// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
// --------------------------------------------------------------------------

public class Solution
{
    public int Jump(int[] nums)
    {
        int n = nums.Length;

        int jumps = 0;
        int left = 0;
        int right = 0;

        while (right < n - 1)
        {
            int farthest = 0;

            for (int i = left; i <= right; i++)
            {
                farthest = Math.Max(farthest, i + nums[i]);
            }

            left = right + 1;
            right = farthest;

            jumps++;
        }

        return jumps;
    }
}

/*
================================================================================
 PROBLEM : nums[i] is the max jump length from index i. You start at index 0.
           Return the minimum number of jumps to reach the last index. The
           last index is always reachable. Example: [2,3,1,1,4] -> 2
           (0->1->4).
 PATTERN : Greedy BFS by levels (implicit BFS over index ranges)
================================================================================
IDEA
  Indices left..right are all reachable with exactly jumps jumps (one BFS
  level).
  Scan that window and track farthest = max(i + nums[i]).
  The next level is right+1..farthest, so jumps++.
  Stop when right reaches n-1.
  Correct because this is BFS on an unweighted graph: the first level that
  contains n-1 gives the fewest jumps, and each level is one contiguous range.
EXAMPLE
  nums = [2,3,0,1,4] (a zero inside a window does no harm)
  lvl0 [0,0]: farthest=2 -> left=1, right=2, jumps=1
  lvl1 [1,2]: i=1 gives 4, i=2 gives 2 -> farthest=4 -> left=3, right=4,
  jumps=2
  right=4 = n-1, loop ends -> answer 2
COMPLEXITY
  Time  O(n)  windows do not overlap, so each index is scanned once in total
  Space O(1)  only the counters left, right, farthest, jumps
PATH TO OPTIMAL
  Brute recursion trying every jump - exponential - baseline, repeats work.
  DP: dp[i] = min jumps to reach i, from all j < i - O(n^2) - no repeats.
  Greedy level-BFS (this file) - O(n) - each index is seen in only one level.
  optimal-variant.cs - same O(n) greedy, written in a different loop form.
KEYWORDS
  greedy, implicit BFS, level order, farthest reach, interval expansion, DP
WATCH OUT
  - If n-1 is not reachable (e.g. [0,1]), right never grows and the while
    loop runs forever. Add "if (farthest <= right) return -1" for that case.
  - The loop condition is right < n - 1, not right < n. Using n counts one
    extra jump. n == 1 correctly returns 0 without entering the loop.
  - Set left = right + 1 before right = farthest. Swapping the order
    breaks the next window.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. The target may be unreachable. What changes?
     -> After the scan, if farthest <= right, return -1. Still O(n) / O(1).
  2. Return the actual path, not just the count?
     -> In each level, store the index i that gave farthest as parent of the
        new range. Walk back from n-1. O(n) time, O(n) space for parents.
  3. Why is greedy safe here? Why not plain DP?
     -> Reachable sets from 0 are always prefixes, so each BFS level is a
        range. Taking the max end of the range loses no option.
  4. Jumps can go left or right, or to equal values (Jump Game III/IV)?
     -> Levels are no longer ranges. Use real BFS with a queue and a visited
        array: O(n) time, O(n) space.
TRIGGER
  Minimum number of steps where each position reaches a contiguous range
  ahead: grow the reachable range level by level, like BFS.
================================================================================
*/
