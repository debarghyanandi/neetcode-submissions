// ##########################################################################
// #  suboptimal.cs         O(n * k) time / O(n) space
// #  sliding window, recomputed validity check
// #  [sliding-window-recompute-validity]
// #  ranks below optimal.cs (O(n) time / O(n) space)
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  Same linear two-pointer window movement, but IsMatch() re-walks all
// #  distinct characters of t on every iteration of both loops, multiplying
// #  the O(n) scan by O(|t| distinct) work per step.
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
 PATTERN : Sliding Window - shrink while valid, rescan to check
 SOURCE  : YOUR OWN SOLUTION - your own annotation at c76939d
 STATUS  : Suboptimal
================================================================================
WHY THIS PATTERN
  Validity is monotone under growth: if s[left..right] already covers the
  multiset of t, so does every window that contains it. That monotonicity is the
  whole license for two pointers - for a fixed right there is one threshold
  start position beyond which the window breaks, so the smallest valid window
  ending at right is found by pushing left forward until validity dies, and left
  never has to move backward. Strip the monotonicity away and you are back to
  checking all pairs.
INVARIANT
  After the add at the top of the outer loop, window is the exact character
  count of s[left..right] - every insertion goes through the ContainsKey /
  add-or-increment pair and every deletion through the (count > 1 ? decrement :
  Remove) pair, so a key is present if and only if its count is at least 1.

  The inner while exits only when the window is no longer valid, which means the
  previous left was the last valid start for this right. minLength and
  minIndices were already updated on that final valid iteration, before the
  character at left was evicted. So at every point minLength is the length of
  the best window seen so far and minIndices delimits it.
WHY THIS LOSES
  IsMatch re-walks every entry of need through LINQ All, and it is called once
  per outer step plus once per shrink step. It recomputes a whole answer when
  exactly one character changed - that is the defect, not the loop nesting.

  The fix is incremental bookkeeping: keep required = need.Count and have =
  number of distinct required characters currently satisfied. When incrementing
  window[c], if c is in need and window[c] just reached need[c], have++. When
  decrementing, if c is in need and the count just fell below need[c], have--.
  Validity is then have == required, one integer comparison, with no extra
  storage. Same pointer motion, the per-step scan of need disappears.
WHY THE NESTED LOOP IS NOT QUADRATIC
  Interviewers push on this. right advances at most s.Length times and adds one
  character each time; left only ever increases and is never reset, so the total
  number of shrink-loop iterations over the entire run is bounded by s.Length.
  Each index enters window once and leaves at most once. The extra factor in
  this solution comes from IsMatch alone - say that explicitly, because the
  nested while invites the wrong guess.
WATCH OUT
  1. The comparison is count >= pair.Value, not ==. For t = "AABC" the window
  may hold three A's and must still count as valid; == would reject valid
  windows and stall the shrink.

  2. window records every character of s in range, not just the ones in t.
  Harmless for correctness since IsMatch only reads keys drawn from need, but
  the dictionary grows with the distinct characters of s, not of t.

  3. Empty t is a crash, not a wrong answer. need is empty, All over an empty
  sequence is vacuously true, so the shrink loop keeps going until left = right
  + 1 with window emptied, and window[s[left]] throws KeyNotFoundException. If
  the constraints allow t to be empty, guard it next to the s.Length < t.Length
  check.

  4. minIndices starts as {0,0}, which decodes to the single character s[0]. The
  only thing keeping that sentinel from being returned as a real answer is the
  minLength == int.MaxValue test at the end - do not remove it. Two ints would
  carry the same information as this two-element List<int>.
RECONSTRUCTION
  Storing indices instead of the substring is the right call: an improvement is
  recorded with two int writes rather than materializing a new string each time
  a shorter window is found. The final StringBuilder loop over
  minIndices[0]..minIndices[1] is just s.Substring(minIndices[0], minLength)
  spelled out.
TRIGGER
  "Shortest contiguous window satisfying a containment or count condition",
  where the condition survives growing the window - reach for the variable-width
  window with an explicit shrink phase and a counter-based validity test.
  Contrast the fixed-width case (permutation in a string), where left moves in
  lockstep with right and there is no inner loop at all.
COMPLEXITY
  Time  : O(n * k)
  Space : O(n)
================================================================================
*/
