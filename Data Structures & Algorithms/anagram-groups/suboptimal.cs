// --------------------------------------------------------------------------
// -  suboptimal.cs         O(n * k log k) time / O(n) space
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
 PROBLEM : Given an array of strings strs, group the words that are anagrams
           of each other. Anagrams use the same letters the same number of
           times. Return the groups in any order. Example:
           ["act","pots","tops","cat","stop","hat"] ->
           [["act","cat"],["pots","tops","stop"],["hat"]].
 PATTERN : Hash Map grouping by canonical key (sorted string)
================================================================================
IDEA
  All anagrams become the same string when their letters are sorted. For each
  word, sort its characters and build sortedKey, then add the word to the list
  stored under that key in groupsByKey. Words share a key only if they have
  the
  same letter counts, so each list is exactly one anagram group. Unlike
  optimal.cs, the key comes from sorting instead of a 26-letter count array.
EXAMPLE
  strs = ["eat","tea","tan","ate","nat","bat","",""]
  keys: aet, aet, ant, aet, ant, abt, "", "" (the empty string is a valid key)
  -> [["eat","tea","ate"],["tan","nat"],["bat"],["",""]]
COMPLEXITY
  Time  O(n * k log k)  n words, and sorting each word of length k costs k log
                        k
  Space O(n)            the map stores each word once, plus one sorted key per
                        group
WATCH OUT
  - Build the key with new string(characters). characters.ToString() returns
    "System.Char[]" for every word, so everything lands in one group.
  - Do not treat group order as guaranteed. Dictionary.Values order is not
    promised by the spec. The problem accepts any order, but tests may not.
  - Keep duplicate words. ["a","a"] must give [["a","a"]], so do not use a
    set for the values.
  - Sorting compares char codes, so "Ab" and "ab" get different keys. Ask
    whether the input is lowercase only.
================================================================================
*/
