// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
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
 PROBLEM : Given a string s, return true if it reads the same forward and
           backward. Only letters and digits count, and case is ignored (A
           equals a). Spaces and punctuation are skipped. Example: "A man, a
           plan" -> false, "race a car" -> false, "Was it a car or a cat I
           saw?" -> true.
 PATTERN : Two Pointers (converging from both ends)
================================================================================
IDEA
  Put left at the start and right at the end of s.
  Move each pointer past characters that are not letters or digits.
  Compare the two characters after lowering them. If they differ, return
  false.
  Otherwise step both pointers inward. When they meet, all pairs matched.
  This is correct because a palindrome means s[i] == s[n-1-i] for the
  filtered string, and the pointers visit exactly those pairs in order.
EXAMPLE
  s = "A b,A" (index 0..4). left=0,right=4: 'a'=='a', so left=1, right=3.
  Skip ' ' so left=2. Skip ',' so right=2. Compare 'b'=='b'. left=3, right=1.
  Loop ends (left > right), return true. Tricky case: "0P" gives '0' vs 'p'.
COMPLEXITY
  Time  O(n)  left and right only move inward, so each char is looked at once
  Space O(1)  only two int pointers, no cleaned copy of s
PATH TO OPTIMAL
  Build a cleaned lowercase copy, reverse it, compare - O(n)/O(n) - simple.
  Build the cleaned copy, then two pointers on it - O(n)/O(n) - no reversal.
  Two pointers on s itself, skip in place (this file) - O(n)/O(1) - no copy.
KEYWORDS
  valid palindrome, two pointers, alphanumeric filter, case-insensitive,
  in-place
WATCH OUT
  - Inner skip loops need the left < right check, or "!!!" walks out of
    bounds.
  - Use IsLetterOrDigit, not IsLetter: with IsLetter, "0P" wrongly returns
    true.
  - char.ToLower uses the current culture (Turkish I); ToLowerInvariant is
    safe.
  - An empty or all-symbol string returns true; say so, the interviewer may
    ask.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Valid Palindrome II - you may delete at most one character.
     -> Same two pointers; at the first mismatch, check if s[left+1..right] or
        s[left..right-1] is a palindrome. Still O(n) time, O(1) space.
  2. Why not just clean the string and reverse it?
     -> It is easier to read, but it uses O(n) extra memory. Two pointers keep
        O(1) space for the same O(n) time.
  3. What if the input is a huge stream you cannot hold in memory?
     -> You cannot go from both ends. Use rolling hashes of the forward and
        the reverse text and compare them: O(n) time, O(1) space, rare false
        positives.
  4. Could you do it recursively?
     -> Yes, recurse on (left+1, right-1), but the call stack costs O(n) space
        and can overflow on long input, so the loop is better.
TRIGGER
  When a problem compares a sequence with its mirror (palindrome, symmetry,
  matching ends), start two pointers at both ends and move them inward.
================================================================================
*/
