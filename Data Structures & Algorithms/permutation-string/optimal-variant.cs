// --------------------------------------------------------------------------
// -  optimal-variant.cs    O(n + m) time / O(1) space
// --------------------------------------------------------------------------

public class Solution
{
    public bool CheckInclusion(string s1, string s2)
    {
        if (s1.Length > s2.Length)
            return false;

        int[] need = new int[26];
        int[] window = new int[26];
        int matches = 0; // how many of the 26 letters currently have need[c] == window[c]

        for (int i = 0; i < s1.Length; i++)
        {
            need[s1[i] - 'a']++;
        }

        // A letter with need[c] == 0 already "matches" window[c] == 0 before anything is added.
        for (int c = 0; c < 26; c++)
        {
            if (need[c] == window[c])
                matches++;
        }

        void Add(char ch)
        {
            int c = ch - 'a';
            if (window[c] == need[c])
                matches--;   // about to break equality (if it was equal)
            window[c]++;
            if (window[c] == need[c])
                matches++;   // may have restored equality
        }

        void Remove(char ch)
        {
            int c = ch - 'a';
            if (window[c] == need[c])
                matches--;
            window[c]--;
            if (window[c] == need[c])
                matches++;
        }

        for (int i = 0; i < s1.Length; i++)
            Add(s2[i]);
        if (matches == 26)
            return true;

        int left = 0;
        for (int right = s1.Length; right < s2.Length; right++)
        {
            Add(s2[right]);
            Remove(s2[left]);
            left++;
            if (matches == 26)
                return true;
        }

        return false;
    }
}

/*
================================================================================
 PATTERN : Fixed Sliding Window - count letter matches in O(1)
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Optimal variant - ties the best complexity by another route
================================================================================
VARIABLES
  need     need[c] = how many times letter c appears in s1
  window   window[c] = how many times letter c appears in the current window of s2
  matches  number of letters c (out of 26) where need[c] == window[c]
  left     index in s2 of the oldest char in the window, removed on the next slide
WHY THIS PATTERN
  A permutation of s1 is any string with the same letter counts. So the question
  is whether some substring of s2 with length s1.Length has counts equal to
  need. Every candidate has the same length, so a window of fixed size that
  slides one step at a time covers all of them. Each slide changes only two
  counts in window: one char comes in (right) and one goes out (left).
BRUTE FORCE
  For each start in s2, take the substring of length s1.Length, sort it, and
  compare it with sorted s1. That costs O((m - n + 1) * n log n), where n =
  s1.Length and m = s2.Length. It is correct, but it rebuilds each window from
  nothing, even though two neighbor windows share all chars except two.
INVARIANT
  After every Add or Remove, matches equals the exact number of letters c with
  need[c] == window[c]. It starts right because the setup loop counts every
  letter where need[c] == 0 as already matching the empty window. So matches ==
  26 means all 26 counts are equal, which means the current window is a
  permutation of s1. Every window of length s1.Length is checked once: the first
  one after the initial Add loop, and the rest inside the right loop.
CHECK BEFORE AND AFTER THE CHANGE
  Add and Remove do not need to know if the count moves toward need[c] or away
  from it. They subtract 1 from matches if the letter was equal before the
  change, and add 1 if it is equal after. This handles all cases the same way:
  equal to not equal, not equal to equal, and not equal to still not equal (the
  -1 and +1 do not fire, or they cancel out).
WATCH OUT
  The code assumes only the letters 'a' to 'z'. An uppercase letter, a digit, or
  any other char makes ch - 'a' fall outside 0..25, and the code throws
  IndexOutOfRangeException. An empty s1 returns true, because matches starts at
  26. That is correct only if your problem defines an empty string as a
  permutation that every s2 contains. All the comments match what the code does.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How would you return every start index where a permutation of s1 begins,
  not just true or false?
     Keep the same loop but do not return early. When matches == 26, record the
     start: 0 after the first fill, and left after each slide. The time stays
     linear, and the output list is extra memory.
  2. What if the chars can be any Unicode, not just lowercase letters?
     Use a Dictionary<char,int> for need and window, and let matches count
     distinct chars of s1 whose counts are equal (target = need.Count, not 26).
     It still runs in linear time, but memory grows with the number of distinct
     chars, and hashing costs more than indexing an array.
  3. What if the window should be the shortest substring of s2 that contains all
  chars of s1, with extra chars allowed?
     That is Minimum Window Substring. The window size is no longer fixed: move
     right until all needs are met, then move left to shrink the window while it
     stays valid. Count a letter as satisfied when window[c] >= need[c], not
     only when the counts are equal.
TRIGGER
  Look for a question about whether some substring of a fixed length is an
  anagram or permutation of a pattern, or has the same letter counts as the
  pattern.
C# NOTE
  Add and Remove are local functions. They capture need, window and matches from
  the method, so they can update matches without ref parameters and without
  writing the same bookkeeping in two places.
COMPLEXITY
  Time  : O(n + m)
  Space : O(1)
================================================================================
*/
