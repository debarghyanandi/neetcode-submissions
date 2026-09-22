// --------------------------------------------------------------------------
// -  suboptimal-3.cs       O(n^2) time / O(n^2) space
// -  Recursive memoization, pick or not pick   [pick-not-pick-dp]
// -  ranks below optimal.cs (O(n log n) time / O(n) space)
// -
// -  Reference solution - not one you solved yourself (from submission-5)
// -
// -  Explores n^2 states recursively, each computed once with memoization;
// -  call stack adds O(n) but dominated by DP table.
// --------------------------------------------------------------------------

public class Solution
{
    public int LengthOfLIS(int[] nums)
    {
        int n = nums.Length;

        int[,] dp = new int[n, n + 1];

        for (int i = 0; i < n; i++)
        {
            for (int prevIndex = 0; prevIndex <= n; prevIndex++)
            {
                dp[i, prevIndex] = -1;
            }
        }

        return F(0, -1, nums, dp);
    }

    private int F(int i, int prevIndex, int[] nums, int[,] dp)
    {
        if (i == nums.Length)
            return 0;

        if (dp[i, prevIndex + 1] != -1)
            return dp[i, prevIndex + 1];

        // Recurrence:
        // notPick = F(i + 1, prevIndex)
        // pick    = 1 + F(i + 1, i)
        //           only if prevIndex == -1 || nums[i] > nums[prevIndex]
        // answer  = max(pick, notPick)

        int notPick = F(i + 1, prevIndex, nums, dp);

        int pick = 0;

        if (prevIndex == -1 || nums[i] > nums[prevIndex])
        {
            pick = 1 + F(i + 1, i, nums, dp);
        }

        return dp[i, prevIndex + 1] = Math.Max(pick, notPick);
    }
}

/*
================================================================================
 PATTERN : Take-or-skip recursion with memo on (index, prevIndex)
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-5.cs when it was first processed
 STATUS  : Suboptimal
================================================================================
VARIABLES
  dp           dp[i, prevIndex+1] = LIS length using nums[i..n-1] when the last picked element is nums[prevIndex]
  prevIndex    index of the last element taken so far, -1 when nothing is taken yet
  notPick      best answer if we skip nums[i]
  pick         1 + best answer if we take nums[i], 0 when taking is not allowed
WHY THIS PATTERN
  The problem asks for the longest strictly increasing subsequence, so at every
  position there are exactly two choices: take nums[i] or skip it. Whether
  taking is legal depends only on the last value we kept, so the state is the
  pair (i, prevIndex) and nothing else from the past matters. F explores both
  branches and returns the max, and dp caches each state so the same pair is
  solved once. The +1 shift on prevIndex exists only so the sentinel -1 fits in
  a zero-based array column.
BETTER APPROACH
  The better approach is patience sorting: keep a list tails where tails[k] is
  the smallest possible tail of an increasing subsequence of length k+1, and for
  each value binary search for the first element >= it and overwrite it (or
  append). That is O(n log n) time and O(n) space, and the answer is
  tails.Count. This file loses because it builds an n by (n+1) table and fills
  most of it, so both time and memory grow with n squared even though only the
  tails array is really needed. There is also a middle option, the O(n^2)
  bottom-up dp[i] = 1 + max(dp[j]) over j < i with nums[j] < nums[i], which
  matches this file in time but uses only O(n) space and no recursion.
INVARIANT
  F(i, prevIndex) always returns the best length obtainable from the suffix
  nums[i..n-1] given that the last accepted value is nums[prevIndex]. The guard
  prevIndex == -1 || nums[i] > nums[prevIndex] keeps every accepted chain
  strictly increasing, so no returned count can come from an illegal
  subsequence. Because the pick branch passes i as the new prevIndex, the state
  always describes the real last choice, and the memo entry written at the end
  is final for that state. The top call F(0, -1, ...) starts with an empty
  chain, so the result is the LIS of the whole array.
THE +1 COLUMN SHIFT
  dp has n+1 columns, not n, purely because prevIndex ranges over -1 up to n-1.
  Every read and write uses dp[i, prevIndex + 1], so column 0 means "nothing
  picked yet". If you ever forget the +1 on one of the three accesses, you get a
  silent off-by-one that mixes two different states instead of an exception.
WATCH OUT
  Recursion depth is one frame per element, since notPick walks i, i+1, i+2 ...
  to the end before returning, so a long input can overflow the stack; this is
  the main practical failure mode. If nums is empty, n is 0 and dp becomes a 0
  by 1 array, F returns 0 at the first check without touching dp, so that case
  is safe. The initialization loop lets prevIndex reach n, which is a valid
  column index but a state never actually visited, so it is harmless extra work.
  -1 is used as the "not computed" marker, which is safe only because every real
  answer is 0 or larger.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How do you remove the recursion?
     Fill the same table bottom-up with i going from n-1 down to 0 and prevIndex
     from i-1 down to -1. Same O(n^2) work, no stack frames, but you still hold
     the full table unless you also switch to the one-dimensional dp[i] form.
  2. How would you print the actual subsequence, not just its length?
     Keep a parent array alongside the dp, record which branch won at each
     state, then walk the chain back from the start state. That adds O(n^2)
     memory here, or O(n) if you move to the dp[i] formulation and store the
     predecessor index.
  3. What changes if the subsequence may be non-decreasing instead of strictly
  increasing?
     Change the guard from nums[i] > nums[prevIndex] to nums[i] >=
     nums[prevIndex]. In the binary-search version the matching change is to
     search for the first element strictly greater than the value instead of
     greater or equal.
  4. What if you must also count how many distinct LIS there are?
     Store a second table of counts next to the lengths and combine them: when
     pick and notPick tie, add the counts; when one wins, copy its count. Same
     complexity, twice the memory.
TRIGGER
  A choice of take-or-skip over a sequence where legality depends only on the
  last item kept - that last item becomes the second dimension of the state.
C# NOTE
  int[,] is a true rectangular array, so dp is one contiguous block and indexing
  costs a multiply plus an add; a jagged int[][] would let you size row i to
  only the columns it needs, at the price of one extra reference hop. Note also
  that dp is passed on every call even though it never changes - making F a
  private instance method with dp and nums as fields would shrink each frame.
COMPLEXITY
  Time  : O(n^2)
  Space : O(n^2)
================================================================================
*/
