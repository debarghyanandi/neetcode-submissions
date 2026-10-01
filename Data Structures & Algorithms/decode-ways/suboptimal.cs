// --------------------------------------------------------------------------
// -  suboptimal.cs         O(n) time / O(n) space
// --------------------------------------------------------------------------

public class Solution
{
    public int NumDecodings(string s)
    {
        // Memoization
        int[] dp = new int[s.Length + 1];
        Array.Fill(dp, -1);
        return Dfs(0, s, dp);
    }

    private int Dfs(int i, string s, int[] dp)
    {
        if (i == s.Length)
            return 1;
        if (s[i] == '0')
            return 0;

        if (dp[i] != -1)
            return dp[i];

        //pick
        int res = Dfs(i + 1, s, dp);

        //pick 2
        if (i < s.Length - 1)
        {
            if (s[i] == '1' || (s[i] == '2' && s[i + 1] < '7'))
            {
                res += Dfs(i + 2, s, dp);
            }
        }

        dp[i] = res;
        return res;
    }
}

/*
================================================================================
 PROBLEM : A digit string s encodes letters with 'A'=1 ... 'Z'=26. Return how
           many ways s can be decoded. A '0' cannot stand alone, and "06" is
           not a valid code. Example: "226" -> 3 (2 2 6, 22 6, 2 26).
 PATTERN : 1-D DP, top-down DFS + memoization
================================================================================
IDEA
  Dfs(i) counts the decodings of the suffix that starts at index i.
  At each i we take one digit (Dfs(i + 1)). We also take two digits
  (Dfs(i + 2)) if s[i..i+1] is 10..26. A '0' at s[i] returns 0, because no
  code starts with 0. Reaching i == s.Length is one full decoding, so it
  returns 1. dp[i] caches each suffix count, so every state is solved once.
  It differs from optimal.cs: optimal.cs goes bottom-up and keeps only two
  values, with no array and no recursion.
EXAMPLE
  s = "2101": Dfs(3) '1' -> 1, Dfs(2) '0' -> 0,
  Dfs(1) '1' = Dfs(2) + "10" Dfs(3) = 0 + 1 = 1,
  Dfs(0) '2' = Dfs(1) + "21" Dfs(2) = 1 + 0 = 1.
  Answer 1 (2 10 1). The zero kills both "2 1 0 1" and "21 0 1".
COMPLEXITY
  Time  O(n)  each index i fills dp[i] once, with O(1) work per call
  Space O(n)  dp array plus a recursion stack up to n deep
WATCH OUT
  - The two-digit test is s[i]=='1' or (s[i]=='2' and s[i+1] < '7').
    Writing <= '6' is fine, but writing < '6' silently drops "26".
  - The zero check must come before the two-digit branch. Otherwise "06"
    is counted as a valid code.
  - A deep recursion on a very long string can overflow the stack. Say this
    out loud, then offer the iterative version.
  - A suffix that starts with '0' is never stored in dp. That is fine,
    because the s[i]=='0' check already returns in O(1).
================================================================================
*/
