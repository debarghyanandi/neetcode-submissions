// ##########################################################################
// #  YOU SOLVED THIS YOURSELF  (submission-6, marked '//My solution.')
// #  merged with submission-7 - identical logic, Math.Max form kept
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
 PATTERN : Sliding Window - Variable Size (jump-the-left-pointer)
 SOURCE  : YOUR OWN SOLUTION (submission-6, marked '//My solution.'), merged
           with submission-7 - the `>= left` check you wrote and the Math.Max
           form are the same idea; Math.Max states it in one term
 STATUS  : Optimal
================================================================================

WHY THIS PATTERN
  Same window as suboptimal.cs, but with a sharper move. When s[right]
  duplicates something inside the window, you already KNOW where the offender
  is - so there is no need to crawl left toward it, evicting characters one
  by one. Jump `left` to one past it in a single step.

BRUTE FORCE (and why it fails)
  Every substring checked for uniqueness: O(n^2) with an incremental set,
  O(n^3) naively. The window reuses work; the index map removes the crawl.

THE SUBTLETY THAT BREAKS NAIVE VERSIONS
  lastSeenIndex is never cleaned up - entries survive after the character has
  fallen out of the window. So a hit may be STALE.
  Example: "abba".
      right=0 'a' -> left=0
      right=1 'b' -> left=0
      right=2 'b' -> seen at 1, left = max(0, 2) = 2
      right=3 'a' -> seen at 0. Without Math.Max, left = 1, which REOPENS the
                     window over 'b','b' and returns 3 instead of the correct 2.
  Math.Max enforces that `left` is monotonically non-decreasing. That single
  guard is the entire difference between correct and subtly wrong here, and
  "abba" is the test case that exposes it. Keep it in your pocket.

INVARIANT
  s[left..right] contains no repeated character, and `left` never decreases.

ALGORITHM (NeetCode: "Sliding Window (Optimal)")
  1. Empty map character -> last index, left = 0.
  2. Extend right across the string.
  3. If s[right] was seen at index p, set left = max(left, p + 1).
  4. Record s[right] -> right, and update the best length.

COMPLEXITY
  Time  : O(n) - a single pass with NO inner loop at all. Strictly better
          constants than the shrink-one-step version.
  Space : O(min(n, alphabet)) - one entry per distinct character.

TRIGGER
  "Longest substring with all distinct characters", or any window where the
  reason for invalidity has a KNOWN POSITION you can jump past. If invalidity
  is a count or a sum with no single culprit, use shrink-one-step instead.

C# NOTES
  - TryGetValue does the lookup and the read in one hash probe;
    ContainsKey + indexer does two.
  - `lastSeenIndex[c] = right` overwrites rather than throwing, which is
    exactly the "remember the most recent" semantic wanted. Add() would throw.
  - int[128] indexed by the char, pre-filled with -1, replaces the Dictionary
    entirely for ASCII input and removes hashing from the hot loop.

WATCH OUT
  - Update the map AFTER computing left, not before - otherwise the character
    looks like its own duplicate.
  - Empty string returns 0: the loop body never runs and `longest` stays 0.
================================================================================
*/
