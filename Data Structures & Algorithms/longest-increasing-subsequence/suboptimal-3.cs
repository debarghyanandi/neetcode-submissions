// --------------------------------------------------------------------------
// -  suboptimal-3.cs       O(n^2) time / O(n^2) space
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
 PATTERN : DP on Subsequences - memoized pick / not-pick with prev index
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-5.cs when it was first processed
 STATUS  : Suboptimal
================================================================================
VARIABLES
  dp         dp[i, prevIndex + 1] = length of the longest increasing subsequence (LIS) you can build from nums[i..n-1] when the last picked element is nums[prevIndex]
  prevIndex  index of the last element taken into the subsequence; -1 means nothing taken yet
  notPick    best length if nums[i] is skipped
  pick       best length if nums[i] is taken; stays 0 when taking it is not allowed
WHY THIS PATTERN
  The problem asks for the longest subsequence, so for each element you choose
  to keep it or skip it. That is the pick / not-pick shape. The only rule is
  that a kept element must be larger than the last kept one. So the state needs
  just two numbers: the current position i and the last kept index prevIndex.
  Many different choice paths reach the same (i, prevIndex) pair, and the memo
  table dp stores each answer so it is computed only once.
BETTER APPROACH
  The better approach is patience sorting. Keep an array tails, where tails[k] =
  the smallest possible last value of an increasing subsequence of length k+1.
  For each number, binary search tails and replace the first value that is >=
  the number, or append the number if none is. This runs in O(n log n) time and
  O(n) space. This file loses because it checks every (i, prevIndex) pair and
  stores a full n x (n+1) table. Most of those states are never useful for the
  final answer.
INVARIANT
  F(i, prevIndex) returns the longest increasing subsequence that uses only
  nums[i..n-1] and whose first element is larger than nums[prevIndex]. The base
  case i == nums.Length returns 0 because no elements are left. At each step the
  code tries both choices, and pick is only allowed when prevIndex == -1 ||
  nums[i] > nums[prevIndex]. Every valid subsequence is one path of choices, so
  the max over both branches is the true best. F(0, -1) then covers the whole
  array with no limit on the first element.
COLUMN SHIFT BY ONE
  prevIndex can be -1, and an array index cannot be negative. So the table has n
  + 1 columns, and the code always reads and writes dp[i, prevIndex + 1]. Column
  0 means "nothing taken yet". If you forget the +1 in any one of the three
  places it is used, you get an IndexOutOfRangeException or read the wrong
  state.
WATCH OUT
  The recursion goes one level deeper for each index, so the call stack reaches
  depth n. A very long nums could cause a StackOverflowException, and C# cannot
  catch that exception. Also, the memo only works because -1 is never a real
  answer. Every stored value is 0 or more, so -1 can safely mean "not computed
  yet". If you change the value type or the sentinel, check that this is still
  true. The comment above the recurrence matches the code, so there is no
  mismatch to report.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How do you return the actual subsequence, not just its length?
     Keep the table, then start at (0, -1) and walk forward. At each i, if
     picking is allowed and 1 + dp for (i+1, i) equals the stored value, take
     nums[i] and set prevIndex = i. Otherwise skip it. This costs O(n) extra
     time. The tails method cannot rebuild the sequence by itself. It also needs
     a parent-index array.
  2. What if the subsequence only needs to be non-decreasing?
     Change nums[i] > nums[prevIndex] to >=. In the tails version, use an
     upper-bound binary search (first value strictly greater) instead of a
     lower-bound one.
  3. How would you count how many longest increasing subsequences exist?
     Use a 1D O(n^2) DP. Keep two arrays: len[i] (LIS length ending at i) and
     cnt[i] (how many LIS reach that length ending at i). When a longer length
     is found, reset cnt[i]. When an equal length is found, add to cnt[i]. The
     O(n log n) trick does not extend easily to counting.
TRIGGER
  When a problem asks for the longest or best subsequence and each new element
  is only valid relative to the last element you chose, think pick / not-pick
  with a "previous index" in the state.
C# NOTE
  C# fills a new int[,] with zeros, and 0 is a valid answer here. That is why
  the double loop that writes -1 is needed. Array.Fill only works on
  one-dimensional arrays, so you cannot use it on int[,]. Another option is
  int?[,], where null means "not computed", and then you can drop the fill loop.
COMPLEXITY
  Time  : O(n^2)
  Space : O(n^2)
================================================================================
*/
