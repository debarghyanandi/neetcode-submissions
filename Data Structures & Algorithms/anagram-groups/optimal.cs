// --------------------------------------------------------------------------
//  Reference solution - from NeetCode / other resource (submission-2 + submission-3)
//  Not one you solved yourself.
// --------------------------------------------------------------------------

public class Solution
{
    public List<List<string>> GroupAnagrams(string[] strs)
    {
        // Key   = character-frequency signature, e.g. "1,0,2,0,...,0"
        // Value = all words sharing that signature
        var groupsByKey = new Dictionary<string, List<string>>();

        foreach (string word in strs)
        {
            // letterCounts[0] = number of 'a', ... letterCounts[25] = 'z'
            int[] letterCounts = new int[26];

            foreach (char c in word)
            {
                letterCounts[c - 'a']++;
            }

            // Arrays hash by REFERENCE, not by contents, so int[] cannot be a
            // dictionary key directly. Flatten the counts into a string.
            var keyBuilder = new System.Text.StringBuilder();

            foreach (int count in letterCounts)
            {
                keyBuilder.Append(count);
                keyBuilder.Append(',');   // separator: "1,11" must not equal "11,1"
            }

            string signature = keyBuilder.ToString();

            if (!groupsByKey.TryGetValue(signature, out List<string> group))
            {
                group = new List<string>();
                groupsByKey[signature] = group;
            }

            group.Add(word);
        }

        return groupsByKey.Values.ToList();
    }
}

/*
================================================================================
 PATTERN : Hashing - Frequency Signature as Key
 SOURCE  : NeetCode / other resource (submission-2 + submission-3 merged)
 STATUS  : Optimal
================================================================================

WHY THIS PATTERN
  Same canonical-form idea as suboptimal.cs, but a cheaper canonical form.
  Two words are anagrams iff their character COUNTS match, and counts can be
  produced in O(m) - no ordering required. Sorting answers a stronger
  question than the problem asks, and you pay log m for the surplus.

BRUTE FORCE (and why it fails)
  Pairwise anagram checks: O(n^2 * m). Sorting keys: O(n * m log m).
  Counting keys: O(n * m). Each step removes work the problem never needed.

INVARIANT
  Every word maps to exactly one signature, and two words share a signature
  iff they are anagrams. That is what makes one dictionary pass sufficient.

ALGORITHM (NeetCode: "Hash Table")
  1. Dictionary from signature -> list of words.
  2. For each word, count its 26 letters into an int[26].
  3. Serialise the counts into a delimited string - that is the signature.
  4. Append the word to that signature's list.
  5. Return the dictionary's values.

COMPLEXITY
  Time  : O(n * m) where n = number of words, m = average word length.
          The 26-slot key build is a constant per word.
          Strictly O(n * (m + 26)), and 26 is dominated whenever m is not tiny.
  Space : O(n * m) for the output, plus O(26) scratch per word.

TRIGGER
  "Group by content, ignoring order" over a SMALL FIXED ALPHABET.
  If the alphabet is unbounded (Unicode), fall back to the sorting key.

C# NOTES
  - StringBuilder over repeated string concatenation: `key += count + ","` in
    a loop allocates a new string every iteration - O(n^2) in the key length.
    This is the single most common accidental C# slowdown in DSA code.
  - The comma separator is NOT decoration. Without it counts 1 and 11 would
    produce the same key as 11 and 1 - a real, silent wrong answer.
  - A tuple or ValueTuple of 26 ints would also work as a by-value key but is
    unreadable. In .NET you could also hash the array contents manually with
    a rolling hash; the string is the honest, debuggable choice.

WATCH OUT
  - Crashes on any character outside a-z (negative index). Same constraint
    dependency as is-anagram/optimal.cs - check the problem statement.
  - `new int[26]` inside the loop is a fresh zeroed array each word. Hoisting
    it out and calling Array.Clear works too, and trades an allocation for a
    clear; measure before assuming that is faster.
================================================================================
*/
