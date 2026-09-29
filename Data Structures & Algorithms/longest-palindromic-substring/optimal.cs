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
 PATTERN : Expand Around Center - grow palindromes from 2n-1 centers
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-1.cs when it was first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  start      index in s where the best palindrome found so far begins
  maxLength  length of the best palindrome found so far (0 before any match)
  left       left edge of the window being grown outward in Expand
  right      right edge of the window being grown outward in Expand
  length     right - left + 1, the size of the palindrome that was just checked
WHY THIS PATTERN
  The problem asks for the longest contiguous substring that reads the same both
  ways. A palindrome is mirrored around its middle. So if you fix the middle and
  step outward while s[left] == s[right], you find every palindrome with that
  middle. The loop tries every i as a middle, so the longest one cannot be
  missed.
BRUTE FORCE
  The simplest approach is to list every substring (i, j) and check each one
  with two pointers. That is O(n^2) substrings times an O(n) check, so O(n^3)
  time. It loses because it checks the same inner characters again and again. A
  palindrome DP table, where isPal[i][j] depends on isPal[i+1][j-1], gets to
  O(n^2) time but needs O(n^2) memory. This file gets the same time with O(1)
  extra space.
INVARIANT
  Inside Expand, every time the while condition passes, s[left..right] is a
  palindrome. The outer part matched, and the inner part was already a
  palindrome on the step before. Each of these palindromes is compared with
  maxLength at once, so after all centers are tried, start and maxLength
  describe the longest one. When a match fails, the loop stops, because no wider
  window around that same center can be a palindrome.
TWO CENTERS PER INDEX
  Expand(i, i) finds odd-length palindromes, where the middle is one character.
  Expand(i, i + 1) finds even-length palindromes, like "abba", where the middle
  falls between two characters. If you skip the second call, the code silently
  misses every even-length answer. When i is the last index, i + 1 equals
  s.Length, and the right < s.Length check stops that call right away.
WATCH OUT
  length is computed inside the loop, before left-- and right++. If you refactor
  so the length is computed once after the loop ends, the correct formula
  becomes right - left - 1, not + 1. The check length > maxLength is strict, so
  when two palindromes tie, the first one found (the leftmost center) wins. If
  the problem wants a different tie rule, this has to change. A null s throws at
  s.Length. An empty s returns "" because Substring(0, 0) is valid.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you do better than O(n^2)?
     Yes. Manacher's algorithm runs in O(n). It inserts separators so every
     palindrome has odd length. It then reuses mirror radii inside the rightmost
     palindrome found so far. The cost is O(n) extra memory and code that is
     much harder to get right in an interview.
  2. How would you count all palindromic substrings instead (LeetCode 647)?
     Keep the same two Expand calls, but add 1 to a counter each time the while
     condition passes instead of tracking the max. The time and space stay the
     same.
  3. What if the question asks for the longest palindromic subsequence, where
  the characters do not have to be next to each other?
     Expanding around a center no longer works, because gaps are allowed. Use
     interval DP: dp[i][j] = dp[i+1][j-1] + 2 if s[i] == s[j], otherwise
     max(dp[i+1][j], dp[i][j-1]). That takes O(n^2) time, and memory can drop to
     O(n) by keeping only rolling rows.
TRIGGER
  When a problem asks about a contiguous substring that must be symmetric or
  mirrored, try expanding outward from each of the 2n-1 possible centers.
C# NOTE
  Expand is a local function that captures s, start and maxLength from the
  enclosing method, so it updates the answer directly and can return void with
  no out parameters. The code tracks only indices and calls Substring once at
  the end, so it creates just one string instead of one for every candidate.
COMPLEXITY
  Time  : O(n^2)
  Space : O(1)
================================================================================
*/
