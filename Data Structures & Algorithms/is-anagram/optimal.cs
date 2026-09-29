// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
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
 PROBLEM : Given two strings s and t, return true if t is an anagram of s. An
           anagram uses exactly the same letters with the same counts, in any
           order. Example: s = "anagram", t = "nagaram" -> true; s = "rat", t
           = "car" -> false.
 PATTERN : Frequency Counting (fixed-size count array)
================================================================================
IDEA
  Two strings are anagrams exactly when every letter appears equally often.
  letterBalance has one slot per letter 'a'..'z'.
  In one loop, s[i] adds 1 to its slot and t[i] takes 1 away.
  If the strings are anagrams, all slots end at zero. Any other slot shows an
  extra letter on one side. So a single nonzero slot means false.
EXAMPLE
  s = "aab", t = "aba": i=0 a+1,a-1 -> all 0; i=1 a+1,b-1 -> a=1,b=-1;
    i=2 b+1,a-1 -> a=0,b=0. All slots are zero -> true.
  s = "ab", t = "aa": i=0 a+1,a-1 -> a=0; i=1 b+1,a-1 -> a=-1,b=1 -> false.
  Slots can be nonzero during the loop. Only the final values matter.
COMPLEXITY
  Time  O(n)  one pass over the n characters, then a scan of 26 fixed slots
  Space O(1)  letterBalance always has 26 ints, whatever n is
PATH TO OPTIMAL
  Sort both strings and compare - O(n log n) time - simple but slow.
  Dictionary of char counts - O(n) time, O(k) space - removes the sort.
  Array of 26 counts (this file) - O(n) / O(1) - no hashing, fixed memory.
  optimal-variant.cs reaches the same bounds in a different way.
KEYWORDS
  anagram, frequency count, hash map, character array, counting, strings
WATCH OUT
  - Any character outside 'a'..'z' breaks this code. 'A' - 'a' is -32, so
    letterBalance throws IndexOutOfRangeException. The same happens for
    spaces.
  - The length check is required. Without it, t[i] goes out of range or
    chars are skipped when the lengths differ.
  - Do not return early on a negative slot inside the loop. With s = "ab"
    and t = "ba", slot b is -1 at i=0 even though the answer is true.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. What if the input contains Unicode characters?
     -> Use a Dictionary<char,int> (or Rune) for the counts. Time stays O(n)
        and space becomes O(k) for k distinct characters. You pay for hashing.
  2. How do you group many words into anagram sets (Group Anagrams)?
     -> Key each word by its sorted form or by its 26-count signature. Put the
        words into a Dictionary of lists. Time is O(m * n) with the count key.
  3. t arrives as a stream you cannot store. What changes?
     -> First count s into letterBalance. Then decrement for each char of t as
        it arrives. Check all slots are zero at the end. Memory stays O(1).
  4. Why is one array enough instead of two?
     -> Anagrams need equal counts. Adding for s and subtracting for t leaves
        the difference, and a zero difference means equal counts.
TRIGGER
  When a problem asks whether two strings are the same letters rearranged,
  count each letter's frequency.
================================================================================
*/
