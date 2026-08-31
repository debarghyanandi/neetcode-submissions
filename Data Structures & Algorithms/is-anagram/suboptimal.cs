// --------------------------------------------------------------------------
// -  suboptimal.cs         O(n) time / O(n) space
// -  dictionary frequency map with budget consumption
// -  [hashmap-frequency-consume]
// -  ranks below optimal.cs (O(n) time / O(1) space)
// -
// -  Reference solution - not one you solved yourself
// -
// -  builds a Dictionary<char,int> counting s (up to O(n) distinct keys in
// -  the worst case for an unbounded alphabet), then decrements per
// -  character of t, failing on missing/zero counts.
// --------------------------------------------------------------------------

public class Solution
{
    public bool IsAnagram(string s, string t)
    {
        // Different lengths can never be anagrams. O(1) rejection.
        if (s.Length != t.Length)
            return false;

        // Build the frequency map of the first string.
        var charFrequency = new Dictionary<char, int>();

        foreach (char c in s)
        {
            charFrequency.TryGetValue(c, out int currentCount);
            charFrequency[c] = currentCount + 1;
        }

        // Walk the second string and spend one unit of each character's budget.
        foreach (char c in t)
        {
            if (!charFrequency.TryGetValue(c, out int remaining))
                return false;              // character not in s at all

            if (remaining == 0)
                return false;              // t uses this character more often than s

            charFrequency[c] = remaining - 1;
        }

        // Lengths matched and every character in t was covered by s's budget,
        // so no character can be left over. No second sweep needed.
        return true;
    }
}

/*
================================================================================
 PATTERN : Hash Map Frequency Budget - build then drain
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Suboptimal
================================================================================
WHY THIS PATTERN
  Anagram is multiset equality: the two strings are equal as bags of characters,
  order discarded. A Dictionary<char,int> is the direct encoding of a bag. The
  build loop over s turns s into counts; the loop over t spends those counts.
  Sorting both strings and comparing also decides multiset equality, but pays
  O(n log n) for an ordering nobody looks at. Counting is the cheaper route to
  the same fact.
THE INVARIANT
  At every point during the second loop, charFrequency[c] equals (occurrences of
  c in s) minus (occurrences of c consumed so far from t), and it is never
  negative. The two guards are exactly what maintain non-negativity: the
  TryGetValue miss rejects a character s never had, and the remaining == 0 test
  rejects spending a budget already drained to zero. Every surviving iteration
  decrements exactly one unit.
WHY NO SECOND SWEEP
  The closing comment is the load-bearing part of the argument, so make it
  explicit. Let n = s.Length. The build loop deposits exactly n units total
  across all keys. If the t loop runs to completion it withdraws exactly
  t.Length units, and the guard at the top proved t.Length == n. Withdrawing n
  units from a pool of n without any balance going negative forces every balance
  to zero, so no leftover scan is needed. Delete the length check and the
  argument collapses: s = "aab", t = "ab" drains cleanly and wrongly returns
  true.
WHY THIS LOSES
  When the alphabet is bounded - the usual lowercase-English constraint - an
  int[26] indexed by c - 'a' does the same job. Increment on s, decrement on t,
  early-exit the moment a slot would go negative, and the same
  n-units-from-n-units argument still closes it. That version is O(1) space
  instead of space proportional to the distinct characters of s, and each of the
  2n operations is a bounds-checked array index rather than a hash, bucket probe
  and possible resize. This file is the right answer only when the character
  domain is unbounded (full Unicode, arbitrary symbols), where a 26-slot array
  is not an option.
WATCH OUT
  1. TryGetValue(c, out int currentCount) writes 0 into currentCount on a miss,
  which is why the build loop needs no ContainsKey branch. That is the
  documented behavior of the out parameter, not an accident.
  2. Keys are never removed. A character fully spent stays in the map with value
  0, so the missing-key check and the remaining == 0 check are two different
  failures and you need both. Drop the zero check and "aab" vs "abb" is
  accepted.
  3. foreach over a string yields UTF-16 code units, not code points. Surrogate
  pairs are counted as two unrelated chars and combining-mark or normalization
  differences are invisible. Fine for the stated constraints; say so out loud
  before an interviewer asks.
THE FOLLOW-UP
  "Now the input is arbitrary Unicode" is the question this file already answers
  - point at it and note the int[26] version breaks. "Make it one pass": since
  the lengths are equal you can walk both strings in a single loop, incrementing
  for s[i] and decrementing for t[i], but you then lose the early exit reasoning
  and must scan the map at the end, because a temporarily negative balance may
  be repaid later. "Case-insensitive or ignore whitespace" is a normalization
  step before counting, not a change to the algorithm.
TRIGGER
  Reach for build-then-drain whenever the question is whether two sequences hold
  the same items in any order. The same budget map, kept over a moving window
  instead of a whole string, is what permutation-in-string and minimum-window
  need - recognizing that this is the fixed-window degenerate case of those is
  most of the transfer value here.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
