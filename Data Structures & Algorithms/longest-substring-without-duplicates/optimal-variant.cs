// --------------------------------------------------------------------------
// -  optimal-variant.cs    O(n) time / O(n) space
// -  sliding window, HashSet with left shrinking one step at a time
// -  [sliding-window-shrink-one]
// -  ties with optimal.cs on O(n) time / O(n) space
// -
// -  Reference solution - not one you solved yourself
// -
// -  Each character enters and leaves the HashSet at most once across the
// -  whole run, giving amortized O(n) despite the nested while loop.
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
 PATTERN : Sliding window + HashSet - shrink left one step at a time
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Optimal variant - ties the best complexity by another route
================================================================================
INVARIANT
  At the top of every iteration of the for loop, windowChars holds exactly the
  characters of s[left..right-1], and they are all distinct. Two halves, both
  load-bearing:

  - "exactly": every Add is paired with a Remove of the character that left
  drops, so the set never drifts from the window. This is why Contains is a
  correct duplicate test.
  - "all distinct": the while loop refuses to let s[right] in until the old copy
  is gone, so the set's Count equals right - left + 1 after the Add. That
  equality is what licenses measuring the window as right - left + 1 instead of
  tracking a length variable.
WHY THE WHILE LOOP TERMINATES
  windowChars.Contains(s[right]) is true only because some index i in [left,
  right-1] has s[i] == s[right]. Each pass removes s[left] and advances left, so
  left marches toward that i. When left reaches i the offending character is
  removed and Contains goes false. left therefore never passes right, and the
  window is never empty at the measurement - worst case it shrinks to the single
  character s[right] itself, giving length 1.

  That is also the answer for "bbbb": each right removes the one b, left lands
  on right, and longest stays 1.
THE NESTED-LOOP OBJECTION
  An interviewer will point at the while inside the for and ask if this is
  quadratic. It is not, and the argument is amortization, not hand-waving: left
  is only ever incremented, never reset, and it is bounded by s.Length. So
  across the whole run the while body executes at most s.Length times total, no
  matter how it clusters. Each character is added once and removed at most once.

  Say "each pointer makes one monotone pass" - that is the whole proof.
ORDER OF OPERATIONS - THE TRAP
  Three orderings here are not interchangeable:

  1. The Add of s[right] must come after the while, not before. Add first and
  Contains is trivially true forever - an infinite loop that also never
  terminates by the argument above, since left would keep advancing past right.
  2. Inside the loop, Remove(s[left]) must precede left++. Swap them and you
  evict the wrong character, silently corrupting the set while the window
  indices say otherwise.
  3. longest is updated after the Add. Updating before it would measure a window
  that does not yet include s[right].
VERSUS THE INDEX-JUMP VARIANT
  The other route to the same complexity replaces the HashSet with a
  Dictionary<char,int> of last-seen index and jumps left straight to lastIndex +
  1 rather than stepping. Same asymptotics; fewer set operations in practice on
  long duplicate runs.

  Its cost is a real correctness trap this file does not have: the dictionary
  retains entries for characters already outside the window, so a stale index
  can drag left backwards. That version needs left = Math.Max(left, lastIndex +
  1). Because windowChars only ever describes the live window, no such guard
  exists or is needed here. If you are asked to write one under pressure, this
  is the safer one; if asked to optimize it, the jump is the answer to offer.
WATCH OUT
  Empty input falls straight through the for loop and returns the initialized
  longest = 0 - no special case needed, but say so out loud rather than letting
  it look accidental.

  Also note what the routine does not return: only the length. Reconstructing
  the substring itself means recording left at the moment longest improves,
  which the Math.Max form hides - you would have to expand it into an explicit
  if to capture the start index.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
