// --------------------------------------------------------------------------
//  Reference solution - from NeetCode / other resource (submission-1)
//  Not one you solved yourself.
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
 PATTERN : Hashing - Fixed-Alphabet Frequency Array
 SOURCE  : NeetCode / other resource (submission-1)
 STATUS  : Optimal
================================================================================

WHY THIS PATTERN
  When the key space is small, dense and known ahead of time (26 letters),
  an ARRAY IS A PERFECT HASH TABLE. c - 'a' is the hash function, it never
  collides, and it costs one subtraction instead of a hash + bucket probe.

BRUTE FORCE (and why it fails)
  Sorting both strings: O(n log n). Counting is O(n). The dictionary version
  in suboptimal.cs is also O(n) but pays hashing on every single access.

THE TRICK THAT MAKES THIS ONE PASS
  Instead of counting s, then counting t, then comparing two arrays, keep a
  single BALANCE array: += for s, -= for t. Anagrams cancel to all zeroes.
  This halves the passes and halves the memory versus two count arrays.

INVARIANT
  letterBalance[k] = (occurrences of letter k in s[0..i]) minus
                     (occurrences of letter k in t[0..i]).
  For true anagrams this reaches 0 for every k at i = n-1.

ALGORITHM (NeetCode: "Hash Table (Using Array)")
  1. Length mismatch -> false.
  2. One loop, one index i, touching s[i] and t[i] together.
  3. Increment the s letter's slot, decrement the t letter's slot.
  4. Scan the 26 slots; any non-zero -> false.

COMPLEXITY
  Time  : O(n) - one pass of length n, plus a fixed 26-slot scan.
  Space : O(1) - exactly 26 ints, independent of input size.
          (Formally O(k) for alphabet size k, but k is a constant here.)

TRIGGER
  "Anagram / permutation / same multiset of characters" AND the problem
  constrains input to lowercase English letters. That constraint sentence in
  the problem statement is the signal to reach for int[26] over Dictionary.

C# NOTES
  - `s[i] - 'a'` is char arithmetic promoted to int. No conversion needed.
  - `new int[26]` is zero-initialised by the CLR - no Array.Fill required.
  - stackalloc int[26] avoids the heap allocation entirely if this ran in a
    hot loop. Overkill for a single call, but it is the .NET-idiomatic move.

WATCH OUT
  - THIS CRASHES on any character outside a-z: an uppercase 'A' gives index
    -32 and throws IndexOutOfRangeException. Always confirm the constraint
    before using this shape. If Unicode is possible, use suboptimal.cs.
  - The length check is not an optimisation, it is REQUIRED for correctness:
    the single loop indexes both strings with the same i.
================================================================================
*/
