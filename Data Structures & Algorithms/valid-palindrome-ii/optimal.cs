// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
// --------------------------------------------------------------------------

public class Solution
{
    public bool ValidPalindrome(string s)
    {

        bool IsPalindrome(int l, int r)
        {
            while (l < r)
            {
                if (s[l] != s[r])
                    return false;
                l++;
                r--;
            }
            return true;
        }


        int l = 0;
        int r = s.Length - 1;

        while (l < r)
        {
            if (s[l] != s[r])
            {
                return IsPalindrome(l + 1, r) || IsPalindrome(l, r - 1);
            }
            l++;
            r--;
        }
        return true;
    }
}

/*
================================================================================
 PROBLEM : Given a string s, return true if s can become a palindrome after
           deleting at most one character. Deleting zero characters is
           allowed, so a string that is already a palindrome returns true.
           Example: "abca" -> true (delete 'c'), "abc" -> false.
 PATTERN : Two Pointers (converging) + one skip, try both sides
================================================================================
IDEA
  l starts at the left end and r at the right end. They move inward while
  s[l] == s[r]. At the first mismatch, one of s[l] or s[r] must be deleted.
  We do not know which, so we check both halves with IsPalindrome(l + 1, r)
  and IsPalindrome(l, r - 1). We return true if either half is a palindrome.
  This is correct because every pair outside [l, r] already matched. So the
  only useful deletion is at l or at r, and after that no deletion is left.
EXAMPLE
  s = "bcba": l=0,r=3 'b' vs 'a' mismatch.
  IsPalindrome(1,3) "cba": 'c' vs 'a' -> false.
  IsPalindrome(0,2) "bcb": 'b'=='b', then l=r=1 stops -> true. Answer: true.
  s = "abc": "bc" false, "ab" false -> false.
COMPLEXITY
  Time  O(n)  outer scan plus at most two inner scans, each at most n steps
  Space O(1)  only index variables, no substrings are built
PATH TO OPTIMAL
  Delete each index in turn and check the rest - O(n^2) time, O(n) space.
  Two pointers, branch once at the first mismatch - O(n) time, O(1) space.
  It is better because the matched outer pairs never need a recheck. This is
  optimal.cs.
KEYWORDS
  palindrome, two pointers, delete one character, greedy, string, branching
WATCH OUT
  - Trying only one side, e.g. only skipping s[l], fails "bcba".
    Always check both IsPalindrome(l + 1, r) and IsPalindrome(l, r - 1).
  - Do not move on after a mismatch. Return right away, or a second
    deletion gets allowed without you noticing.
  - The local function's parameters l and r shadow the outer l and r.
    This needs C# 8 or later. Renaming them avoids confusion.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. What if up to k deletions are allowed?
     -> Recurse with a budget k that branches at each mismatch: O(2^k * n). Or
        use DP for the minimum deletions (n minus the longest palindromic
        subsequence): O(n^2) time, O(n^2) or O(n) space.
  2. Why is it enough to branch only at the first mismatch?
     -> All pairs outside [l, r] already match. Deleting any other character
        would shift those pairs, so the deleted char must be s[l] or s[r].
  3. What if you must return which index to delete?
     -> Use the same code. Return l if IsPalindrome(l + 1, r) is true, else r
        if the other check passes, else -1. It is still O(n) time and O(1) space.
  4. What if we ignore case and non-alphanumeric characters?
     -> Skip those characters with inner while loops, as in Valid Palindrome
        I, and compare with char.ToLower. The complexity does not change.
TRIGGER
  A palindrome or symmetry check where at most one mismatch can be "fixed"
  should make you reach for converging two pointers with one branch.
================================================================================
*/
