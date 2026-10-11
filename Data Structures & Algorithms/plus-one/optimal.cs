// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
// --------------------------------------------------------------------------

public class Solution
{
    public int[] PlusOne(int[] digits)
    {
        int n = digits.Length;
        for (int i = n - 1; i >= 0; i--)
        {
            if (digits[i] != 9)
            {
                digits[i]++;
                return digits;
            }
            digits[i] = 0;
        }

        //reached here means every one is 9.
        int[] ans = new int[n + 1];
        ans[0] = 1;

        return ans;
    }
}

/*
================================================================================
 PROBLEM : You get a non-empty array digits for a non-negative integer, most
           significant digit first, with no leading zeros. Add one to the
           number. Return the result as a digit array in the same format.
           Example: [1,2,9] -> [1,3,0], and [9,9] -> [1,0,0].
 PATTERN : Right-to-left carry propagation (grade-school addition)
================================================================================
IDEA
  Walk i from the last digit to the first. If digits[i] is not 9, add one
  to it and return at once, because no carry goes further left. If it is
  9, it becomes 0 and the carry moves left to i - 1. If the loop ends,
  every digit was 9. Then the answer is 1 followed by n zeros, so ans has
  size n + 1 and only ans[0] = 1 is set. This is just hand addition, and
  the carry can only stop at the first non-9 digit seen from the right.
EXAMPLE
  [1,9,9]: i=2 is 9 -> 0; i=1 is 9 -> 0; i=0 is 1 -> 2, return [2,0,0]
  [9,9]: i=1 -> 0, i=0 -> 0, loop ends; ans = [1,0,0]
COMPLEXITY
  Time  O(n)  at most one pass over digits, and it stops at the first non-9
  Space O(1)  digits is changed in place; the n+1 array is only the output
PATH TO OPTIMAL
  Build an int/long, add 1, split back - O(n) - overflows on long arrays.
  BigInteger parse and add - O(n) time, O(n) extra - correct but heavy.
  In-place carry loop with early return - O(n) / O(1) - this optimal.cs.
KEYWORDS
  array, math, carry, digit addition, in-place, big integer simulation
WATCH OUT
  - Forgetting the all-9 case: [9] must give [1,0], a longer array.
  - This code mutates the input digits. Copy it first if the caller
    still needs the original array.
  - Do not convert the digits to int or long: long inputs overflow.
  - ans relies on C# arrays starting as zeros; only ans[0] needs setting.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Add any integer k, not just 1?
     -> Same right-to-left loop, carrying a running value (k + digit). Grow at
        the front while carry > 0. O(n + digits of k) time.
  2. Add two digit arrays, or two binary strings (Add Binary)?
     -> Two pointers from both ends with a carry. Pad the shorter one with 0.
        Reverse or prepend at the end. O(max(n, m)) time and space.
  3. The digits are in a singly linked list, head first?
     -> Reverse the list, add, reverse back. Or recurse to the tail and return
        the carry. Or track the last non-9 node. O(n) time, O(1) for the last.
  4. Why can we return early?
     -> Once a digit is not 9, adding one makes no carry, so every digit to
        its left stays the same.
TRIGGER
  Arithmetic on a number stored as a digit array or list, too big for int.
================================================================================
*/
