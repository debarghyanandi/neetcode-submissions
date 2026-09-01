// --------------------------------------------------------------------------
// -  optimal.cs            O(n * k) time / O(n) space
// -  hash by 26-letter frequency-count signature
// -  [frequency-signature-hashmap]
// -  ranks above suboptimal.cs (O(n * k log k) time / O(n * k) space)
// -
// -  Reference solution - not one you solved yourself
// -
// -  counting letters is O(k) per word with no sort, and the serialized
// -  signature key has a fixed number of fields (26) independent of k, so
// -  key storage is O(n) rather than scaling with word length.
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

            foreach (char letter in word)
            {
                letterCounts[letter - 'a']++;
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
 PATTERN : Hash map on a canonical key - 26-slot letter count
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  Grouping means partitioning into equivalence classes, and the cheap way to do
  that is never to compare pairs of words - it is to map each word to a
  canonical representative of its class and bucket on that. Here the class is
  "same multiset of letters" and the representative is letterCounts, a 26-slot
  tally serialized into signature. Each word is touched once and dropped into a
  dictionary; strs is never scanned against itself.
CORRECTNESS
  Two directions, both needed.
  1. No group is split. letterCounts is built by ++ over the characters of word,
  and increments commute, so the array depends only on which letters appear and
  how often - not on their order. Anagrams therefore produce identical arrays
  and identical signature strings, so they always land in the same bucket.
  2. No two classes merge. Non-anagrams differ in at least one of the 26 counts.
  The counts are appended in fixed index order 0..25, each followed by a
  delimiter, so signature is an injective encoding of the int[26]: different
  arrays give different strings.
  Together the buckets are exactly the anagram classes.
THE SEPARATOR TRAP
  The Append(',') is load-bearing for the injectivity argument above. Without
  it, counts of 1 then 11 and 11 then 1 both flatten to "111" and two unrelated
  groups silently fuse. The comma makes each field self-delimiting so
  multi-digit counts cannot bleed into their neighbor. Note how quietly this
  fails: no count reaches 10 until a word has ten copies of one letter, so the
  broken version compiles and passes every small hand-written test.
WHY NOT USE INT[] AS THE KEY DIRECTLY
  int[] inherits reference equality and the default object hash, so two arrays
  holding identical counts are distinct dictionary keys and every word would get
  a bucket of its own. That is the reason for flattening to a string at all. The
  alternatives to the StringBuilder: pass a custom IEqualityComparer<int[]> to
  the Dictionary, or use a sorted-characters key such as new
  string(word.OrderBy(c => c).ToArray()), which is a canonical form too but pays
  a sort per word instead of a linear count.
WHY TRYGETVALUE AND NOT CONTAINSKEY
  TryGetValue does one hash lookup that both answers "is it there" and hands
  back the list. Just as important, group is a reference to the very
  List<string> stored in the dictionary - on the miss branch the new list is
  inserted at groupsByKey[signature] first, so the later group.Add(word) mutates
  the stored object and no write-back is required. ContainsKey followed by the
  indexer would hash signature twice and read the same list twice.
WATCH OUT
  letter - 'a' hard-codes the lowercase a-z alphabet. An uppercase 'A' indexes
  at -32 and throws IndexOutOfRangeException; digits and spaces are equally
  unsafe. Confirm that constraint before writing the fixed array - if the input
  can be arbitrary characters, switch to a Dictionary<char,int> emitted in
  sorted key order, or fall back to sorting the word.
  Also, groupsByKey.Values.ToList() yields groups in the dictionary's own
  enumeration order and each group in first-seen order within strs. The problem
  accepts any order, so this is fine, but do not build later code on that
  ordering.
INTERVIEW FOLLOW-UPS
  Expect "what if the alphabet is Unicode" (the 26-slot array stops being
  viable; a per-word map of only the characters present, canonicalized by
  sorting its keys, keeps the counting idea) and "is the count key always better
  than the sorted-string key". The honest answer from this code: the
  key-building loop runs all 26 slots for every word regardless of length, so a
  one-letter word still emits a 26-field signature, while sorting a one-letter
  word does almost nothing. Counting pulls ahead as words get long; the fixed 26
  fields are the price of admission.
COMPLEXITY
  Time  : O(n * k)
  Space : O(n)
================================================================================
*/
