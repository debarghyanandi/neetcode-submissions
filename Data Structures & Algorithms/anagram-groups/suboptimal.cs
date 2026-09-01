// --------------------------------------------------------------------------
// -  suboptimal.cs         O(n * k log k) time / O(n * k) space
// -  hash by sorted-character canonical key   [sorted-key-hashmap]
// -  ranks below optimal.cs (O(n * k) time / O(n) space)
// -
// -  Reference solution - not one you solved yourself
// -
// -  each word is sorted (O(k log k)) to form the dictionary key, and the
// -  sorted key itself is length O(k), so total auxiliary key storage
// -  scales with n*k on top of the n*k log k sorting cost.
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
 PATTERN : Canonical-form hash map - sort each word into its group key
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Suboptimal
================================================================================
WHY THIS PATTERN
  Two words are anagrams exactly when they are permutations of each other. "Is a
  permutation of" is an equivalence relation, so the only real work is picking a
  canonical representative for each class and bucketing by it. Sorting the
  characters is the most obvious representative: every member of a class sorts
  to the identical string, and no member of a different class can. Once you
  frame the problem as "choose a class representative, then group", the
  dictionary falls out.
CORRECTNESS ARGUMENT
  sortedKey is a function of the multiset of characters in word, and of nothing
  else.

  Soundness: if two words land in the same groupsByKey bucket their sorted forms
  are equal, so they hold the same characters with the same multiplicities -
  they are anagrams.

  Completeness: if two words are anagrams they have the same character multiset,
  so Array.Sort produces the same char[] and the same string, so they hit the
  same bucket.

  Every input word is added to exactly one group, so the groups partition strs
  with nothing duplicated and nothing dropped.
THE REFERENCE-ALIASING STEP
  group is inserted into groupsByKey while it is still empty, and only then does
  group.Add(word) run. This is correct because List<string> is a reference type
  - the dictionary holds the same object the local points at, so every later Add
  through group is visible through the dictionary. If group were a struct, this
  would silently store an empty copy.

  TryGetValue is doing double duty: the lookup and the "does this bucket exist"
  test are one probe, not the two probes that ContainsKey followed by an indexer
  would take.
WHY THIS LOSES
  The sort is pure overhead. The input is constrained to lowercase English
  letters, so the character multiset - the only thing the key needs to encode -
  is just 26 counts. Build the key from an int[26] tally (a 26-char string with
  each count at its letter's position, or the counts joined by a separator) and
  you get the same canonical form in one pass over the word instead of a
  comparison sort, dropping the log k factor per word at the same asymptotic
  space.

  The sorted key is easier to write and to explain, which is why it is the first
  version people reach for. It is not the version to defend when the interviewer
  asks whether you can do better.
WATCH OUT
  1. Canonicalization runs over UTF-16 code units. Array.Sort on the char[]
  would split a surrogate pair and scramble combining sequences, so this key is
  only a valid canonical form under the lowercase-ASCII constraint. The
  counting-array alternative carries the same restriction, just more visibly.

  2. Output order is unspecified and this returns groupsByKey.Values.ToList(),
  so group order follows dictionary enumeration order, not input order. Do not
  write a test asserting a particular ordering.

  3. ToList() on Values copies the outer collection only - the inner
  List<string> objects stay shared with the dictionary. Harmless here since
  groupsByKey dies at return, but it bites when this shape gets reused.

  4. group.Add takes word, not sortedKey. Easy to fumble under time pressure,
  and the result would be groups of identical sorted strings.
TRIGGER
  Reach for canonical-form bucketing whenever the question groups items by an
  equivalence relation you can compute a representative for: anagrams, shifted
  strings, isomorphic strings, points sharing a slope from an origin. The entire
  exercise is picking a representative that is cheap to compute and
  collision-free - after that, the grouping loop is always this same
  TryGetValue-or-create shape.
COMPLEXITY
  Time  : O(n * k log k)
  Space : O(n * k)
================================================================================
*/
