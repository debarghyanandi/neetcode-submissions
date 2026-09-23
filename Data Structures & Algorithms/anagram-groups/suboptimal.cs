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
  groupsByKey   key = word with its letters sorted, value = all input words that sort to that key
  characters    the current word's letters as a mutable array, sorted in place
  sortedKey     the canonical form of the current word, built from characters
  group          the live list inside groupsByKey for sortedKey
WHY THIS PATTERN
  Two words are anagrams exactly when they hold the same letters with the same
  counts, so the test is an equality test on some canonical form - one value
  that every anagram of a word produces. Sorting the letters gives such a form,
  so sortedKey is the same string for every member of a group. A Dictionary
  turns "find the group this word belongs to" into one O(1) lookup instead of
  comparing the word against every group seen so far.
BETTER APPROACH
  The better key is a 26-slot count array: count the letters of word, then turn
  the counts into a fixed string such as "1#0#2#...". That removes the k log k
  sort and makes key building linear in the word length, so the whole run is O(n
  * k). This file loses because Array.Sort(characters) is run once per word; for
  long words the log k factor is real work, and each word also costs two extra
  allocations, ToCharArray plus new string.
INVARIANT
  After each iteration of the foreach, groupsByKey holds exactly one entry per
  distinct canonical form seen so far, and that entry's list holds every word
  processed so far with that form, in input order. The TryGetValue-else-create
  step preserves this: an existing group is reused, a missing one is created
  empty and stored before the Add. When the loop ends every input word sits in
  exactly one list, so Values is the full partition into anagram groups.
WATCH OUT
  group is captured by reference, so group.Add(word) also updates the list
  stored in the dictionary - the later assignment back into groupsByKey is not
  needed and is not done. Because of that, the lists returned by ToList() are
  the same objects still held by groupsByKey; if a caller mutates one, the
  dictionary changes too. Sorting the raw chars means non-ASCII input is grouped
  by UTF-16 code units, so a surrogate pair can be split and two different words
  can collide on one key. An empty strs returns an empty list, which is correct,
  but note that "" is a valid word and becomes its own group under the empty
  key.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How would you drop the sort and keep the same grouping?
     Build the key from a count array over the alphabet and join the counts with
     a separator. Linear in word length, but the key is longer than the word for
     short words and it hard-codes an alphabet size.
  2. The input does not fit in memory. What changes?
     Stream the words, compute the key, and write each word to a file or shard
     named by a hash of the key; then group each shard on its own. You trade RAM
     for disk IO and a second pass.
  3. Output must be sorted - groups by size, words inside each group
  alphabetically. How?
     Sort each List<string> after the loop and sort the outer list by Count.
     That adds O(total * log) work and gives up the input-order property the
     current code has for free.
  4. You only need the size of the largest anagram group. Can you save space?
     Keep Dictionary<string,int> of counts instead of lists and track the
     running maximum. Still O(number of distinct keys) space, but you stop
     storing every input word.
TRIGGER
  When items must be bucketed by "same up to reordering or relabelling", invent
  a canonical form and use it as a hash map key.
C# NOTE
  TryGetValue with the out variable does one hash lookup for the miss case
  instead of the ContainsKey-then-indexer pair, and the second write
  groupsByKey[sortedKey] = group only happens on a miss. Array.Sort on a char[]
  is the in-place sort here; word.OrderBy(c => c) would work but allocates an
  iterator chain and needs string.Concat to rebuild the key.
COMPLEXITY
  Time  : O(n * k log k)
  Space : O(n)
================================================================================
*/
