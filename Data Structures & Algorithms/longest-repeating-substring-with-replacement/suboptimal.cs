// ##########################################################################
// #  suboptimal.cs         O(n * k) time / O(1) space
// #  sliding window rerun per candidate target character
// #  [sliding-window-per-char]
// #  ranks below optimal.cs (O(n) time / O(1) space)
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  enumerates each of the (bounded, constant) distinct characters as the
// #  fixed target and reruns a linear window per candidate, multiplying the
// #  single-pass cost by alphabet size
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
 PATTERN : Sliding Window - one pass per candidate target character
 SOURCE  : YOUR OWN SOLUTION - your own annotation at c76939d
 STATUS  : Suboptimal
================================================================================
THE REFRAME
  The hard version of this problem asks "which character should the window end
  up as?" while the window is still moving. This file refuses to answer that
  question and brute-forces it instead: fix targetChar, then the problem
  collapses into an ordinary sliding window where the rule is dead simple -
  every character in [left, right] that is not targetChar costs one replacement.
  Run that window once per distinct character of s and keep the global max in
  longest.

  The payoff is that inside one pass there is no ambiguity at all: (right - left
  + 1) - targetCount is exactly the number of replacements the current window
  needs, so the feasibility test is a single arithmetic comparison against k.
WHY ONLY CHARACTERS IN S
  The obvious follow-up: the final substring could in principle be made of a
  letter that never appears in s, so why is it safe to iterate only over
  distinctCharacters?

  Split on window length. If the best window is longer than k, then at least one
  of its characters survives unreplaced, and that survivor is the target - it is
  in s by construction. If the best window has length <= k, every character in
  it can be replaced, so its length is at most k - but that window is also found
  by any pass, including targetChar = s[0], since a window of size <= k needs at
  most k replacements regardless of contents. Either way the answer is reached
  by some character already in s. Nothing is missed.

  Corollary edge case: s empty gives an empty distinctCharacters, the foreach
  body never runs, and longest stays 0.
INVARIANT
  At the point where longest is updated, two things hold for the pass over
  targetChar:

  1. targetCount is exactly the number of occurrences of targetChar in
  s[left..right]. It is incremented when right steps onto a targetChar and
  decremented when left steps off one - the two branches are mirror images,
  which is the whole bookkeeping.

  2. (right - left + 1) - targetCount <= k. The while loop is the only thing
  enforcing this, and it runs to completion before Math.Max, so every value fed
  into longest is a genuinely achievable window.

  Read together: longest is the largest window that could be turned into a run
  of targetChar with at most k edits, maximized over every targetChar.
THE SHRINK STEP
  Order matters inside the while body. targetCount must be decremented while
  s[left] is still the character about to leave, then left++ - swap those two
  lines and you decrement based on the wrong index.

  The while could be an if without changing behavior: extending right adds at
  most one non-targetChar, so the deficit grows by at most 1 per iteration and a
  single left++ always restores the invariant. Keeping it as a while is harmless
  and matches the general shape of the pattern, but do not read it as "the
  window can collapse by many positions here" - it cannot.

  Also note left is not reset inside the for loop, only per targetChar. Within
  one pass left only moves right, which is what makes a single pass linear.
WHY THIS LOSES
  This does one full traversal of s for every distinct character, so the same
  string is walked up to 26 times for lowercase or 52 for mixed case. The
  optimal solution walks it once.

  The single-pass version keeps int[26] count for the current window plus
  maxCount, the highest single-character frequency the window has ever held. The
  window is valid when (right - left + 1) - maxCount <= k. The trick that makes
  it work is that maxCount is deliberately never decreased when left advances: a
  stale maxCount is an overestimate of the current window's best character, so
  it can only make the window look worse than it is, never better. The window
  then never shrinks - left advances at most in lockstep with right - and the
  answer is simply the final window size. Stale maxCount can only prevent
  growth, never permit an invalid window, and the size recorded was legitimately
  achieved earlier.

  So the trade here is explicit: this file buys a trivially provable inner loop
  by paying for repeated passes.
TRIGGER
  Reach for the per-target outer loop whenever a window's validity depends on a
  choice you cannot make until the window is known - "longest substring after at
  most k changes to make it uniform" is the archetype. Fixing the choice and
  looping over the small alphabet is the escape hatch when you cannot see the
  maxCount argument under interview pressure.

  The general tell for the inner loop is the phrasing "at most k of X" over a
  contiguous range: expand right unconditionally, and let a monotone left
  restore the budget.
COMPLEXITY
  Time  : O(n * k)
  Space : O(1)
================================================================================
*/
