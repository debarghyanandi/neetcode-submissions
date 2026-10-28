// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(n) space
// --------------------------------------------------------------------------

public class Solution
{
    public int SubarraySum(int[] nums, int k)
    {
        Dictionary<int, int> map = new Dictionary<int, int>();//val->count
        map[0] = 1;

        int res = 0, preSum = 0;

        foreach (int num in nums)
        {
            preSum += num;

            int remove = preSum - k;

            if (map.TryGetValue(remove, out int count)) // we can omit if
            {
                res += count;
            }

            map.TryGetValue(preSum, out int freq);
            map[preSum] = freq + 1;

        }

        return res;
    }
}

/*
================================================================================
 PROBLEM : Given an int array nums (it may hold negatives and zeros) and an
           int k, return how many contiguous, non-empty subarrays have a sum
           of exactly k. You count subarrays; you do not return them. Example:
           nums = [1,1,1], k = 2 -> 2.
 PATTERN : Prefix Sum + Hash Map (count of earlier prefix sums)
================================================================================
IDEA
  preSum is the sum of nums[0..i]. A subarray ending at i sums to k exactly
  when some earlier prefix equals preSum - k (the variable remove). map counts
  how often each prefix sum has appeared, so res += map[remove] adds every
  valid start at once. map[0] = 1 is the empty prefix, so subarrays that
  start at index 0 are counted too. Each pair of prefixes that differ by k is
  one subarray, so every subarray is counted once.
EXAMPLE
  nums = [1,2,-1,1], k = 2, map {0:1}
  1: preSum 1, remove -1 miss, res 0 | 2: preSum 3, remove 1 hit, res 1
  -1: preSum 2, remove 0 hit, res 2 | 1: preSum 3, remove 1 hit, res 3
  Answer 3: [2], [1,2,-1], [2,-1,1]
COMPLEXITY
  Time  O(n)  one pass, O(1) average hash lookup and update per element
  Space O(n)  map may hold up to n+1 distinct prefix sums
PATH TO OPTIMAL
  Brute force: all start/end pairs, re-sum each - O(n^3) - baseline.
  Running sum per start index - O(n^2) - removes the inner re-summing.
  Prefix sum + hash map (optimal.cs) - O(n) - finds all starts in one lookup.
KEYWORDS
  prefix sum, hash map, subarray sum, count subarrays, negative numbers
WATCH OUT
  - Forgetting map[0] = 1 misses subarrays that start at index 0.
  - Update map[preSum] AFTER the lookup. Doing it first, with k = 0, counts
    the empty subarray.
  - Sliding window does not work here: negatives break the "shrink when too
    big" rule.
  - preSum is int; very large sums can overflow. Use long if values are big.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Why not two pointers / sliding window?
     -> With negatives the sum is not monotonic as the window grows. Sliding
        window works only for all-positive nums, in O(1) space.
  2. Return the longest subarray with sum k instead of the count?
     -> Store the first index of each prefix sum, not its count. Length is i -
        firstIndex[remove]. Still O(n) time and space.
  3. Count subarrays whose sum is divisible by k?
     -> Key the map by ((preSum % k) + k) % k and look up the same remainder.
        Same O(n) time, O(k) space.
  4. Binary array, or very little memory allowed?
     -> For 0/1 arrays, use an int array indexed by prefix sum instead of a
        dictionary. Still O(n) space, but lower constant factors.
TRIGGER
  Count or find subarrays with an exact sum when values can be negative: use
  a prefix sum plus a hash map of earlier prefixes.
================================================================================
*/
