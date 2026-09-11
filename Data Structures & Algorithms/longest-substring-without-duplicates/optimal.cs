// ##########################################################################
// #  optimal.cs            O(n) time / O(n) space
// #  sliding window, jump left via last-seen index map
// #  [sliding-window-jump-index]
// #  ties with optimal-variant.cs on O(n) time / O(n) space
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  Dictionary of last-seen index lets left pointer jump directly past a
// #  duplicate in O(1) per step instead of scanning.
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
 PATTERN : Sliding window - jump left past the last duplicate
 SOURCE  : YOUR OWN SOLUTION - your own annotation at c76939d
 STATUS  : Optimal
================================================================================
INVARIANT
  After the left update and before longest is recomputed, the window
  s[left..right] contains no repeated character, and left is the SMALLEST index
  for which that is true. lastSeenIndex maps each character seen so far to the
  largest index where it occurred; at the moment of the TryGetValue call,
  previousIndex is the last occurrence of current strictly before right. If that
  occurrence sits inside the window, the only way to restore uniqueness is to
  push left to previousIndex + 1 - one index past the offender, never further.
WHY LEFT NEVER MOVES BACKWARD
  The map is never pruned, so it holds indices of characters that have already
  fallen out of the window. Those entries are stale and Math.Max is the guard
  that neutralizes them. Trace "abba": right=2 sees b at previousIndex=1, so
  left = max(0, 2) = 2. right=3 sees a at previousIndex=0, and previousIndex + 1
  = 1 is BEHIND left. Assigning left = 1 unconditionally would re-admit the
  window "bba" and return 3; the correct answer is 2. Keeping stale entries is
  what buys the single forward pass - there is no inner loop deleting characters
  one at a time.
WHY THE ANSWER IS MAXIMAL
  Each iteration measures right - left + 1, which by the invariant is the
  longest duplicate-free substring ENDING at index right. The optimal substring
  ends at some index r, and on iteration right = r the window is exactly that
  substring, so longest picks it up. Taking the max over every right therefore
  cannot miss it. The empty string needs no special case: the loop body never
  runs and longest stays 0.
ORDER OF THE TWO WRITES
  lastSeenIndex[current] = right must come AFTER the left update. Write it first
  and TryGetValue returns right itself, giving left = right + 1 and a window
  length of right - (right + 1) + 1 = 0 on every repeat. The length formula is
  inclusive on both ends, which is why it is +1 and not right - left.
TRIGGER
  Reach for the jump variant when the window predicate is broken by exactly one
  identifiable position and you can compute the new left in O(1) from a stored
  index. Uniqueness qualifies: the duplicate's previous index tells you
  precisely where the window must restart. The plain HashSet version - while the
  set contains current, remove s[left] and advance left - returns the same
  answer but walks left forward one character at a time instead of jumping.
FOLLOW-UPS AN INTERVIEWER WILL ASK
  1. Return the substring, not the length: record bestLeft = left whenever
  longest grows, then s.Substring(bestLeft, longest).
  2. Input restricted to ASCII: replace the Dictionary with int[128] filled with
  -1, indexed by current; same logic, fixed-size table, no hashing.
  3. At most k distinct characters instead of zero repeats: the jump trick
  breaks, because one stored index no longer tells you how far left must travel.
  You fall back to a count map plus a while loop that shrinks from the left
  until the distinct count is back to k.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
