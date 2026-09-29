// --------------------------------------------------------------------------
// -  suboptimal-4.cs       O(n^2) time / O(n^2) space
// -  Bottom-up tabulation, pick or not pick   [pick-not-pick-dp]
// -  ranks below optimal.cs (O(n log n) time / O(n) space)
// -
// -  Reference solution - not one you solved yourself (from submission-6)
// -
// -  Double nested loop fills an (n+1)×(n+1) DP table iteratively from
// -  bottom up.
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
        int[,] dp = new int[n + 1, n + 1];

        // Base case:
        // dp[n, *] = 0
        // Already 0 by default in C#

        for (int i = n - 1; i >= 0; i--)
        {
            for (int prevIndex = i - 1; prevIndex >= -1; prevIndex--)
            {
                // Not pick current
                int notPick = dp[i + 1, prevIndex + 1];

                int pick = 0;

                // Pick current
                if (prevIndex == -1 || nums[i] > nums[prevIndex])
                {
                    pick = 1 + dp[i + 1, i + 1];
                }

                dp[i, prevIndex + 1] = Math.Max(pick, notPick);
            }
        }

        return dp[0, 0];
    }
}

/*
================================================================================
 PATTERN : 2D DP / Take or Skip - index plus previous picked index
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-6.cs when it was first processed
 STATUS  : Suboptimal
================================================================================
VARIABLES
  dp         dp[i, prevIndex + 1] = longest increasing subsequence using nums[i..n-1], when the last picked element was nums[prevIndex]
  prevIndex  index of the last element already picked; -1 means nothing picked yet
  notPick    best length if nums[i] is skipped
  pick       best length if nums[i] is taken; stays 0 when taking it is not allowed
WHY THIS PATTERN
  The problem asks for the longest subsequence, which is not a subarray. So at
  every index you make one choice: take nums[i] or skip it. Whether you may take
  it depends only on the last value you took. Because of that, the state (i,
  prevIndex) is enough. The dp table stores each state once, so overlapping
  choices are not solved again.
BETTER APPROACH
  A better approach is patience sorting. You keep a tails array, where tails[k]
  is the smallest possible last value of an increasing subsequence of length
  k+1. For each number, you binary search for the first tail that is greater
  than or equal to it, and replace that tail. This runs in O(n log n) time with
  O(n) space. This file loses because it fills a table over every (i, prevIndex)
  pair. It also keeps all n+1 rows, even though each row only needs the row
  below it.
INVARIANT
  When row i is being filled, row i+1 is already complete for every prevIndex
  below i+1. Row n is all zeros, because an empty suffix adds nothing. Each cell
  takes the max of the two choices, skip or take. These two choices cover every
  subsequence of the suffix that fits after nums[prevIndex]. So dp[0, 0] is the
  answer for the whole array with nothing picked yet.
PICK READS COLUMN I+1
  After you take nums[i], the new "last picked" index is i. So the next state is
  dp[i + 1, i + 1], not dp[i + 1, prevIndex + 1]. This cell exists because, when
  row i+1 was filled, its inner loop started at prevIndex = i. Mixing up these
  two columns is the easiest way to get this recurrence wrong.
WATCH OUT
  The check nums[i] > nums[prevIndex] is strict. Equal values can never both be
  picked, which is correct for strictly increasing but wrong if the problem
  wants non-decreasing. Only cells with prevIndex < i are ever written, so about
  half of the (n+1) x (n+1) table is allocated and never used. The outer loop
  must run from i = n-1 down to 0. If you change it to go upward, it reads rows
  that are not filled yet, and it fails silently because they are still zero.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How do you cut the memory without changing the recurrence?
     Row i reads only row i+1, so keep two 1D arrays of size n+1 and swap them.
     Space drops to O(n). Time stays O(n^2).
  2. How would you return the actual subsequence, not only its length?
     Store, for each index, the index of its predecessor in the best chain. Then
     walk back from the end of the best chain. The tails method needs an extra
     parent array to do the same thing.
  3. How do you count how many longest increasing subsequences exist?
     Use the 1D form, where len[i] is the longest chain ending at i, plus
     cnt[i]. When a longer chain is found, overwrite cnt[i]. When an equal
     length is found, add to it. This stays O(n^2). The plain tails method
     cannot count.
TRIGGER
  The problem asks for the longest or best subsequence where taking an element
  depends only on the last element you took.
C# NOTE
  new int[n + 1, n + 1] is zero-filled by the runtime, which is why the base row
  needs no code. For the O(n log n) version, Array.BinarySearch(tails, 0, len,
  x) returns the bitwise complement of the insertion point when x is not found.
  So apply ~ to a negative result before you use it.
COMPLEXITY
  Time  : O(n^2)
  Space : O(n^2)
================================================================================
*/
