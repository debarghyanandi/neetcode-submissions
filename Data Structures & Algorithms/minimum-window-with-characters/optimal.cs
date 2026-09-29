// --------------------------------------------------------------------------
// -  optimal.cs            O(n + m) time / O(1) space
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
 PROBLEM : Given strings s and t, return the shortest substring of s that
           contains every character of t, counting duplicates (t = "AAB" needs
           two A's). Return "" if no such window exists. Characters are
           case-sensitive. Example: s = "ADOBECODEBANC", t = "ABC" -> "BANC".
 PATTERN : Sliding Window (variable size) + frequency counts
================================================================================
IDEA
  need counts each char of t; window counts chars between left and right.
  Move right one step at a time. have goes up when a char count reaches its
  need exactly. While have == required, save the window if it is shorter,
  then drop s[left] and move left. This is correct because every shortest
  valid window ending at right is checked before left passes its start.
EXAMPLE
  s = "ADOBECODEBANC", t = "ABC", required = 3
  r=5: "ADOBEC" is valid, minLength=6; drop A -> have=2, left=1
  r=10: valid again, shrink to "CODEBA" (6, not < 6); drop C, left=6
  r=12: "ODEBANC" -> "EBANC" (5) -> "BANC" (4, resultStart=9) -> "BANC"
COMPLEXITY
  Time  O(n + m)  need is built in m steps; right and left each cross s once
  Space O(1)      both dictionaries hold at most one key per alphabet
                  character
PATH TO OPTIMAL
  Brute force: test every substring - O(n^2 * k) - simple, but far too slow.
  Sliding window that compares the full count maps at each step - O(n * k)
  - each index moves once (suboptimal.cs).
  have/required counter - O(n + m) - one check per step, no map compare.
KEYWORDS
  sliding window, two pointers, frequency map, minimum window substring,
  have/required counter, shrink while valid
WATCH OUT
  - Using >= instead of == for have++ counts the same char again for each
    extra copy, so have reaches required too early.
  - Record minLength BEFORE removing s[left]; after removal it may be invalid.
  - Duplicates in t: compare window[c] with need[c], not with 1.
  - Return s.Substring(resultStart, minLength), not (left, right) at the end.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you make it faster in practice when s has many chars not in t?
     -> First build a list of (index, char) for chars that are in t, then
        slide over that list. Same O(n + m), but fewer steps when t is small.
  2. What if t must appear as a subsequence, in order?
     -> That is Minimum Window Subsequence. Use DP or scan forward then
        backward from each match. O(n * m) time, because the counts no longer
        suffice.
  3. Chars are only ASCII. Can you avoid the dictionaries?
     -> Use int[128] arrays for need and window. Same O(1) space, faster
        lookups, but it breaks for full Unicode input.
  4. Why is the nested while loop still linear?
     -> left only moves forward and never passes right, so all inner loops
        together run at most n times. The cost is amortized O(1) per step.
TRIGGER
  You need the shortest (or longest) contiguous range that satisfies a
  count-based condition, and the condition stays true as the range grows.
================================================================================
*/
