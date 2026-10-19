// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(k) space
// --------------------------------------------------------------------------

public class Solution
{
    public bool ContainsNearbyDuplicate(int[] nums, int k)
    {
        HashSet<int> window = new HashSet<int>();

        for (int right = 0; right < nums.Length; right++)
        {
            if (window.Contains(nums[right]))
                return true;

            window.Add(nums[right]);

            if (window.Count > k)
            {
                window.Remove(nums[right - k]);
            }

        }

        return false;
    }
}

/*
================================================================================
 PROBLEM : Given an int array nums and an int k, return true if two different
           indices i and j exist with nums[i] == nums[j] and abs(i - j) <= k.
           Equal values far apart do not count. Example: nums = [1,2,3,1], k =
           3 -> true.
 PATTERN : Sliding Window (fixed size) + HashSet
================================================================================
IDEA
  Keep a HashSet called window with the values of the last k indices only.
  For each right, a hit in window means a match at distance <= k: return true.
  Otherwise add nums[right]. If window.Count > k, remove nums[right - k].
  This works because any value still in window is at most k steps behind.
EXAMPLE
  nums = [1,0,1,1], k = 1
  r=0 add 1 {1}; r=1 add 0, Count 2>1, drop nums[0]=1 -> {0}
  r=2 1 not in {0} (gap 2 > k), add, drop nums[1]=0 -> {1}; r=3 1 in -> true
COMPLEXITY
  Time  O(n)  one pass over right, each set operation is O(1) on average
  Space O(k)  window never holds more than k values after the trim
PATH TO OPTIMAL
  Brute force: compare each i with the next k indices - O(n*k) - no extra
  memory.
  Map value -> last index - O(n) time, O(n) space - one pass (suboptimal.cs).
  Fixed-size set window - O(n) time, O(k) space - forgets indices too far
  back.
KEYWORDS
  sliding window, hash set, contains duplicate, index distance, fixed window
WATCH OUT
  - Trim with "Count > k", not ">= k". With ">=" the window keeps only k-1
    values, so a match at distance exactly k is missed.
  - Check Contains before Add. The set never holds duplicates, and that is
    why removing nums[right - k] always removes the right element.
  - k = 0: the value is added, then removed right away. Result is false, and
    that is correct.
  - Real space is O(min(n, k)). When k >= n the window is never trimmed.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Also need abs(nums[i] - nums[j]) <= t (Contains Duplicate III)?
     -> Keep the same k-size window but use a SortedSet and look for the
        ceiling of nums[right] - t. Or use buckets of width t+1. O(n log k) or
        O(n).
  2. Values arrive as a stream?
     -> The same code works online. Keep the k-size set or a queue of the last
        k values. O(1) per item and O(k) memory.
  3. Return the indices, not a bool?
     -> Use a Dictionary of value -> last index. On a hit with right - last <=
        k, return (last, right). O(n) time, O(n) space unless you trim it.
  4. Why not sort first?
     -> Sorting loses the original indices unless you store pairs. That costs
        O(n log n), and it is worse than one hash pass.
TRIGGER
  A pair must be equal or near equal and its indices at most k apart: slide a
  k-size hash window.
================================================================================
*/
