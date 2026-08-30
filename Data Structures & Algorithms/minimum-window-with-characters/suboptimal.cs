// ##########################################################################
// #  YOU SOLVED THIS YOURSELF  (submission-0, marked '//My Solution')
// #  the window logic is completely right; the validity TEST is the
// #  expensive part. See optimal.cs.
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
 PATTERN : Sliding Window - Variable Size, with a RECOMPUTED validity test
 SOURCE  : YOUR OWN SOLUTION (submission-0, marked '//My Solution')
 STATUS  : Sub-optimal (O(n * a) - correct, and the hard part is already right)
================================================================================

WHY THIS PATTERN
  Two things have to be true at once: the window must CONTAIN all of t
  (which pushes it to grow) and it must be as SHORT as possible (which pushes
  it to shrink). A variable-size window resolves that by alternating: extend
  right until valid, then shrink left while it stays valid, recording as you
  go. Every index enters once and leaves once, so the whole scan is linear
  in moves - regardless of how the validity test is implemented.

  That structure is correct here and it is the part people get wrong. What
  is left is the cost of asking "is it valid?".

BRUTE FORCE (and why it fails)
  Every substring, checking each against t: O(n^2) substrings x O(n) to
  count = O(n^3), or O(n^2 * a) with per-substring histograms. Unusable at
  n = 10^5. The window's key idea is that `left` never moves backward, which
  is licensed by monotonicity: if s[l..r] is invalid, so is every s[l'..r]
  with l' > l, so there is nothing to go back for.

INVARIANT
  window is the exact multiset of s[left..right]; minLength / minIndices
  hold the shortest valid window seen so far.

WHY THIS IS SUB-OPTIMAL
  IsMatch() is O(distinct characters of t) and runs on every iteration of
  both loops - overall O(n * a). The window changes by ONE character per
  move, so validity can change by at most one character's worth. Recomputing
  the whole test throws that away.

  optimal.cs keeps two ints - `have` (how many required characters are
  currently satisfied) and `required` (how many there are) - and updates
  `have` in O(1) at the exact moment a count crosses its threshold. The test
  becomes `have == required`.

  Secondary costs, all of them real but smaller:
    - need.All(...) allocates an enumerator and a closure on every call.
    - ContainsKey + indexer is two hash probes where TryGetValue is one.
    - List<int> minIndices for two values is a heap allocation standing in
      for two ints.
    - The final StringBuilder loop copies the answer character by character
      where s.Substring(start, length) does it in one memcpy.

ALGORITHM
  1. Build need from t. Empty window, left = 0, minLength = MaxValue.
  2. Extend right, admitting s[right] into window.
  3. While the window satisfies need: record it if shorter, evict s[left],
     left++.
  4. Return the recorded window, or "" if none was ever valid.

COMPLEXITY
  Time  : O(n * a) - the O(n) window scan multiplied by the O(a) validity
          test.
  Space : O(a) - two maps bounded by the alphabet.

TRIGGER
  "Smallest / shortest window containing all of X" - the minimum-window
  family. Contrast with permutation-string, where the window is FIXED size
  and the test is exact equality; here the window is variable and the test
  is containment (>=, not ==), which is why extra characters are tolerated.

C# NOTES
  - `bool IsMatch()` as a local function is the right shape - it captures
    need and window without a delegate allocation. The allocation cost here
    is inside the LINQ .All, not in the local function itself.
  - Removing the key at count zero is not required by the >= test, but it
    keeps the map small and matches the mental model of "what is in the
    window".
  - s.Substring(minIndices[0], minLength) replaces the whole StringBuilder
    block. For a zero-copy result, s.AsSpan(start, len) avoids the string
    allocation entirely when the caller only reads it.

WATCH OUT
  - Record the window BEFORE evicting s[left], or the recorded bounds are
    off by one.
  - The shrink loop is a while, not an if: after admitting one character
    several left evictions can be legal.
  - `count >= pair.Value`, not ==. Duplicates in t are the trap - t = "AABC"
    needs two As, and a window with three As is still valid.
  - minLength == int.MaxValue is the only reliable "never found" signal;
    minIndices is initialised to {0, 0}, which is a legitimate-looking window.
================================================================================
*/
