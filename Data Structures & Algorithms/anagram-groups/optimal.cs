// --------------------------------------------------------------------------
// -  optimal.cs            O(n * k) time / O(n) space
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
 PROBLEM : Given an array of strings strs, group the words that are anagrams
           of each other. Anagrams use the same letters with the same counts.
           Return the list of groups; the order of groups and of words inside
           a group does not matter. ["eat","tea","tan","ate","nat","bat"] ->
           [[eat,tea,ate],[tan,nat],[bat]]
 PATTERN : Hash Map grouping by canonical key (letter counts)
================================================================================
IDEA
  Two words are anagrams exactly when their 26 letter counts are equal.
  For each word, fill letterCounts, then join the counts with commas into
  signature. signature is the key in groupsByKey, and the word is added to
  that key's list. Equal counts give equal strings, so anagrams always land
  in the same group, and non-anagrams never do.
EXAMPLE
  strs = ["eat","tea","tan","ate","nat","bat"]
  eat, tea, ate -> "1,0,0,0,1,...,1(t),..." (a=1, e=1, t=1) -> same key
  tan, nat -> a=1, n=1, t=1 -> new key; bat -> a=1, b=1, t=1 -> new key
  Result: [[eat,tea,ate],[tan,nat],[bat]]
COMPLEXITY
  Time  O(n * k)  each char is counted once, plus a fixed 26-count key per
                  word
  Space O(n)      every word is stored once in groupsByKey, plus one key per
                  group
PATH TO OPTIMAL
  Compare every pair with a count check - O(n^2 * k) - slow, many pairs.
  Sort each word as its key (suboptimal.cs) - O(n * k log k) - one pass.
  Count letters as the key (this file) - O(n * k) - no sort per word.
KEYWORDS
  group anagrams, hash map, canonical form, frequency count, signature key
WATCH OUT
  - No separator in the key: counts 1,11 and 11,1 both become "111" and
    wrong words get grouped. Keep the ',' after each count.
  - Using int[] letterCounts as the key: arrays hash by reference, so every
    word ends up in its own group.
  - Uppercase or non a-z input: c - 'a' goes out of range and throws.
  - "" is valid: its key is 26 zeros, and all empty strings group together.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. What if words contain Unicode or any characters?
     -> Use the sorted word as the key, or a Dictionary<char,int> turned into
        a sorted string. Time becomes O(n * k log k), but it works on any
        alphabet.
  2. Can you avoid building a 26-part string per word?
     -> Map letters to primes and multiply them for the key. It is fast, but
        the product overflows on long words, so it is risky.
  3. What if the words arrive as a stream?
     -> Keep groupsByKey alive and add each word as it comes, O(k) per word.
        Memory grows with the number of distinct groups and stored words.
TRIGGER
  When items must be grouped by "same content, any order", build a canonical
  key for each one and bucket them in a hash map.
================================================================================
*/
