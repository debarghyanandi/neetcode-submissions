// --------------------------------------------------------------------------
// -  optimal-variant.cs    O(n + m) time / O(1) space
// --------------------------------------------------------------------------

public class Solution
{
    public bool CheckInclusion(string s1, string s2)
    {
        if (s1.Length > s2.Length)
            return false;

        int[] need = new int[26];
        int[] window = new int[26];
        int matches = 0; // how many of the 26 letters currently have need[c] == window[c]

        for (int i = 0; i < s1.Length; i++)
        {
            need[s1[i] - 'a']++;
        }

        // A letter with need[c] == 0 already "matches" window[c] == 0 before anything is added.
        for (int c = 0; c < 26; c++)
        {
            if (need[c] == window[c])
                matches++;
        }

        void Add(char ch)
        {
            int c = ch - 'a';
            if (window[c] == need[c])
                matches--;   // about to break equality (if it was equal)
            window[c]++;
            if (window[c] == need[c])
                matches++;   // may have restored equality
        }

        void Remove(char ch)
        {
            int c = ch - 'a';
            if (window[c] == need[c])
                matches--;
            window[c]--;
            if (window[c] == need[c])
                matches++;
        }

        for (int i = 0; i < s1.Length; i++)
            Add(s2[i]);
        if (matches == 26)
            return true;

        int left = 0;
        for (int right = s1.Length; right < s2.Length; right++)
        {
            Add(s2[right]);
            Remove(s2[left]);
            left++;
            if (matches == 26)
                return true;
        }

        return false;
    }
}

/*
================================================================================
 PROBLEM : Given strings s1 and s2 of lowercase letters, return true if s2
           contains a permutation of s1 as a contiguous substring, else false.
           The substring must have the same letters with the same counts as
           s1. Example: s1="ab", s2="eidbaooo" -> true (substring "ba").
 PATTERN : Sliding Window (fixed size) + match counter
================================================================================
IDEA
  Keep a window of length s1.Length on s2 with letter counts in window.
  Keep need for s1. matches counts how many of the 26 letters have
  need[c] == window[c]. Add and Remove update matches only for the one
  letter that changed, so each step costs O(1). No step compares all 26
  counts. When matches == 26, the window is an anagram of s1, so it is
  correct.
EXAMPLE
  s1="ab", s2="eidbaooo". Start: matches=24 (every letter except a, b).
  window "ei": 22 -> "id": 22 -> "db": 24 -> "ba": 26
  matches == 26 at left=3, so return true.
COMPLEXITY
  Time  O(n + m)  build need over s1, then each s2 char is added once and
                  removed once
  Space O(1)      two fixed arrays of 26 ints, whatever the input size
WATCH OUT
  - Letters with need 0 must start as matches. Starting matches at 0
    means it never reaches 26.
  - Check matches after the first s1.Length Adds, before the loop.
    Without this, s2="ba" with s1="ab" returns false.
  - In Add/Remove, test equality both before and after the change.
    A single check miscounts when a count moves away from need.
  - ch - 'a' assumes lowercase a-z only. An uppercase letter or a space
    gives a negative index and crashes.
================================================================================
*/
