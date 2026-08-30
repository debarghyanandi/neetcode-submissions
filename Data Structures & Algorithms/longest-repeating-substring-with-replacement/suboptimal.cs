// ##########################################################################
// #  YOU SOLVED THIS YOURSELF  (submission-3, marked '// My solution')
// #  your own per-character window - see optimal.cs for the standard trick
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
 PATTERN : Sliding Window - Variable Size, run once PER CANDIDATE CHARACTER
 SOURCE  : YOUR OWN SOLUTION (submission-3, marked '// My solution')
 STATUS  : Sub-optimal (O(26n) - correct, and a genuinely good first answer)
================================================================================

WHY THIS PATTERN
  The problem has a hidden unknown: WHICH character the final run is made of.
  The honest first move is to remove the unknown by brute-forcing it - try
  every candidate, and for each one the problem collapses to a clean, easy
  window: "longest stretch where at most k characters are not targetChar."

  This is a real technique, not a beginner's detour. "Enumerate the small
  unknown, then solve the easy version" is how a lot of hard problems get
  cracked. Keep it; it is worth more than memorising the trick in 03.

BRUTE FORCE (and why it fails)
  Check every substring, counting its most frequent character: O(n^2) or
  worse. This version is already linear-with-a-constant.

INVARIANT (within one target pass)
  s[left..right] can be turned into a run of targetChar using at most k
  replacements, because (window size - targetCount) <= k.

WHY THIS IS SUB-OPTIMAL
  It repeats the whole scan once per distinct character: O(26n) for lowercase
  input. optimal.cs collapses all 26 passes into one by tracking the
  highest frequency seen instead of a fixed target. Both are O(n) formally -
  the alphabet is a constant - so this is a CONSTANT-FACTOR loss, not an
  asymptotic one. Worth saying precisely in an interview: "mine is O(26n),
  the standard one is O(n); same class, 26x the work."

ALGORITHM
  1. Collect the distinct characters of s.
  2. For each one, slide a window over s:
       - extend right, counting occurrences of targetChar
       - while (window size - targetCount) > k, shrink from left
       - record the window size
  3. Return the largest window over all targets.

COMPLEXITY
  Time  : O(a * n) where a = distinct characters (<= 26 here). Each pass is
          O(n) because left never moves backward within that pass.
  Space : O(a) for the set of distinct characters.

TRIGGER
  A window problem where one parameter is unknown but drawn from a SMALL
  FIXED SET. Enumerate it and solve the easy inner problem.

C# NOTES
  - new HashSet<char>(s) works because string implements IEnumerable<char>.
  - Resetting left and targetCount at the top of each foreach body is
    essential - leaking state between passes is the bug to look for.

WATCH OUT
  - `(right - left + 1) - targetCount` is the number of replacements needed.
    Deriving that expression is the actual insight of this problem; the rest
    is standard window mechanics.
  - Decrement targetCount BEFORE left++ when the evicted character is the
    target, or the count drifts out of sync with the window.
================================================================================
*/
