// --------------------------------------------------------------------------
// -  optimal.cs            O(n * k) time / O(n) space
// -  Character frequency signature hashing   [frequency-hash]
// -  ranks above suboptimal.cs (O(n * k log k) time / O(n) space)
// -
// -  Reference solution - not one you solved yourself
// -
// -  Each word processes through its characters once and builds a
// -  constant-size signature, avoiding sorting overhead.
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
 PATTERN : Hashing / Grouping - letter-count signature as the key
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Optimal
================================================================================
VARIABLES
  groupsByKey    signature -> list of all words that have that signature
  letterCounts   letterCounts[x] = how many times letter ('a' + x) appears in word
  keyBuilder     builds the text form of letterCounts, like "1,0,2,..."
  signature      the finished key string; words with the same signature are anagrams
  group          the list in groupsByKey for this signature (new or existing)
WHY THIS PATTERN
  The task says "put words into groups when they are anagrams of each other".
  Grouping by a shared property is a job for a hash map from a key to a bucket.
  Two words are anagrams exactly when they have the same count of each letter.
  So letterCounts, written as signature, is a key that all anagrams share and no
  other word has. One pass puts each word into its bucket in groupsByKey.
BRUTE FORCE
  The first correct version most people write sorts each word's letters and uses
  the sorted string as the dictionary key. It costs O(n * k log k), because each
  word of length k must be sorted. It loses the log k factor that counting
  avoids. An even simpler version compares every pair of words, which costs
  O(n^2 * k).
INVARIANT
  After each word is handled, every word seen so far sits in exactly one list in
  groupsByKey, and that list is under the key made from its own letter counts.
  Two words get the same signature if and only if their 26 counts are all equal,
  which is the definition of an anagram. So at the end, each list in
  groupsByKey.Values is one complete anagram group. No group is split in two and
  no two groups are mixed.
WHY THE COMMA SEPARATOR
  Without the ',' the numbers run together, and the key stops being unique.
  Counts [1, 11] and [11, 1] would both become "111", so two words that are not
  anagrams would share a group. The separator makes each count's edges clear, so
  the string maps back to exactly one count array.
WATCH OUT
  letterCounts[c - 'a'] only works for lowercase 'a' to 'z'. An uppercase
  letter, a digit or a space gives an index outside 0..25 and throws
  IndexOutOfRangeException. The order of the groups comes from the Dictionary's
  internal order, so do not write tests that expect a fixed order. The file has
  no using lines, so ToList() only compiles if System.Linq is imported somewhere
  else (for example, by the judge's implicit usings).
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. What if the input can contain any Unicode characters?
     A fixed array of 26 no longer works. Count into a Dictionary<char, int>,
     then build the key from its entries sorted by character, or just use the
     sorted-characters key. This works for any alphabet, but each key costs more
     to build.
  2. Can the key be shorter?
     Yes. Cast each count to a char and build new string(char[26]). That gives a
     fixed 26-character key with no separators. It only works while every count
     fits in a char (up to 65535).
  3. Why not use a product of primes (one prime per letter) as the key?
     It is unique in theory, but the product overflows long for long words, and
     then different words can share a key. You would need BigInteger, which is
     slower than the string key.
  4. What if the input is too large for one machine's memory?
     Treat it as map-reduce. Each worker computes the signature for its words
     and sends each word to the machine that owns that signature (chosen by
     hash). Each machine then groups its words on its own. You pay network cost,
     but no single dictionary has to hold every word.
TRIGGER
  When items must be put into groups where "equal up to reordering" (or any
  other equivalence) matters, find a canonical key that all members share and
  use it as a hash map key.
C# NOTE
  The TryGetValue-then-assign pattern does two hash lookups when a signature is
  new. CollectionsMarshal.GetValueRefOrAddDefault(groupsByKey, signature, out _)
  returns a ref to the slot and does it in one lookup.
COMPLEXITY
  Time  : O(n * k)
  Space : O(n)
================================================================================
*/
