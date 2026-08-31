// ##########################################################################
// #  suboptimal.cs         O(n * k) time / O(1) space
// #  sliding window, full multiset comparison per position
// #  [sliding-window-map-compare]
// #  ranks below optimal.cs (O(n) time / O(1) space)
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  Fixed-size window over s2, but re-compares the two frequency
// #  dictionaries (bounded by alphabet size k<=26) from scratch at every
// #  window position instead of maintaining the comparison result
// #  incrementally.
// ##########################################################################

public class Solution
{
    public bool CheckInclusion(string s1, string s2)
    {
        if (s1.Length > s2.Length)
        {
            return false;
        }

        var s1Counts = new Dictionary<char, int>();
        var windowCounts = new Dictionary<char, int>();

        // Seed both maps in one pass: s1's full multiset, and the first
        // s1.Length characters of s2 (the first window).
        for (int i = 0; i < s1.Length; i++)
        {
            if (s1Counts.ContainsKey(s1[i]))
            {
                s1Counts[s1[i]]++;
            }
            else
            {
                s1Counts.Add(s1[i], 1);
            }

            if (windowCounts.ContainsKey(s2[i]))
            {
                windowCounts[s2[i]]++;
            }
            else
            {
                windowCounts.Add(s2[i], 1);
            }
        }

        int left = 0;
        int right = s1.Length - 1;

        while (right < s2.Length)
        {
            // FULL comparison of the two multisets - O(distinct) every step.
            // This is the line optimal.cs removes.
            if (s1Counts.Count == windowCounts.Count &&
                s1Counts.All(pair =>
                    windowCounts.TryGetValue(pair.Key, out int value) &&
                    value == pair.Value))
            {
                return true;
            }

            // Evict the left character. Removing the key when it hits zero
            // is what makes the .Count comparison above meaningful.
            if (windowCounts[s2[left]] > 1)
            {
                windowCounts[s2[left]]--;
            }
            else
            {
                windowCounts.Remove(s2[left]);
            }

            left++;
            right++;

            if (right == s2.Length)
            {
                return false;
            }

            if (windowCounts.ContainsKey(s2[right]))
            {
                windowCounts[s2[right]]++;
            }
            else
            {
                windowCounts.Add(s2[right], 1);
            }
        }

        return false;
    }
}

/*
================================================================================
 PATTERN : Fixed-width window + full count-map compare per step
 SOURCE  : YOUR OWN SOLUTION - your own annotation at c76939d
 STATUS  : Suboptimal
================================================================================
WHY THIS PATTERN
  A permutation of s1 living inside s2 is exactly a contiguous block of length
  s1.Length whose character multiset equals s1's. Order inside the block is
  irrelevant, so the only state worth carrying is a count map - and because the
  block length is pinned to s1.Length, this is the fixed-width window, not the
  grow/shrink kind. left and right move in lockstep; there is no condition under
  which the window widens.
SETUP AND THE OFF-BY-ONE
  The single seed loop over i in [0, s1.Length) does double duty: it accumulates
  s1Counts from s1[i] and windowCounts from s2[i], so after it runs windowCounts
  already describes the first candidate window. That is why right is initialized
  to s1.Length - 1 and not s1.Length - the window is the closed interval [left,
  right], which holds exactly s1.Length characters, matching what the seed loop
  consumed. Initializing right to s1.Length would double-count the first slide
  and shift every window by one.
INVARIANT
  Two things hold at the top of every while iteration. (1) windowCounts is
  exactly the multiset of s2[left..right], a block of size s1.Length. (2) No key
  in windowCounts ever maps to 0 - the eviction branch calls Remove when the
  count is about to hit zero instead of decrementing to zero. Property (2) is
  load-bearing: it is what lets s1Counts.Count == windowCounts.Count mean 'same
  set of distinct characters'. Decrement-to-zero without Remove would leave dead
  keys behind and the Count test would go false on windows that actually match.
WHY IT LOSES
  Every iteration re-derives equality from scratch: the Count test plus
  s1Counts.All(...) walks all distinct characters of s1 on each of the roughly
  s2.Length window positions. But a slide changes exactly two characters -
  s2[left] leaves, s2[right] enters - so at most two entries of the comparison
  can change. Carry the answer instead of recomputing it: keep int matches = the
  number of characters whose s1 count equals the window count, and when you
  decrement or increment a character's window count, adjust matches by checking
  that one character against s1Counts before and after. Report true when matches
  == s1Counts.Count. That is O(1) per slide and O(n) overall. If the alphabet is
  known lowercase, replace both dictionaries with int[26] indexed by c - 'a' and
  matches ranges over 26.
INTERVIEWER FOLLOW-UP
  Is the s1Counts.Count == windowCounts.Count guard necessary? No, and you can
  prove it. The window is always exactly s1.Length characters, so the values in
  windowCounts sum to s1.Length; the values in s1Counts sum to the same. If the
  All(...) clause passes, windowCounts agrees with s1Counts on every key of
  s1Counts, and those keys already account for the entire sum s1.Length. Any
  extra key in windowCounts would therefore have to carry count 0 - impossible,
  by the no-zero-keys invariant. So All(...) alone is sufficient here. Keeping
  the Count test is a cheap short-circuit, not a correctness requirement.
WATCH OUT
  The bounds check that actually matters is the inner if (right == s2.Length)
  return false, placed after left++/right++ and before the character at
  s2[right] is added. The while condition right < s2.Length only ever gets
  evaluated as true: the loop never falls out of the bottom, because that inner
  guard returns first. The trailing return false after the loop is structurally
  unreachable and exists only to satisfy the compiler. Do not 'clean up' the
  inner guard on the assumption the while header covers it - deleting it indexes
  s2[s2.Length].
EDGE CASES
  The s1.Length > s2.Length early return is not just a shortcut: it also
  protects the seed loop, which indexes s2[i] for i up to s1.Length - 1. Empty
  s1 falls through with right = -1 and two empty maps, and the first comparison
  is 0 == 0 plus a vacuously true All, so it returns true without ever touching
  s2 - the conventional answer for an empty pattern.
COMPLEXITY
  Time  : O(n * k)
  Space : O(1)
================================================================================
*/
