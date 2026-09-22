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
 PATTERN : Patience sorting - tails array with binary search
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-12.cs when it was first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  dp     dp[i] = smallest possible tail of an increasing subsequence of length i+1
  LIS    current length of dp, returned as the answer
  idx    insertion point for nums[i] in dp, from BinarySearch (bit-complemented if not found)
WHY THIS PATTERN
  The problem asks only for the LENGTH of the longest strictly increasing
  subsequence, not the subsequence itself. That lets us throw away shape and
  keep one number per length: dp[i] is the best (smallest) ending value for a
  subsequence of length i+1. dp stays sorted by construction, so finding where
  nums[i] belongs is a binary search instead of a scan, and LIS grows only when
  nums[i] beats the whole of dp.
BRUTE FORCE
  The natural first answer is O(n^2) dynamic programming: best[i] = 1 +
  max(best[j]) over all j < i with nums[j] < nums[i], then take the max of best.
  It is correct and easy to argue, but every i rescans all earlier indices.
  Replacing the inner scan with dp.BinarySearch over a sorted array of tails is
  what drops the inner work to log n.
INVARIANT
  After processing nums[0..i], dp is strictly increasing and dp[k] holds the
  smallest tail value over all increasing subsequences of length k+1 seen so
  far. Overwriting dp[idx] = nums[i] never changes the length of dp, it only
  lowers a tail, which can never make a future extension harder. So dp.Count is
  always exactly the best length reachable so far, and at the end LIS is that
  length.
WHY OVERWRITING IS SAFE
  dp is not an actual subsequence of nums - its entries can come from unrelated
  positions and the final contents are usually not a valid answer. Only its
  length is meaningful. That is why the code can freely stomp dp[idx] without
  tracking where the value came from.
WATCH OUT
  nums[0] is read before any length check, so an empty array throws
  IndexOutOfRangeException; the O(n^2) DP version would return 0 without special
  casing. LIS is kept as a separate counter that must stay in sync with dp.Count
  - the two are only equal because every Add is paired with LIS++; returning
  dp.Count directly would remove that risk. List.BinarySearch on a list with
  duplicates may return any matching index, but here an exact match means idx
  points at an equal value and writing nums[i] over it is a no-op, so strict
  increase is preserved. If the problem ever asked for non-decreasing
  subsequences, this exact-match path would be wrong and you would need an
  upper-bound search instead.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How do you return the actual subsequence, not just its length?
     Keep a parallel array pos where pos[i] records which dp slot nums[i] landed
     in, plus a prev array pointing at the element that was at dp[idx-1] at that
     moment, then walk the links back from the last element added. Costs O(n)
     extra memory and some care, since dp itself is not a real subsequence.
  2. What if the array is huge and streamed, so you cannot hold nums?
     This algorithm already works online - it only ever looks at nums[i] once
     and keeps dp, whose size is at most the answer. You would just drop the
     indexed loop and read values one at a time.
  3. Could you use an array instead of List<int>?
     Yes - allocate int[] of length nums.Length, keep an explicit size counter
     (LIS already is one), and call Array.BinarySearch(dp, 0, LIS, nums[i]).
     That removes List growth and resizing at the cost of always paying
     full-length allocation.
  4. What changes for longest NON-DECREASING subsequence?
     You need the first index with dp[idx] > nums[i], not >=. BinarySearch
     cannot express that directly, so write a manual binary search for the upper
     bound; the rest of the loop is unchanged.
TRIGGER
  A subsequence question that asks only for a length or a count of piles, where
  each new element either extends the best chain or improves an existing one.
C# NOTE
  List<int>.BinarySearch returns the bitwise complement of the insertion point
  when there is no exact match, which is why idx = ~idx is needed; forgetting it
  leaves idx negative and the indexer throws.
COMPLEXITY
  Time  : O(n log n)
  Space : O(n)
================================================================================
*/
