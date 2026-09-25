// --------------------------------------------------------------------------
// -  optimal-variant.cs    O(n + m) time / O(1) space
// -  sliding window with frequency counter   [sliding-window-match-counter]
// -  ties with optimal.cs on O(n + m) time / O(1) space
// -
// -  Reference solution - not one you solved yourself (was optimal.cs)
// -
// -  Constant-time validity check via character frequency match counter
// -  eliminates repeated multiset comparisons each iteration.
// --------------------------------------------------------------------------

public class Solution
{
    public bool CheckInclusion(string s1, string s2)
    {
        if (s1.Length > s2.Length)
            return false;

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
            if (need[c] == window[c])
                matches++;
        }

        void Add(char ch)
        {
            int c = ch - 'a';
            if (window[c] == need[c])
                matches--;   // about to break equality (if it was equal)
            window[c]++;
            if (window[c] == need[c])
                matches++;   // may have restored equality
        }

        void Remove(char ch)
        {
            int c = ch - 'a';
            if (window[c] == need[c])
                matches--;
            window[c]--;
            if (window[c] == need[c])
                matches++;
        }

        for (int i = 0; i < s1.Length; i++)
            Add(s2[i]);
        if (matches == 26)
            return true;

        int left = 0;
        for (int right = s1.Length; right < s2.Length; right++)
        {
            Add(s2[right]);
            Remove(s2[left]);
            left++;
            if (matches == 26)
                return true;
        }

        return false;
    }
}

/*
================================================================================
 PATTERN : Fixed-size Sliding Window - 26-letter match counter
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Optimal variant - ties the best complexity by another route
================================================================================
VARIABLES
  need      need[c] = count of letter c in s1
  window    window[c] = count of letter c in the current window of s2
  matches   how many of the 26 letters satisfy window[c] == need[c]
  left      index in s2 of the character leaving the window
WHY THIS PATTERN
  The question asks whether some substring of s2 is a permutation of s1, so
  every candidate substring has exactly the same length, s1.Length. A window of
  fixed width that slides one step at a time visits all candidates, and the only
  thing that matters about a window is its letter counts, not their order.
  Keeping window updated on each slide is cheaper than rebuilding it, and
  matches turns "are all 26 counts equal" into a single integer test.
BRUTE FORCE
  The first thing most people write: for every start index i in s2, take the
  substring of length s1.Length, count its letters (or sort it), and compare
  with s1's counts. That is O(n * m) work because each window is counted from
  scratch, even though two neighbouring windows differ by only two characters.
  This file keeps the same answer but pays O(1) per slide.
INVARIANT
  After every Add/Remove pair, window holds the exact letter counts of s2[left
  .. right] and matches is the exact number of letters c with window[c] ==
  need[c]. Add and Remove keep matches honest by subtracting the letter's
  contribution when equality is about to break and adding it back if the new
  count restores equality. So matches == 26 holds if and only if all counts
  agree, which means the current window is a permutation of s1.
WHY THE INITIAL MATCHES LOOP IS NEEDED
  matches counts equalities, not just satisfied positive needs, so letters
  absent from s1 also count. Before any Add, window is all zeros, so every c
  with need[c] == 0 is already equal and must be counted; starting matches at 0
  would make the 26 test unreachable. That initial loop is what lets the final
  check be a single comparison against the constant 26.
ORDER OF ADD AND REMOVE INSIDE THE SLIDE
  Inside the loop the code calls Add(s2[right]) before Remove(s2[left]), so for
  a moment the window holds s1.Length + 1 characters. That is safe only because
  matches == 26 is tested after Remove: with one extra character the total
  counts cannot all equal need, so no false positive can be read mid-slide.
  Swapping the test into the middle of the pair would be wrong.
WATCH OUT
  Both arrays are size 26 and every index is ch - 'a', so any uppercase letter,
  digit, or space in s1 or s2 gives a negative or out-of-range index and throws.
  An empty s1 returns true immediately: the initial loop sets matches to 26, the
  priming loop adds nothing, and the first check fires - defensible, but know it
  is the behaviour. The comment on Add says "about to break equality (if it was
  equal)" and the guard already checks equality, so the parenthetical is
  redundant, not wrong. Finally, matches is captured by the two local functions,
  so it is shared state; reading it anywhere other than right after a completed
  Add/Remove pair can see a half-updated value.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How would you handle a full Unicode alphabet instead of 26 lowercase
  letters?
     Replace the two arrays with Dictionary<char,int> for need and window, and
     compare matches against need.Count plus the letters seen in the window; you
     then must only track letters that appear in either string, since you can no
     longer enumerate the whole alphabet.
  2. Return every start index where a permutation of s1 occurs, not just true or
  false.
     Same loop, but instead of returning on matches == 26, append left (or right
     - s1.Length + 1) to a List<int> and keep sliding; still one pass, output
     size becomes the extra space.
  3. Can you drop the matches counter?
     Yes - compare the two 26-length arrays on each slide, which is O(26) per
     position instead of O(1); simpler code, same asymptotic result for a fixed
     alphabet, but slower per step and it loses the cheap early exit.
  4. What if s2 arrives as a stream you cannot index backwards?
     Buffer the last s1.Length characters in a circular array to know which one
     to Remove; that keeps the pass single but adds O(s1.Length) space instead
     of O(1).
TRIGGER
  The problem asks about substrings of one fixed length where only the multiset
  of characters matters - anagram or permutation wording.
C# NOTE
  Add and Remove are local functions that mutate need, window and matches from
  the enclosing method, so the compiler puts those locals in a closure object;
  making them static local functions is not possible here without passing all
  three as ref/array parameters. int[26] is the right container - direct
  indexing by ch - 'a' with no hashing - and it is zero-initialised by the
  runtime, which the initial matches loop relies on.
COMPLEXITY
  Time  : O(n + m)
  Space : O(1)
================================================================================
*/
