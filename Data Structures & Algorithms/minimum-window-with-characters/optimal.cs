// --------------------------------------------------------------------------
//  Reference solution - from NeetCode / other resource (submission-3)
//  Not one you solved yourself.
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
 PATTERN : Sliding Window - Variable Size, with an INCREMENTALLY MAINTAINED
           satisfaction counter
 SOURCE  : NeetCode / other resource (submission-3)
 STATUS  : Optimal - O(n + m) time, O(a) space
================================================================================

WHY THIS PATTERN
  The window scan in suboptimal.cs is already optimal - every index enters
  once and leaves once. The only thing left to remove is the cost of the
  question "is this window valid?", asked once per move.

  Collapse the answer into two integers:

      required = need.Count                    (fixed)
      have     = characters whose count in the window has REACHED its quota

  A window is valid exactly when have == required: an O(1) test. And `have`
  itself is cheap to maintain, because a character's satisfied/unsatisfied
  status can only flip at the precise moment its count CROSSES the quota -
  which is why the updates use `== need[c]` on admit and `< need[c]` on
  evict, never `>=`.

  This is the same manoeuvre as `matches` in permutation-string and the
  running sum in minimum-size-subarray-sum: do not recompute an aggregate
  that changes by a bounded amount.

BRUTE FORCE (and why it fails)
  All O(n^2) substrings with a per-substring histogram: O(n^2 * a). The
  window removes the outer factor; this counter removes the inner one.

INVARIANT
  window is the exact multiset of s[left..right];
  have == |{ c in need : window[c] >= need[c] }|;
  (minLength, resultStart) describe the shortest valid window seen so far.

WHY >= IS WRONG IN THE UPDATES, AND == IS RIGHT
  With need['A'] = 1 and a window gaining a second 'A': under `>=` the
  condition holds again and have is incremented twice for one character, so
  have can exceed required and the loop misfires. Under `==` it fires only
  on the transition 0 -> 1. The eviction side is the mirror image: `<` fires
  only on the transition 1 -> 0. Counters that track transitions must be
  updated on transitions, not on states. This is the single most common way
  to get this problem subtly wrong.

ALGORITHM (NeetCode: "Sliding Window")
  1. Build need from t; required = need.Count; have = 0.
  2. Extend right: window[c]++, and have++ if window[c] just reached need[c].
  3. While have == required: record the window if shorter, then evict
     s[left] - window[lc]--, have-- if it just dropped below need[lc] -
     and left++.
  4. Return s.Substring(resultStart, minLength), or "" if none was valid.

COMPLEXITY
  Time  : O(n + m). right advances n times, left advances at most n times in
          total, and every step inside is O(1) hashing.
  Space : O(a) - two maps bounded by the distinct characters involved.

TRIGGER
  "Minimum window containing all of t" and its relatives ("smallest subarray
  covering every distinct value", "shortest span covering all k colours").
  Whenever validity is a conjunction of per-key thresholds, count the
  satisfied keys instead of re-checking them.

C# NOTES
  - GetValueOrDefault(c) + write is clean but still two probes.
    CollectionsMarshal.GetValueRefOrAddDefault(dict, c, out _)++ does the
    whole read-modify-write in one hash lookup - the genuinely fast form,
    and a good thing to know exists.
  - need.ContainsKey(c) followed by need[c] is another double probe;
    TryGetValue merges them.
  - s.Substring allocates a new string. s.AsSpan(resultStart, minLength)
    is allocation-free when the caller only needs to read it, though the
    signature here demands a string.
  - int[128] indexed by the raw char beats Dictionary outright when the
    input is ASCII, which LeetCode's constraints give.

WATCH OUT
  - window[lc]-- can legally drive a count to zero or leave foreign
    characters at positive counts; neither breaks the invariant, because
    only characters in `need` are ever consulted.
  - Record the window BEFORE the eviction.
  - minLength == int.MaxValue is the "never valid" sentinel; resultStart
    defaults to 0, which would otherwise return a real-looking substring.
  - The shrink loop must run to exhaustion. Stopping at the first valid
    window finds A window, not the SHORTEST one ending at `right`.
================================================================================
*/
