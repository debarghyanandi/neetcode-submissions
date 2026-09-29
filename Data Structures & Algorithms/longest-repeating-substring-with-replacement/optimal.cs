// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
// --------------------------------------------------------------------------

public class Solution
{
    public int CharacterReplacement(string s, int k)
    {
        var windowCounts = new Dictionary<char, int>();

        int left = 0;
        int longest = 0;

        // Highest single-character frequency seen in ANY window so far.
        // Deliberately never decreased - see the note below.
        int maxFrequency = 0;

        for (int right = 0; right < s.Length; right++)
        {
            windowCounts.TryGetValue(s[right], out int count);
            windowCounts[s[right]] = count + 1;

            maxFrequency = Math.Max(maxFrequency, count + 1);

            // Replacements needed = window size - the most common character.
            while ((right - left + 1) - maxFrequency > k)
            {
                windowCounts[s[left]]--;
                left++;
            }

            longest = Math.Max(longest, right - left + 1);
        }

        return longest;
    }
}

/*
================================================================================
 PROBLEM : Given a string s of uppercase letters and an int k, you may change
           at most k characters to any other letter. Return the length of the
           longest substring that can be made of one repeated letter. Example:
           s = "AABABBA", k = 1 -> 4 ("AABA" becomes "AAAA").
 PATTERN : Sliding Window (variable size) + running max frequency
================================================================================
IDEA
  Grow the window with right and count letters in windowCounts. A window is
  valid if (size - maxFrequency) <= k, because those extra letters get
  replaced. If it is invalid, move left one step. maxFrequency is never
  lowered: a stale value can only stop the window from shrinking below the
  best size, and longest only grows when a real higher frequency appears.
EXAMPLE
  s = "AABABBA", k = 1. r=3: window "AABA", maxF=3, 4-3=1 ok, longest=4.
  r=4: "AABAB" needs 2 > 1, drop s[0], left=1. r=5, r=6: each slides by 1.
  r=6: window "ABBA" (left=3), maxF stays 3 (stale, real max is 2). Ans 4.
COMPLEXITY
  Time  O(n)  right and left each move forward at most n times
  Space O(1)  windowCounts holds at most 26 uppercase letters
PATH TO OPTIMAL
  Brute force: every substring, count letters - O(n^2) - simple baseline.
  suboptimal.cs - O(n * k) - one window pass, but extra work per step.
  optimal.cs - O(n) - keep maxFrequency as a running max, no rescans.
KEYWORDS
  sliding window, two pointers, character frequency, at most k replacements
WATCH OUT
  - The comment says "see the note below", but no note exists in the code.
    Be ready to explain the stale maxFrequency trick out loud yourself.
  - Do not decrease maxFrequency when left moves. It is not a bug here, and
    recomputing it correctly costs a 26-count scan each step.
  - The while runs at most once per step, so an if works too. It still
    must use the window size (right - left + 1), not right - left.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Why is a stale maxFrequency still correct?
     -> The answer is only bigger when a window has a larger true max freq.
        Until then the window just slides at the best size, so longest is not
        wrong.
  2. Binary string, flip at most k zeros (Max Consecutive Ones III)?
     -> Same window: count zeros and shrink while zeros > k. O(n) time, O(1)
        space. It is simpler because the target letter is fixed.
  3. Any Unicode letters, not only A-Z?
     -> Keep the Dictionary instead of int[26]. Time stays O(n). Space becomes
        O(distinct chars), still fine because maxFrequency is kept separately.
  4. Another way to solve it?
     -> Binary search on the answer length L. Check each window of size L in
        O(n). Total O(n log n), slower but easy to prove correct.
TRIGGER
  Longest substring where at most k elements break a rule: grow right,
  shrink left.
================================================================================
*/
