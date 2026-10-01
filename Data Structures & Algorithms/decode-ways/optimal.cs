// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
// --------------------------------------------------------------------------

public class Solution
{
    public int NumDecodings(string s)
    {
        // Tabulation Space Optimized
        int n = s.Length;
        int next = 1;
        int next2 = 1;

        for (int i = n - 1; i >= 0; i--)
        {
            int curr = 0;

            if (s[i] != '0')
            {
                //pick 1
                curr = next;

                //pick 2
                if (i < n - 1)
                {
                    if (s[i] == '1' || (s[i] == '2' && s[i + 1] < '7'))
                    {
                        curr += next2;
                    }
                }
            }

            next2 = next;
            next = curr;
        }

        return next;
    }
}

/*
================================================================================
 PROBLEM : A digit string s is decoded with 'A'=1 ... 'Z'=26. Return how many
           ways the whole string can be decoded. "0" alone is not valid, and
           "06" is not a valid two-digit code. Example: "226" -> 3 (2 2 6, 22
           6, 2 26).
 PATTERN : 1D DP (Fibonacci-style, right to left, two variables)
================================================================================
IDEA
  Let dp[i] be the number of ways to decode s[i..]. Scan i from right to left.
  next holds dp[i+1] and next2 holds dp[i+2], so only two values are kept.
  If s[i] is '0', curr = 0. Otherwise curr = next for a one-digit code.
  Add next2 if s[i..i+1] is 10..26. This is correct because every decoding
  starts with a 1-digit or a 2-digit code, and the two cases never overlap.
EXAMPLE
  s = "1206", start next=1, next2=1
  i=3 '6': curr=1 | i=2 '0': curr=0 | i=1 '2': 0 + "20" adds 1 -> 1
  i=0 '1': next=1 + "12" adds next2=0 -> 1. Answer 1 (1 20 6).
COMPLEXITY
  Time  O(n)  one pass over i, O(1) work per index
  Space O(1)  only next, next2, curr
PATH TO OPTIMAL
  Plain recursion trying 1 or 2 digits - O(2^n) - correct but very slow.
  Memoization on index - O(n)/O(n) - each suffix is solved once (suboptimal*).
  Tabulation dp array - O(n)/O(n) - no recursion stack (suboptimal*).
  Two rolling variables - O(n)/O(1) - dp[i] needs only dp[i+1], dp[i+2].
KEYWORDS
  decode ways, 1D DP, tabulation, memoization, Fibonacci DP, counting ways
WATCH OUT
  - A '0' can't start a code: curr must be 0, so "06" or "0" gives 0.
  - Pair check: s[i]=='2' needs s[i+1] < '7', so "27" is one code only.
  - Shift order: set next2 = next before next = curr, or next2 is lost.
  - Empty s returns 1 here (next starts at 1); confirm what is expected.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. What if s can have '*' meaning any digit 1-9 (Decode Ways II)?
     -> Same two-variable DP, but multiply by the count of valid digits or
        pairs for each '*' case, and take the result mod 1e9+7. O(n)/O(1).
  2. Can you return all decodings, not just the count?
     -> Use backtracking with the same 1- or 2-digit choices. Time becomes
        output-sized, which can be exponential.
  3. How does this relate to Climbing Stairs?
     -> Same recurrence dp[i] = dp[i+1] + dp[i+2], but each step is allowed
        only if the digit or pair is valid.
TRIGGER
  Count ways to split a sequence where each step takes 1 or 2 items under a
  validity rule: think Fibonacci-style 1D DP with two rolling variables.
================================================================================
*/
