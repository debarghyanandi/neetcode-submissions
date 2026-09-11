// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
// -  sliding window, stale monotonic max-frequency
// -  [sliding-window-max-freq]
// -  ranks above suboptimal.cs (O(n * k) time / O(1) space)
// -
// -  Reference solution - not one you solved yourself
// -
// -  single pass window where validity check uses a never-decreased
// -  best-seen character frequency, so window size is monotonic
// -  non-decreasing and each index is visited O(1) amortized times
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
 PATTERN : Sliding window with a never-decreasing max frequency
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  A window s[left..right] is feasible when you can make it uniform with at most
  k edits. The cheapest way is to keep the most common character in the window
  and replace everything else, so cost = (right - left + 1) - (count of the most
  common char). Feasibility is monotone in the window: shrinking a
  feasible-or-infeasible window never increases its cost, so once a window is
  too expensive, the only way back is to advance left. That monotonicity is what
  licenses two pointers that each move forward only, and it is why left is never
  reset to right + 1 or rewound.
INVARIANT
  At the bottom of each iteration of the for loop, (right - left + 1) -
  maxFrequency <= k. The while loop is the thing that restores it after s[right]
  joins the window. Because longest is only updated after the while loop,
  longest is always the size of some window that satisfies this inequality,
  which is the whole correctness obligation on the lower-bound side: every value
  ever assigned to longest is achievable.
THE STALE MAXFREQUENCY, AND WHY IT IS SAFE
  maxFrequency is the highest count of any single character seen in ANY window
  so far, never recomputed when left advances and windowCounts is decremented.
  So it can be strictly larger than the true max frequency of the current
  window, which means the while loop sometimes stops too early and the window it
  leaves behind is genuinely infeasible.

  That does not corrupt the answer. The window size can only grow past its
  previous record when maxFrequency itself grows, and maxFrequency only grows at
  the line maxFrequency = Math.Max(maxFrequency, count + 1), where count + 1 is
  the count of s[right] in the CURRENT window - an honest number. Right at that
  moment the window satisfies size - (true max freq) <= k, so the record is set
  on a real feasible window. Later iterations carrying a stale maxFrequency can
  only reproduce a window of that same size or smaller, so Math.Max never lifts
  longest above a value that some feasible window actually attained. The answer
  is exact; only the intermediate window is allowed to be a lie.
ALGORITHM
  1. windowCounts maps char to its count inside [left, right]; left = 0, longest
  = 0, maxFrequency = 0.
  2. Expand: TryGetValue on s[right] to read count (0 if absent, which is why
  out int count plus count + 1 is used instead of indexing), then store count +
  1.
  3. Raise maxFrequency to count + 1 if that is larger.
  4. While (right - left + 1) - maxFrequency > k, decrement
  windowCounts[s[left]] and advance left. Note maxFrequency is untouched here.
  5. Record longest = max(longest, right - left + 1).
  6. Return longest.
WATCH OUT
  - The while loop body runs at most once per iteration: adding s[right] grows
  the size by exactly 1 and maxFrequency by at most 1, so the cost rises by at
  most 1 above a value that was already <= k. It could be written as an if;
  written as while it still amortizes to O(n) total left moves, which is what
  gives the linear bound.
  - windowCounts IS decremented on shrink even though maxFrequency is not.
  Dropping that decrement breaks the counts for every later character and the
  answer with it.
  - Entries are left in windowCounts at count 0 rather than removed. Harmless,
  and the map never exceeds the alphabet size.
  - Return longest, not right - left + 1. The final window is not necessarily
  the best one, and may be one of the infeasible stale-maxFrequency windows.
  - k = 0 degenerates correctly to the longest run of one repeated character;
  empty s skips the loop and returns 0.
IF ASKED TO DEFEND IT
  Two likely interviewer probes. First, "print the window you found" - you
  cannot, from this code, because left/right at the end need not bracket an
  optimal window; you would have to also store the left index at each point
  where longest improves. Second, "make the window always valid" - recompute the
  current max inside the shrink step, either by scanning the 26 letter counts or
  by tracking the max properly, which costs an extra alphabet factor per shrink
  and buys nothing for the returned number. Being able to say why the cheap
  version is still exact, rather than calling it a trick you memorized, is the
  actual point of this problem.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
