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
 PATTERN : 1D DP - LIS by checking every earlier index
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-11.cs when it was first processed
 STATUS  : Suboptimal
================================================================================
VARIABLES
  dp        dp[i] = length of the longest increasing subsequence that ends exactly at nums[i]
  prev      an earlier index whose value might come right before nums[i]
  maximum   the best dp[i] seen so far, which is the answer
WHY THIS PATTERN
  The problem asks for the longest strictly increasing subsequence. A
  subsequence keeps the original order but may skip items. Any such subsequence
  that ends at index i is a shorter one ending at some earlier index prev, plus
  nums[i]. So the answer for i is built from answers that are already known, and
  that is dynamic programming. dp[i] tries every prev with nums[prev] < nums[i]
  and keeps the best 1 + dp[prev].
BETTER APPROACH
  A faster method is patience sorting, which runs in O(n log n). Keep a list
  tails, where tails[k] is the smallest value that can end an increasing
  subsequence of length k+1. For each number, binary search tails for the first
  entry >= that number and replace it, or append the number if no entry is big
  enough. The final length of tails is the answer. This file is slower because
  of its inner prev loop: for every i it scans all earlier indices, while tails
  needs only one binary search per number.
INVARIANT
  When the outer loop finishes index i, dp[i] is exactly the length of the
  longest increasing subsequence that ends at nums[i]. This holds because every
  such subsequence is either nums[i] alone (length 1, the fill value) or ends
  with some nums[prev] < nums[i], and the inner loop checks every one of those
  prev values. All dp[prev] with prev < i are already final. The best
  subsequence must end somewhere, so the maximum over all dp[i] is the answer.
ANSWER IS NOT DP[N-1]
  dp[i] is the best subsequence that ends at i, not the best one inside 0..i.
  The longest subsequence may end before the last index, for example nums = [3,
  4, 1]. This is why the code keeps maximum across all i and does not return
  dp[n - 1].
WATCH OUT
  If nums is empty, maximum starts at 1, so the method returns 1 when the
  correct answer is 0. The comment "Lowest Lis is 1" is only true when n >= 1.
  The check nums[prev] < nums[i] uses strict less-than on purpose. Changing it
  to <= would count equal values and give the longest non-decreasing
  subsequence, which is a different problem.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How do you return the subsequence itself, not only its length?
     Add a parent array. When dp[i] improves through prev, set parent[i] = prev.
     Then start from the index with the largest dp and follow parent back. This
     costs O(n) more memory, and the time stays the same.
  2. How many different longest increasing subsequences are there?
     Keep count[i] next to dp[i]. When 1 + dp[prev] is bigger than dp[i], set
     count[i] = count[prev]. When it is equal, add count[prev]. At the end, sum
     count[i] for every i where dp[i] equals maximum. The time stays O(n^2), and
     the fast tails method cannot easily count.
  3. How does this change for Russian Doll Envelopes, which is a 2D version?
     Sort by width ascending. For equal widths, sort height descending, so two
     envelopes with the same width can never be chained. Then run LIS on the
     heights, with the O(n log n) tails method if n is large.
TRIGGER
  Look for this pattern when a problem asks for the longest chain in a sequence,
  kept in original order, where each item must be bigger than (or compatible
  with) the one before it.
C# NOTE
  Array.Fill(dp, 1) is needed here because new int[n] starts every entry at 0,
  not 1. In the O(n log n) version, Array.BinarySearch gives the insert point
  directly: when the value is not found, it returns the bitwise complement (~)
  of that index.
COMPLEXITY
  Time  : O(n^2)
  Space : O(n)
================================================================================
*/
