// --------------------------------------------------------------------------
//  Reference solution - from NeetCode / other resource (submission-4)
//  Not one you solved yourself.
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
 PATTERN : Sliding Window - Variable Size, with a MONOTONIC best-so-far counter
 SOURCE  : NeetCode / other resource (submission-4)
 STATUS  : Optimal
================================================================================

WHY THIS PATTERN
  suboptimal.cs removes the unknown by trying all 26 targets.
  This version removes it a different way: never name the target at all.
  For ANY window, the cheapest conversion keeps whichever character is most
  frequent and replaces the rest:

      replacements needed = window size - maxFrequency(window)

  One window, one pass, all characters handled simultaneously.

BRUTE FORCE (and why it fails)
  Every substring, counting its dominant character: O(n^2 * 26).
  Per-character windows: O(26n). This: O(n).

THE PART THAT LOOKS LIKE A BUG AND IS NOT
  maxFrequency is NEVER DECREASED when the window shrinks, so it can be
  stale - larger than the true maximum inside the current window. Everyone
  notices this and assumes it is broken. It is not, and here is why:

  A stale maxFrequency only makes the shrink condition FIRE LESS OFTEN, so
  the window may be kept "too wide" for a while. But `longest` can only grow
  past its current value if some window genuinely beats the old record - and
  to do that, some character must reach a HIGHER frequency, which would
  update maxFrequency legitimately. So a stale value can never produce an
  answer larger than the true one. It just avoids recomputing the maximum.

  The window is therefore not guaranteed valid at every step - only the
  RECORDED MAXIMUM is guaranteed correct. That distinction is the whole
  interview question for this problem. Recomputing the true max on every
  shrink is also correct and costs O(26) per step; this skips it.

INVARIANT (the honest one)
  longest never exceeds the true answer, and the true answer is reached at
  the moment maxFrequency legitimately hits its peak.

ALGORITHM (NeetCode: "Sliding Window")
  1. Empty frequency map, left = 0, maxFrequency = 0.
  2. Extend right, incrementing that character's count.
  3. Update maxFrequency with the new count.
  4. While (window size - maxFrequency) > k, decrement s[left]'s count, left++.
  5. Record the window size.

COMPLEXITY
  Time  : O(n) - right advances n times, left advances at most n times total.
  Space : O(a) - one map entry per distinct character; O(1) for a fixed
          alphabet. int[26] makes that literal.

TRIGGER
  "Longest window where at most k elements may be changed / removed /
  tolerated." Same skeleton as "longest subarray with at most k zeros
  flipped" and "at most k distinct characters" - only the validity test
  changes.

C# NOTES
  - int[26] indexed by (c - 'A') is faster here and the constraint is
    uppercase letters; the Dictionary form is kept because it survives a
    constraint change without a rewrite.
  - windowCounts[s[left]]-- is safe: the character is guaranteed present,
    since it entered through the right pointer first.
  - TryGetValue + write is one probe for the read and one for the write.
    CollectionsMarshal.GetValueRefOrAddDefault would make it one total.

WATCH OUT
  - Do not "fix" the stale maxFrequency by recomputing it - it is not a bug,
    and the naive fix costs O(26) per shrink for no gain in the answer.
  - The window size is right - left + 1, not right - left.
================================================================================
*/
