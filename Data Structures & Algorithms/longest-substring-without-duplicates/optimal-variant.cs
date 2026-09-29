// --------------------------------------------------------------------------
// -  optimal-variant.cs    O(n) time / O(1) space
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
 PATTERN : Sliding Window - shrink left until the new char is unique
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Optimal variant - ties the best complexity by another route
================================================================================
VARIABLES
  windowChars  the exact set of chars in s[left..right-1] (then s[left..right] after Add)
  left         index of the first char of the current window
  longest      the longest duplicate-free window length seen so far
WHY THIS PATTERN
  The problem asks for the longest contiguous substring with no repeated
  characters. "Contiguous" and "longest" together point to a window with two
  ends. If a window has no duplicates, every smaller window inside it also has
  none. So when s[right] creates a duplicate, we only need to move left forward
  and never back. windowChars lets us check in constant time whether s[right] is
  already inside the window.
BRUTE FORCE
  Try every start index i. From i, extend j to the right and add s[j] to a fresh
  set. Stop at the first repeat and record j - i. This is correct, and it runs
  in O(n^2) time, or O(n * alphabet) if you note that a run can never be longer
  than the alphabet. It is slower because each new start rebuilds the set from
  nothing. The window keeps the work from the previous start and only removes
  the chars that fall off the left.
INVARIANT
  After the while loop, windowChars holds exactly the chars of s[left..right-1],
  and none of them is s[right]. After the Add, s[left..right] has no duplicates.
  left only moves when a duplicate forces it to. So for each right, left is the
  smallest start that gives a valid window ending at right. That means right -
  left + 1 is the best length for windows ending at right. longest takes the
  maximum over all right, so it is the answer.
THE NESTED WHILE IS NOT QUADRATIC
  The while loop inside the for loop looks like O(n^2), but it is not. Each
  index is added to windowChars once and removed at most once. left only moves
  forward and never passes right. So across the whole run, the while loop does
  at most s.Length steps in total. This is called "amortized" cost: the total
  work is spread over all iterations.
WATCH OUT
  C# char is one UTF-16 code unit, not one visible character. An emoji or other
  surrogate pair counts as two chars here, so the length is counted in code
  units. The O(1) space bound only holds if the character set is fixed.
  windowChars grows with the number of distinct chars, so if the alphabet is not
  bounded, space is really O(min(n, alphabet)). The check is case-sensitive: 'a'
  and 'A' count as different characters.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can left jump straight past the duplicate instead of stepping one char at a
  time?
     Yes. Keep a Dictionary<char,int> from each char to its last index, and set
     left = Math.Max(left, last[c] + 1). The Max is needed because an old index
     may sit before left. You get the same bound with fewer operations, but you
     must think about stale entries (old indexes that are no longer inside the
     window).
  2. What if the input is known to be ASCII?
     Replace the HashSet with bool[128] or int[128] indexed by the char. This
     removes hashing and gives a truly fixed-size table. The cost is that it
     breaks on any char above 127 unless you make the array larger.
  3. Longest substring with at most k distinct characters?
     Use the same window, but store a count per char in a Dictionary<char,int>.
     Shrink from the left while the dictionary has more than k keys, and remove
     a key when its count reaches 0. This version needs counts because a single
     yes/no set is not enough.
TRIGGER
  When you see "longest or shortest contiguous substring or subarray where some
  condition holds", and shrinking a valid window keeps it valid, use a sliding
  window.
C# NOTE
  HashSet.Add returns false when the item is already in the set. So the Contains
  check and the Add can be merged into one loop: while
  (!windowChars.Add(s[right])) windowChars.Remove(s[left++]);
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
