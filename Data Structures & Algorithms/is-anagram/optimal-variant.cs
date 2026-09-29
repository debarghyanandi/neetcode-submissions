// --------------------------------------------------------------------------
// -  optimal-variant.cs    O(n) time / O(1) space
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
                return false;              // character not in first at all

            if (remaining == 0)
                return false;              // second uses this character more often than first

            charFrequency[c] = remaining - 1;
        }

        // Lengths matched and every character in t was covered by s's budget,
        // so no character can be left over. No second sweep needed.
        return true;
    }
}

/*
================================================================================
 PROBLEM : Given two strings s and t, return true if t is an anagram of s.
           That means t uses exactly the same characters as s, each the same
           number of times, only in a different order. Example: s = "anagram",
           t = "nagaram" -> true; s = "rat", t = "car" -> false.
 PATTERN : Hash Map frequency count (count up, then spend down)
================================================================================
IDEA
  First reject if s.Length != t.Length. Then fill charFrequency with the
  count of each char in s. Walk t and spend one unit of that char's count.
  Stop with false if the char is missing or its count is already 0. Why it
  is correct: the lengths are equal and no count went below 0, so every
  count must end at exactly 0. Unlike optimal.cs, it uses a Dictionary, not
  a fixed array, so it works for any char and can exit early.
EXAMPLE
  s = "aab", t = "abb" (same length, but the counts differ)
  build: charFrequency = {a:2, b:1}
  t: 'a' -> {a:1, b:1}; 'b' -> {a:1, b:0}; 'b' -> remaining == 0
  answer: false (t uses 'b' more often than s does)
COMPLEXITY
  Time  O(n)  one pass over s and one pass over t, each lookup O(1) on average
  Space O(1)  one map entry per distinct char, bounded for a fixed alphabet
WATCH OUT
  - The length check is required. Without it, s = "aab", t = "ab" returns
    true, because t only spends part of s's budget.
  - A key stays in the map at count 0, so TryGetValue still succeeds. The
    check remaining == 0 is what catches too many uses of a char.
  - O(1) space holds only for a fixed alphabet. With full Unicode input,
    the map grows with the number of distinct chars.
  - A null s or t throws on s.Length. Ask the interviewer whether null is
    possible.
================================================================================
*/
