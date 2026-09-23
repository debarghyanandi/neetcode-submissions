// --------------------------------------------------------------------------
// -  optimal-variant.cs    O(n) time / O(1) space
// -  Hash map frequency counting   [hash-map-frequency]
// -  ties with optimal.cs on O(n) time / O(1) space
// -
// -  Reference solution - not one you solved yourself (was suboptimal.cs)
// -
// -  Dictionary space is O(1) when bounded by fixed alphabet; two
// -  sequential passes over input strings give O(n) time.
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
                return false;              // character not in first at all

            if (remaining == 0)
                return false;              // second uses this character more often than first

            charFrequency[c] = remaining - 1;
        }

        // Lengths matched and every character in t was covered by s's budget,
        // so no character can be left over. No second sweep needed.
        return true;
    }
}

/*
================================================================================
 PATTERN : Hash Map Counting - build budget, then spend it
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Optimal variant - ties the best complexity by another route
================================================================================
VARIABLES
  charFrequency   charFrequency[c] = how many of c in s are still unspent
  currentCount    count of c seen so far while building the map (0 if absent)
  remaining       budget left for c when t asks for one more
WHY THIS PATTERN
  An anagram means the two strings hold the same characters with the same
  counts, and order does not matter. Anything that ignores order but cares about
  multiplicity is a counting problem, so a map from character to count is the
  natural shape. charFrequency records what s offers, and the second loop checks
  that t asks for exactly that and no more.
BRUTE FORCE
  The first thing most people write is: sort both strings into char arrays and
  compare them element by element. That is correct and short, but it costs O(n
  log n) time for the sort and allocates two arrays. Counting reaches the same
  answer in one pass over each string, so the sort is wasted work.
INVARIANT
  After the first loop, charFrequency[c] equals the exact number of times c
  appears in s. During the second loop, charFrequency[c] is the number of c in s
  not yet matched by a character in t, and it never goes below zero because the
  code returns false the moment remaining is 0. If the loop finishes, every
  character of t was matched to a distinct character of s, and since s.Length ==
  t.Length was already checked, no character of s can be left unmatched - so the
  multisets are equal.
WHY NO SECOND SWEEP
  A common version of this solution loops over the map at the end to confirm
  every count fell to zero. That check is unnecessary here. The length guard
  means t consumes exactly s.Length units of budget, and the total budget is
  exactly s.Length, so if no consume ever failed then every unit was spent.
WATCH OUT
  The remaining == 0 test is doing real work: without it, TryGetValue would
  still succeed for a character that has run out and the count would go
  negative, and a string like "aab" vs "aaa" would wrongly pass. Null s or t
  throws a NullReferenceException on the first line, not a false. foreach over a
  string walks UTF-16 code units, not characters, so a character outside the
  basic range is counted as two halves; two different strings made of rearranged
  surrogate halves would be reported as anagrams. The O(1) space claim also
  rests on the alphabet being bounded - over full Unicode input the dictionary
  grows with the number of distinct code units.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. The input is guaranteed lowercase a-z. How does this change?
     Replace the dictionary with int[26] indexed by c - 'a'. Same two loops, but
     array indexing instead of hashing, and a fixed 104-byte allocation with no
     hash collisions.
  2. What if the strings are huge and stored on disk, read once, and cannot be
  held in memory?
     The first pass still works, since you only keep counts, not the text. But
     the length check must be tracked as a running count instead of s.Length,
     and you cannot bail early on mismatch without reading both streams.
  3. Group a list of words into anagram groups instead of comparing two.
     Compare-by-pairs is quadratic. Instead map each word to a canonical key -
     the sorted word, or a 26-slot count tuple - and bucket words by that key in
     a Dictionary of key to List of string.
TRIGGER
  The problem asks whether two collections match while explicitly saying order
  does not matter but repeats do.
C# NOTE
  TryGetValue with an out parameter is the right idiom here: it avoids the
  ContainsKey-then-index pattern that hashes the key twice. The write back,
  charFrequency[c] = currentCount + 1, is still a second lookup;
  CollectionsMarshal.GetValueRefOrAddDefault would give a ref to the slot and
  update it in place with one hash.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
