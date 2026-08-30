// ##########################################################################
// #  YOU SOLVED THIS YOURSELF  (submission-1, marked '//My solution')
// #  right idea - fixed window - but the two maps are compared from
// #  scratch on every single position. See optimal.cs.
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
 PATTERN : Sliding Window - FIXED SIZE, comparing two frequency maps
 SOURCE  : YOUR OWN SOLUTION (submission-1, marked '//My solution')
 STATUS  : Sub-optimal (O(n * a) - correct, and the right shape)
================================================================================

WHY THIS PATTERN
  "s2 contains a permutation of s1" means: some window of s2 of length
  |s1| has EXACTLY s1's character multiset. Permutation = same multiset,
  order irrelevant. Once that translation is made, the window size is fixed
  and known up front, so there is no shrink phase at all - the window just
  marches, admitting one character and evicting one on every move.

  Getting to "fixed-size window + multiset equality" is the actual insight
  of this problem, and this submission has it. What it does not yet have is
  a cheap way to test equality.

BRUTE FORCE (and why it fails)
  Sort every length-|s1| substring and compare: O(n * m log m). Or generate
  all permutations of s1 and search for each: O(m! * n), unusable past m = 8.
  Both re-derive from scratch what the previous window already established.

INVARIANT
  windowCounts is the exact multiset of s2[left..right], and right - left + 1
  == s1.Length at every comparison.

WHY THIS IS SUB-OPTIMAL
  The two-map comparison runs on every one of the n - m + 1 positions, and
  each comparison walks all distinct characters of s1: O(n * a) with a <= 26.
  The window itself changes by only TWO characters per step, so the ANSWER to
  "are these equal?" also changes by a bounded amount - it should be
  maintained incrementally, not recomputed.

  optimal.cs keeps a single `matches` counter (how many of the 26 letters
  currently agree) and updates it in O(1) inside Add/Remove. Same window,
  same eviction, O(1) equality test.

  Secondary cost: Dictionary hashing per character where the alphabet is a
  known 26 letters. int[26] indexed by (c - 'a') is a direct array write -
  no hashing, no allocation, and it makes the comparison a fixed 26-slot
  loop rather than a LINQ enumeration with a closure allocation per call.

ALGORITHM
  1. Reject if s1 is longer than s2.
  2. Build s1Counts from s1 and windowCounts from s2's first m characters.
  3. Loop: compare the maps; if equal, return true. Otherwise evict s2[left],
     advance both edges, admit s2[right].
  4. Return false if the right edge runs off the end.

COMPLEXITY
  Time  : O(n * a) - n positions, each doing an O(a) multiset comparison.
  Space : O(a) - two maps bounded by the alphabet.

TRIGGER
  "Does a window of exactly length m satisfy X?" - anagram / permutation /
  "all characters of t appear exactly once" phrasing. Fixed size is the
  signal: no shrink loop, one in and one out per step.

C# NOTES
  - s1Counts.All(...) allocates an enumerator and a closure on every call.
    Inside a per-position loop that is real garbage-collector pressure, not
    a micro-nit.
  - ContainsKey + indexer is two hash probes for what TryGetValue does in
    one; CollectionsMarshal.GetValueRefOrAddDefault does the increment in
    one probe total.
  - Removing the key at zero is load-bearing here: the s1Counts.Count ==
    windowCounts.Count guard is only correct if zero-count keys never linger.
    That is subtle, and it is a good thing to be able to explain out loud.

WATCH OUT
  - The comparison must happen BEFORE the eviction, or the first window is
    never tested.
  - The `right == s2.Length` guard after the increment is what stops the
    final admit from reading past the end. Losing it is an
    IndexOutOfRangeException on the last window.
  - Seeding both maps in the same loop is fine only because both strings are
    indexed by the same i over the first m positions - correct here, and
    easy to break if the window size is ever decoupled from s1.Length.
================================================================================
*/
