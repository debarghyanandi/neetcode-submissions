// --------------------------------------------------------------------------
//  Reference solution - from NeetCode / other resource (submission-0 + submission-1)
//  Not one you solved yourself.
// --------------------------------------------------------------------------

public class Solution
{
    public List<List<string>> GroupAnagrams(string[] strs)
    {
        // Key   = the string's characters in sorted order (its canonical form)
        // Value = every input string that reduces to that same canonical form
        var groupsByKey = new Dictionary<string, List<string>>();

        foreach (string word in strs)
        {
            char[] characters = word.ToCharArray();
            Array.Sort(characters);
            string sortedKey = new string(characters);

            if (!groupsByKey.TryGetValue(sortedKey, out List<string> group))
            {
                group = new List<string>();
                groupsByKey[sortedKey] = group;
            }

            group.Add(word);
        }

        return groupsByKey.Values.ToList();
    }
}

/*
================================================================================
 PATTERN : Hashing - Canonical Form as Key
 SOURCE  : NeetCode / other resource (submission-0 + submission-1 merged,
           refactored: TryGetValue holds the list reference)
 STATUS  : Sub-optimal (the sorting key costs an extra log m factor)
================================================================================

WHY THIS PATTERN
  Grouping means "things that are equal under some definition of equal".
  Turn that definition into a CANONICAL FORM - one value every member of the
  group maps to - and the grouping becomes a single dictionary pass.
  Here two words are equal iff their sorted characters match.

BRUTE FORCE (and why it fails)
  Compare every word against every existing group's representative with an
  is-anagram check: O(n^2 * m). The canonical key removes the comparison
  entirely - you never compare words, you just hash them into place.

WHY THIS ONE IS SUB-OPTIMAL
  Sorting each word is O(m log m) where m is word length; counting characters
  is O(m). Across n words that is O(n * m log m) versus O(n * m).
  In exchange, this version works for ANY alphabet with no code change,
  where optimal.cs is hard-wired to lowercase a-z. Same trade as
  is-anagram: dictionary-general vs array-fast.

ALGORITHM (NeetCode: "Sorting")
  1. Dictionary from canonical key -> list of words.
  2. For each word: sort its characters into a string key.
  3. Append the ORIGINAL word (not the sorted one) to that key's list.
  4. Return the dictionary's values.

COMPLEXITY
  Time  : O(n * m log m) - n words, each sorted in O(m log m).
  Space : O(n * m) - every input word is stored once in the output, plus
          one key string per distinct group.

TRIGGER
  "Group / bucket / partition things that are equivalent under <rule>".
  The move is always: find the canonical form of the rule, then hash on it.

C# NOTES
  - TryGetValue(key, out list) returns the LIST REFERENCE, so the subsequent
    Add mutates it in place. The ContainsKey + indexer version does three
    hash lookups where this does one.
  - Array.Sort on a char[] uses introsort, in place, no allocation beyond
    the ToCharArray copy.
  - .Values.ToList() copies the value collection into a new List. Needed
    because the return type is List<List<string>>, not IEnumerable.

WATCH OUT
  - Add the original word, not the sorted key. Easy slip when refactoring.
  - `new string(characters)` allocates a fresh string per word. Unavoidable
    while sorting is the key strategy - another reason counting wins.
================================================================================
*/
