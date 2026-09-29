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
 PATTERN : Sliding Window - jump left past the last repeat
 SOURCE  : YOUR OWN SOLUTION - your own annotation at c76939d
 STATUS  : Optimal
================================================================================
VARIABLES
  lastSeenIndex  lastSeenIndex[c] = the latest index in s where char c was seen
  left           start index of the current window; s[left..right] has no repeats
  current        s[right], the char that just entered the window
  previousIndex  the last index where current was seen before right
WHY THIS PATTERN
  The problem asks for the longest contiguous substring with no repeated
  characters. Contiguous plus "longest" plus a property that breaks and can then
  be fixed points to a sliding window. The right end only moves forward. When
  current is a repeat, left moves forward just enough to remove the old copy.
  Every valid window is then checked once, and longest keeps the best size.
BRUTE FORCE
  Take every start index. Extend the end one char at a time, using a HashSet,
  and stop at the first repeat. This is correct, but it is O(n^2) time in the
  worst case, because each start scans forward again. The window version never
  scans backward, and left never moves back, so each index is visited once.
INVARIANT
  At the end of each loop step, s[left..right] has no repeated characters. Also,
  lastSeenIndex holds the latest position of every char seen so far. If current
  appeared inside the window at previousIndex, then previousIndex + 1 is the
  smallest left that removes that copy, so the window stays as large as it can
  be. Every longest valid substring ends at some right. At that step the window
  is at least as long as that substring, so longest records its length.
STALE ENTRIES STAY IN THE MAP
  Old chars are never removed from lastSeenIndex. So previousIndex can point to
  a spot before left. Math.Max(left, previousIndex + 1) ignores these old
  entries. For "abba", at the last 'a' left is 2 and previousIndex is 0. Without
  Math.Max, left would move back to 1 and the answer would be 3 instead of 2.
  The comment in the code is correct about this.
WATCH OUT
  The order inside the loop matters. Read previousIndex first, then write
  lastSeenIndex[current] = right. If you swap them, TryGetValue returns right
  itself and left jumps past current. Also, char is a UTF-16 code unit, not a
  full character. An emoji is a surrogate pair (two chars), so it counts as two
  characters, and two emojis that share a high surrogate count as a repeat.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How do you return the substring itself, not only its length?
     Store bestStart when longest grows, then return s.Substring(bestStart,
     longest). This costs one extra int and one string copy at the end.
  2. Change it to "longest substring with at most k distinct characters."
     Keep a count per char, not a last index. When the number of distinct chars
     goes above k, move left one step at a time and decrease counts until it is
     k again. You lose the direct jump, but it is still linear, because left
     only moves forward.
  3. What if the input is a stream you cannot index back into?
     This code never reads s[left], only indexes, so it already works on a
     stream. Keep a running position and use it in place of right. It only fails
     if you must output the substring, because then you need to buffer the
     window.
TRIGGER
  The problem asks for the longest or shortest contiguous substring or subarray
  where a "no duplicates" or "at most k" rule must hold.
C# NOTE
  If the input is known to be ASCII, an int[128] filled with -1 can replace
  Dictionary<char, int>. The lookup becomes a plain array index, with no hashing
  and no TryGetValue. The Math.Max logic stays the same, because -1 + 1 = 0
  never moves left backward.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
