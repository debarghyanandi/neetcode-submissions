// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
// -  Sliding window, fixed max frequency   [sliding-window-fixed-maxfreq]
// -  ranks above suboptimal.cs (O(n * k) time / O(1) space)
// -
// -  Reference solution - not one you solved yourself
// -
// -  Single pass through string; maxFrequency never decreases, so window
// -  validity checked in amortized O(n) total.
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
 PATTERN : Sliding Window - never-shrinking max frequency count
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Optimal
================================================================================
VARIABLES
  windowCounts    windowCounts[c] = times c appears in s[left..right]
  count           old count of s[right] before this step, from TryGetValue
  maxFrequency    highest single-char count seen in ANY window so far, never lowered
  left            left edge of the current window
  longest         best window length found so far = the answer
WHY THIS PATTERN
  The problem asks for the longest substring you can make uniform using at most
  k replacements. A substring is feasible when (length - count of its most
  common character) <= k, and that cost only grows as the substring grows, so
  the feasible set is a window property: extend right, and pull left forward
  when the cost passes k. windowCounts gives the character counts of the current
  window, maxFrequency stands in for "most common character", and longest
  records the widest window that ever satisfied the test.
BRUTE FORCE
  Try every substring: two nested loops over start and end, counting characters
  inside and checking length - maxCount <= k. That is O(n^2) time with an
  O(alphabet) count array, or O(n^3) if you recount from scratch each time. It
  loses because it re-derives counts for windows that the single left/right pass
  already knows are hopeless.
INVARIANT
  maxFrequency only ever rises, and it rises only to count + 1, the true count
  of s[right] inside the current window at that moment. So each time the window
  is allowed to grow past its previous size, the window really does hold
  maxFrequency copies of one character, and after the while loop it satisfies
  size - maxFrequency <= k, i.e. it is genuinely feasible. Windows that stay the
  same size cannot increase longest, so longest is only ever set from a real
  feasible window - and it can never miss the optimum, because the optimum
  window would force maxFrequency up to its own majority count.
THE WHILE LOOP RUNS AT MOST ONCE
  Each iteration adds exactly one character, so the cost (size - maxFrequency)
  rises by at most one, and maxFrequency never falls. The condition can
  therefore be violated by at most one, and a single left++ fixes it. Replacing
  while with if is equivalent here; this is also why left moves at most n times
  in total.
WATCH OUT
  The comment says "see the note below" but there is no note below in the file -
  the explanation was lost, so write it back or drop the reference. Because
  maxFrequency is stale (possibly larger than the real max of the current
  window), left and right are NOT guaranteed to bound a feasible substring at
  every step; only longest is trustworthy, so do not print s.Substring(left,
  right - left + 1) at the end and expect a valid answer.
  windowCounts[s[left]]-- leaves keys with value 0 in the dictionary, so
  windowCounts.Count is the number of distinct characters seen, not distinct
  characters in the window. Empty s returns 0 correctly, but s is dereferenced
  without a null check.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Return the substring, not just its length.
     You cannot read left/right at the end. Record bestLeft = left inside the
     same if that updates longest - at that moment the window is provably
     feasible - then Substring(bestLeft, longest).
  2. Make maxFrequency exact, so the window is always valid?
     On each left++ recompute the maximum over the counts, O(alphabet) per
     shrink, or keep a count-of-counts bucket array to do it in O(1) amortized.
     The final answer is unchanged; you pay extra work only to get a valid
     window at every step.
  3. Variant - all k replacements must turn characters into one specific target
  letter, chosen in advance.
     Then the window test is simply "number of characters != target <= k", one
     linear pass per candidate target, so O(alphabet * n) total with a single
     counter instead of a map.
  4. The string arrives as a stream you cannot index twice.
     The algorithm already works: it reads s[right] once, and the only backward
     reference is windowCounts[s[left]], so you need a buffer of just the
     current window rather than the whole input.
TRIGGER
  "Longest substring where at most k positions may be changed or violated" - a
  feasibility test that gets monotonically harder as the window widens.
C# NOTE
  TryGetValue(s[right], out int count) followed by windowCounts[s[right]] =
  count + 1 is the right way to avoid ContainsKey plus a read plus a write, but
  it still hashes twice; since the alphabet here is characters, an int[26]
  indexed by s[right] - 'A' (or int[128]) removes the hashing and the leftover
  zero-valued keys entirely.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
