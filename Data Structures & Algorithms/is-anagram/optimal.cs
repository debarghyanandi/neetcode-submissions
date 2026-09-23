// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
// -  Fixed-size array character balance   [array-balance-fixed]
// -  ties with optimal-variant.cs on O(n) time / O(1) space
// -
// -  Reference solution - not one you solved yourself
// -
// -  Fixed 26-slot array bounds space to constant; single pass processes
// -  both strings in O(n) time.
// --------------------------------------------------------------------------

public class Solution
{
    public bool IsAnagram(string s, string t)
    {
        if (s.Length != t.Length)
            return false;

        // One slot per lowercase letter. Index 0 = 'a', index 25 = 'z'.
        int[] letterBalance = new int[26];

        // Single pass over both strings at once: credit for s, debit for t.
        for (int i = 0; i < s.Length; i++)
        {
            letterBalance[s[i] - 'a']++;
            letterBalance[t[i] - 'a']--;
        }

        // Anagrams cancel out exactly, so every slot must be back to zero.
        foreach (int balance in letterBalance)
        {
            if (balance != 0)
                return false;
        }

        return true;
    }
}

/*
================================================================================
 PATTERN : Fixed-size counting array - one pass, credit and debit
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Optimal
================================================================================
VARIABLES
  letterBalance  letterBalance[c] = count of c in s minus count of c in t
  balance        one slot's value during the final zero check
WHY THIS PATTERN
  An anagram means the two strings hold the same letters with the same
  multiplicities, and order does not matter. That turns the problem into
  comparing two multisets, and since the alphabet is a known fixed set of 26
  lowercase letters, a plain int[26] is enough to hold both counts. Adding for s
  and subtracting for t in the same loop collapses two count tables into one:
  letterBalance ends at all zeros exactly when the multisets match.
BRUTE FORCE
  The first thing most people write is sorting: turn both strings into char
  arrays, Array.Sort each, and compare. That is correct and short but costs O(n
  log n) time plus O(n) extra space for the copies. Counting wins because each
  character needs only a single increment, so no ordering work is needed at all.
INVARIANT
  After processing index i, letterBalance[c] equals the number of times c
  appeared in s[0..i] minus the number of times it appeared in t[0..i]. The
  early length check guarantees both strings feed the loop the same number of
  characters, so at the end each slot is the true full-string difference for
  that letter. A difference of zero in every slot means every letter appears
  equally often in both, which is the definition of an anagram; any nonzero slot
  is a concrete letter that proves they differ.
WHY THE LENGTH CHECK IS NOT OPTIONAL
  Without `s.Length != t.Length` the loop cannot even run over both strings
  safely, and more subtly the all-zero test alone would not be a proof. If you
  only scanned s fully and t partially, missing letters would still leave zeros
  elsewhere and you could wrongly return true. The length guard is what lets a
  single shared loop counter stand in for two separate passes.
WATCH OUT
  The subtraction `s[i] - 'a'` silently assumes every character is in 'a'..'z'.
  An uppercase letter, a space, a digit or any non-ASCII character produces an
  index outside 0..25 and throws IndexOutOfRangeException, or worse indexes into
  a valid but wrong slot for characters just above 'z'. The comment says "One
  slot per lowercase letter" but nothing in the code enforces it - the input
  contract is assumed, not checked. Also note a null s or t throws
  NullReferenceException on `.Length` before any of this.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. What if the input can be any Unicode string, including characters outside
  the Basic Multilingual Plane?
     Replace the array with a Dictionary<int, int> keyed on code points, and
     iterate with StringInfo or by pairing surrogate chars so a single emoji
     counts as one unit; you gain generality and lose the O(1) space bound,
     since the dictionary grows with the number of distinct characters.
  2. You are given many strings and must group all anagrams together. Does this
  method still apply?
     Not directly, because pairwise comparison is quadratic in the number of
     strings. Build a canonical key per string instead - either the sorted
     characters or the 26 counts joined into a string - and bucket by that key
     in a Dictionary<string, List<string>>.
  3. Can you exit earlier than scanning all 26 slots at the end?
     Track a running count of how many slots are currently nonzero, adjusting it
     inside the main loop when a slot enters or leaves zero; then the answer is
     just that counter being zero, which removes the second loop at the cost of
     two extra comparisons per character.
  4. How would you handle the case-insensitive version?
     Normalize before indexing, for example with char.ToLowerInvariant on each
     character, and keep the same 26 slots; this adds a per-character call but
     no extra memory.
TRIGGER
  Reach for a fixed counting array whenever the question compares two
  collections by content and not by order, and the symbol alphabet is small and
  known in advance.
C# NOTE
  `new int[26]` is zero-initialized by the runtime, so no manual clearing loop
  is needed before counting. Indexing a string with `s[i]` returns a char and
  the subtraction `s[i] - 'a'` promotes both to int automatically, which is why
  the result can legally be used as an array index without an explicit cast.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
