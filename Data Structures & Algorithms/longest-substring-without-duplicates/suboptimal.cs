// --------------------------------------------------------------------------
//  Reference solution - from NeetCode / other resource (submission-8)
//  Not one you solved yourself.
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
 PATTERN : Sliding Window - Variable Size (shrink-until-valid)
 SOURCE  : NeetCode / other resource (submission-8, which you labelled
           'sub optimal but easy')
 STATUS  : Sub-optimal (same O(n), but the window crawls instead of jumping)
================================================================================

WHY THIS PATTERN
  "Longest CONTIGUOUS run satisfying a property" is the sliding window
  signature. Contiguity is what makes a window meaningful; if the problem
  allowed skipping elements it would not be a window problem at all.

  The universal shape:
     expand `right` by one
     while the window is INVALID, shrink `left`
     record the answer

BRUTE FORCE (and why it fails)
  Check every substring for uniqueness: O(n^3), or O(n^2) with an incremental
  set. The window version reuses the previous window instead of rebuilding it.

INVARIANT
  windowChars holds exactly the characters of s[left..right], and that window
  never contains a duplicate at the point the length is recorded.

WHY THIS IS SUB-OPTIMAL vs optimal.cs
  Not in Big-O - both are O(n), because `left` never moves backward and so
  the inner while-loop does at most n total steps across the whole run.
  The difference is the CONSTANT: this version walks `left` forward one
  character at a time, removing each from the set. The optimal version
  remembers each character's last index and JUMPS `left` straight past the
  duplicate in one move, with no removals at all.

  Keep this version in your head anyway: it is the shape that generalises.
  The jump trick only works because the invalidating condition is "a
  duplicate character", whose position is knowable. For a window validated by
  a COUNT (see longest-repeating-substring-with-replacement) there is nothing
  to jump to and shrink-one-step is the only option.

ALGORITHM (NeetCode: "Sliding Window")
  1. Empty set, left = 0.
  2. Extend right across the string.
  3. While s[right] is already in the window, evict s[left] and left++.
  4. Insert s[right], update the best length.

COMPLEXITY
  Time  : O(n) amortised - right advances n times, left advances at most n
          times in total. Nested loops, still linear.
  Space : O(min(n, alphabet)) - the set can never exceed the alphabet size.

TRIGGER
  "Longest/shortest CONTIGUOUS substring or subarray such that <condition>",
  where the condition breaks monotonically as the window grows.

C# NOTES
  - HashSet<char>.Remove and .Contains are both O(1) average.
  - Window length is `right - left + 1`. Getting the +1 wrong is the single
    most common sliding-window bug - write it once and trust it.
  - For byte-level speed a bool[128] beats HashSet<char> on ASCII input:
    same idea as int[26] in is-anagram.

WATCH OUT
  - Add s[right] AFTER the shrink loop. Adding first makes the while-loop see
    the character it just inserted and spin the window to nothing.
================================================================================
*/
