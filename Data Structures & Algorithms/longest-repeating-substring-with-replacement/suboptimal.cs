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
 PATTERN : Sliding Window - one pass per candidate kept character
 SOURCE  : YOUR OWN SOLUTION - your own annotation at c76939d
 STATUS  : Suboptimal
================================================================================
VARIABLES
  longest             best valid window length seen across all passes
  distinctCharacters  the set of characters that actually occur in s
  targetChar          the character this pass keeps; everything else in the window is replaced
  left                left edge of the current window
  targetCount         how many characters equal to targetChar sit inside [left, right]
WHY THIS PATTERN
  The problem asks for the longest substring that becomes one repeated character
  after at most k replacements. A substring is valid exactly when (length -
  count of the most common character) <= k, so the cost only depends on which
  character you decide to keep. Fixing that decision as targetChar turns the
  question into a plain "longest window with at most k bad characters" problem,
  which a two-pointer window solves: grow right, and while (right - left + 1) -
  targetCount > k, push left forward. Trying every character in
  distinctCharacters guarantees the true best keeper is tried.
BETTER APPROACH
  The better solution is a single pass with one frequency table instead of one
  pass per character. Keep count[c] for the whole window and maxCount = the
  largest frequency seen so far; the window is valid when (right - left + 1) -
  maxCount <= k, and left only ever moves forward. That answers in one sweep of
  s, while this file sweeps s once for every entry in distinctCharacters, so the
  work multiplies by the alphabet size actually present in the string.
INVARIANT
  After the while loop on each iteration of right, the window [left, right]
  satisfies (right - left + 1) - targetCount <= k, and targetCount is the exact
  number of targetChar in that window. So every value fed into Math.Max is a
  genuinely achievable length for this targetChar. left never moves backward
  inside a pass, so no valid window is skipped: for each right, left sits at the
  smallest index that keeps the window valid. The optimal answer keeps some
  character, and that character is in distinctCharacters, so one pass finds it.
EXACT COUNT INSTEAD OF STALE MAX
  The famous one-pass version never decreases maxCount even when characters
  leave the window, which makes people doubt it. This file avoids that argument
  entirely: targetCount is decremented in the shrink loop, so it is always the
  true count for the current window and every measured length is really valid.
  The price for that clarity is the outer loop over distinctCharacters.
WATCH OUT
  There is no guard on k. If k is negative, the while condition stays true even
  after left passes right, and left keeps incrementing until s[left] throws
  IndexOutOfRangeException. For empty s, distinctCharacters is empty, the
  foreach body never runs, and 0 is returned, which is correct. Note also that
  the k in the complexity line is the number of distinct characters, not the k
  parameter of this method - two different things with the same name.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Rewrite it as a single pass.
     Replace distinctCharacters and targetChar with an int[26] count array plus
     a maxCount that only grows; shrink with (right - left + 1) - maxCount > k.
     One sweep of s, and maxCount can be stale without breaking the answer
     because a stale value can never report a window longer than one already
     seen.
  2. Return the actual substring, not just its length.
     Store bestLeft alongside longest whenever Math.Max would increase it, then
     return s.Substring(bestLeft, longest). Same cost, one extra int.
  3. The input arrives as a stream you can only read once.
     This version fails, because it re-reads s once per candidate character.
     Only the single-pass frequency version works, since it touches each
     character exactly once.
  4. What if the alphabet is full Unicode instead of a small letter set?
     The outer loop then runs once per distinct code point, which can approach
     the length of s. Switch to the single-pass form with a Dictionary<char,int>
     for counts.
TRIGGER
  A "longest substring after at most k edits/replacements" question, where
  validity depends on which single character you decide to keep.
C# NOTE
  new HashSet<char>(s) works because string implements IEnumerable<char>, but it
  allocates a set and gives no defined iteration order; if the input is known to
  be uppercase letters, a bool[26] seen array filled by one scan does the same
  job with no hashing.
COMPLEXITY
  Time  : O(n * k)
  Space : O(1)
================================================================================
*/
