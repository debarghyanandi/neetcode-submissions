// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
// --------------------------------------------------------------------------

public class Solution
{
    public int MissingNumber(int[] nums)
    {

        int len = nums.Length;
        int total = (len * (len + 1)) / 2;

        foreach (int num in nums)
        {
            total = total - num;
        }

        return total;
    }
}

/*
================================================================================
 PROBLEM : You get an array nums of n distinct numbers, all taken from the
           range 0..n. Exactly one number in that range is missing. Return it.
           Note the range has n+1 values but the array has only n. Example:
           [3,0,1] -> 2.
 PATTERN : Math (Gauss sum formula) / running subtraction
================================================================================
IDEA
  The numbers 0..len add up to len*(len+1)/2, so the code stores that in
  total. It then subtracts every num in nums from total. Every present
  number cancels its own share of the full sum. What is left is the one
  value that was never subtracted, which is the missing number.
EXAMPLE
  nums=[3,0,1]: len=3, total=6 -> 6-3=3 -> 3-0=3 -> 3-1=2 -> return 2.
  Tricky (n itself missing): nums=[0,1]: len=2, total=3 -> 3-0=3
  -> 3-1=2 -> return 2. This is correct, because 2 is the top of the range.
COMPLEXITY
  Time  O(n)  one pass over nums, and the formula costs O(1)
  Space O(1)  only two int variables, len and total
PATH TO OPTIMAL
  Brute force: for each v in 0..n, scan nums for v - O(n^2) - no extra mem.
  Sort, then find the first i with nums[i]!=i - O(n log n) - fewer checks.
  HashSet of nums, then probe 0..n - O(n) time, O(n) space - linear time.
  Sum formula (this file) - O(n)/O(1) - same speed, no set needed. The
  other O(1) way (for example XOR) is in optimal-variant.cs.
KEYWORDS
  missing number, Gauss sum, arithmetic series, XOR trick, bit manipulation,
  cyclic sort
WATCH OUT
  - len*(len+1) is int math. It overflows once len >= 46341, and /2 then
    gives a wrong total. Use (long)len*(len+1)/2, or use the XOR method.
  - The range is 0..n, not 1..n. Using (len-1)*len/2 breaks every input.
  - Use nums.Length (n) as the top of the range, not n-1. Otherwise the
    case where n itself is missing, like [0,1], fails.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How do you avoid overflow completely?
     -> XOR all indices 0..n and all nums. Pairs cancel and the missing value
        is left. Still O(n)/O(1), and it needs no big intermediate sum.
  2. What if nums is sorted?
     -> Binary search for the first index i where nums[i] != i. This runs in
        O(log n) time and O(1) space, but sorting first would cost O(n log n).
  3. What if two numbers are missing?
     -> Get both their sum and their sum of squares (or their XOR), then solve
        for the two values. Or split by one set bit of the XOR. O(n)/O(1).
  4. What if there are duplicates, or the values are arbitrary (First Missing
     Positive)?
     -> The sum trick fails here. Use cyclic sort: swap each value to index
        value-1 in place, then scan. O(n) time, O(1) extra space, but it changes
        the input.
TRIGGER
  The input holds n distinct values from a known range 0..n with one value
  missing, so the expected sum or XOR is known before you look at the data.
================================================================================
*/
