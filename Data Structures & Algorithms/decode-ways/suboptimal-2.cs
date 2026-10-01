// --------------------------------------------------------------------------
// -  suboptimal-2.cs       O(n) time / O(n) space
// --------------------------------------------------------------------------

public class Solution
{
    public int NumDecodings(string s)
    {
        // Tabulation
        int n = s.Length;
        int[] dp = new int[n + 1];

        dp[n] = 1;

        for (int i = n - 1; i >= 0; i--)
        {
            //Pick 1
            if (s[i] == '0')
            {
                dp[i] = 0;
                continue;
            }

            dp[i] = dp[i + 1];

            //pick 2
            if (i < n - 1)
            {
                if (s[i] == '1' || (s[i] == '2' && s[i + 1] < '7'))
                {
                    dp[i] += dp[i + 2];
                }
            }
        }
        return dp[0];
    }
}

/*
================================================================================
 PROBLEM : A string s of digits encodes letters with 'A'=1 ... 'Z'=26. Return
           how many different ways s can be decoded back to letters. A code
           can never start with '0', so "06" is not valid, but "10" and "20"
           are. Example: "226" -> 3 ("2 2 6", "22 6", "2 26").
 PATTERN : 1-D Dynamic Programming (bottom-up tabulation)
================================================================================
IDEA
  dp[i] is the number of ways to decode the suffix s[i..n-1].
  The base case is dp[n] = 1: the empty suffix has one decoding.
  Going from right to left, a '0' at s[i] gives dp[i] = 0.
  Otherwise take one digit (dp[i+1]), and add dp[i+2] if s[i]s[i+1] is 10..26.
  It is correct because every decoding starts with exactly one 1-digit or
  2-digit code. Unlike optimal.cs, it keeps the whole dp array.
EXAMPLE
  s = "2106", n = 4, dp[4] = 1
  i=3 '6': dp[3]=1 | i=2 '0': dp[2]=0 | i=1 '1': 0 + dp[3] = 1
  i=0 '2': dp[1] + dp[2] = 1 + 0 = 1 ("21" is valid but leaves "06")
  Answer dp[0] = 1, the only decoding is "2 10 6".
COMPLEXITY
  Time  O(n)  one pass from i = n-1 down to 0, O(1) work at each i
  Space O(n)  the dp array has n + 1 entries
WATCH OUT
  - The '0' check must come first. A leading '0' kills that suffix ("06" ->
    0),
    so the two-digit rule should never be reached for it.
  - The bound is s[i+1] < '7' only when s[i] == '2'. "27" must not count,
    and "1x" counts for every digit x, including '0'.
  - The i < n - 1 guard keeps s[i+1] and dp[i+2] in range at the last index.
  - An empty s returns dp[0] = 1, not 0. State your assumption if asked.
================================================================================
*/
