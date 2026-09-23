// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
// -  Two-pointer, converging ends   [two-pointer]
// -  the only solution in this folder
// -
// -  Reference solution - not one you solved yourself
// -
// -  Each character is visited at most once by the converging pointers, and
// -  all character checks are constant-time operations.
// --------------------------------------------------------------------------

public class Solution
{
    public bool IsPalindrome(string s)
    {
        int left = 0;
        int right = s.Length - 1;

        while (left < right)
        {
            // Skip anything that is not part of the comparison.
            while (left < right && !char.IsLetterOrDigit(s[left]))
                left++;

            while (right > left && !char.IsLetterOrDigit(s[right]))
                right--;

            if (char.ToLower(s[left]) != char.ToLower(s[right]))
                return false;

            left++;
            right--;
        }

        return true;
    }
}

/*
================================================================================
 PATTERN : Two Pointers - skip non-alphanumeric, compare inward
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Optimal
================================================================================
VARIABLES
  left     index scanning from the front, first unchecked character
  right    index scanning from the back, last unchecked character
WHY THIS PATTERN
  The question asks whether the string reads the same forward and backward after
  dropping punctuation and case. That is a symmetric comparison between position
  left and its mirror position right, so one pass from both ends is enough. The
  two inner while loops push left and right past characters that do not count,
  so the outer comparison only ever sees letters and digits.
BRUTE FORCE
  The obvious first attempt is to build a cleaned string: loop over s, keep only
  char.IsLetterOrDigit, lowercase it, then compare that string with its reverse.
  It is still O(n) time but it allocates two extra strings of size n, so O(n)
  space. This file loses nothing in speed and keeps space at a constant two
  integers.
INVARIANT
  At the top of the outer while loop, every pair of positions already consumed
  outside the window [left, right] has matched after case folding, and all
  characters outside that window are either matched or ignorable. Each iteration
  either shrinks the window or returns false. If left and right cross without a
  mismatch, every kept character has been paired with its mirror, so the string
  is a palindrome.
THE GUARDS INSIDE THE SKIP LOOPS
  Both skip loops re-test left < right (and right > left) inside their own
  condition. Without that guard a string with no letters or digits at all, like
  ",.;", would run left past the end of the string and throw an
  IndexOutOfRangeException. With the guard, left and right meet in the middle,
  the outer loop condition left < right fails, and the method returns true.
WATCH OUT
  When the skip loops stop because left == right, the code still runs the
  comparison char.ToLower(s[left]) != char.ToLower(s[right]). That compares a
  character with itself, so it is always false and harmless, but it means one
  wasted comparison on odd-length inputs and on all-punctuation inputs.
  char.ToLower uses the current culture; in a Turkish culture 'I' does not lower
  to 'i', which can change the answer for inputs containing I or i.
  char.ToLowerInvariant is the safer call here. Also note s is never
  null-checked, so a null argument throws on s.Length.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. The input is a stream too large to hold in memory. How does this change?
     Two pointers need random access from both ends, so it breaks. You would
     need to buffer the whole stream, or read from two file handles seeking from
     each end, which trades memory for seek cost.
  2. What if the palindrome check must treat accented letters as equal to their
  plain form, so "Café" matches "éfac"?
     Normalize each character with string.Normalize(NormalizationForm.FormD) and
     drop combining marks before comparing. That costs an allocation per
     character or a pre-pass, so the O(1) space is lost unless you fold per
     character in place.
  3. Allow removing at most one character and still call it a palindrome.
     On the first mismatch, recurse or loop twice: check the substring (left+1,
     right) and (left, right-1). Still O(n) time because each helper scan runs
     at most once, still O(1) space.
  4. Return the longest palindromic substring instead.
     Two pointers from the ends no longer applies; you expand around each of the
     2n-1 centers for O(n^2) time and O(1) space, or use Manacher's algorithm
     for O(n).
TRIGGER
  Reach for this when a problem compares a sequence against its mirror, or pairs
  the ends of a sorted or symmetric input, and you want to avoid building a
  copy.
C# NOTE
  char.IsLetterOrDigit and char.ToLower take a char, so no substring or string
  allocation happens anywhere in this method; indexing s[left] on a string is a
  direct read. Prefer char.ToLowerInvariant to make the case folding independent
  of the thread's current culture.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
