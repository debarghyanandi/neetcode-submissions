// ##########################################################################
// #  suboptimal.cs         O(n * k) time / O(1) space
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
 PROBLEM : Given a string s of uppercase letters and an int k, you may replace
           at most k characters with any other letter. Return the length of
           the longest substring that can be made of one repeated letter.
           Example: s = "AABABBA", k = 1 -> 4.
 PATTERN : Sliding Window (variable size), once per target letter
================================================================================
IDEA
  For each letter targetChar in distinctCharacters, run its own window.
  Inside the window, keep targetChar and replace every other letter.
  The cost is (right - left + 1) - targetCount. While it is above k, shrink
  from left. Every valid window is a real answer, and every letter gets its
  own pass, so the true best letter is always tried. optimal.cs uses one pass
  with a max frequency count instead.
EXAMPLE
  s = "AABABBA", k = 1. Target A: window grows to "AABA" (cost 1) -> 4, then
  at right=4 it shrinks down to left=3. Target B: best is "BABB" (2..5) -> 4.
  Answer: 4.
COMPLEXITY
  Time  O(n * k)  one O(n) window pass per distinct letter; left and right
                  only move right
  Space O(1)      HashSet holds at most the alphabet, a fixed 26 letters
WATCH OUT
  - The "k" in O(n * k) is the number of distinct letters, not the parameter
    k.
    Say this clearly in the interview, or it sounds wrong.
  - Reset left and targetCount for each targetChar. If you reuse them across
    passes, the windows mix and the result is wrong.
  - Lower targetCount only when s[left] == targetChar. If you lower it for
    every letter, the cost looks too high and the answer comes out too small.
  - Take longest after the while loop, not before it. Before the loop the
    window can still cost more than k.
================================================================================
*/
