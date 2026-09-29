// --------------------------------------------------------------------------
// -  suboptimal.cs         O(n^2) time / O(n) space
// --------------------------------------------------------------------------

public class Solution
{
    public int LengthOfLIS(int[] nums)
    {
        int n = nums.Length;

        // +1 shift for prevIndex
        // prevIndex = -1 -> column 0
        // prevIndex =  0 -> column 1
        // prevIndex =  1 -> column 2
        int[] next = new int[n + 1];
        int[] curr = new int[n + 1];

        // Base case:
        // dp[n, *] = 0
        // Already 0 by default in C#

        for (int i = n - 1; i >= 0; i--)
        {
            for (int prevIndex = i - 1; prevIndex >= -1; prevIndex--)
            {
                // Not pick current
                int notPick = next[prevIndex + 1];

                int pick = 0;

                // Pick current
                if (prevIndex == -1 || nums[i] > nums[prevIndex])
                {
                    pick = 1 + next[i + 1];
                }

                curr[prevIndex + 1] = Math.Max(pick, notPick);
            }
            (next, curr) = (curr, next);
        }

        return next[0];
    }
}

/*
================================================================================
 PROBLEM : Given an integer array nums, return the length of the longest
           strictly increasing subsequence. A subsequence keeps the original
           order but may skip elements, so it need not be contiguous. Equal
           values do not count as increasing. Example: [10,9,2,5,3,7,101,18]
           -> 4 (2,3,7,101).
 PATTERN : 1D DP (take / not-take), tabulated with two rows
================================================================================
IDEA
  State (i, prevIndex) means: the best LIS length from index i onward, when
  nums[prevIndex] was the last picked value (-1 means nothing picked yet).
  notPick skips nums[i]. pick is allowed only if nums[i] > nums[prevIndex],
  and it gives 1 + the answer at (i+1, prev=i). Column prevIndex+1 stores
  prevIndex, so -1 fits in column 0. Row i reads only row i+1, so we keep
  just next and curr and swap them. It is correct because every subsequence
  is one chain of pick/skip choices, and the max covers all of them.
  Unlike optimal.cs, there is no tails array and no binary search.
EXAMPLE
  nums = [1,3,2]
  i=2: prev=1 (3): 2>3 fails, 0; prev=0 (1): pick=1; prev=-1: 1 ->
  next=[1,1,0,0]
  i=1: prev=0: max(notPick=1, pick=1+next[2]=1)=1; prev=-1: 1 ->
  next[0..1]=[1,1]
  i=0: prev=-1: pick=1+next[1]=2, notPick=1 -> return next[0]=2
COMPLEXITY
  Time  O(n^2)  about n^2/2 pairs (i, prevIndex), each filled in O(1)
  Space O(n)    two rows next and curr of size n+1
WATCH OUT
  - In pick, read next[i+1], not next[i]. Column i+1 means "prev is i".
    The +1 shift is the most common off-by-one here.
  - The swap runs after each row, so the answer is next[0], not curr[0].
  - Use strict >. With >=, input [2,2] gives 2, but the answer is 1.
  - The inner loop must go prevIndex = i-1 down to -1. It must include -1,
    or no chain can start and every answer becomes 0.
================================================================================
*/
