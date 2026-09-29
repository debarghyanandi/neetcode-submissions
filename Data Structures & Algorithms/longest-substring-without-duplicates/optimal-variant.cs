// --------------------------------------------------------------------------
// -  optimal-variant.cs    O(n) time / O(1) space
// --------------------------------------------------------------------------

public class Solution
{
    public int LengthOfLongestSubstring(string s)
    {
        // The exact set of characters currently inside the window.
        var windowChars = new HashSet<char>();

        int left = 0;
        int longest = 0;

        for (int right = 0; right < s.Length; right++)
        {
            // Shrink from the left ONE STEP AT A TIME until the duplicate is gone.
            while (windowChars.Contains(s[right]))
            {
                windowChars.Remove(s[left]);
                left++;
            }

            windowChars.Add(s[right]);
            longest = Math.Max(longest, right - left + 1);
        }

        return longest;
    }
}

/*
================================================================================
 PROBLEM : Given a string s, return the length of the longest substring with
           no repeated character. A substring is contiguous. A subsequence
           does not count. Example: "pwwkew" -> 3 ("wke").
 PATTERN : Sliding Window (variable size) + hash set
================================================================================
IDEA
  windowChars holds exactly the characters of s[left..right]. For each new
  s[right], if it is already in the set, we remove s[left] and move left
  one step, again and again, until the old copy is gone. Then we add
  s[right] and update longest with right - left + 1. It is correct because
  the window is always duplicate-free, and it is the longest such window
  that ends at right. Unlike a jump-by-index version, left only ever moves
  one step at a time here.
EXAMPLE
  s = "abba": r=0 {a} len 1; r=1 {a,b} len 2, longest=2
  r=2 'b' dup: remove a (left=1), remove b (left=2), add b -> {b} len 1
  r=3 'a' not in set (removed at r=2) -> {b,a}, left=2, len 2
  Answer: 2 (the stale 'a' at index 0 is never a problem here)
COMPLEXITY
  Time  O(n)  each char is added once and removed at most once; left, right <=
              n
  Space O(1)  set holds at most one copy of each distinct char (fixed
              alphabet)
WATCH OUT
  - Use while, not if. At r=2 in "abba" one removal is not enough, and
    with if the set still contains 'b'.
  - Remove s[left], not s[right], and do it before left++. Add s[right]
    only after the loop, or the loop never ends.
  - The O(1) space holds only for a fixed charset. For any Unicode input
    the set can grow to O(min(n, k)), where k is the alphabet size.
  - char is a UTF-16 unit, so an emoji counts as two chars. Say this if
    asked about Unicode.
================================================================================
*/
