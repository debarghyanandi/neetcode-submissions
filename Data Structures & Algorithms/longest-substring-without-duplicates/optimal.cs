// ##########################################################################
// #  optimal.cs            O(n) time / O(n) space
// #  sliding window, jump left via last-seen index
// #  [sliding-window-jump-index]
// #  ranks above optimal-variant.cs (O(n) time / O(n) space)
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  Single pass tracking last seen index per character; left pointer jumps
// #  directly past the duplicate in O(1) instead of stepping through it.
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
 PATTERN : Sliding window + last-seen map, jump left forward
 SOURCE  : YOUR OWN SOLUTION - your own annotation at c76939d
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  The property "this substring has no repeated character" is monotone under
  shrinking: any substring of a duplicate-free substring is also duplicate-free.
  So for each right endpoint there is a single smallest left that makes
  s[left..right] valid, and every larger left is also valid but shorter. That
  means you only ever need the minimal valid left per right, and left never
  needs to move backward as right advances. A two-pointer window is therefore
  sufficient; no restart from an earlier index is ever needed.

  lastSeenIndex upgrades the window from "shrink one char at a time until the
  duplicate is gone" to "jump left straight past the duplicate," because the map
  already tells you where the offending character was.
INVARIANT
  Two invariants, and they are deliberately different in scope.

  1. At the point where longest is updated, s[left..right] contains no repeated
  character. This is what makes right - left + 1 a legitimate candidate length
  on every single iteration.

  2. lastSeenIndex[c] is the most recent index of c ANYWHERE in s[0..right], not
  the most recent index inside the window. Entries are never removed, so the map
  contains characters that have already fallen out to the left of left.

  Invariant 2 is what forces the Math.Max in the left update. If entries were
  pruned when they left the window, a plain left = previousIndex + 1 would be
  correct - but then you would be doing per-character removal work instead of a
  jump.
THE MAX IS LOAD-BEARING
  Trace "abba" and watch left:

  right=0 'a': not seen. map{a:0}. left=0, longest=1.
  right=1 'b': not seen. map{a:0,b:1}. left=0, longest=2.
  right=2 'b': previousIndex=1, left = max(0, 2) = 2. map{a:0,b:2}. longest
  stays 2.
  right=3 'a': previousIndex=0. This sighting of 'a' is STALE - index 0 is
  behind left=2. Without the max, left = 0 + 1 = 1, and the window becomes
  s[1..3] = "bba", which contains a duplicate 'b'; longest would be reported as
  3. With the max, left = max(2, 1) = 2, window "ba", answer 2 - correct.

  So the failure mode of dropping Math.Max is not a crash or an off-by-one on an
  edge case: it silently overcounts on inputs where a character recurs after an
  earlier eviction. "abba" is the minimal witness and is worth memorizing as the
  test.
ORDER OF THE THREE STATEMENTS
  Inside the loop the order is: adjust left, then write lastSeenIndex[current] =
  right, then update longest.

  The write must come after the read of previousIndex - TryGetValue has already
  captured the old value, but overwriting before the left adjustment would
  compare current against itself and pin left to right + 1 wrongly. The longest
  update must come after the left adjustment, otherwise it measures a window
  that still contains the duplicate. Every reordering of these three lines
  breaks something.
EDGE CASES ALREADY COVERED
  Empty string: the loop body never runs, longest stays 0.

  All identical, "bbbb": each iteration sets left = max(left, right - 1 + 1) =
  right, so the window is a single character and longest stays 1.

  All distinct: left is never touched, longest grows to s.Length.

  No separate guards are needed for any of these - they fall out of the same two
  lines.
INTERVIEWER FOLLOW-UPS
  "Drop the Dictionary." If the input is ASCII, replace it with int[] lastSeen =
  new int[128] filled with -1, and use left = Math.Max(left, lastSeen[current] +
  1). The -1 sentinel makes the not-yet-seen case fold into the same expression,
  so the if disappears entirely.

  "Return the substring, not the length." Record bestLeft = left whenever
  longest is updated, then s.Substring(bestLeft, longest).

  "Allow at most k distinct characters instead." The jump trick dies here.
  Validity is no longer decided by one offending index, so you need a
  count-per-character map plus a while loop that shrinks left and decrements
  counts. Being able to say why the jump stops working - the violation is no
  longer attributable to a single known position - is the point of the question.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
