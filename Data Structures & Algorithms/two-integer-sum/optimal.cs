// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(n) space
// -  hash map complement lookup, one pass   [hashmap-complement]
// -  ranks above suboptimal.cs (O(n^2) time / O(1) space)
// -
// -  No '//My solution' marker in the source
// -
// -  single pass storing seen values in a dictionary, checking complement
// -  before insertion gives O(n) time and O(n) auxiliary space for the map
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
 PATTERN : Hashing - Complement Lookup (one pass)
 SOURCE  : NeetCode / other resource (submission-0, refactored: for-loop
           index, TryGetValue)
 STATUS  : Optimal
================================================================================

WHY THIS PATTERN
  The inner loop of the brute force asks the same question every time:
  "does target - nums[i] exist somewhere else in the array?" That is a
  LOOKUP, and a lookup belongs in a hash map, not in a loop.

BRUTE FORCE (and why it fails)
  for i, for j > i: check nums[i] + nums[j] == target. O(n^2) time, O(1)
  space. Fine at n = 100, dead at n = 10^5. The rewrite is mechanical:
  every time you catch an inner loop SEARCHING, replace it with a hash map.

WHY NOT SORT + TWO POINTERS?
  Sorting is O(n log n) - slower - and it destroys the original indices,
  which is what this problem asks you to return. That approach IS correct
  when the input arrives sorted, which is exactly problem two-integer-sum-ii.
  Same problem, one changed constraint, completely different optimal tool.
  Know why each is chosen, that is the actual interview question.

INVARIANT
  Before handling index i, valueToIndex holds every value of nums[0..i-1]
  mapped to its index. So a hit is always a genuinely different element.

ALGORITHM (NeetCode: "Hash Map (One Pass)")
  1. Empty dictionary value -> index.
  2. For each index, compute complement = target - nums[index].
  3. If complement is already in the map, the pair is (its index, index).
  4. Otherwise store nums[index] -> index and continue.

COMPLEXITY
  Time  : O(n) - one pass, O(1) average lookup and insert.
  Space : O(n) - worst case every element is stored before the pair is found.

TRIGGER
  "Find two elements that satisfy an ARITHMETIC RELATION" where one operand
  determines the other exactly (a + b = k, b - a = k, a * b = k).
  If the target relation is an INEQUALITY, hashing fails - sort instead.

C# NOTES
  - TryGetValue is one hash lookup; ContainsKey + indexer is two.
  - `valueToIndex[key] = value` overwrites silently, Add() would throw on a
    duplicate key. Overwriting is what we want here: for duplicate values the
    later index is kept, and it still yields a valid answer.
  - Array.Empty<int>() returns a cached singleton - zero allocation, better
    than new int[0] when signalling "no result".

WATCH OUT
  - Order matters: check BEFORE inserting. Insert first and [3,x], target 6
    would return [0,0], pairing the element with itself.
  - Assumes exactly one valid answer (the problem guarantees it). If multiple
    pairs were possible this returns the one that completes earliest.
================================================================================
*/
