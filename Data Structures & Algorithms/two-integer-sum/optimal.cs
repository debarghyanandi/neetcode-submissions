// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(n) space
// -  hash map complement lookup, one pass   [hashmap-complement]
// -  the only solution in this folder
// -
// -  Reference solution - not one you solved yourself
// -
// -  single pass storing seen value->index in a dictionary, checking for
// -  the complement before inserting the current value gives O(n) time with
// -  O(n) auxiliary space for the map
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
 PATTERN : Hash Map Complement Lookup - check before insert
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  The nested-loop version asks, for every index, "does some other position hold
  target - nums[index]?" That is a pure membership question over values already
  seen, and a Dictionary answers it without rescanning. The only extra thing
  needed beyond membership is the position, which is why valueToIndex stores
  value -> index rather than being a HashSet.
WHY ONE PASS SUFFICES
  The usual doubt on re-reading: the loop only ever looks backward, so how does
  it find a pair whose partner comes later? Take any valid pair (i, j) with i <
  j. When index reaches j, nums[i] was already written into valueToIndex on
  iteration i, and complement = target - nums[j] equals nums[i], so TryGetValue
  hits. Every pair is discovered exactly once, at its larger index. Looking
  forward would be redundant work.
INVARIANT
  At the top of each iteration, valueToIndex holds one entry per distinct value
  in nums[0 .. index-1], each mapped to a position where that value actually
  occurs. Nothing from index or beyond is ever in the map at the moment of the
  lookup - that is the whole reason the returned pair has two distinct
  positions.
ORDER OF THE WRITE
  The insert sits after the TryGetValue on purpose. Flip the two lines and the
  self-pairing case breaks: with nums = [3, 4] and target = 6, index 0 would
  write 3 -> 0, then find complement 3 in the map and return [0, 0], using one
  element twice. Placing the write last makes the current element structurally
  unreachable to its own lookup, so no explicit complementIndex != index guard
  is needed.
DUPLICATES AND OVERWRITES
  valueToIndex[nums[index]] = index overwrites the stored position when a value
  repeats, keeping only the most recent index. That is safe: any stored index
  for a value v is as good as any other, since a later lookup for v only needs
  some position holding v. nums = [3, 3], target 6 still works - index 0 records
  3 -> 0, index 1 finds it and returns [0, 1] before the overwrite ever matters.
RETURN CONTRACT
  Indices come back as [complementIndex, index], always ascending, because
  complementIndex was written on an earlier iteration. The fallthrough returns
  Array.Empty<int>() rather than null, so a caller can check Length without a
  null test; under the standard "exactly one solution" guarantee that line is
  unreachable, but it keeps the method total if the guarantee is dropped.
FOLLOW-UPS TO EXPECT
  1. Sorted input: two pointers from both ends beat this on space, since no map
  is needed. 2. Return all pairs, not the first: the early return has to go, and
  valueToIndex must map value -> list of indices, since a repeated value can
  complete several pairs. 3. Return values instead of indices: the map collapses
  to a HashSet, and duplicate results need explicit dedup. 4. Why TryGetValue
  instead of ContainsKey then indexer: one probe that also hands back
  complementIndex, versus two probes for the same key.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
