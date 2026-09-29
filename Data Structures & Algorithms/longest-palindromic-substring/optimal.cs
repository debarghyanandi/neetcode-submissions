// --------------------------------------------------------------------------
// -  optimal.cs            O(n^2) time / O(1) space
// --------------------------------------------------------------------------

public class Solution
{
    public string LongestPalindrome(string s)
    {
        int start = 0;
        int maxLength = 0;

        for (int i = 0; i < s.Length; i++)
        {

            // Odd-length palindrome
            Expand(i, i);

            // Even-length palindrome
            Expand(i, i + 1);
        }

        return s.Substring(start, maxLength);

        void Expand(int left, int right)
        {
            while (left >= 0 && right < s.Length &&
                   s[left] == s[right])
            {

                int length = right - left + 1;

                if (length > maxLength)
                {
                    maxLength = length;
                    start = left;
                }

                left--;
                right++;
            }
        }
    }
}

/*
================================================================================
 PROBLEM : Given a string s, return its longest substring that reads the same
           forwards and backwards. A substring must be contiguous. If two
           answers have the same length, any one is accepted. Example: "babad"
           -> "bab" ("aba" is also valid).
 PATTERN : Expand Around Center (two pointers moving outward)
================================================================================
IDEA
  Every palindrome has a center: one char (odd length) or a gap between two
  chars (even length). For each i, the code calls Expand(i, i) and then
  Expand(i, i + 1). Expand moves left and right outward while the chars
  match, and saves a longer match in start and maxLength. The code tries all
  2n - 1 centers, and each one grows to its widest palindrome, so it cannot
  miss the longest one.
EXAMPLE
  s = "abbab". i=0: odd "a" -> maxLength=1, start=0.
  i=1: even (1,2) "bb" -> 2, start=1; then (0,3) "abba" -> 4, start=0.
  i=3: odd (2,4) "bab" has length 3, which is not > 4, so no update.
  Return s.Substring(0, 4) = "abba".
COMPLEXITY
  Time  O(n^2)  2n - 1 centers, and each can expand up to n/2 steps
  Space O(1)    only index variables; one Substring for the result
PATH TO OPTIMAL
  Check every substring for palindrome - O(n^3) - the simple baseline.
  DP table dp[i][j] = s[i]==s[j] && dp[i+1][j-1] - O(n^2) time and space;
  each check is O(1) because it reuses the smaller answer.
  Expand around center (this file) - O(n^2) time, O(1) space; no table.
KEYWORDS
  palindrome, expand around center, two pointers, odd/even center, DP,
  Manacher
WATCH OUT
  - Forgetting the even center Expand(i, i + 1): "abba" would then return
    only "bb"? No, only "a". Even palindromes are missed completely.
  - If you compute the length after the while loop ends, it is
    right - left - 1, not + 1. The pointers have gone one step too far.
  - C# Substring(start, length) takes a length, not an end index.
  - Ties keep the first one found (strict >). s = "" returns "".
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you do better than O(n^2)?
     -> Manacher's algorithm reuses mirror radii inside the current rightmost
        palindrome, so it runs in O(n) time with O(n) space. It is hard to code,
        so mention it and explain the idea, then write the center version.
  2. Count all palindromic substrings (LC 647).
     -> Use the same two Expand calls, but add 1 to a counter at each
        successful step instead of tracking the max. O(n^2) time, O(1) space.
  3. Longest palindromic subsequence (chars need not be contiguous)?
     -> Centers do not work there. Use DP: dp[i][j] = dp[i+1][j-1] + 2 if the
        ends match, otherwise the max of the two sides. O(n^2) time and space.
  4. Why is this better than the DP table?
     -> Same time, O(1) space instead of O(n^2), and it stops early when chars
        do not match.
TRIGGER
  When a problem asks for the longest or the count of contiguous palindromes,
  grow two pointers outward from every char and every gap.
================================================================================
*/
