// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
// -  fixed-size array as perfect hash, balance counting
// -  [array-frequency-balance]
// -  ranks above suboptimal.cs (O(n) time / O(n) space)
// -
// -  Reference solution - not one you solved yourself
// -
// -  single pass increments/decrements a 26-slot array keyed by char-'a',
// -  then checks all slots are zero, giving constant extra space since the
// -  alphabet is fixed.
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
 PATTERN : Fixed counting array - one signed balance pass
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  Anagram is multiset equality over characters. The alphabet here is fixed and
  known (26 lowercase letters), so the whole multiset fits in an int[26]
  addressed by c - 'a', and "compare two multisets" collapses to "compare 26
  integers". The signed trick folds both counts into one table: s credits (++),
  t debits (--). That is why there is no second array and no table-vs-table
  comparison at the end - only a scan for a nonzero slot.
INVARIANT
  After the loop body runs for index i, letterBalance[c - 'a'] equals
  (occurrences of c in s[0..i]) minus (occurrences of c in t[0..i]). Slots are
  fully independent: a surplus in one letter can never cancel a deficit in
  another, because they live at different indices. Combined with the
  equal-length precondition, "every slot is zero at the end" is exactly "every
  letter appears the same number of times in both" - which is the definition of
  anagram. That is the whole correctness argument.
THE LENGTH GUARD IS LOAD-BEARING
  The early return on s.Length != t.Length is not a shortcut, it is a
  precondition for the loop. The single pass indexes t[i] while i is bounded
  only by s.Length, so with s = "ab" and t = "a" you get an
  IndexOutOfRangeException, not a wrong answer. Contrast with the two-loop shape
  (count all of s, then decount all of t): there, unequal lengths would
  necessarily leave some slot nonzero and the guard would be a pure
  optimization. This version trades that safety for the fused pass, so the guard
  has to stay.
WATCH OUT
  1. c - 'a' silently assumes every character is in 'a'..'z'. An uppercase 'A'
  produces index -32 and a space produces -65, so bad input throws rather than
  lying - loud, but still a crash. If the interviewer widens the input, this is
  the first thing to fix.
  2. Do not "optimize" by returning false the moment a slot goes negative inside
  the loop. Negative only means t has led on that letter so far, and s can still
  catch up. Concretely, s = "ba" and t = "ab": at i = 0 letterBalance['a' - 'a']
  is already -1, yet the strings are anagrams. Only the final values carry
  meaning.
THE OBVIOUS FOLLOW-UP
  "What if the input is Unicode, or the alphabet is unbounded?" Swap int[26] for
  Dictionary<char, int> with the same +1 / -1 balance, then verify every value
  is zero - or maintain a running count of nonzero entries so the final check is
  O(1). Space then scales with the number of distinct characters instead of a
  flat 26. Mention too that iterating char by char breaks on surrogate pairs and
  combining marks; a truly correct Unicode version compares text elements, not
  chars. The alphabet-free alternative is sorting both strings and comparing,
  which needs no counting table but pays n log n.
RECALL TRIGGER
  Reach for a fixed counting array whenever the question is "same contents,
  order irrelevant" over a small known symbol set: anagram grouping,
  permutation-in-a-string sliding windows, character frequency checks. The tell
  is a bounded alphabet plus an answer that depends only on counts. The signed
  single-array variant specifically applies when you are comparing exactly two
  sequences of equal length.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
