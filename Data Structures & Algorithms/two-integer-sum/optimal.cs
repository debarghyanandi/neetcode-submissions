// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(n) space
// -  Hash map complement lookup   [hashmap-complement]
// -  the only solution in this folder
// -
// -  Reference solution - not one you solved yourself
// -
// -  Single pass through array with O(1) dictionary lookups and insertions;
// -  stores seen values to find complements.
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
 PATTERN : Hash Map Lookup - store seen values, search for complement
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Optimal
================================================================================
VARIABLES
  valueToIndex      value already passed -> the index where it sat
  complement        target - nums[index], the partner this element needs
  complementIndex   index stored earlier for that partner
WHY THIS PATTERN
  The problem asks for two positions whose values add to target, and addition is
  fully determined by one side: once you fix nums[index], the partner you need
  is exactly complement. So the question turns from "search all pairs" into
  "have I seen this one number before?", which a hash map answers in constant
  time. valueToIndex carries every earlier value along with where it was, so the
  moment complement is found the pair of indices is ready.
BRUTE FORCE
  Two nested loops: for every index, scan every later index and test whether the
  two values sum to target. That is correct and needs no extra memory, but it
  costs O(n^2) time because it re-scans the prefix for each element. The map
  replaces that inner scan with one lookup.
INVARIANT
  Before the body of iteration index runs, valueToIndex holds exactly the values
  at positions 0..index-1 mapped to an index where each occurs. So a hit on
  complement is always a strictly earlier position, never the current one, and
  the returned pair is two distinct indices. If no answer exists among pairs
  ending at index, the loop adds nums[index] and moves on, so every pair (a, b)
  with a < b is tested exactly once at the step b.
ORDER OF CHECK AND INSERT
  The check happens before the insert, and this is what makes target = 2 *
  nums[index] safe. If you inserted first, nums[index] would find itself as its
  own complement and return a pair like [3, 3]. The comment in the code says
  this, and the code matches it.
WATCH OUT
  Writing valueToIndex[nums[index]] = index overwrites the stored index when the
  same value appears twice, so the map keeps the latest position, not the first.
  That is harmless here because the check runs before the write, but it means
  the returned pair is not always the earliest possible pair of indices - if the
  problem demanded the lexicographically smallest answer you would need TryAdd
  instead. The no-solution path returns Array.Empty<int>(), an empty array, not
  null; a caller that does result[0] without a length check will throw
  IndexOutOfRangeException. Also note the sum target - nums[index] can overflow
  int if target and nums values sit near int.MinValue or int.MaxValue.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. What if the array is sorted and you must use O(1) extra space?
     Use two pointers, one at each end; move left in when the sum is too small,
     right in when too large. That is O(n) time with no map, but it only works
     because sorting lets you discard half the search space, and sorting an
     unsorted input first costs O(n log n).
  2. Return all distinct pairs that sum to target, not just one.
     Do not return on a hit; instead record the pair and keep going, and make
     valueToIndex map a value to a list of indices so repeated values are all
     reachable. Output size can become quadratic, so the time bound is no longer
     O(n).
  3. The array does not fit in memory and arrives as a stream.
     The same single pass works as long as the map fits, since only past values
     are needed - but the map grows to the number of distinct values seen, which
     is the real memory limit. If that does not fit, you would shard values by
     hash across machines or files so each shard holds a value and its
     complement together.
  4. Three numbers that sum to target instead of two.
     Fix one index in an outer loop and run this exact map scan on the rest,
     giving O(n^2) time; or sort and use the two-pointer sweep inside one loop
     for O(n^2) time with O(1) extra space beyond the sort.
TRIGGER
  You are asked for a pair of elements satisfying a relation where one element
  fully determines the other.
C# NOTE
  TryGetValue with the out parameter does one hash lookup and hands back
  complementIndex in the same call, where ContainsKey followed by the indexer
  would hash twice; declaring out int complementIndex inline keeps the variable
  scoped to the branch that uses it.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
