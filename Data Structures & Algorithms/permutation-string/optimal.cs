// --------------------------------------------------------------------------
//  Reference solution - from NeetCode / other resource (submission-2)
//  Not one you solved yourself.
// --------------------------------------------------------------------------

public class Solution
{
    public bool CheckInclusion(string s1, string s2)
    {
        if (s1.Length > s2.Length) return false;

        int[] need = new int[26];
        int[] window = new int[26];
        int matches = 0; // how many of the 26 letters currently have need[c] == window[c]

        for (int i = 0; i < s1.Length; i++)
        {
            need[s1[i] - 'a']++;
        }

        // A letter with need[c] == 0 already "matches" window[c] == 0 before anything is added.
        for (int c = 0; c < 26; c++)
        {
            if (need[c] == window[c]) matches++;
        }

        void Add(char ch)
        {
            int c = ch - 'a';
            if (window[c] == need[c]) matches--;   // about to break equality (if it was equal)
            window[c]++;
            if (window[c] == need[c]) matches++;   // may have restored equality
        }

        void Remove(char ch)
        {
            int c = ch - 'a';
            if (window[c] == need[c]) matches--;
            window[c]--;
            if (window[c] == need[c]) matches++;
        }

        for (int i = 0; i < s1.Length; i++) Add(s2[i]);
        if (matches == 26) return true;

        int left = 0;
        for (int right = s1.Length; right < s2.Length; right++)
        {
            Add(s2[right]);
            Remove(s2[left]);
            left++;
            if (matches == 26) return true;
        }

        return false;
    }
}

/*
================================================================================
 PATTERN : Sliding Window - FIXED SIZE, with an INCREMENTALLY MAINTAINED
           equality counter
 SOURCE  : NeetCode / other resource (submission-2)
 STATUS  : Optimal - O(n) time, O(1) space
================================================================================

WHY THIS PATTERN
  suboptimal.cs asks "are these two multisets equal?" from scratch at every
  position. This version never asks. It maintains the ANSWER.

  `matches` counts how many of the 26 letters currently satisfy
  window[c] == need[c]. The window changes by one character in and one out
  per step, so at most two letters can change status, and each status change
  is detectable with two comparisons around the mutation:

      was it equal before the change?   -> if so, matches--
      is it equal after the change?     -> if so, matches++

  matches == 26 then means every letter agrees, which is exactly multiset
  equality, tested in O(1).

  This "maintain the aggregate instead of recomputing it" move is the same
  idea as the running sum in minimum-size-subarray-sum and the `have`
  counter in minimum-window-with-characters. Three problems, one technique.

BRUTE FORCE (and why it fails)
  Sorting each substring is O(n m log m); permuting s1 is factorial. Even
  the map comparison in suboptimal.cs is O(n * 26) - fine at these limits,
  wasteful in principle, and it is the version that stops scaling first if
  the alphabet grows.

INVARIANT
  window[] is the exact letter histogram of s2[left..right], the window is
  always exactly s1.Length wide after the seeding loop, and
  matches == |{ c : window[c] == need[c] }|.

WHY 26 AND NOT need.Count
  Counting all 26 letters, including those needed zero times, is deliberate:
  it turns "the window contains at least what is needed" into "the window
  contains exactly what is needed" for free. A letter with need[c] == 0 stops
  matching the moment it enters the window, so any foreign character drops
  matches below 26 automatically. No separate "extra characters" check.
  That is why the seed loop counts the zero-need letters up front.

ALGORITHM (NeetCode: "Sliding Window")
  1. Reject if s1 is longer than s2. Build need[] from s1.
  2. Seed matches by counting the letters where need[c] == window[c] == 0.
  3. Add s2[0..m-1] to the window. If matches == 26, return true.
  4. For right = m..n-1: Add(s2[right]), Remove(s2[left]), left++,
     and test matches == 26.
  5. Return false.

COMPLEXITY
  Time  : O(n + m + 26) = O(n). Every character is added once and removed at
          most once, and each does O(1) work.
  Space : O(1) - two fixed 26-int arrays regardless of input size.

TRIGGER
  Fixed-size window plus an exact-match condition over a small fixed
  alphabet. The moment the validity test is a comparison of two histograms,
  reach for a counter of agreeing buckets rather than the comparison itself.

C# NOTES
  - `ch - 'a'` is char arithmetic promoting to int; it assumes lowercase
    ASCII, which the constraints give. For arbitrary Unicode this becomes a
    Dictionary again and the 26 becomes need.Count with a different framing.
  - Add and Remove are LOCAL FUNCTIONS, not lambdas: they capture `window`,
    `need` and `matches` by reference through a compiler-generated struct,
    with no delegate allocation and no closure on the heap. A
    `Action<char>` lambda here would allocate. This is a real C#-specific
    point worth making in an interview.
  - int[26] on the stack via stackalloc would remove even those two heap
    allocations: `Span<int> need = stackalloc int[26];`.

WATCH OUT
  - Order inside Add/Remove is exact: compare, mutate, compare. Collapsing
    it to a single post-check silently drifts the counter.
  - Add BEFORE Remove in the sliding loop. Removing first would briefly make
    the window m-1 wide and, worse, can underflow a count to -1 when the
    same character is entering and leaving.
  - The seeding `if (matches == 26) return true;` before the loop is not
    redundant - the very first window is never re-tested inside the loop.
================================================================================
*/
