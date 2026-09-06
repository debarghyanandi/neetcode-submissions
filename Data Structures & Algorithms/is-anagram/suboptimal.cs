// --------------------------------------------------------------------------
// -  suboptimal.cs         O(n) time / O(n) space
// -  hash map frequency count, build then drain
// -  [hashmap-frequency-consume]
// -  ranks below optimal.cs (O(n) time / O(1) space)
// -
// -  Reference solution - not one you solved yourself
// -
// -  builds a Dictionary<char,int> from the first string (up to O(n)
// -  distinct keys for an unbounded alphabet), then decrements per
// -  character of the second, failing on missing or zero counts
// --------------------------------------------------------------------------

public class Solution
{
    public bool IsAnagram(string first, string second)
    {
        // Different lengths can never be anagrams. O(1) rejection.
        if (first.Length != second.Length)
            return false;

        // Build the frequency map of the first string.
        var charFrequency = new Dictionary<char, int>();

        foreach (char letter in first)
        {
            charFrequency.TryGetValue(letter, out int currentCount);
            charFrequency[letter] = currentCount + 1;
        }

        // Walk the second string and spend one unit of each character's budget.
        foreach (char letter in second)
        {
            if (!charFrequency.TryGetValue(letter, out int remaining))
                return false;              // character not in first at all

            if (remaining == 0)
                return false;              // second uses this character more often than first

            charFrequency[letter] = remaining - 1;
        }

        // Lengths matched and every character in t was covered by s's budget,
        // so no character can be left over. No second sweep needed.
        return true;
    }
}

/*
================================================================================
 PATTERN : Hash map frequency budget - count first, spend on second
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Suboptimal
================================================================================
INVARIANT
  Read charFrequency[c] as the budget of c that second has not yet claimed. The
  first loop sets budget(c) to the count of c in first, so the total budget
  across all entries is exactly first.Length. The second loop spends one unit
  per character of second and refuses to spend from an absent or zero budget, so
  it either bails early or spends exactly second.Length units, each from a
  genuinely available count. Because first.Length == second.Length was already
  established, total spent equals total budget, which forces every entry to sit
  at exactly 0 when the loop ends. That is why the closing comment holds: there
  is no leftover positive count to find, so no verification sweep over the
  dictionary is needed.
WHY THE LENGTH CHECK IS LOAD-BEARING
  Delete the first two lines and the method becomes wrong, not merely slower.
  Take first = "aab", second = "ab": every character of second finds a positive
  budget, the loop runs to completion, and true is returned while one 'a' is
  still unspent. The guard is what upgrades "second is covered by first" into
  "second equals first as a multiset", by pinning the two totals together. It is
  labelled as an O(1) rejection, but its real job is correctness. Say that out
  loud if an interviewer asks why the code never re-scans the map.
THE TWO REJECTIONS
  There are two distinct false exits and both are required because entries are
  never removed. TryGetValue returning false means the character never occurred
  in first at all. remaining == 0 means it did occur, but second has now used it
  more times than first had. If instead you called charFrequency.Remove(letter)
  once its budget hits its last unit, the zero test would become dead code and
  the missing-key branch would cover both cases - one fewer conditional, at the
  cost of a removal per exhausted key. Note also that the build loop relies on
  out int currentCount defaulting to 0 on a miss, which is what lets a single
  line serve as both insert and increment; each update in either loop is a probe
  plus an indexer store, i.e. the key is hashed twice.
WHY THIS LOSES
  Valid Anagram constrains both inputs to lowercase English letters, and this
  file ignores that constraint. A single int[26] indexed by letter - 'a' carries
  the same information in fixed space with no hashing and no dictionary
  allocation: increment while walking first, decrement while walking second, and
  the same length guard plus a went-negative check reproduces the budget
  argument word for word. That is the intended solution. The dictionary buys
  generality over arbitrary char values that the problem never asks for. The
  other common answer - sort both strings and compare - is strictly worse on
  time and only earns its place when you cannot afford any counting structure.
WATCH OUT
  foreach over a string yields char, that is UTF-16 code units, so this compares
  multisets of code units rather than of code points. Two strings assembled from
  different astral-plane characters that happen to reuse the same surrogate
  halves will be declared anagrams. Irrelevant under the lowercase-only
  constraint, but worth naming if pushed on Unicode; the honest fix is to
  enumerate runes and key the map on int. Empty inputs need no special case:
  lengths match, neither loop body executes, true is returned.
TRIGGER
  Reach for count-then-spend whenever the question is multiset equality - same
  characters in any order, permutation check, rearrangement. The decrement half
  is the reusable piece: freeze the budget map and slide a window over the
  second string and you have the answer to find-all-anagrams-in-a-string and
  permutation-in-string, where budget is handed back as characters leave the
  window instead of being rebuilt from scratch.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
