// --------------------------------------------------------------------------
// -  optimal.cs            O(n log n) time / O(n) space
// -  Greedy approach with binary search   [greedy-binary-search]
// -  ranks above suboptimal.cs (O(n^2) time / O(n) space)
// -
// -  Reference solution - not one you solved yourself (from submission-12)
// -
// -  Maintains list of smallest tails for each LIS length; each element
// -  either appends or binary-searches for replacement.
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
 PATTERN : Patience Sorting / Binary Search - smallest tail per length
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-12.cs when it was first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  dp     dp[k] = smallest tail value of any increasing subsequence of length k+1 seen so far
  LIS    length of the longest increasing subsequence found; always equal to dp.Count
  idx    position in dp where nums[i] replaces the old tail
WHY THIS PATTERN
  The problem asks only for the length of the longest strictly increasing
  subsequence, not the subsequence itself. So we do not need to remember every
  subsequence. For each length we only need its smallest possible last value,
  because a smaller tail leaves more room to extend. The list dp stays sorted,
  so each nums[i] can find its place with a binary search instead of a scan.
BRUTE FORCE
  The first correct version is the classic DP. Let best[i] be the LIS that ends
  at index i. Then best[i] = 1 + max(best[j]) over all j < i with nums[j] <
  nums[i]. This is O(n^2) time, because every element looks back at all earlier
  elements. The tails list replaces that backward scan with one binary search.
INVARIANT
  After processing nums[0..i], dp is strictly increasing. dp[k] is the smallest
  value that can end an increasing subsequence of length k+1. If nums[i] is
  larger than dp's last value, it extends the longest chain, so dp grows by one.
  Otherwise nums[i] replaces the first tail that is greater than or equal to it.
  That keeps the tail for that length as small as possible and does not break
  the sorted order. dp.Count only grows when a truly longer chain exists, so LIS
  is correct.
THE ~IDX FROM BINARYSEARCH
  List.BinarySearch returns a negative number when the value is missing. That
  number is the bitwise complement (all bits flipped) of the index of the first
  larger element. So ~idx is the lower bound, which is the exact slot to
  overwrite. When nums[i] is already in dp, the returned index points to that
  equal value. Writing it again changes nothing, and this is right for a
  strictly increasing subsequence.
WATCH OUT
  dp.Add(nums[0]) throws an exception when nums is empty. Add a guard that
  returns 0. The name dp suggests a normal DP table, and dp at the end looks
  like an answer, but it is usually NOT a real subsequence. Only its length
  means something. LIS is a second copy of dp.Count. If you edit one path and
  forget LIS++, the two numbers drift apart. Returning dp.Count is safer.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How do you return the actual subsequence, not just its length?
     Store indices in dp instead of values, and keep a parent[i] array that
     points to the index in the slot before. At the end, walk back from the last
     tail. This costs O(n) extra space for parent.
  2. What changes for the longest non-decreasing subsequence?
     Equal values must now extend a chain. Use an upper bound (the first element
     strictly greater) instead of a lower bound, and use <= in the append check.
     List.BinarySearch does not give an upper bound directly, so write your own
     binary search.
  3. How would you count how many longest increasing subsequences there are?
     Tails alone cannot count. Use the O(n^2) DP with a count[i] array. For O(n
     log n), use a Fenwick tree (an array that answers prefix queries fast) over
     compressed values that stores (length, count) pairs.
TRIGGER
  When a problem asks for the length of a longest increasing (or chained, like
  nested envelopes) subsequence and O(n^2) is too slow, keep the smallest tail
  for each length and binary search into it.
C# NOTE
  dp can never grow past nums.Length. So a preallocated int[] plus a length
  counter, searched with Array.BinarySearch(tails, 0, len, nums[i]), gives the
  same ~idx behavior and never has to regrow the list.
COMPLEXITY
  Time  : O(n log n)
  Space : O(n)
================================================================================
*/
