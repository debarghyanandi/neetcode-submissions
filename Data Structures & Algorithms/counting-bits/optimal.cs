// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
// --------------------------------------------------------------------------

public class Solution
{
    public int[] CountBits(int n)
    {
        int[] dp = new int[n + 1];

        //Base case.
        dp[0] = 0;

        for (int i = 1; i <= n; i++)
        {
            //i >> 1 creates a 1 right shift
            //so the ans is already built.
            //as we remove the last digit we need to add that back
            //i & 1 return 1 if last digit was 1 (odd)
            //and return 0 is the last digit was 0(even)
            dp[i] = dp[i >> 1] + (i & 1);
        }
        return dp;
    }
}

/*
================================================================================
 PROBLEM : Given an integer n, return an array ans of length n + 1. ans[i] is
           the number of 1 bits in the binary form of i, for 0 <= i <= n.
           Example: n = 5 -> [0,1,1,2,1,2].
 PATTERN : 1D DP on bits (dp[i] from dp[i >> 1])
================================================================================
IDEA
  i >> 1 is i with its last bit removed. It is smaller than i, so
  dp[i >> 1] is already filled. Add back the removed bit with (i & 1),
  which is 1 for odd i and 0 for even i. So dp[i] = dp[i >> 1] + (i & 1).
  It is correct because every 1 bit of i is either in i >> 1 or is the last
  bit.
EXAMPLE
  n = 5, dp[0] = 0
  i=1: dp[0]+1=1 i=2: dp[1]+0=1 i=3: dp[1]+1=2 i=4: dp[2]+0=1 i=5: dp[2]+1=2
  Answer: [0,1,1,2,1,2]
COMPLEXITY
  Time  O(n)  one O(1) step per index i from 1 to n
  Space O(1)  only the returned dp array, no extra memory
PATH TO OPTIMAL
  Count bits of each i by shifting until 0 - O(n log n) - simple baseline.
  Kernighan trick (x &= x - 1) per i - O(n * bits set) - skips zero bits.
  DP reuse of dp[i >> 1] (this file) - O(n) - each answer costs one step.
  There is no sibling file for the slower steps.
KEYWORDS
  counting bits, popcount, Hamming weight, bit manipulation, DP, right shift
WATCH OUT
  - Precedence: in C# + binds tighter than &. Without the parentheses,
    dp[i >> 1] + i & 1 means (dp[i >> 1] + i) & 1, which is wrong.
  - The array size is n + 1, and the loop must use i <= n, not i < n.
  - n = 0 works and returns [0]. A negative n would throw on new int[n + 1].
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Is there another DP recurrence?
     -> dp[i] = dp[i & (i - 1)] + 1, since i & (i - 1) drops the lowest set
        bit. It is also O(n) time. Pick whichever you can explain clearly.
  2. Why not just call BitOperations.PopCount for each i?
     -> That is O(n) with a hardware instruction, but it hides the idea.
        Interviewers want the DP. Mention PopCount as the practical choice.
  3. How do you count the 1 bits of only one number?
     -> Use x &= x - 1 in a loop and count the steps. That is O(set bits) time
        and O(1) space (LeetCode "Number of 1 Bits").
TRIGGER
  You need a per-number bit property for all 0..n, and i's answer can come
  from a smaller number, such as i >> 1 or i & (i - 1).
================================================================================
*/
