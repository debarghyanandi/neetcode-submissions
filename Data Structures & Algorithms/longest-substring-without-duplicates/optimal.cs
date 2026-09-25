// ##########################################################################
// #  optimal.cs            O(n) time / O(1) space
// #  sliding window, hash-map position jump   [hashmap-lastindex]
// #  ties with optimal-variant.cs on O(n) time / O(1) space
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  Single pass with O(1) dictionary lookups; left pointer jumps directly
// #  past previous duplicate occurrence.
// ##########################################################################

public class Solution
{
    public int LengthOfLongestSubstring(string s)
    {
        // character -> the LAST index at which it was seen
        var lastSeenIndex = new Dictionary<char, int>();

        int left = 0;
        int longest = 0;

        for (int right = 0; right < s.Length; right++)
        {
            char current = s[right];

            if (lastSeenIndex.TryGetValue(current, out int previousIndex))
            {
                // Math.Max is doing real work here, not defensive coding:
                // the previous sighting may be OUTSIDE the current window,
                // in which case it is stale and left must NOT move backward.
                left = Math.Max(left, previousIndex + 1);
            }

            lastSeenIndex[current] = right;
            longest = Math.Max(longest, right - left + 1);
        }

        return longest;
    }
}

/*
================================================================================
 PATTERN : Sliding Window - jump left past last duplicate
 SOURCE  : YOUR OWN SOLUTION - your own annotation at c76939d
 STATUS  : Optimal
================================================================================
VARIABLES
  lastSeenIndex   lastSeenIndex[c] = last index where char c appeared
  left            start of the current duplicate-free window
  longest         best window length found so far
  current         s[right], the character entering the window
  previousIndex   the stored last index of current, may be stale
WHY THIS PATTERN
  The answer is a contiguous run of characters with no repeats, so every
  candidate is a window [left, right]. Growing right by one can only break the
  rule in one way: the new character current already sits inside the window.
  Storing lastSeenIndex lets you fix that in one move - push left to
  previousIndex + 1 - instead of shrinking one step at a time. Each index is
  visited once by right and left never goes back.
BRUTE FORCE
  Take every start i, walk forward with a HashSet, and stop when a character
  repeats; keep the longest run. That is O(n^2) time and correct, but it
  re-scans the same prefix for every start. The dictionary of last positions
  removes the rescan because a repeat tells you exactly where the next legal
  window begins.
INVARIANT
  At the top of each iteration, s[left..right-1] contains no repeated character,
  and lastSeenIndex holds the newest index of every character seen so far in the
  whole string. After the Math.Max, left is past any earlier copy of current, so
  s[left..right] is also repeat-free, and its length right - left + 1 is
  compared against longest. Since every valid window ends at some right, and at
  that right the code holds the longest valid window ending there, the maximum
  over all right is the answer.
WHY THE STALE ENTRY IS SAFE
  Entries are never deleted when left moves forward, so lastSeenIndex can name a
  position before left. The Math.Max clamp is what makes that harmless: a
  previousIndex below left produces a smaller candidate and is ignored. The
  alternative design - removing characters from a HashSet as left advances -
  needs a loop per step and gives no speed win.
WATCH OUT
  The O(1) space figure rests on the key type being char, so the dictionary
  holds at most one entry per distinct character in s; it is not constant in n
  in a literal sense, and stale entries are never pruned, so the dictionary only
  grows. An empty string returns 0 correctly because the loop body never runs
  and longest stays 0. If you ever change left = Math.Max(left, previousIndex +
  1) to a bare left = previousIndex + 1, the window silently moves backward on a
  stale hit and longest becomes too large - that single Math.Max is the whole
  correctness of the jump.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. The input is ASCII only. Can you drop the Dictionary?
     Use an int[128] array of last-seen indexes filled with -1, and index it by
     current. Same logic, no hashing and no allocation per distinct char, but it
     hard-codes the alphabet.
  2. Return the substring itself, not just its length.
     Record bestLeft = left whenever right - left + 1 beats longest, then return
     s.Substring(bestLeft, longest) at the end. One extra int, no change to the
     loop.
  3. Allow at most k repeats of any character instead of zero.
     Switch lastSeenIndex to a count map and shrink left in a while loop while
     the count of current exceeds k. The single jump no longer works, because
     the fix point depends on counts, not on one previous index.
  4. The input arrives as a stream you cannot index.
     The algorithm still works if you keep a running position counter instead of
     right, since it only ever reads the current character and past positions;
     you just cannot recover the substring text without buffering from left
     onward.
TRIGGER
  Longest or shortest contiguous run under a rule that a single arriving element
  can break, where the last position of that element tells you exactly where the
  next legal window starts.
C# NOTE
  TryGetValue with the out int previousIndex pattern does one hash lookup and
  gives both the presence check and the value; ContainsKey followed by an
  indexer read would hash current twice.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
