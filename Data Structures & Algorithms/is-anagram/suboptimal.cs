// --------------------------------------------------------------------------
//  Reference solution - from NeetCode / other resource (submission-0)
//  Not one you solved yourself.
// --------------------------------------------------------------------------

public class Solution
{
    public bool IsAnagram(string s, string t)
    {
        // Different lengths can never be anagrams. O(1) rejection.
        if (s.Length != t.Length)
            return false;

        // Build the frequency map of the first string.
        var charFrequency = new Dictionary<char, int>();

        foreach (char c in s)
        {
            charFrequency.TryGetValue(c, out int currentCount);
            charFrequency[c] = currentCount + 1;
        }

        // Walk the second string and spend one unit of each character's budget.
        foreach (char c in t)
        {
            if (!charFrequency.TryGetValue(c, out int remaining))
                return false;              // character not in s at all

            if (remaining == 0)
                return false;              // t uses this character more often than s

            charFrequency[c] = remaining - 1;
        }

        // Lengths matched and every character in t was covered by s's budget,
        // so no character can be left over. No second sweep needed.
        return true;
    }
}

/*
================================================================================
 PATTERN : Hashing - Frequency Map
 SOURCE  : NeetCode / other resource (submission-0, refactored: TryGetValue,
           no negative-count trick)
 STATUS  : Sub-optimal on space; the general-purpose version
================================================================================

WHY THIS PATTERN
  An anagram is defined purely by character COUNTS, order is irrelevant.
  Anything defined by counts wants a frequency map.

BRUTE FORCE (and why it fails)
  Sort both strings and compare: O(n log n) time, O(n) space for the char
  arrays. Correct and only three lines, but the log n factor is unnecessary -
  counting is strictly cheaper than ordering when you only need counts.

INVARIANT
  charFrequency[c] holds the number of unconsumed occurrences of c from s.
  If t ever needs a character with zero budget left, the strings differ.

WHY THIS ONE IS SUB-OPTIMAL
  Only against optimal.cs, and only on space and constant factor:
  a Dictionary costs O(k) memory for k distinct characters plus hashing on
  every access, where the array version is O(1) memory and a raw index.
  In exchange this version handles ANY character set - Unicode, digits,
  spaces, emoji. optimal.cs assumes lowercase a-z and breaks otherwise.
  That is the actual trade, and the right interview answer is "which alphabet?"

ALGORITHM (NeetCode: "Hash Map")
  1. Length mismatch -> false immediately.
  2. Count every character of s into a dictionary.
  3. For each character of t, decrement its count; fail if missing or zero.
  4. Equal lengths + full coverage => the map is exactly drained => true.

COMPLEXITY
  Time  : O(n) - two passes over strings of length n, O(1) average per lookup.
  Space : O(k) where k = distinct characters. Bounded by the alphabet, so
          O(1) for a fixed alphabet, O(n) in the worst theoretical case.

TRIGGER
  "Is X a rearrangement/permutation of Y", "same characters, any order",
  "group by character content". Any time ORDER IS EXPLICITLY IRRELEVANT.

C# NOTES
  - TryGetValue(c, out int n) does ONE hash lookup. ContainsKey followed by
    the indexer does two - a habit worth breaking early.
  - out int currentCount defaults to 0 when the key is absent, which is
    exactly the seed value a counter wants.
  - For counting, CollectionsMarshal.GetValueRefOrAddDefault gives a ref to
    the slot and beats even TryGetValue in hot paths. Niche, but real.

WATCH OUT
  - The original version allowed the count to go NEGATIVE and then tested for
    it. Checking `remaining == 0` before decrementing is the same logic with
    one less state to reason about.
================================================================================
*/
