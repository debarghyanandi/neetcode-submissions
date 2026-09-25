// ##########################################################################
// #  optimal.cs            O(n + m) time / O(1) space
// #  sliding window with dictionary comparison
// #  [sliding-window-dict-compare]
// #  ties with optimal-variant.cs on O(n + m) time / O(1) space
// #
// #  YOU SOLVED THIS YOURSELF (was suboptimal.cs)
// #
// #  Each iteration performs full multiset comparison via .All() over
// #  dictionary entries, adding constant but measurable overhead.
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
 PATTERN : Fixed-Size Sliding Window - compare two char multisets
 SOURCE  : YOUR OWN SOLUTION - your own annotation at c76939d
 STATUS  : Optimal
================================================================================
VARIABLES
  s1Counts       s1Counts[c] = how many times c appears in s1 (never changes after seeding)
  windowCounts   windowCounts[c] = count of c inside s2[left..right]; key is deleted at zero
  left           first index of the current window in s2
  right          last index of the current window in s2, inclusive
WHY THIS PATTERN
  A permutation of s1 has exactly s1.Length characters and exactly the same
  character counts, so only windows of that one fixed length can match, and
  order inside the window does not matter. That turns the question into "does
  any window of length s1.Length have the same multiset as s1", where a multiset
  is just a bag of characters with counts. Sliding a fixed window means each
  move is one eviction of s2[left] and one insertion of s2[right], so
  windowCounts is rebuilt in two dictionary operations instead of from scratch.
BRUTE FORCE
  The first thing most people write is: for every start index in s2, take the
  substring of length s1.Length, sort it, and compare it with a sorted copy of
  s1. That is O(m * n log n) for m = s2.Length and n = s1.Length, and it throws
  away the fact that two neighbouring windows differ by only two characters.
  Counting fresh per window instead of sorting is still O(m * n).
INVARIANT
  At the top of every while iteration, windowCounts holds the exact character
  counts of s2[left..right], it contains no key mapped to zero, and right - left
  + 1 == s1.Length. Because zero counts are erased, two equal multisets must
  have the same number of keys, so the s1Counts.Count == windowCounts.Count test
  plus the per-key value check is a correct full comparison. Every window of
  that length is visited in order, so returning false after the loop means no
  window matched.
WHY THE KEY IS REMOVED AT ZERO
  The eviction branch decrements only when the count is above 1 and calls Remove
  otherwise. If it decremented to 0 and left the key, windowCounts.Count would
  grow forever and the Count == Count guard would start rejecting real matches,
  while the All check alone would still pass. The Remove is not a tidy-up; it is
  what makes the cheap size test sound.
WATCH OUT
  The comment says "This is the line optimal.cs removes", so this file is the
  teaching version: the All comparison costs O(distinct characters) per step, a
  constant factor the sibling file drops with a running match counter. The
  leading s1.Length > s2.Length guard is load-bearing - without it the seeding
  loop indexes s2[i] past the end. The inner if (right == s2.Length) return
  false is also load-bearing: after right++ the code reads s2[right] before the
  while condition is rechecked. An empty s1 returns true, because both
  dictionaries are empty and Enumerable.All on an empty sequence is true - check
  whether that is the answer you want. windowCounts[s2[left]] is a raw indexer,
  so any drift in the bookkeeping surfaces as a KeyNotFoundException rather than
  a wrong answer.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How do you remove the O(distinct) comparison per step?
     Keep an int matches counter of how many characters currently have
     windowCounts[c] == s1Counts[c]. Each insertion or eviction touches one
     character, so you adjust matches in O(1) and answer true when matches
     equals the number of distinct characters in s1. Same memory, more careful
     update code.
  2. The variant asks for all start indices where a permutation of s1 occurs.
     Do not return on the first match; add left to a List<int> and keep sliding
     to the end. Output size can be O(m), so extra space stops being O(1).
  3. What if the input is a stream of characters you cannot index twice?
     A fixed window only needs the last s1.Length characters, so hold them in a
     circular buffer of that size and evict the oldest as each new one arrives;
     the counting logic is unchanged.
  4. What if s1 can contain any Unicode character, not just letters?
     The dictionary version already handles it unchanged, which is the reason to
     keep it; swapping to a fixed int[26] array would break on anything outside
     lowercase a-z.
TRIGGER
  The question asks whether some contiguous piece of one string is a
  rearrangement of another - a window of known fixed length where only counts
  matter.
C# NOTE
  The s1Counts.All(pair => ...) lambda captures windowCounts, so a closure plus
  an enumerator is created on every iteration of the while loop; a plain foreach
  over s1Counts with TryGetValue does the same work with no allocation. If the
  alphabet is known to be lowercase a-z, two int[26] arrays indexed by s1[i] -
  'a' replace both dictionaries and all the ContainsKey/Add branching.
COMPLEXITY
  Time  : O(n + m)
  Space : O(1)
================================================================================
*/
