// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
// -  sliding window, incremental match counter
// -  [sliding-window-incremental-counter]
// -  ranks above suboptimal.cs (O(n * k) time / O(1) space)
// -
// -  Reference solution - not one you solved yourself
// -
// -  Fixed-size window over s2 maintains a `matches` counter of how many of
// -  the 26 letter counts currently agree with need[], updated in O(1) per
// -  add/remove instead of recomputing equality.
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
 PATTERN : Fixed-size sliding window + matched-letter counter
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  "Is some permutation of s1 a substring of s2" is really "does some
  length-s1.Length window of s2 have the same letter multiset as s1".
  Permutation means order is irrelevant, so the only thing a window has to
  expose is its 26 counts. Every candidate window has exactly the same length,
  so the window never has to grow or shrink adaptively - left and right advance
  in lockstep, one char in and one char out per step. That fixed width is what
  lets the counts be maintained incrementally instead of rebuilt.

  The recognition cue: fixed-length window + an equality test on counts. The
  moment you see those two together, reach for a scalar that summarizes the
  equality test so the per-step check is a single comparison rather than a scan.
BRUTE FORCE
  Slide the window and, at each of the s2.Length - s1.Length + 1 positions,
  compare need and window slot by slot (or sort the substring and compare to
  sorted s1). Correct, but every step redoes work the previous step already
  established: adding s2[right] and dropping s2[left] can only change two of the
  26 slots, yet the naive comparison re-reads all of them. The matches counter
  is exactly the memo that removes that rescan.
INVARIANT
  matches = the number of letters c in 0..25 for which window[c] == need[c].

  That is the whole correctness argument. Add and Remove are written to preserve
  it under a single-count change: before touching window[c] they subtract the
  letter's current contribution (if window[c] == need[c], matches--), then
  mutate, then re-add it (if window[c] == need[c], matches++). No other slot
  moved, so no other letter's contribution can be stale.

  When matches == 26 every slot agrees, so the window's multiset equals s1's
  multiset. The window is structurally always exactly s1.Length characters wide
  (right - left == s1.Length - 1 at the point of the check), so equal multisets
  means the window IS a permutation of s1. Both halves are needed - the counter
  proves the contents, the index arithmetic proves the length.
WHY ALL 26 AND NOT JUST THE LETTERS IN S1
  The letters with need[c] == 0 are doing real work: they are what rejects a
  window containing a character s1 never had. If you only tracked the distinct
  letters of s1 and stopped at matches == (distinct count), a window padded with
  a foreign letter would still "match" - but that window is the wrong length or
  has too few of something, so this is really a redundancy that keeps the
  invariant uniform rather than a bug you must handle separately.

  That uniformity is why the second setup loop exists. Before anything is added,
  window is all zeros, so every c with need[c] == 0 already satisfies window[c]
  == need[c] and must be credited up front. Skipping that loop and starting
  matches at 0 would make matches == 26 unreachable whenever s1 uses fewer than
  26 distinct letters, i.e. always. Equivalently: matches starts at 26 minus the
  number of distinct letters in s1.
ALGORITHM
  1. Guard s1.Length > s2.Length - no window of the required width exists, so
  return false before the arrays are even built.
  2. Fill need from s1. Seed matches by counting the c where need[c] ==
  window[c] (all the zero-need letters).
  3. Prime the first window: Add(s2[i]) for i in 0..s1.Length-1.
  4. Check matches == 26 once, right here. This check is not optional decoration
  - the main loop only tests after admitting a NEW character at index right >=
  s1.Length, so window [0, s1.Length-1] would never be examined otherwise.
  5. For right from s1.Length to s2.Length-1: Add(s2[right]), Remove(s2[left]),
  left++, then test matches == 26.
  6. Fall out with false.
WATCH OUT
  Test matches only at the top of a loop iteration's end, never inside Add.
  Between Add(s2[right]) and Remove(s2[left]) the window transiently holds
  s1.Length + 1 characters; a check there would be comparing counts for a window
  that is one char too wide. Here the ordering is harmless precisely because
  both mutations complete before the read - Remove-then-Add would work equally
  well.

  In Add, the two ifs cannot both fire: window[c] == need[c] before the
  increment and window[c] == need[c] after it are mutually exclusive, so matches
  moves by at most 1 per call. Same for Remove. If you ever write these as "if
  equal after, matches++" without the symmetric decrement first, matches drifts
  upward and the function starts returning true for windows that do not match.

  s2[i] - 'a' assumes lowercase ASCII input; there is no bounds check, so any
  other character indexes out of the array or silently into the wrong slot.

  Edge cases fall out for free: s1.Length == s2.Length runs zero loop iterations
  and is decided entirely by the step-4 check; an empty s1 leaves need all
  zeros, matches seeded at 26, and returns true immediately.
FOLLOW-UP: DROP THE 26-LETTER ASSUMPTION
  The interviewer's natural next question is arbitrary characters. Replace the
  two arrays with dictionaries and replace the constant 26 with need.Count:
  matches then counts only the keys present in need, and Add/Remove use the same
  subtract-mutate-re-add ritual around a TryGetValue lookup. The zero-need
  letters can no longer be enumerated, so you lose the free rejection described
  above and must reintroduce the window-width guarantee explicitly - which the
  fixed right - left spacing already gives you, so the algorithm survives
  intact. The 26-slot array is the specialization, not the idea.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
