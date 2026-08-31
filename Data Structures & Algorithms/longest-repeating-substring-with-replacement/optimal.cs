// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
// -  sliding window, monotonic max-frequency tracker
// -  [sliding-window-max-freq]
// -  ranks above suboptimal.cs (O(n * k) time / O(1) space)
// -
// -  Reference solution - not one you solved yourself
// -
// -  single pass window where replacements-needed = window size minus a
// -  never-decreased best-seen character frequency, valid because staleness
// -  can only make the answer conservative, never wrong
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
 PATTERN : Sliding window with a deliberately stale max count
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Optimal
================================================================================
THE REDUCTION
  A window is usable exactly when (right - left + 1) - maxFrequency <= k. Pick
  the character that already dominates the window, leave it alone, and replace
  everything else; the number of replacements needed is window size minus the
  count of the most common character. That single line is the whole problem.
  Everything else in this file is bookkeeping to keep that test cheap.

  windowCounts holds the live per-character counts for [left, right]. longest
  records the best window size ever reached.
THE STALE MAX - THE ONE TRICK
  maxFrequency is the highest single-character count seen in ANY window so far.
  It is raised on line maxFrequency = Math.Max(maxFrequency, count + 1) and is
  never lowered when the shrink loop decrements windowCounts[s[left]].

  So maxFrequency can be larger than the true max count of the current window.
  That is intentional, not a bug. It means the while test is too permissive,
  never too strict: the window may sit in a state that is not actually
  convertible with k replacements, but its size is never larger than something
  already earned.
CORRECTNESS - THE PART TO REHEARSE
  Direction 1, it never under-reports. Every substring of a valid window is
  itself valid: the non-dominant characters of the substring are a subset of the
  non-dominant characters of the parent, so it needs no more than k
  replacements. Now take an optimal window [a, b]. Once left reaches a, every
  window [a, r] with r <= b is genuinely valid, and since maxFrequency is only
  ever an over-estimate the shrink test is even easier to satisfy - so left
  cannot advance past a before right reaches b. The full length b - a + 1 gets
  measured.

  Direction 2, it never over-reports. maxFrequency only ever becomes f because
  some window genuinely contained f copies of one character, and window sizes
  never shrink, so f <= the current window size. Any size L the loop accepts
  satisfies L <= maxFrequency + k: f identical characters plus at most k others
  - a length that really exists in s.
THE WINDOW NEVER SHRINKS
  right advances every iteration; left advances only inside the while loop.
  Because the window was acceptable before this character arrived, the size
  overshoots by at most one, so the while body runs at most once per iteration -
  it could be written as an if with no change in behaviour.

  Consequence worth remembering: the window size is monotonically
  non-decreasing, so longest ends up equal to the final right - left + 1. The
  Math.Max on longest is defensive, not load-bearing.
WATCH OUT
  1. Counts in windowCounts go to zero but the key is never removed. Harmless
  here because the dictionary is only read through TryGetValue on the incoming
  character and decremented on the outgoing one - nothing iterates it looking
  for a max.

  2. Do not "fix" maxFrequency by recomputing it after each shrink. It is
  correct to do so and it still returns the right answer, but it costs a scan of
  the alphabet per shrink and buys nothing.

  3. TryGetValue(s[right], out int count) yields count = 0 for an unseen
  character, which is why the write is unconditionally windowCounts[s[right]] =
  count + 1 and why maxFrequency compares against count + 1, the post-increment
  value.

  4. k = 0 works without a special case: the test becomes size == maxFrequency,
  i.e. the longest run of one character.

  5. The window at return time may be one of the invalid ones. Never report the
  final window as an answer substring without re-validating it.
TRIGGER
  Reach for this shape when the question is "longest / shortest contiguous span
  such that some budget of edits, removals or mismatches is not exceeded," and
  the budget check can be written as a function of the window aggregate rather
  than of the window contents. Here the aggregate is one number, the dominant
  count.
LIKELY FOLLOW-UPS
  "Why is not decreasing maxFrequency correct?" - the answer above; expect this
  one, it is the entire point of the problem.

  "Constrain the alphabet." If s is uppercase A-Z, swap the Dictionary for an
  int[26] indexed by s[i] - 'A'. Same algorithm, fewer hash lookups, and it
  makes an honest recompute-the-max variant cheap enough to mention as a
  fallback.

  "Return the substring, not the length." Store the left index alongside each
  improvement to longest, and re-verify that window against k, since the running
  window can be in an over-permissive state.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
