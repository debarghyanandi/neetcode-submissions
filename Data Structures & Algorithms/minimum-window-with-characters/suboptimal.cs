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
  need         need[c] = how many times c must appear (built from t)
  window       window[c] = how many times c appears in s[left..right]
  minLength    length of the best window so far; int.MaxValue means none found
  minIndices   minIndices[0], minIndices[1] = start and end of the best window
  IsMatch      local function: true when window covers every count in need
WHY THIS PATTERN
  The problem asks for the shortest contiguous substring of s that contains all
  of t, with repeats counted. A substring means a contiguous range, so two
  pointers can mark it. Moving right forward can only add characters, and moving
  left forward can only remove them. So the code grows window with right until
  IsMatch() is true. Then it shrinks from left to find the shortest valid window
  that ends at right.
BETTER APPROACH
  The better approach keeps an int named formed. It counts how many distinct
  keys of need are fully met. It changes only when window[c] crosses exactly
  need[c]: formed goes up when the count rises to need[c], and down when it
  falls below need[c]. Then the validity check is just formed == need.Count,
  which costs O(1) and gives O(|s| + |t|) total time. This file calls IsMatch()
  on every step of both loops. Each call walks all of need, and that walk is the
  extra factor k.
INVARIANT
  When the inner while loop exits, s[left..right] is not valid. Every valid
  window that ends at right and starts at or after the old left has been
  measured against minLength. left never needs to move back. If s[l..r] is not
  valid, then s[l'..r] is not valid for any larger l'. If s[l..r] is valid, then
  s[l..r+1] is valid too. So no skipped start can give a shorter valid window
  later. Every right is visited, and each one records its shortest valid window,
  so minIndices ends up holding the overall shortest.
WATCH OUT
  An empty t breaks the code. need is empty, so need.All(...) returns true every
  time. The inner loop then keeps moving left past right, calls window[s[left]]
  on a key that is not in window, and throws. It can also go past the end of s.
  Add a guard like "if (t.Length == 0) return string.Empty". The comment on
  IsMatch is accurate: the code does walk every key of need on every call. That
  comment points to the real cost, not a bug.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. s and t use only ASCII. Can you drop the dictionaries?
     Yes. Use two int[128] arrays indexed by the char. Each lookup is a plain
     array index with no hashing, and you avoid the extra lookup in each
     ContainsKey-then-index pair. The trade-off: this only works for a small,
     known alphabet.
  2. s is huge and most of its characters are not in t. What do you change?
     First build a list of (index, char) pairs, only for chars that are in need.
     Then slide the window over that list. Each step skips characters that can
     never help. You still measure length with the real indices in s. The
     trade-off is O(|s|) extra memory in the worst case.
  3. What if the characters of t must appear in order, as a subsequence?
     Counts no longer work. Scan forward from each possible start until all of t
     is matched in order. Then scan backward from that end to find the latest
     start that still works. That costs O(|s| * |t|), or you can use a DP table
     over positions in s and t.
TRIGGER
  The problem asks for the shortest (or longest) contiguous substring or
  subarray that satisfies a coverage or count condition. Adding elements only
  helps meet the condition, and removing elements only hurts it.
C# NOTE
  The StringBuilder loop at the end can be replaced by
  s.Substring(minIndices[0], minLength). That is one call and one copy.
  minIndices can also be two plain int variables instead of a List<int>.
COMPLEXITY
  Time  : O(n * k)
  Space : O(1)
================================================================================
*/
