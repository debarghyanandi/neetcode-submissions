// ##########################################################################
// #  suboptimal.cs         O(n * k) time / O(1) space
// #  Sliding window with dictionary scan validation
// #  [sliding-window-dictionary-scan]
// #  ranks below optimal.cs (O(n + m) time / O(1) space)
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  IsMatch() scans all k distinct characters in t on each iteration,
// #  adding a k-factor to the sliding window baseline.
// ##########################################################################

public class Solution
{
    public string MinWindow(string s, string t)
    {
        if (s.Length < t.Length)
            return string.Empty;

        var need = new Dictionary<char, int>();
        var window = new Dictionary<char, int>();

        int left = 0;
        int right = 0;
        int minLength = int.MaxValue;

        var minIndices = new List<int> { 0, 0 };

        for (int i = 0; i < t.Length; i++)
        {
            if (need.ContainsKey(t[i]))
                need[t[i]]++;
            else
                need.Add(t[i], 1);
        }

        // Walks EVERY required character on every call - O(|t| distinct) per
        // invocation, and it is invoked on every loop iteration.
        bool IsMatch()
        {
            return need.All(pair =>
                window.TryGetValue(pair.Key, out int count) &&
                count >= pair.Value);
        }

        while (right < s.Length)
        {
            if (window.ContainsKey(s[right]))
                window[s[right]]++;
            else
                window.Add(s[right], 1);

            // Once valid, shrink as far as validity survives - the smallest
            // window ending at `right` is what matters.
            while (IsMatch())
            {
                int currentLength = right - left + 1;

                if (currentLength < minLength)
                {
                    minIndices[0] = left;
                    minIndices[1] = right;
                    minLength = currentLength;
                }

                if (window[s[left]] > 1)
                    window[s[left]]--;
                else
                    window.Remove(s[left]);

                left++;
            }

            right++;
        }

        if (minLength == int.MaxValue)
            return string.Empty;

        var result = new StringBuilder();

        for (int i = minIndices[0]; i <= minIndices[1]; i++)
        {
            result.Append(s[i]);
        }

        return result.ToString();
    }
}

/*
================================================================================
 PATTERN : Sliding Window - expand right, shrink left while valid
 SOURCE  : YOUR OWN SOLUTION - your own annotation at c76939d
 STATUS  : Suboptimal
================================================================================
VARIABLES
  need         need[c] = how many copies of c the answer must contain
  window       window[c] = count of c inside s[left..right]
  minLength    length of the best valid window found so far
  minIndices   minIndices[0] = start, minIndices[1] = end of that best window
  IsMatch      local function: true when window covers every count in need
WHY THIS PATTERN
  The task asks for the shortest substring of s that contains every character of
  t with multiplicity, and substrings are contiguous, so a window with two
  moving ends can visit every candidate. Growing right can only make a window
  valid, and shrinking left can only make it invalid, so each end moves forward
  only. need fixes the target counts once; window tracks the current contents;
  minLength and minIndices remember the best seen.
BETTER APPROACH
  The better solution keeps two integers instead of rescanning: a counter have
  of how many distinct characters already reach their required count, and
  required = need.Count. Increment have when window[c] hits need[c] after an
  add, decrement when it drops below after a removal, and validity is just have
  == required, checked in constant time. That gives O(|s| + |t|) overall. This
  file instead calls IsMatch on every add and on every shrink step, and IsMatch
  walks all distinct characters of t, which is where the extra factor k comes
  from.
INVARIANT
  At the top of the outer loop body, window holds the exact character counts of
  s[left..right], and no valid window starting before left exists for any
  earlier right. The inner loop records the candidate before removing s[left],
  so every recorded window is valid at the moment it is measured, and it stops
  at the first left where validity breaks, so the recorded window is the
  shortest one ending at right. Taking the minimum over all right therefore
  gives the global minimum.
WATCH OUT
  If t is the empty string and s is not, need is empty and need.All(...) is true
  by definition, so the inner loop never stops: left walks past right, window is
  emptied, and window[s[left]] on an unseen character throws
  KeyNotFoundException. minIndices starts as {0, 0}, which would name the
  substring s[0..0]; that is only harmless because the minLength == int.MaxValue
  check returns early, so do not remove that guard. Note also that IsMatch reads
  need and window by closure, so any later change to how window is emptied
  silently changes what IsMatch reports.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. s and t are ASCII only - can you drop both dictionaries?
     Use int[128] for need and window and an int for the number of distinct
     required characters. Lookups become array indexing with no hashing, at the
     cost of a fixed 128-entry allocation per call.
  2. What if you must return all minimum-length windows, not just one?
     Keep a List<int> of start indices; clear it when currentLength < minLength,
     append when currentLength == minLength. Same time, extra space proportional
     to the number of ties.
  3. What if t may contain characters absent from s?
     The code already handles it: IsMatch never becomes true, minLength stays
     int.MaxValue, and the method returns string.Empty. No extra check is
     needed.
  4. The window must contain the characters of t in order, as a subsequence?
     That is a different problem; the two-pointer shrink no longer works because
     validity is not preserved under removal. Use dynamic programming over
     positions of s and t, O(|s| * |t|).
TRIGGER
  A question asking for the shortest or longest contiguous stretch that
  satisfies a counting condition, where extending one end helps and trimming the
  other end hurts.
C# NOTE
  The StringBuilder loop at the end can be one call: s.Substring(minIndices[0],
  minLength). With that, minIndices can be two plain int fields instead of a
  List<int>, since its length is fixed at two.
COMPLEXITY
  Time  : O(n * k)
  Space : O(1)
================================================================================
*/
