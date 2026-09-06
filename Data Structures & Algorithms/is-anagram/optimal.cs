// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
// -  fixed 26-slot array, signed balance counting
// -  [array-frequency-balance]
// -  ranks above suboptimal.cs (O(n) time / O(n) space)
// -
// -  Reference solution - not one you solved yourself
// -
// -  single pass increments/decrements a fixed-size 26-element array keyed
// -  by char-'a', then checks all slots are zero, giving constant space
// -  since the alphabet is bounded
// --------------------------------------------------------------------------

public class Solution
{
    public bool IsAnagram(string original, string candidate)
    {
        if (original.Length != candidate.Length)
            return false;

        // One slot per lowercase letter. Index 0 = 'a', index 25 = 'z'.
        int[] letterBalance = new int[26];

        // Single pass over both strings at once: credit for s, debit for t.
        for (int i = 0; i < original.Length; i++)
        {
            letterBalance[original[i] - 'a']++;
            letterBalance[candidate[i] - 'a']--;
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
 PATTERN : Fixed counting array - balances must cancel to zero
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  Anagram means "same multiset of characters, order irrelevant." The alphabet
  here is bounded and contiguous, so original[i] - 'a' is a perfect hash: every
  letter gets its own dense slot in letterBalance, no hashing, no collisions, no
  resizing. The array IS the multiset. Once you see "bounded alphabet + multiset
  equality," reach for the count array before you reach for a Dictionary.
BRUTE FORCE
  Two obvious alternatives, both worse here:
  1. Sort both strings and compare - correct, but n log n and it allocates two
  char arrays.
  2. For each character of original, scan candidate for a match and cross it off
  - quadratic.
  Counting collapses multiset equality into a single tally pass, and the tally
  is what the final zero-check reads.
INVARIANT
  After iteration i of the loop, for every letter c:
    letterBalance[c - 'a'] == (occurrences of c in original[0..i]) -
    (occurrences of c in candidate[0..i])
  The increment credits original, the decrement debits candidate. When the loop
  ends, that difference is over the whole strings, so "every slot is 0" is
  literally the statement "the two letter multisets are identical" - which is
  the definition of anagram. Nothing else needs proving.

  A corollary worth having ready: since the lengths are equal, the balances sum
  to zero. So a mismatch can never be a single nonzero slot - any surplus letter
  is paid for by a deficit somewhere else.
THE LENGTH GUARD IS LOAD-BEARING
  The early return on original.Length != candidate.Length is not a fast-path
  optimization; delete it and the method breaks two different ways.
  Candidate longer: the loop is bounded by original.Length, so candidate's tail
  is never read. "ab" vs "abab" would leave every slot at zero and return true.
  Candidate shorter: candidate[i] runs off the end and throws
  IndexOutOfRangeException.
  The single fused loop over both strings is only legal because the guard
  already established the lengths match.
NO EARLY EXIT INSIDE THE LOOP
  Tempting instinct: bail out the moment a slot goes negative or nonzero. It is
  wrong. Take original = "ba", candidate = "ab". After i = 0, letterBalance['b']
  is +1 and letterBalance['a'] is -1 - two nonzero slots - yet these are
  anagrams; i = 1 cancels both. A slot can wander away from zero and come back
  at any point. Only the state after the final iteration carries information,
  which is why the zero-check is a separate foreach over all 26 slots.
WATCH OUT
  The subtraction original[i] - 'a' is an unchecked contract that the input is
  lowercase a-z. It has no validation behind it:
  - 'A' is 65 and 'a' is 97, so an uppercase input indexes at -32 and throws
  IndexOutOfRangeException.
  - Any non-ASCII char (an accented letter, a digit, a space) indexes far past
  25 and throws too.
  This is a hard crash, not a wrong answer, so it will surface - but say the
  assumption out loud in an interview rather than letting them find it. To
  generalize: swap the array for Dictionary<char, int> keyed on the raw char
  (same increment/decrement/zero-check structure), and if full Unicode is in
  play, iterate runes rather than chars so surrogate pairs are not split.
TRIGGER
  Bounded alphabet plus a permutation or multiset-equality question. The same
  letterBalance idea is the engine underneath the sliding-window family - find
  all anagrams in a string, permutation in string - where you add the entering
  char, remove the leaving char, and keep a running count of how many slots are
  nonzero so the window check is constant work instead of a 26-slot rescan.
  Learn the balance array here and those problems become bookkeeping.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
