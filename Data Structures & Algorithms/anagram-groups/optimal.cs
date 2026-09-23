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
 PATTERN : Hash Map Grouping - canonical count signature as key
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Optimal
================================================================================
VARIABLES
  groupsByKey    signature string -> every word that has that letter count
  letterCounts   letterCounts[i] = how many times char ('a'+i) appears in word
  keyBuilder     builds the flattened signature text for one word
  signature      the finished key, e.g. "1,0,2,0,...,0,"
  group          the live list inside groupsByKey for this signature
WHY THIS PATTERN
  Two words are anagrams exactly when their letter counts match, so the relation
  is an equivalence: every word belongs to one and only one bucket. That means
  you never need to compare word against word; you only need a canonical form
  (one fixed representative value per bucket) that is equal for equal buckets.
  letterCounts is that canonical form, and turning it into signature lets a
  Dictionary do the bucketing in one pass over strs.
BRUTE FORCE
  The first thing most people write is a double loop: for each word, scan the
  groups already built and test "is this word an anagram of the group's first
  word" by sorting both or comparing counts. That is O(n^2 * k) because every
  word may be checked against every existing group. It loses because the anagram
  test is recomputed over and over, while the signature computes each word's
  identity once and lets hashing find the bucket.
INVARIANT
  After processing each word, groupsByKey holds exactly the words seen so far,
  partitioned so that every list contains words with identical letterCounts and
  no two lists share a signature. TryGetValue either finds the existing bucket
  or creates it, so the mapping signature -> bucket is created once and never
  duplicated. Because signature is a function of the counts only, and equal
  counts mean anagram, the final partition is the correct grouping.
THE COMMA IS NOT DECORATION
  Without the separator, counts 1 then 11 and 11 then 1 both flatten to "111",
  merging two different buckets into one wrong group. Appending ',' after every
  count makes the digit boundaries explicit, so distinct count vectors always
  give distinct strings. Any fixed non-digit separator works; the trailing comma
  on the last count is harmless because every key has one.
WATCH OUT
  c - 'a' assumes only lowercase a-z. An uppercase letter, a digit, or a space
  gives an index outside 0..25, so letterCounts[c - 'a']++ throws
  IndexOutOfRangeException or writes past the intended slot. An empty string is
  fine: it produces the all-zero signature and all empty strings group together.
  Also note the file fully qualifies System.Text.StringBuilder but calls
  ToList() unqualified, so it only compiles if System.Linq is in scope from
  global or implicit usings.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How would you build the key without a StringBuilder allocation per word?
     Sort the word's chars into a stack-allocated Span<char> and build one
     string from that, or use a custom IEqualityComparer<int[]> that hashes
     contents so the int[] itself can be the key. Sorting costs k log k per word
     instead of k, but it drops the 26-entry scan and the comma padding.
  2. What if the input is Unicode, not just 26 letters?
     The fixed 26-slot array no longer works. Use a Dictionary<char,int> per
     word and build the key from its entries in sorted char order, or simply
     sort the string's characters. The key becomes proportional to the number of
     distinct characters instead of always 26.
  3. The input does not fit in memory - how do you group it?
     Shard by hash of the signature: send each word to file bucket
     hash(signature) % m, then group each shard independently, since all
     anagrams share a signature and so land in the same shard. Cost is extra
     disk I/O and a merge pass.
  4. The caller wants groups sorted by size, largest first.
     Order groupsByKey.Values by Count descending before materializing the
     result. That adds a sort over the number of groups and makes output order
     deterministic, which the current Dictionary.Values order is not guaranteed
     to be.
TRIGGER
  You must group items where "same group" is decided by some
  rearrangement-invariant property - reach for a canonical key plus a dictionary
  instead of pairwise comparison.
C# NOTE
  TryGetValue with the out parameter does one hash lookup and, on a miss, one
  insert; the common ContainsKey-then-indexer-then-assign version hashes the
  same signature three times. Holding the returned group reference and calling
  group.Add also avoids a fourth lookup.
COMPLEXITY
  Time  : O(n * k)
  Space : O(n)
================================================================================
*/
