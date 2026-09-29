// ##########################################################################
// #  optimal.cs            O(n + m) time / O(1) space
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
 PROBLEM : Given strings s1 and s2, return true if some permutation of s1
           appears as a contiguous substring of s2. Otherwise return false.
           Put another way: does s2 contain a window that is an anagram of s1?
           Example: s1 = "ab", s2 = "eidbaooo" -> true (window "ba").
 PATTERN : Sliding Window (fixed size) + frequency map
================================================================================
IDEA
  An anagram has the same letter counts. So slide a window of size
  s1.Length over s2 and compare counts. s1Counts holds s1's counts, and
  windowCounts holds the counts of s2[left..right]. Each step removes
  s2[left] and adds the new s2[right]. A key is removed when its count
  reaches 0, so equal Count plus equal values means equal multisets.
  Every window of length s1.Length is checked once, so no match is missed.
EXAMPLE
  s1="ab" s2="eidbaooo"; s1Counts={a:1,b:1}
  win "ei"{e,i} no -> "id"{i,d} no -> "db"{d,b} no
  -> "ba"{b:1,a:1} matches at left=3, right=4 -> true
COMPLEXITY
  Time  O(n + m)  each char enters/leaves once; each check scans at most 26
                  keys
  Space O(1)      both maps hold at most 26 keys (lowercase letters)
PATH TO OPTIMAL
  Brute force: build all permutations of s1 and search s2 - O(n!*m).
  Sort each window and compare to sorted s1 - O(m*n log n), no factorial.
  This file: slide the count maps and compare them - O(26*(n+m)).
  optimal-variant.cs likely keeps a "matches" counter - O(1) per step.
KEYWORDS
  sliding window, fixed-size window, anagram, frequency count, hash map
WATCH OUT
  - The comment says "This is the line optimal.cs removes", but THIS is
    optimal.cs. The full All() check is still here. Fix the comment.
  - If you decrement to 0 and do not Remove the key, the .Count test fails
    and real matches are missed.
  - Without the s1.Length > s2.Length guard, the seed loop reads s2[i]
    out of range.
  - The last window is checked before right hits s2.Length. Do not move
    the check after the slide, or the last window is skipped.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you make each step O(1), not O(26)?
     -> Use int[26] arrays and a "matches" count of letters whose counts are
        equal. Update it only for the letters that enter and leave. Still O(n+m).
  2. Return all start indices of anagrams (LeetCode 438)?
     -> Use the same window. Add left to a list on each match, do not return.
        O(n+m) time, plus the size of the output.
  3. What if the alphabet is Unicode or very large?
     -> Keep the Dictionary approach and the matches counter. Space becomes
        O(distinct chars), and the time stays O(n+m).
  4. The smallest window that contains all of s1, in any order?
     -> That is Minimum Window Substring. Use a variable-size window: grow
        right, shrink left while it is valid. Still O(n+m).
TRIGGER
  Reach for a fixed sliding window with counts when you must check whether
  a substring is an anagram or permutation of a given pattern.
================================================================================
*/
