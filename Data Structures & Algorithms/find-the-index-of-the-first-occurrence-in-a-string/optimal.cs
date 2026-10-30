// --------------------------------------------------------------------------
// -  optimal.cs            O(n + m) time / O(m) space
// --------------------------------------------------------------------------

public class Solution
{
    public int StrStr(string haystack, string needle)
    {
        if (needle == "")
            return 0;

        int[] lps = new int[needle.Length];
        int prevLPS = 0, i = 1;

        while (i < needle.Length)
        {
            if (needle[i] == needle[prevLPS])
            {
                lps[i] = prevLPS + 1;
                prevLPS++;
                i++;
            }

            else if (prevLPS == 0)
            {
                lps[i] = 0;
                i++;
            }
            else
            {
                prevLPS = lps[prevLPS - 1];
            }
        }

        i = 0;
        int j = 0;

        while (i < haystack.Length)
        {
            if (haystack[i] == needle[j])
            {
                i++;
                j++;
            }
            else
            {
                if (j == 0)
                    i++;

                else
                    j = lps[j - 1];

            }

            if (j == needle.Length)
            {
                return i - needle.Length;
            }
        }
        return -1;
    }
}

/*
================================================================================
 PROBLEM : Given two strings haystack and needle, return the index where
           needle first appears in haystack, or -1 if it never appears. An
           empty needle returns 0. Example: haystack = "sadbutsad", needle =
           "sad" -> 0.
 PATTERN : String Matching - KMP (prefix function / LPS array)
================================================================================
IDEA
  First build lps: lps[k] is the length of the longest proper prefix of
  needle[0..k] that is also a suffix of it. Then scan haystack with i and
  needle with j. On a match, both move. On a mismatch with j > 0, set
  j = lps[j-1] and keep i still. It is correct because those lps[j-1]
  chars are already known to match, so no possible start is skipped.
EXAMPLE
  needle = "aab": lps = [0,1,0] (at i=2, b!=a, prevLPS falls 1 -> 0)
  haystack = "aaab": i=0,1 match -> j=2; h[2]='a' != 'b' -> j=lps[1]=1
  h[2]='a' == needle[1] -> i=3,j=2; h[3]='b' == 'b' -> i=4,j=3
  j == 3 -> return 4 - 3 = 1 (i never moved backward)
COMPLEXITY
  Time  O(n + m)  i never goes back; j can only fall back as often as it rose
  Space O(m)      the lps array, one int per needle character
PATH TO OPTIMAL
  Brute force: try every start, compare char by char - O(n*m) - simple.
  KMP (this file) - O(n + m) - after a mismatch it reuses the matched
  prefix via lps, so haystack chars are never compared from scratch again.
  No sibling file: only optimal.cs is in this folder.
KEYWORDS
  KMP, LPS array, prefix function, string matching, strStr, rolling hash
WATCH OUT
  - On a mismatch with j > 0, do NOT do i++. Only j falls back. Moving i
    here can skip a real match.
  - Falling back to j = 0 instead of lps[j-1] breaks "aaab" / "aab".
  - In the lps loop, the else branch sets prevLPS = lps[prevLPS-1] but does
    not touch i. It must retry the same i until it matches or hits 0.
  - needle == null throws (only "" is checked). That is fine for LeetCode.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Return all start indices, overlaps included?
     -> When j == needle.Length, record i - needle.Length and set j =
        lps[j-1]. Then keep scanning. Still O(n + m) time.
  2. Can you do it with O(1) extra space?
     -> Rabin-Karp with a rolling hash over a window of length m. Expected O(n
        + m), but hash collisions need a char check, so worst case O(n*m).
  3. The haystack arrives as a stream?
     -> KMP fits well. i never moves back, so each char is read once. Keep
        only lps and j: O(m) memory.
  4. Why is the search linear if j can fall back many times?
     -> j goes up by 1 only when i also goes up. So the total fall-back is at
        most n steps (amortized O(1) per char).
TRIGGER
  Find a pattern in a text without rescanning, when a mismatch should
  reuse the part already matched: reach for KMP / the LPS array.
================================================================================
*/
