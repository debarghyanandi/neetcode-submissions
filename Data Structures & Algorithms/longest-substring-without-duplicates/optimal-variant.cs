// --------------------------------------------------------------------------
// -  optimal-variant.cs    O(n) time / O(n) space
// -  sliding window, shrink left one step at a time
// -  [sliding-window-shrink-one]
// -  ties with optimal.cs on O(n) time / O(n) space
// -
// -  Reference solution - not one you solved yourself (was suboptimal.cs)
// -
// -  HashSet-based window where left advances one character per removal
// -  until the duplicate is evicted; amortized linear but with larger
// -  constant than the jump version.
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
 PATTERN : Sliding Window - HashSet of window chars, shrink one step
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Optimal variant - ties the best complexity by another route
================================================================================
WHY THIS PATTERN
  The answer is the longest contiguous window whose characters are all distinct.
  "All distinct" is monotone in the wrong direction: if s[left..right] already
  contains a repeat, every window that contains it also does. So for a fixed
  right there is a smallest valid left, and that smallest left is non-decreasing
  as right grows. Two pointers that both only move forward capture exactly that,
  which is why no start position ever needs re-scanning.

  Brute force is: for every start index, extend forward until a repeat appears,
  tracking seen chars. That is O(n^2) and throws away the fact that the previous
  start's work already told you where the repeat was.
INVARIANT
  At the top of each for iteration: windowChars holds exactly the characters of
  s[left..right-1], each appearing once, and longest is the best window length
  over all windows ending before right.

  The while loop restores the precondition for adding s[right]; after it,
  s[right] is not in windowChars, so Add keeps the "each appears once" part true
  and s[left..right] is a valid all-distinct window. longest is updated after
  the Add, so right - left + 1 correctly counts s[right] itself.
WHY THE SHRINK TERMINATES
  If windowChars.Contains(s[right]) is true, the invariant says there is exactly
  one index d in [left, right-1] with s[d] == s[right]. Each pass removes
  s[left] and advances left, so after at most d - left + 1 passes left == d + 1
  and the duplicate is gone. The loop cannot run past right because d < right -
  it always finds its target inside the current window.

  "Exactly one" is what makes a plain HashSet enough. Remove(s[left])
  unconditionally deletes the character because there is no second copy of it in
  the window to protect. A frequency-map version of this loop would need to
  decrement and only erase at zero.
LEFT NEVER REWINDS
  left is only ever incremented, and it is bounded by s.Length, so the inner
  while body executes at most n times summed over the entire run - the nesting
  is not a product. Each character enters windowChars once when right passes it
  and leaves at most once when left passes it. This is the standard interview
  follow-up for any nested-loop sliding window: answer with the monotone
  pointer, not with the loop shape.
VS THE LAST-INDEX MAP
  The other common route replaces the set with a Dictionary<char,int> lastSeen
  and, on reading c = s[right], does left = Math.Max(left, lastSeen[c] + 1) -
  one jump, no removals - then lastSeen[c] = right.

  The difference is per-step, not overall. That version does bounded work at
  every right; this one can do a burst of Removes at one right and none at the
  next. The tradeoff is which check you own: the map keeps stale entries for
  characters that have already left the window, so every lookup must be compared
  against left or it will shrink backwards; this set holds only live characters,
  so Contains is the entire test and left needs no guarding. Neither dominates -
  pick the one whose bookkeeping you will not get wrong under pressure.
WATCH OUT
  1. Remove(s[left]) must come before left++. Swapping them removes the
  character just outside the window and leaves the real duplicate behind, so the
  while loop keeps spinning and left walks off past right.
  2. The while condition tests Contains(s[right]), the incoming character - not
  the character at left. Shrinking is driven by the new element, not by the old
  one.
  3. longest is computed after the Add, deliberately. Moving it before the Add
  still gives the same number here (the set is not read for the length), but
  computing it before the while would measure a window that still holds a
  duplicate.
  4. Empty string: the for loop never runs and longest stays 0, which is right.
  No separate guard is needed.
TRIGGER
  Reach for this shape when the problem asks for the longest or shortest
  contiguous run subject to a constraint that only gets harder as the window
  grows. Then pair two forward-only pointers with a structure describing the
  window's current contents: a HashSet when the constraint is "all distinct", a
  count map when repeats are allowed up to some budget k.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
