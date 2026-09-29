// ##########################################################################
// #  suboptimal.cs         O(n * k) time / O(1) space
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
 PROBLEM : Given strings s and t, return the shortest substring of s that
           contains every character of t, counting duplicates (t = "AAB" needs
           two A's). Return "" if no such window exists. Matching is
           case-sensitive. Example: s = "ADOBECODEBANC", t = "ABC" -> "BANC".
 PATTERN : Sliding Window (variable size) + frequency maps
================================================================================
IDEA
  need holds the count of each character in t. window holds counts for
  s[left..right]. Each step moves right one place and adds s[right]. While
  IsMatch() is true, the code saves the window if it is shorter than
  minLength, then drops s[left] and moves left forward. So for each right,
  we record the smallest valid window that ends there, and the best of
  these is the answer. Unlike optimal.cs, IsMatch rescans all of need each
  time instead of keeping a running "formed" counter.
EXAMPLE
  s = "ADBAC", t = "ABC"; need = {A:1, B:1, C:1}
  r=0..3: no C in window yet, so IsMatch fails. r=4 adds C, window valid.
  Shrink: left=0 len 5, left=1 len 4, left=2 len 3 (minIndices=[2,4]);
  removing B breaks the match. Answer: s[2..4] = "BAC".
COMPLEXITY
  Time  O(n * k)  left and right each move at most n times; every move calls
                  IsMatch, which scans k keys
  Space O(1)      need and window hold at most one entry per alphabet
                  character
WATCH OUT
  - t = "" crashes the code. need.All(...) on an empty map is true, so
    the inner loop never stops and window[s[left]] throws KeyNotFound.
  - IsMatch must test count >= pair.Value, not ==. The window may hold
    extra copies of a needed character and still be valid.
  - Record the window before you shrink it. If you remove s[left] first,
    you lose the last valid window.
  - window.Remove when a count hits 1 is required. Leaving a 0 in the map
    still works here, but TryGetValue-only checks break if you swap them.
================================================================================
*/
