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
 PATTERN : Sliding Window - grow-only window, stale max count
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Optimal
================================================================================
VARIABLES
  windowCounts   windowCounts[c] = how many times c appears in s[left..right]
  maxFrequency   highest count of one character seen in any window so far; it never goes down
  count          count of s[right] in the window before this step (0 if the key is new)
  longest        largest window size right - left + 1 seen so far
WHY THIS PATTERN
  The problem asks for the longest substring (a contiguous run) that you can
  make all one letter with at most k changes. A window is valid when (right -
  left + 1) - maxFrequency <= k: every character except the most common one must
  be replaced. Moving right one step and moving left forward only when needed
  means each index enters and leaves the window once. There is no need to test
  every substring.
BRUTE FORCE
  For each start index, move the end forward and keep a count of each character
  in the substring and its top count. Record the length while (length - top
  count) <= k, and stop at the first failure. This is always correct, but it is
  O(n^2) time, because every start scans again from scratch. The sliding window
  reuses the counts from the previous window.
INVARIANT
  The window size (right - left + 1) never gets smaller. At each step it either
  grows by one or slides one place to the right. It can only grow when some
  window reaches a new, higher maxFrequency. So longest only goes up when a
  truly valid window of that size exists. Windows that look valid only because
  maxFrequency is stale cannot be longer than one that was valid earlier, so
  they never raise the answer by mistake.
WHY MAXFREQUENCY IS NEVER LOWERED
  When s[left] leaves the window, the real top count can drop, but maxFrequency
  is not updated. This is safe: a smaller top count can only allow a shorter
  window, and we already found a longer one. To beat longest, the window needs a
  count higher than maxFrequency, and that updates the variable right away.
  Because of this, the while loop runs at most once per step, so it could be an
  if.
WATCH OUT
  The comment "see the note below" points to a note that does not exist. The
  explanation is missing from the file. The comment "Replacements needed =
  window size - the most common character" is true only when maxFrequency is up
  to date. With a stale value the code underestimates the replacements, so the
  final s[left..right] may not be a valid window. Return longest and never the
  window itself. A null s throws on s.Length.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How would you return the substring itself, not only its length?
     Keep maxFrequency exact. After each shrink, recompute it as the max over
     windowCounts, with a real while loop. Then every window is truly valid, and
     you save left when longest changes. The cost is an extra scan over the
     alphabet at each step.
  2. What if the replacements must all be to one given letter c?
     Count only the characters that are not c in the window, and shrink while
     that count is > k. This is the same as "Max Consecutive Ones III", and you
     do not need maxFrequency.
  3. Is there another approach besides the sliding window?
     Binary search on the answer length L. For each L, slide a window of fixed
     size L and check if (L - top count) <= k for some window. This takes O(n
     log n) time, which is slower but easy to prove correct.
TRIGGER
  Look for this: the longest contiguous substring where at most k elements break
  a rule that depends on counts inside the window.
C# NOTE
  TryGetValue followed by windowCounts[s[right]] = count + 1 does two hash
  lookups per character.
  CollectionsMarshal.GetValueRefOrAddDefault(windowCounts, s[right], out _)++
  does one. If the alphabet is known to be small, a fixed int array indexed by
  the character works too.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
