// --------------------------------------------------------------------------
// -  suboptimal-2.cs       O(n^2) time / O(n) space
// -  1D DP with nested iteration over preceding elements
// -  [lis-ending-position]
// -  ranks below optimal.cs (O(n log n) time / O(n) space)
// -
// -  Reference solution - not one you solved yourself (from submission-11)
// -
// -  dp[i] stores max LIS ending at position i; for each i, check all j < i
// -  in nested loop.
// --------------------------------------------------------------------------

public class Solution
{
    public int LengthOfLIS(int[] nums)
    {
        int n = nums.Length;
        int[] dp = new int[n];
        Array.Fill(dp, 1); // Lowest Lis is 1

        int maximum = 1;
        for (int i = 0; i < n; i++)
        {
            for (int prev = 0; prev < i; prev++)
            {
                if (nums[prev] < nums[i])
                    dp[i] = Math.Max(dp[i], 1 + dp[prev]);
            }
            maximum = Math.Max(maximum, dp[i]);
        }
        return maximum;
    }

}

/*
================================================================================
 PATTERN : DP on subsequences - LIS ending at each index
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-11.cs when it was first processed
 STATUS  : Suboptimal
================================================================================
VARIABLES
  dp       dp[i] = length of the longest increasing subsequence that ends exactly at index i
  prev     index of an earlier element we try to extend from
  maximum  best dp value seen so far
WHY THIS PATTERN
  The problem asks for the longest strictly increasing subsequence, so elements
  can be skipped but order must be kept. That makes a natural subproblem: fix
  the last element. dp[i] answers "if the subsequence must end at nums[i], how
  long can it be", and any such subsequence is some shorter one ending at an
  earlier prev with nums[prev] < nums[i], plus nums[i]. The full answer is the
  largest dp[i], which maximum collects.
BETTER APPROACH
  The better approach is patience sorting: keep a list tails where tails[k] is
  the smallest possible tail value of an increasing subsequence of length k+1,
  and for each number binary search for the first tail >= it, replacing it or
  appending. That runs in O(n log n) time with the same O(n) space. This file
  loses because the inner prev loop rescans every earlier index for every i,
  which is the full quadratic work; the binary search replaces that scan with
  about log n comparisons.
INVARIANT
  Before the outer loop body for index i runs, dp[prev] is already final and
  correct for every prev < i, because i moves left to right and dp[i] only reads
  smaller indices. So taking the maximum of 1 + dp[prev] over all valid prev
  gives the true best for i. Since every increasing subsequence ends at some
  index, the maximum over all dp[i] is the global answer.
WATCH OUT
  If nums is empty, n is 0, both loops never run, and the method returns maximum
  = 1, which is wrong - the answer should be 0. The seed value 1 and the comment
  "Lowest Lis is 1" are only right when at least one element exists. The
  comparison nums[prev] < nums[i] is strict, which is correct for strictly
  increasing; changing it to <= silently solves the non-decreasing variant
  instead. Also note dp[i] is initialized to 1 by Array.Fill, so the inner loop
  never needs a separate "no predecessor" case.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How would you also return the actual subsequence, not just its length?
     Keep a second array parent where parent[i] is the prev that produced dp[i],
     remember the index where maximum was achieved, and walk parent backwards,
     then reverse. Costs one more O(n) array and no extra time.
  2. What changes if you want the number of longest increasing subsequences?
     Add a count array; when 1 + dp[prev] beats dp[i] set count[i] =
     count[prev], when it ties add count[prev] to count[i]. Same two loops, same
     complexity.
  3. Can this be done without the dp array at all?
     Not for this formulation - dp[i] reads every earlier entry, so all n values
     must stay live. The patience-sorting version does shrink memory in
     practice, since tails only grows to the length of the answer.
  4. What if the numbers arrive one at a time in a stream?
     The tails-plus-binary-search version handles that directly, appending or
     replacing per element in O(log n); this file would have to re-run its inner
     loop over all stored history for each arrival.
TRIGGER
  Reach for it when the answer is the best chain you can build by picking
  elements in their original order under a pairwise condition, and the natural
  subproblem is "best chain ending here".
C# NOTE
  Array.Fill(dp, 1) is the clean way to seed the base case; without it you would
  need a loop, since new int[n] in C# zero-fills and a dp of 0 would make 1 +
  dp[prev] off by one for elements with no valid predecessor.
COMPLEXITY
  Time  : O(n^2)
  Space : O(n)
================================================================================
*/
