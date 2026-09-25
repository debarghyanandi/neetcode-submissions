// --------------------------------------------------------------------------
// -  optimal-variant.cs    O(n) time / O(1) space
// -  sliding window, hash-set incremental shrink   [hashset-sliding-shrink]
// -  ties with optimal.cs on O(n) time / O(1) space
// -
// -  Reference solution - not one you solved yourself
// -
// -  Two-pointer window; each character added/removed at most once despite
// -  nested while loop, giving O(n) total.
// --------------------------------------------------------------------------

public class Solution
{
    public int LengthOfLongestSubstring(string s)
    {
        // The exact set of characters currently inside the window.
        var windowChars = new HashSet<char>();

        int left = 0;
        int longest = 0;

        for (int right = 0; right < s.Length; right++)
        {
            // Shrink from the left ONE STEP AT A TIME until the duplicate is gone.
            while (windowChars.Contains(s[right]))
            {
                windowChars.Remove(s[left]);
                left++;
            }

            windowChars.Add(s[right]);
            longest = Math.Max(longest, right - left + 1);
        }

        return longest;
    }
}

/*
================================================================================
 PATTERN : Sliding Window - grow right, shrink left on duplicate
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Optimal variant - ties the best complexity by another route
================================================================================
VARIABLES
  windowChars  the exact set of characters sitting in s[left..right-1]
  left         first index still inside the window
  longest      best window length seen so far
WHY THIS PATTERN
  The problem asks for the longest contiguous piece of s with no repeated
  character. "Contiguous" plus "a condition that only breaks when you add a
  character" is the sliding window signal: if s[left..right] has a duplicate, no
  longer window starting at the same left can fix it, so left only needs to move
  forward. windowChars answers "is this character already inside?" in constant
  time, which is the only question the window has to answer.
BRUTE FORCE
  The first thing most people write is two nested loops: for every start index,
  extend the end and keep a HashSet until a repeat appears, then record the
  length. That is O(n^2) time because every start index rescans the characters
  after it. It loses because it throws away the work done for start index left
  when it moves to left+1, even though most of that window is still
  duplicate-free.
INVARIANT
  At the top of each iteration of the for loop, windowChars holds exactly the
  characters of s[left..right-1] and they are all distinct. The while loop
  removes s[left] and advances left until s[right] is gone from the set, so
  after it the window with s[right] added is still all distinct; longest is then
  updated with right - left + 1. Since left never moves backward and the loop
  checks the longest valid window ending at every right, the maximum over all
  right is the global answer.
LEFT NEVER MOVES BACKWARD
  The while loop is nested inside the for loop, but it is not quadratic work.
  left starts at 0, only ever increases, and never passes right, so across the
  whole run it advances at most n times in total. Each character is added to
  windowChars once and removed at most once. That amortized argument - not the
  shape of the loops - is what makes the pass linear.
WATCH OUT
  The while loop is safe only because s[right] can be in windowChars just once,
  at some index >= left; if you ever added a character without removing its old
  copy, the loop could run past right or never stop. The order inside the while
  matters: remove s[left] first, then increment left - swapping the two lines
  drops the wrong character and silently corrupts the set. char in C# is a
  UTF-16 code unit, so a character outside the Basic Multilingual Plane is two
  chars here and its two surrogate halves are treated as separate, unrelated
  characters. The O(1) space claim rests on a bounded alphabet; with arbitrary
  Unicode code units windowChars can hold up to the number of distinct units. An
  empty string returns 0 because the for loop never runs, which is correct.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Return the substring itself, not its length.
     Record bestLeft = left whenever longest is updated, then return
     s.Substring(bestLeft, longest). Same complexity, one extra int.
  2. Can you remove the inner while loop entirely?
     Keep a Dictionary<char,int> lastIndex and set left = Math.Max(left,
     lastIndex[c] + 1) when c repeats, so left jumps in one step. Still linear,
     but the dictionary keeps every character ever seen, not just the window.
  3. Variant - longest substring with at most K distinct characters.
     Replace the set with a Dictionary<char,int> of counts; after adding
     s[right], shrink from the left while the dictionary has more than K keys,
     removing a key when its count hits zero.
  4. The input arrives as a stream you cannot index.
     The algorithm is already a single forward pass, but it reads s[left] to
     shrink, so you must buffer the current window in a queue and dequeue
     instead of indexing; memory then grows with the window, not the input.
TRIGGER
  Longest or shortest contiguous run where the condition can only be broken by
  the newly added element and is fixed by dropping elements from the front.
C# NOTE
  HashSet<char> hashes every Add, Remove and Contains and allocates on the heap;
  if the alphabet is known to be ASCII, Span<bool> seen = stackalloc bool[128]
  indexed by s[right] gives the same three operations with a direct array index
  and no allocation.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
