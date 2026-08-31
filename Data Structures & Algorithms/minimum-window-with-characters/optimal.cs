// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(n) space
// -  sliding window, incremental have/required counter
// -  [sliding-window-have-required-counter]
// -  ranks above suboptimal.cs (O(n * k) time / O(n) space)
// -
// -  Reference solution - not one you solved yourself
// -
// -  Maintains two integers (have, required) that update in O(1) only at
// -  the exact moment a character's count crosses its quota, making the
// -  validity check an O(1) comparison instead of a rescan.
// --------------------------------------------------------------------------

public class Solution
{
    public string MinWindow(string s, string t)
    {
        if (s.Length < t.Length)
            return string.Empty;

        var need = new Dictionary<char, int>();
        foreach (char c in t)
        {
            need[c] = need.GetValueOrDefault(c) + 1;
        }

        var window = new Dictionary<char, int>();

        // have     = how many DISTINCT required characters are fully satisfied
        // required = how many distinct characters t asks for
        int have = 0, required = need.Count;

        int left = 0;
        int minLength = int.MaxValue;
        int resultStart = 0;

        for (int right = 0; right < s.Length; right++)
        {
            char c = s[right];

            window[c] = window.GetValueOrDefault(c) + 1;

            // == not >= : `have` may only tick up on the exact crossing,
            // otherwise it would be counted again on every further copy.
            if (need.ContainsKey(c) && window[c] == need[c])
                have++;

            while (have == required)
            {
                if (right - left + 1 < minLength)
                {
                    minLength = right - left + 1;
                    resultStart = left;
                }

                char lc = s[left];
                window[lc]--;

                // Symmetrically: `have` only ticks down on the crossing.
                if (need.ContainsKey(lc) && window[lc] < need[lc])
                    have--;
                left++;
            }
        }

        return minLength == int.MaxValue ? string.Empty : s.Substring(resultStart, minLength);
    }
}

/*
================================================================================
 PATTERN : Sliding Window - expand right, shrink left while valid
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  Validity here is monotone in the window: if s[left..right] already covers t,
  then s[left..right+1] still covers it, since extending right only adds
  characters. That monotonicity is what licenses two pointers. For each right
  there is exactly one smallest left at which the window is still valid, and as
  right advances that boundary never moves backward - a window that was too
  short at an earlier right cannot become valid at a smaller left later. So left
  sweeps forward once instead of being restarted, and the enumeration of all
  substrings collapses to one pass.
ALGORITHM
  1. need is the multiset of t: char -> count. required = need.Count, the number
  of DISTINCT characters that must be satisfied.
  2. Advance right over s, incrementing window[c] for every character, including
  ones t never asks for.
  3. When window[c] reaches need[c] exactly, one more distinct requirement is
  satisfied: have++.
  4. While have == required the window is valid. Record right - left + 1 into
  minLength / resultStart if it beats the best, then evict s[left], decrement
  its count, drop have if that eviction broke a requirement, and left++.
  5. minLength still int.MaxValue means no valid window ever existed; otherwise
  slice s.Substring(resultStart, minLength).
INVARIANT
  have is the count of distinct characters c in need for which window[c] >=
  need[c]. It is never a raw character count and never counts characters outside
  t - both the increment and the decrement sit behind need.ContainsKey. So have
  == required is exactly the statement 'the window covers t as a multiset', and
  the inner while exits only when have < required, i.e. left has been pushed one
  position past the minimal valid start for this right.
THE == AND < TESTS ARE THE WHOLE TRICK
  have counts satisfied requirements, so it must change only on a crossing of
  the threshold, not on every step past it.

  Growing: window[c] == need[c] fires exactly once, on the copy that completes
  the requirement. Writing >= would fire again on every further copy of c and
  inflate have past required, and the while would then run on windows that do
  not cover t.

  Shrinking: window[lc] < need[lc] fires exactly once, on the eviction that
  drops below the requirement (count goes need[lc] - 1). Writing <= would fire
  while the requirement is still met and abandon a still-valid window
  unrecorded.

  The two tests are mirror images across the same boundary, which is why have
  stays a faithful count in both directions.
WHY NO ANSWER IS MISSED
  Take the true optimal window [L, R]. When right reaches R, have == required,
  so the inner while runs and records a candidate at every left position it
  passes through before stopping. Either left is still at or before L, in which
  case the loop records length R - L + 1 at left == L and minLength is at least
  that good; or left already moved past L, which can only have happened while
  the window was valid at some earlier right < R, meaning a strictly shorter
  valid window was already recorded. Either way minLength is no worse than the
  optimum, and since every recorded candidate is a genuinely valid window
  (recorded only under have == required) it cannot be better either.
WATCH OUT
  window holds characters outside t as well - they are counted and decremented
  but the ContainsKey guards keep them from ever touching have. They still take
  up window length, which is precisely why shrinking matters.

  window[lc]-- cannot go negative: lc was incremented when right passed it, and
  left never overtakes right, so every decrement pairs with an earlier
  increment.

  Duplicates in t are handled by the counts, not by required: t = "AABC" gives
  required = 3, and the window must hold two 'A' before have ticks for 'A' once.

  minLength and resultStart are captured as a pair, so no substring is
  materialized per candidate - only the single slice at return.

  The s.Length < t.Length guard is a shortcut only; without it have would simply
  never reach required and the int.MaxValue check would return string.Empty
  anyway.
TRIGGER
  Reach for this shape when the question asks for the shortest or longest
  contiguous stretch satisfying a multiset or count condition, and the condition
  is monotone under extension. The 'have vs required' counter is the reusable
  piece: it turns 'does this window satisfy the constraint' from an O(alphabet)
  dictionary comparison into an O(1) integer test, updated only at threshold
  crossings. Same skeleton as the longest-substring and permutation-in-string
  windows; only the shrink trigger differs - here you shrink while valid to
  minimize, there you shrink while invalid to maximize.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
