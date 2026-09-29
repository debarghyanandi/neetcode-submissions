// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(n) space
// --------------------------------------------------------------------------

public class Solution
{
    public int[] TwoSum(int[] nums, int target)
    {
        // Maps a value we have already passed -> the index it sat at.
        var valueToIndex = new Dictionary<int, int>();

        for (int index = 0; index < nums.Length; index++)
        {
            int complement = target - nums[index];

            // Has the number that completes this pair already gone by?
            if (valueToIndex.TryGetValue(complement, out int complementIndex))
                return new int[] { complementIndex, index };

            // Record current value only AFTER the check, so an element is
            // never paired with itself.
            valueToIndex[nums[index]] = index;
        }

        return Array.Empty<int>();
    }
}

/*
================================================================================
 PROBLEM : Given an int array nums and an int target, return the indices of
           the two different elements whose values add up to target. Return
           indices, not values, with the smaller index first. Assume exactly
           one valid pair exists. Example: nums = [3,4,5,6], target = 7 ->
           [0,1].
 PATTERN : Hash Map lookup (complement, one pass)
================================================================================
IDEA
  Walk nums once. For each nums[index], the partner we need is complement =
  target - nums[index]. valueToIndex stores every value already passed, so
  one lookup tells us if that partner came earlier. If it did, return
  [complementIndex, index]. Any valid pair (i < j) is found at step j, when
  nums[i] is already stored.
EXAMPLE
  nums = [5,3,3,1], target = 6 (duplicate values, self-pair trap)
  i=0: comp 1 miss, store 5->0 | i=1: comp 3 miss (3 not stored yet), store
  3->1
  i=2: comp 3 hit at 1 -> return [1,2]
COMPLEXITY
  Time  O(n)  one pass, each lookup and insert is O(1) on average
  Space O(n)  valueToIndex can hold up to n entries
PATH TO OPTIMAL
  Brute force: check every pair i<j - O(n^2) time, O(1) space.
  Sort (value, index) pairs + two pointers - O(n log n) - no pair scan.
  One-pass hash map (this file) - O(n) - lookup replaces the search.
KEYWORDS
  two sum, hash map, complement lookup, one pass, array, indices
WATCH OUT
  - Storing before checking breaks self-pairs: nums=[3,2,4], target=6
    would return [0,0]. The code checks first, then stores.
  - Use the indexer valueToIndex[x] = i, not Add: Add throws on duplicates.
  - Return [complementIndex, index] in that order; it is already sorted.
  - target - nums[index] can overflow int for extreme values; use long.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. What if nums is sorted?
     -> Use two pointers from both ends and move one inward based on the sum.
        O(n) time, O(1) space, no hash map needed (Two Sum II).
  2. Return all pairs, or count them?
     -> Keep going instead of returning. Store a count or a list of indices
        per value. Still O(n) average time, plus the size of the output.
  3. Find three numbers that sum to target (3Sum)?
     -> Sort, fix one element, run two pointers on the rest. O(n^2) time, O(1)
        extra space; skip equal neighbours to avoid duplicate triples.
  4. Numbers arrive as a stream, with add() and find(target) calls?
     -> Keep a value->count map. add is O(1); find scans the map, O(n). Or
        precompute all sums for O(1) find but O(n) add and O(n^2) memory.
TRIGGER
  You need a pair (or an earlier element) that completes the current one to
  a fixed target, and a single pass with O(1) lookups would replace a scan.
================================================================================
*/
