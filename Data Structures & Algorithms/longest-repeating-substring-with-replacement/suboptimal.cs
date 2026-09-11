// ##########################################################################
// #  suboptimal.cs         O(n * k) time / O(1) space
// #  sliding window rerun per candidate target character
// #  [sliding-window-per-char]
// #  ranks below optimal.cs (O(n) time / O(1) space)
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  reruns a full linear sliding-window pass once per distinct character
// #  in the alphabet (bounded constant), multiplying the single-pass cost
// #  by alphabet size, not by k
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
 PATTERN : Per-target sliding window - fix the kept char first
 SOURCE  : YOUR OWN SOLUTION - your own annotation at c76939d
 STATUS  : Suboptimal
================================================================================
WHY THIS PATTERN
  The hard part of this problem is that the window's validity depends on a
  quantity you do not know in advance: which character survives. Fixing
  targetChar up front deletes that unknown. Once targetChar is fixed, the cost
  of a window is exactly (right - left + 1) - targetCount, the count of
  characters that are not targetChar and therefore must be paid for out of k.
  That cost is monotone in the usual two-pointer sense: moving right rightward
  never decreases it, moving left rightward never increases it. Monotone cost is
  precisely the precondition for a shrinking window, so the outer foreach buys
  you a textbook inner loop.
CORRECTNESS
  Two claims carry the proof.
  1. For a fixed targetChar, the inner loop reports the longest valid window
  ending at each right, so its maximum over all right is the best answer for
  that target. Standard, because left is never moved back.
  2. Enumerating only distinctCharacters is enough to cover the true optimum.
  Take any optimal answer window W. The cheapest final character for W is one
  that already occurs most often inside W, so it occurs in W, so it occurs in s,
  so it is in distinctCharacters and gets its own pass. Even the degenerate case
  where W is short enough to be fully rewritten (length <= k) is covered: for
  any target, the deficit is at most the window length, which is at most k, so
  every pass finds that window valid anyway.
  Empty s gives an empty HashSet, the foreach body never runs, and longest stays
  0.
INVARIANT
  At the point where longest is updated, three things hold: targetCount equals
  the number of occurrences of targetChar in s[left..right]; (right - left + 1)
  - targetCount <= k, i.e. the window is affordable; and left is the smallest
  index for which that is true given this right. The measurement sits after the
  while loop precisely so it only ever sees a restored, legal window.
  Note that longest is declared outside the foreach and is deliberately never
  reset - the answer is the max over all target choices, so each pass only ever
  raises the shared high-water mark. left and targetCount, by contrast, are
  re-declared per pass; carrying either across targets would be a bug.
WHY WHILE AND NOT IF
  It is tempting to reason that each new right adds at most one unit of deficit,
  so one shrink step suffices, and to write if instead of while. That is false
  here. Dropping a character equal to targetChar decrements both the window size
  and targetCount, leaving the deficit unchanged - the shrink made no progress.
  Trace s = "aab", k = 0, targetChar = 'a'. At right = 2 the window is "aab"
  with deficit 1. Shrinking past s[0] = 'a' gives "ab", still deficit 1.
  Shrinking past s[1] = 'a' gives "b", still deficit 1. Only the third step,
  which drops the non-target 'b', restores deficit 0. The while ran three times
  for one right. It always terminates because an empty window has deficit 0 <=
  k.
WHY THIS LOSES
  The optimal version is one pass, not one pass per distinct character. Keep an
  int[26] frequency table for the window plus maxFreq, and call the window valid
  when (right - left + 1) - maxFreq <= k. The subtlety interviewers probe is
  that maxFreq is never decremented when left advances, which leaves it stale
  and possibly too large. That is safe because longest can only increase when
  maxFreq genuinely increases; a stale maxFreq makes the window look valid, but
  such a window is never longer than the one that legitimately set maxFreq, so
  it cannot produce a wrong answer, only a wrong intermediate. That gives O(n)
  time with a fixed-size table.
  This file instead re-scans the whole string once per distinct character. For a
  lowercase-only input that is a bounded constant factor, but on an
  unconstrained alphabet a string of n pairwise-distinct characters triggers n
  full passes, i.e. quadratic behaviour on an input the single-pass version
  handles linearly.
WATCH OUT
  The two k's are different quantities. In the code, k is the replacement budget
  from the problem statement. In the complexity line above, k is the number of
  distinct characters, that is distinctCharacters.Count - the number of outer
  passes. They are unrelated; do not conflate them when reciting the analysis.
  Inside the shrink step, the targetCount-- test must read s[left] before left++
  executes. Swapping those two lines silently decrements based on the wrong
  character and corrupts the invariant.
  The shrink loop can push left to right + 1, making the window size 0. That is
  fine and is in fact the natural terminating state for a target that does not
  appear near right; Math.Max just ignores it. It relies on k >= 0, which the
  problem guarantees.
  HashSet iteration order is unspecified and irrelevant here - longest is
  order-independent because it is a max over independent passes.
COMPLEXITY
  Time  : O(n * k)
  Space : O(1)
================================================================================
*/
