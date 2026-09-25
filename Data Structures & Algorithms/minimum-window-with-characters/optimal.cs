// --------------------------------------------------------------------------
// -  optimal.cs            O(n + m) time / O(1) space
// -  Sliding window with match counter   [sliding-window-match-counter]
// -  ranks above suboptimal.cs (O(n * k) time / O(1) space)
// -
// -  Reference solution - not one you solved yourself
// -
// -  O(1) validity checks via counter increment/decrement eliminate
// -  repeated dictionary scans on each iteration.
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
 PATTERN : Sliding Window - expand right, shrink left on a match counter
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Optimal
================================================================================
VARIABLES
  need         need[c] = how many copies of char c the string t demands
  window       window[c] = copies of c inside s[left..right]
  have         how many distinct required chars currently meet their full quota
  required     need.Count, the number of distinct chars t asks for
  left         left edge of the current window
  minLength    length of the best window found so far, int.MaxValue if none
  resultStart  start index in s of that best window
WHY THIS PATTERN
  The problem asks for the shortest contiguous piece of s that contains every
  character of t with multiplicity. Contiguous plus "shortest" is the sliding
  window signal: growing the window can only make it more valid, shrinking can
  only make it less valid, so the two pointers never need to go backwards. The
  have == required test turns "is this window valid?" into one integer
  comparison instead of a full pass over need, and the inner while loop pulls
  left forward until the window is minimal for that right.
BRUTE FORCE
  The first thing most people write is a double loop: for every start index,
  extend the end until the window covers t, record the length. Checking coverage
  by rebuilding a count map makes that O(n^2 * m), and even with an incremental
  count it is O(n^2). It loses because each restart throws away the counts
  already computed for the previous start, while the window here keeps them and
  moves left only forward.
INVARIANT
  After processing index right, window holds the exact character counts of
  s[left..right], and have equals the number of chars c in need with window[c]
  >= need[c]. The inner loop only exits when the window is one character short
  of valid, so at each right the recorded candidate is the shortest valid window
  ending at or before right. Taking the minimum over all right therefore gives
  the global shortest.
WHY THE INNER WHILE IS STILL LINEAR
  The while loop looks nested, but left is declared outside the for loop and is
  never reset. Over the whole run left moves from 0 to at most s.Length, so the
  total number of shrink steps is bounded by the number of expand steps. That is
  what keeps the scan of s to a single amortized pass.
WATCH OUT
  An empty t is not rejected by the s.Length < t.Length guard, and it makes
  required == 0, so have == required holds immediately; the inner loop keeps
  advancing left past right and reads s[left] out of range on the last index.
  Guard t.Length == 0 explicitly. Also, window counts every character of s, not
  only the ones in need, so it grows with the distinct alphabet of s rather than
  of t - harmless but wasteful. The decrement branch uses window[lc] < need[lc]
  rather than ==; that is safe only because the count drops by exactly one per
  step, so do not change the shrink to remove several copies at once without
  revisiting it.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How do you drop the dictionaries?
     If the input is a known fixed alphabet, replace need and window with two
     int arrays indexed by the character, and required with a count of non-zero
     slots. Same logic, no hashing, but it hard-codes the alphabet.
  2. What if you must return every minimum-length window, not just one?
     Keep a list of start indices; clear it when a strictly shorter window
     appears and append when the length equals minLength. Memory then grows with
     the number of ties.
  3. s is huge and arrives as a stream you can read only once.
     The algorithm already works: right consumes the stream and left never goes
     back. But you must buffer s[left..right] to be able to read s[left] on
     shrink and to return the substring, so worst-case memory is the size of the
     largest valid window.
  4. Variant - longest substring containing at most k distinct characters.
     Same two pointers, but the validity test flips: expand always, and shrink
     while window.Count > k, recording the maximum instead of the minimum. need
     and required disappear.
TRIGGER
  Asked for the shortest or longest contiguous stretch that satisfies a counting
  condition over characters or numbers.
C# NOTE
  need.ContainsKey(c) followed by need[c] hashes the same key twice;
  need.TryGetValue(c, out int want) does it once and reads just as clearly.
  GetValueOrDefault is the right idiom here since the default 0 is exactly the
  wanted starting count.
COMPLEXITY
  Time  : O(n + m)
  Space : O(1)
================================================================================
*/
