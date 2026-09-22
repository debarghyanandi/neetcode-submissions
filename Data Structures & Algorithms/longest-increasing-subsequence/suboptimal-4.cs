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
 PATTERN : Take/Skip DP over (index, prevIndex) - tabulated LIS
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-6.cs when it was first processed
 STATUS  : Suboptimal
================================================================================
VARIABLES
  n          nums.Length, also the row/column count minus one
  dp         dp[i, p+1] = longest increasing subsequence length inside nums[i..n-1] when the last taken element is nums[p]
  prevIndex  index of the last element already taken, -1 means nothing taken yet
  pick       1 + best from i+1 when nums[i] is taken, 0 when taking is not allowed
  notPick    best from i+1 with the same prevIndex, i.e. nums[i] skipped
WHY THIS PATTERN
  The problem asks for the longest subsequence that is strictly increasing, so
  every element is a take-or-skip choice, and whether you may take nums[i]
  depends only on the last value you kept. That gives a state of two numbers, i
  and prevIndex, which is exactly what dp is indexed by. The recurrence
  Math.Max(pick, notPick) covers both choices, and the guard nums[i] >
  nums[prevIndex] enforces "strictly increasing".
BETTER APPROACH
  The better approach is the tails array with binary search: keep tails[len] =
  the smallest possible last value of an increasing subsequence of length len+1,
  and for each value binary search the first tail that is >= it and overwrite
  it. That runs in O(n log n) time and O(n) space, and the answer is just the
  tails length. This file loses because it materialises every (i, prevIndex)
  pair, so it pays a full quadratic table even though row i never looks further
  back than row i+1.
INVARIANT
  When the outer loop finishes row i, every cell dp[i, p+1] with p < i holds the
  final answer for the suffix nums[i..n-1] under the constraint "the next taken
  value must beat nums[p]". It is final because the row only reads row i+1,
  which the descending outer loop already completed, and row n is all zeros,
  which is correct for an empty suffix. Therefore dp[0, 0] is the whole array
  with no constraint, which is the answer.
WHY PICK READS COLUMN I+1
  When nums[i] is taken, i itself becomes the new prevIndex, so pick reads dp[i
  + 1, i + 1] - the shifted column for prevIndex = i. This is the only reason
  the table needs n+1 columns instead of n. Row i+1 fills columns i+1 down to 0,
  so that cell is always written before row i reads it.
WATCH OUT
  The inner loop must reach prevIndex = -1, not 0, because column 0 is the
  "nothing taken yet" state and holds the returned value. The outer loop must go
  downward; reversing it to 0..n-1 would read untouched zeros and silently
  return a wrong, smaller number instead of crashing. Roughly half the table is
  dead weight: in row i, columns above i+1 are never written and never read, yet
  they are still allocated. If nums is null the method throws at nums.Length; an
  empty array is safe and returns dp[0, 0] = 0 from a 1x1 table.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you cut the memory down without changing the algorithm?
     Yes. Row i only reads row i+1, so keep two arrays of length n+1, fill the
     current one, then move it into the "next" slot. Time stays quadratic, space
     drops to linear.
  2. How do you return the actual subsequence, not only its length?
     With this table you walk forward from (0, 0): at each step compare pick
     against notPick and move to (i+1, i+1) when pick wins, recording nums[i],
     otherwise to (i+1, prevIndex+1). With the binary-search method you would
     instead need a parent-index array, since the tails array does not hold a
     real subsequence.
  3. What changes for a longest non-decreasing subsequence?
     Only the guard: nums[i] > nums[prevIndex] becomes nums[i] >=
     nums[prevIndex]. The state, loop order and base case stay the same.
  4. What if you had to count how many longest subsequences exist?
     Each cell must carry a pair (length, count): on a tie between pick and
     notPick you add the counts, otherwise you copy the winner's count. Same
     quadratic cost, twice the storage.
TRIGGER
  A subsequence choice where "can I take this element" depends only on the last
  element I kept - that last element becomes the second DP dimension.
C# NOTE
  dp is a rectangular int[,], so a single row cannot be handed out as an int[]
  or swapped by reference; if you go to the two-row version you need int[][] or
  two plain int[] variables you re-point each iteration.
COMPLEXITY
  Time  : O(n^2)
  Space : O(n^2)
================================================================================
*/
