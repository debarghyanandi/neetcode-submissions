// --------------------------------------------------------------------------
// -  optimal.cs            O(n^2) time / O(1) space
// -  expand around center   [expand-around-center]
// -  the only solution in this folder
// -
// -  Reference solution - not one you solved yourself (from submission-1)
// -
// -  Each of n center positions expands outward until characters mismatch,
// -  taking O(n) per position in worst case
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
 PATTERN : Expand Around Center - two pointers from every index
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-1.cs when it was first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  start      left index of the best palindrome found so far
  maxLength  length of that best palindrome
  Expand     local function: grows left/right outward while chars match
  left/right the two pointers inside Expand, moving apart
  length     right - left + 1, size of the current matching window
WHY THIS PATTERN
  The problem asks for the longest contiguous substring that reads the same both
  ways. Every palindrome is fully described by its center and grows
  symmetrically, so if you stand on each center and push outward you will meet
  every palindrome exactly once. There are 2n - 1 centers: n single characters
  (Expand(i, i)) and n - 1 gaps between characters (Expand(i, i + 1)), which is
  why both calls sit in the same loop body. Each expansion costs at most n
  steps, and start plus maxLength carry the winner across all of them.
BRUTE FORCE
  The first thing most people write is a triple loop: pick every pair of
  indices, then walk the substring to check if it is a palindrome. That is
  O(n^3) time. It loses because the palindrome check restarts from scratch for
  every pair and throws away the fact that a palindrome of length L contains a
  palindrome of length L - 2 at the same center.
INVARIANT
  After each call to Expand, s.Substring(start, maxLength) is the longest
  palindrome whose center has already been visited. Inside Expand, the loop only
  continues while s[left] == s[right] and both indices are in range, so the
  window left..right is always a true palindrome when the body runs. Because the
  loop visits every one of the 2n - 1 centers, the final winner is the global
  longest.
UPDATING INSIDE THE LOOP
  The comparison length > maxLength happens on every expansion step, not once
  after the while loop ends. That is deliberate: when the loop exits, left and
  right have already moved one step past the valid range, so an after-the-loop
  update would need right - left - 1 and start = left + 1. Checking inside
  sidesteps that off-by-one completely. The extra comparisons are free in
  complexity terms since length only grows within one expansion.
WATCH OUT
  Expand returns void and communicates only by mutating the captured start and
  maxLength. That is fine here, but anyone refactoring it into a separate helper
  method will silently break it unless they return a value or pass state by
  reference. The call Expand(i, i + 1) passes right == s.Length when i is the
  last index; the right < s.Length guard is the only thing stopping an
  out-of-range read, so do not reorder the two conditions in the while. An empty
  string works by accident: the loop never runs and Substring(0, 0) returns "",
  but a null s throws NullReferenceException on s.Length with no guard.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you do better than quadratic time?
     Manacher's algorithm runs in O(n). It reuses palindrome radii already
     computed inside a known larger palindrome, using a mirror index, so each
     position is expanded past only once. The cost is much harder code and a
     transformed string (or separate odd/even passes) to handle even lengths.
  2. What if the interviewer asks for the dynamic programming version instead?
     Build a table where dp[i][j] is true when s[i..j] is a palindrome, filling
     by increasing length with dp[i][j] = s[i] == s[j] && dp[i+1][j-1]. Same
     O(n^2) time but it adds O(n^2) space, so this center version is strictly
     better on memory.
  3. How would you count all palindromic substrings instead of the longest?
     Keep the same double call to Expand, but replace the maxLength comparison
     with a counter increment on every successful expansion step, since each
     step is one distinct palindromic substring.
  4. What changes if it is the longest palindromic subsequence, not substring?
     Centers no longer work because the characters need not be adjacent. You
     switch to the DP over intervals, or run longest common subsequence between
     s and its reverse.
TRIGGER
  A problem about contiguous palindromes where the answer has a symmetry point -
  think centers, not pairs of endpoints.
C# NOTE
  Expand is a local function that writes to start and maxLength, so the compiler
  must box those locals into a closure object; making it static would force you
  to return the (start, length) pair instead. The final s.Substring(start,
  maxLength) allocates a new string - if the caller only needs the boundaries or
  will slice again, returning a ReadOnlySpan<char> over s avoids that copy.
COMPLEXITY
  Time  : O(n^2)
  Space : O(1)
================================================================================
*/
