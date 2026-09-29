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
 PATTERN : DP on subsequences - pick/skip with previous index
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-9.cs when it was first processed
 STATUS  : Suboptimal
================================================================================
VARIABLES
  next       next[p+1] = LIS length from index i+1 onward, when the last picked index is p
  curr       curr[p+1] = LIS length from index i onward, when the last picked index is p
  prevIndex  index of the last element already taken; -1 means none taken yet
  notPick    best length if nums[i] is skipped
  pick       best length if nums[i] is taken; stays 0 when taking it is not allowed
WHY THIS PATTERN
  The problem asks for the longest subsequence, and a subsequence is built by
  making a take-or-skip choice at each element. Whether you may take nums[i]
  depends only on the last value you took. So the state is (i, prevIndex), and
  each state picks the larger of pick and notPick. Row i only reads row i+1, so
  the code keeps two rows, next and curr, instead of the full n x (n+1) table.
BETTER APPROACH
  A faster method keeps a "tails" list, sometimes called patience sorting.
  tails[k] holds the smallest possible last value of an increasing subsequence
  of length k+1. For each number, a binary search finds the first tail that is
  >= the number and replaces it. If no such tail exists, the number is appended.
  That runs in O(n log n) time. This file loses because it tries every (i,
  prevIndex) pair, which is O(n^2) work.
INVARIANT
  After the outer loop has finished row i and swapped the arrays, next[p+1] is
  the true LIS length of nums[i..n-1] when every element must be greater than
  nums[p]. With p = -1 there is no such limit. The claim holds for row n because
  all cells start at 0. Each row is correct if the row below it is correct,
  because the best choice at i is either to skip it (notPick) or to take it and
  continue from i+1 with prevIndex = i (pick). So next[0] at the end is the LIS
  of the whole array with no limit.
COLUMN SHIFT FOR PREVINDEX -1
  prevIndex can be -1, and an array has no index -1. So every access adds 1:
  column 0 means nothing taken yet. In pick, the lookup next[i + 1] is really
  "prevIndex becomes i", after the shift. It is not "move to row i+1 at the same
  column". Mixing up these two meanings is the most likely bug to make when you
  rewrite this from memory.
WATCH OUT
  The code returns next[0], not curr[0]. The swap at the end of each row moves
  the newest row into next, so returning curr would give an old row. After a
  swap, curr still holds values from two rows back in columns above i+1. This is
  safe only because row i writes columns 0..i and reads only columns up to i+1.
  If you change the loop bounds, you can read those stale values. The check
  nums[i] > nums[prevIndex] is strict, so equal values do not extend the
  sequence. Changing it to >= turns the answer into the longest non-decreasing
  subsequence.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you write a simpler O(n^2) DP?
     Yes. Let dp[i] = the LIS that ends at index i, and set dp[i] = 1 + the max
     of dp[j] over all j < i with nums[j] < nums[i]. The time is the same and it
     uses one array, but the answer is max(dp), not the last cell.
  2. How do you return the subsequence itself, not only its length?
     Keep a parent[i] array in the dp[i] version, or keep the index of each tail
     in the tails version, and follow the links back from the end. This costs
     O(n) extra memory.
  3. How do you count how many LIS exist?
     Next to dp[i], keep cnt[i]. When a j gives a longer length, set cnt[i] =
     cnt[j]. When it ties the best length, add cnt[j] to cnt[i]. This stays
     O(n^2). The simple tails method cannot count.
TRIGGER
  Reach for this pattern when you must choose a subsequence and whether you can
  take an element depends only on the last element you took.
C# NOTE
  The line (next, curr) = (curr, next) swaps two array references with a tuple.
  It copies no elements, so rolling the rows costs O(1) per row. This is simpler
  than Array.Copy and does less work.
COMPLEXITY
  Time  : O(n^2)
  Space : O(n)
================================================================================
*/
