// --------------------------------------------------------------------------
// -  optimal.cs            O(n log n) time / O(n) space
// --------------------------------------------------------------------------

public class Solution
{
    public int LengthOfLIS(int[] nums)
    {
        List<int> dp = new List<int>();
        dp.Add(nums[0]);

        int LIS = 1;
        for (int i = 1; i < nums.Length; i++)
        {
            if (dp[dp.Count - 1] < nums[i])
            {
                dp.Add(nums[i]);
                LIS++;
                continue;
            }

            int idx = dp.BinarySearch(nums[i]);
            if (idx < 0)
                idx = ~idx;
            dp[idx] = nums[i];
        }

        return LIS;
    }
}

/*
================================================================================
 PROBLEM : Given an integer array nums, return the length of the longest
           strictly increasing subsequence. A subsequence keeps the original
           order but may skip elements, and it does not need to be contiguous.
           Example: [10,9,2,5,3,7,101,18] -> 4 (for example 2,3,7,101).
 PATTERN : Patience sorting (tails array) + binary search
================================================================================
IDEA
  dp[k] holds the smallest possible tail of any increasing subsequence of
  length k+1 seen so far, so dp is always strictly increasing. If nums[i] is
  bigger than the last tail, it extends the longest one: append it, LIS++.
  Otherwise, binary search the first tail >= nums[i] and replace it. A smaller
  tail can only help future numbers, and a length never shrinks, so
  dp.Count (= LIS) is the answer.
EXAMPLE
  nums = [4,10,4,3,8,9]
  4:[4] 10:append [4,10] 4:found at idx 0, no change 3:~idx->0 [3,10]
  8:~idx->1 [3,8] 9:append [3,8,9], LIS=3
  Answer 3 (4,8,9). Note that [3,10] was never a real subsequence.
COMPLEXITY
  Time  O(n log n)  n elements, each does one binary search on dp in O(log n)
  Space O(n)        dp holds at most n tails
PATH TO OPTIMAL
  Recursion take/skip with prev value - O(2^n) - tries every subsequence.
  Memo on (i, prev) - O(n^2) time and space - reuses states (suboptimal-3.cs).
  Bottom-up dp[i] = LIS ending at i - O(n^2)/O(n) - less memory
  (suboptimal.cs).
  Tails + binary search - O(n log n) - one log-time step per element.
KEYWORDS
  LIS, dynamic programming, patience sorting, binary search, lower bound,
  tails
WATCH OUT
  - dp is not the actual LIS, only its length is right. Do not return dp as
    the sequence.
  - Empty nums crashes on nums[0]. Guard with if (nums.Length == 0) return 0.
  - Search for the first tail >= x (lower bound) for strict increase. Upper
    bound would count duplicates, e.g. [2,2,2] would give 3.
  - List.BinarySearch returns ~insertIndex when x is missing. Forgetting ~idx
    gives a negative index.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Return the subsequence itself, not only its length.
     -> Store the index of each tail and a parent[i] link to the previous
        tail, then walk back from the last tail. Still O(n log n) time and O(n)
        space.
  2. Longest non-decreasing subsequence instead?
     -> Use upper bound (first tail > x) so equal values extend the run. Same
        complexity.
  3. Count how many LIS there are.
     -> Use O(n^2) DP with len[i] and cnt[i], or a Fenwick tree over
        compressed values for O(n log n). The tails trick alone cannot count.
  4. Russian Doll Envelopes (2D)?
     -> Sort by width ascending and height descending, then run this LIS on
        the heights. O(n log n). The descending order stops equal widths from
        nesting.
TRIGGER
  You need the longest ordered chain where each item must beat the previous
  one, and O(n^2) pair checks are too slow.
================================================================================
*/
