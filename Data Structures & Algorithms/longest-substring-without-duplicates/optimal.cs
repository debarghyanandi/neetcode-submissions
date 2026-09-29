// ##########################################################################
// #  optimal.cs            O(n) time / O(1) space
// ##########################################################################

public class Solution
{
    public int LengthOfLongestSubstring(string s)
    {
        // character -> the LAST index at which it was seen
        var lastSeenIndex = new Dictionary<char, int>();

        int left = 0;
        int longest = 0;

        for (int right = 0; right < s.Length; right++)
        {
            char current = s[right];

            if (lastSeenIndex.TryGetValue(current, out int previousIndex))
            {
                // Math.Max is doing real work here, not defensive coding:
                // the previous sighting may be OUTSIDE the current window,
                // in which case it is stale and left must NOT move backward.
                left = Math.Max(left, previousIndex + 1);
            }

            lastSeenIndex[current] = right;
            longest = Math.Max(longest, right - left + 1);
        }

        return longest;
    }
}

/*
================================================================================
 PROBLEM : Given a string s, return the length of the longest substring that
           has no repeated character. A substring must be contiguous, so a
           subsequence does not count. Return the length, not the substring
           itself. Example: "abcabcbb" -> 3 ("abc").
 PATTERN : Sliding Window (variable size) + last-seen index map
================================================================================
IDEA
  The window s[left..right] always holds distinct characters. For each
  right, if current was seen before at previousIndex, jump left to
  previousIndex + 1 so the old copy leaves the window. Math.Max stops left
  from moving backward when that old copy is already outside the window.
  Then store lastSeenIndex[current] = right and update longest. This is
  correct because every valid window ending at right starts at left or later.
EXAMPLE
  s = "abba"
  r=0 a: new, left=0, len 1 | r=1 b: new, left=0, len 2 (longest=2)
  r=2 b: prev 1, left=max(0,2)=2, len 1 | r=3 a: prev 0 is stale,
  left=max(2,1)=2, len 2 -> answer 2
COMPLEXITY
  Time  O(n)  right visits each index once; each map lookup is O(1)
  Space O(1)  the map holds at most one entry per distinct character (fixed
              set)
PATH TO OPTIMAL
  Check every substring for duplicates - O(n^3) - the simplest start.
  Grow from each start with a set, stop at the first repeat - O(n^2).
  Window with a set, shrink left one step at a time - O(n), 2n steps
  (likely the approach in optimal-variant.cs).
  This file: left jumps straight past the repeat - O(n), one pass.
KEYWORDS
  sliding window, two pointers, hash map, last seen index, distinct chars
WATCH OUT
  - Without Math.Max, "abba" gives 3: at r=3, left moves back to 1.
  - Look up previousIndex BEFORE you write lastSeenIndex[current] = right,
    or you always find the current index.
  - The window length is right - left + 1. Forgetting the +1 is a common bug.
  - O(1) space holds only for a fixed alphabet. With full Unicode, the map
    grows with the number of distinct characters.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Longest substring with at most k distinct characters?
     -> Keep a count map. Shrink left while the map has more than k keys. O(n)
        time, O(k) space. You cannot jump left here, so shrink step by step.
  2. The input is only ASCII. Can you avoid the dictionary?
     -> Use an int[128] array filled with -1. It is still O(n) time and O(1)
        space, but faster, with no hashing.
  3. Return the substring itself, not the length?
     -> Save left when longest improves. Return s.Substring(bestLeft,
        longest).
  4. Why is the answer never missed when left jumps?
     -> Any window that starts before previousIndex + 1 has current twice, so
        the start positions we skip can never be valid.
TRIGGER
  Look for "longest or shortest contiguous substring or subarray where a
  condition (such as no repeats) must hold"; use a sliding window.
================================================================================
*/
