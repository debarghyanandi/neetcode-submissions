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
 PATTERN : Fixed-Size Sliding Window - compare character counts
 SOURCE  : YOUR OWN SOLUTION - your own annotation at c76939d
 STATUS  : Optimal
================================================================================
VARIABLES
  s1Counts       s1Counts[c] = how many times c appears in s1
  windowCounts   windowCounts[c] = how many times c appears in s2[left..right]; no key ever has a zero value
  right          inclusive end of the window, so the window is always s1.Length chars long
WHY THIS PATTERN
  A permutation of s1 is any string that has the same character counts as s1.
  The problem asks if some substring of s2 is such a permutation. That substring
  must be exactly s1.Length long, so the window has a fixed size. We slide it
  one step at a time across s2. Each step removes s2[left] and adds s2[right],
  so windowCounts is updated in place and never rebuilt.
BRUTE FORCE
  For each start index in s2, take the substring of length s1.Length, sort it,
  and compare it to sorted s1. This is correct. It costs O(m * n log n), where n
  = s1.Length and m = s2.Length. It loses because every window is rebuilt and
  sorted from nothing, even though two neighbor windows share all but two
  characters.
INVARIANT
  At the top of each loop pass, windowCounts holds the exact counts of
  s2[left..right], and right - left + 1 == s1.Length. Every window start from 0
  to s2.Length - s1.Length is checked once, in order. So if any permutation of
  s1 exists in s2, the check finds it at that window and returns true. If the
  loop runs out, no window matched.
COUNT CHECK NEEDS ZERO KEYS REMOVED
  The All(...) check only looks at the keys of s1Counts. A window with extra
  characters would still pass it. The s1Counts.Count == windowCounts.Count test
  blocks those extra characters. That test only works because the eviction code
  calls Remove when a count would drop to zero. If it left a zero entry in the
  map, Count would be too high, and a real match would be missed.
WATCH OUT
  The code comment says optimal.cs removes the full comparison. So this file
  compares both whole maps at every step. The per-step cost grows with the
  number of distinct characters. The banner complexity only holds when the
  alphabet is fixed, for example 26 lowercase letters. Also, the loop condition
  while (right < s2.Length) never ends the loop. The inner check if (right ==
  s2.Length) return false does. If someone moves that check, the next line reads
  s2[right] out of range.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How do you make each slide O(1) no matter how big the alphabet is?
     Keep a "matches" counter: the number of characters whose window count
     equals their s1 count. On each add or remove, update the counter only for
     the one character that changed. Return when matches equals the number of
     distinct characters. The code gets harder, but you drop the full map
     comparison.
  2. How would you return every start index of an anagram of s1 in s2 (LeetCode
  438)?
     Use the same window, but add left to a result list instead of returning
     true, and keep sliding to the end. The time stays the same, plus space for
     the output list.
  3. What if s2 arrives as a stream and is too large to store?
     Keep only the last s1.Length characters in a circular buffer, so you still
     know which character leaves the window. Memory is O(s1.Length) plus the two
     maps, and does not grow with s2.
TRIGGER
  The problem asks whether some substring is a permutation or anagram of a given
  pattern, so the window length is fixed and only the character counts matter.
C# NOTE
  Each ContainsKey followed by windowCounts[key]++ or Add does two hash lookups.
  CollectionsMarshal.GetValueRefOrAddDefault(windowCounts, key, out _)++ does it
  in one. If the input is only lowercase letters, an int[26] indexed by c - 'a'
  removes the hashing completely.
COMPLEXITY
  Time  : O(n + m)
  Space : O(1)
================================================================================
*/
