// ##########################################################################
// #  suboptimal.cs         O(n * k) time / O(1) space
// #  Sliding window per distinct character   [sliding-window-per-char]
// #  ranks below optimal.cs (O(n) time / O(1) space)
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  Runs a separate sliding window for each of k distinct characters in
// #  the input; k ≤ 26 for lowercase English.
// ##########################################################################

public class Solution
{
    public int CharacterReplacement(string s, int k)
    {
        int longest = 0;
        var distinctCharacters = new HashSet<char>(s);

        // Run a separate sliding window for each candidate "final" character.
        // Inside one pass, everything that is NOT targetChar must be replaced.
        foreach (char targetChar in distinctCharacters)
        {
            int left = 0;
            int targetCount = 0;

            for (int right = 0; right < s.Length; right++)
            {
                if (s[right] == targetChar)
                    targetCount++;

                // Replacements needed = window size minus the kept characters.
                while ((right - left + 1) - targetCount > k)
                {
                    if (s[left] == targetChar)
                        targetCount--;

                    left++;
                }

                longest = Math.Max(longest, right - left + 1);
            }
        }

        return longest;
    }
}

/*
================================================================================
 PATTERN : Sliding Window, one pass per target character
 SOURCE  : YOUR OWN SOLUTION - your own annotation at c76939d
 STATUS  : Suboptimal
================================================================================
VARIABLES
  distinctCharacters  every different character in s; each one is tried as the final letter
  targetChar          the letter this pass tries to keep; all other letters get replaced
  targetCount         how many times targetChar appears in s[left..right]
  longest             the biggest valid window size seen in any pass
WHY THIS PATTERN
  The problem asks for the longest contiguous substring that can become all one
  letter with at most k changes. "Longest contiguous substring under a limit"
  points to a sliding window. If you fix the final letter as targetChar, the
  window s[left..right] is valid exactly when (right - left + 1) - targetCount
  <= k. That count only grows when right moves and only shrinks when left moves,
  so two pointers are enough.
BETTER APPROACH
  The better approach uses one window and an int[26] of counts. It tracks
  maxFreq, the highest count of any single letter in the window, and shrinks the
  window while (window size - maxFreq) > k. That is a single pass over s. This
  file runs a full pass for every distinct letter, so it scans s up to 26 times
  (for uppercase input) where one pass is enough.
INVARIANT
  After the while loop, s[left..right] has at most k characters that are not
  targetChar, so the whole window can become targetChar. left only moves
  forward. If the window starting at left is too big for this right, it is also
  too big for any later right. So no valid window is skipped. The best answer
  uses some final letter, and the pass for that letter finds it, so the max over
  all passes is correct.
WATCH OUT
  The comments match the code. The risk is in the input: nothing in the code
  assumes uppercase letters. If s can hold any character, distinctCharacters can
  be large and the number of passes grows with it. Also, the letter k in the
  complexity line means the number of distinct characters, not the parameter k.
  Do not mix them up when you explain the cost out loud.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. In the one-pass version, why can maxFreq stay the same when left moves and
  a count drops?
     The answer only gets bigger when a window with a higher maxFreq shows up. A
     stale, too-high maxFreq can only keep a window at its old size. It can
     never report a size larger than one that was really valid. You save the
     work of rescanning the 26 counts, but the window may hold an invalid state
     for a short time.
  2. How do you return the substring itself, not only its length?
     Each time longest improves, save left (and the size). At the end, return
     s.Substring(bestLeft, longest). This costs O(1) extra space.
  3. What if each replacement has a different cost, with a total budget k?
     Keep a running sum of the replacement costs for the non-target characters
     in the window, and shrink while the sum is over k. The per-target pass in
     this file adapts easily. The maxFreq trick does not, because the letter
     that is cheapest to keep is not always the most frequent one.
TRIGGER
  The problem asks for the longest contiguous substring where "window size minus
  the count of the kept thing" must stay within a budget.
C# NOTE
  new HashSet<char>(s) hashes every character just to find the candidates. If
  the input is only 'A'..'Z', a bool[26] or int[26] indexed by c - 'A' does the
  same job with a plain array and no hashing.
COMPLEXITY
  Time  : O(n * k)
  Space : O(1)
================================================================================
*/
