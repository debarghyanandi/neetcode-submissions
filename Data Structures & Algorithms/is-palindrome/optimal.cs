// --------------------------------------------------------------------------
//  Reference solution - from NeetCode / other resource (submission-0 + submission-1)
//  Not one you solved yourself.
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
 PATTERN : Two Pointers - Converging from Both Ends
 SOURCE  : NeetCode / other resource (submission-0 + submission-1 merged; the
           custom AlphaNum helper replaced by char.IsLetterOrDigit)
 STATUS  : Optimal
================================================================================

WHY THIS PATTERN
  A palindrome is defined by a symmetry: position i must match position
  n-1-i. Two pointers walking inward test exactly that relation, one pair per
  step, and meet after n/2 comparisons.

BRUTE FORCE (and why it fails)
  Build a cleaned lowercase string, reverse it, compare: O(n) time but O(n)
  EXTRA SPACE for two more strings. Correct, and a fine first answer. The
  two-pointer version reads the original in place for O(1) extra space -
  the improvement here is memory, not time.

INVARIANT
  Everything strictly outside [left, right] has already been verified as
  symmetric. The unverified region shrinks every iteration, so it terminates.

WHY THE INNER SKIP LOOPS ALSO CHECK left < right
  Without it, a string of only punctuation ("...") runs a pointer past the
  other and indexes out of range. The guard is correctness, not tidiness.

ALGORITHM (NeetCode: "Two Pointers")
  1. left at 0, right at the last index.
  2. Advance left past non-alphanumerics; retreat right past them.
  3. Compare case-folded characters; mismatch -> false.
  4. Step both inward and repeat until they cross.

COMPLEXITY
  Time  : O(n) - each pointer only ever moves toward the other, so the two
          together traverse the string once. Nested loops, linear work: the
          amortised argument, same shape as longest-consecutive-sequence.
  Space : O(1) - two ints, no copies of the string.

TRIGGER
  "Palindrome", "mirror", "symmetric", or any condition relating position i
  to position n-1-i. More broadly: two pointers converge when the answer
  depends on a pair drawn from OPPOSITE ENDS of an ordered structure.

C# NOTES
  - char.IsLetterOrDigit is Unicode-aware; the hand-rolled ASCII range check
    is not. For the LeetCode constraints both pass, but the built-in is
    correct for a wider input and needs no maintenance.
  - char.ToLower uses the CURRENT CULTURE. char.ToLowerInvariant is the safer
    default in production - the Turkish dotless-i is the classic bug where
    culture-sensitive casing silently changes behaviour by machine locale.
  - For a truly allocation-free variant, ReadOnlySpan<char> over the string
    gives the same indexing with no copies. Already O(1) here, so it buys
    nothing - but it is the .NET move when substrings ARE involved.

WATCH OUT
  - Only one of the two skip loops uses `left < right` and the other `right >
    left` - they are the same condition written twice. Keep both; dropping
    either reintroduces the out-of-range crash on all-punctuation input.
================================================================================
*/
