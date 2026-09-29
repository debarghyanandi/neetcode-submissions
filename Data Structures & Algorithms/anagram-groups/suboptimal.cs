// --------------------------------------------------------------------------
// -  suboptimal.cs         O(n * k log k) time / O(n) space
// -  Sort each word to canonical form   [sort-canonical]
// -  ranks below optimal.cs (O(n * k) time / O(n) space)
// -
// -  Reference solution - not one you solved yourself
// -
// -  Sorting each word to its canonical form introduces O(k log k) per
// -  word, compared to O(k) linear frequency counting.
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
 PATTERN : Hash Map Grouping - sorted string as canonical key
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Suboptimal
================================================================================
VARIABLES
  groupsByKey   groupsByKey[sortedKey] = every word whose letters sort to sortedKey
  characters    a copy of the current word's chars, sorted in place
  sortedKey     the canonical form of word: its letters in sorted order
  group         the list stored in groupsByKey for sortedKey (same object, not a copy)
WHY THIS PATTERN
  The problem says to put words together when they use the same letters. So each
  word needs a label that is equal for all anagrams and different for all other
  words. Sorting the letters gives that label: "eat", "tea" and "ate" all become
  "aet". groupsByKey then collects the words by that label in one pass. The
  answer is groupsByKey.Values.
BETTER APPROACH
  The better approach uses a letter-count key instead of a sorted key. For each
  word, fill an int[26] with how many times each letter appears. Then turn the
  counts into a string key, for example "1#0#0#...#1", and use it the same way.
  Building the key takes O(k) per word, so the total time is O(n * k) instead of
  O(n * k log k). This file loses because Array.Sort does a comparison sort on
  every word. Counting only needs to read each letter once.
INVARIANT
  After each word in the loop, every word seen so far is in exactly one list:
  the list stored under its own sortedKey. Two words get the same sortedKey only
  if they have the same multiset of characters (the same letters, each the same
  number of times). That is the definition of an anagram. So each list holds all
  the anagrams of one word and nothing else, and every word ends up in some
  list.
THE LIST IS SHARED BY REFERENCE
  group is a reference to the list inside groupsByKey. It is not a copy. So
  group.Add(word) changes the dictionary's list directly, and you never write it
  back after adding. On a new key, the code stores the new list first and then
  adds to it. On an existing key, TryGetValue does a single lookup.
WATCH OUT
  If strs contains a null entry, word.ToCharArray() throws a
  NullReferenceException. The code does not check for it. An empty string "" is
  fine: its key is "", and all empty strings go into one group. Array.Sort on a
  char[] sorts by numeric char value, not by culture rules. That is what you
  want here, but a string comparison that uses culture rules could treat
  different letters as equal. The order of the groups comes from Dictionary
  enumeration, and the documentation does not promise any order. So a test that
  expects the groups in a fixed order may fail.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. What if the input can contain any Unicode characters, not only a-z?
     Keep the sorted key. An int[26] count key only works for a fixed small
     alphabet. For a large alphabet you would need a Dictionary<char,int> per
     word, and turning that into a stable key costs a sort anyway.
  2. What if the input is too large to fit in memory?
     Stream the words. Write each (key, word) pair to disk, split into buckets
     by a hash of the key, then group each bucket in memory. Each bucket fits in
     memory, but you pay for extra disk passes.
  3. How do you return only the groups that have more than one word?
     Filter at the end, for example groupsByKey.Values.Where(g => g.Count > 1).
     The grouping pass does not change, and the filter costs one extra O(number
     of groups) pass.
  4. Can you avoid storing a key string for each group?
     Map each letter to a prime number and use the product as a long key. This
     saves the key strings, but the product overflows for long words. After an
     overflow, two different words can get the same key, and the grouping
     becomes wrong.
TRIGGER
  When a problem asks you to group or match items that are "the same up to
  reordering", build a canonical key for each item and group by it in a hash
  map.
C# NOTE
  word.ToCharArray() and new string(characters) each allocate on every word. For
  short words you can copy into a Span<char> made with stackalloc, sort it with
  MemoryExtensions.Sort, and allocate only the final key string.
COMPLEXITY
  Time  : O(n * k log k)
  Space : O(n)
================================================================================
*/
