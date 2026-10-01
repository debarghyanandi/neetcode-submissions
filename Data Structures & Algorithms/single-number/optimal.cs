// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
// --------------------------------------------------------------------------

public class Solution
{
    public int SingleNumber(int[] nums)
    {
        int res = 0;
        foreach (int num in nums)
        {
            res ^= num;
        }
        return res;
    }
}

/*
================================================================================
 PROBLEM : You get an int array nums. Every value appears exactly twice,
           except one value that appears once. Return that single value. You
           must use O(n) time and O(1) extra space. Example: [4,1,2,1,2] -> 4.
 PATTERN : Bit Manipulation (XOR cancellation)
================================================================================
IDEA
  Start with res = 0 and XOR every num into it. XOR has three useful rules:
  a ^ a = 0, a ^ 0 = a, and order does not matter. So each pair cancels to
  0, no matter where its two copies sit in nums. Only the single value is
  left in res, and that is the answer.
EXAMPLE
  nums = [4,1,2,1,2], res starts at 0 (binary shown)
  0^4=100, ^1=101, ^2=111, ^1=110, ^2=100
  The pairs 1 and 2 cancel even though they are not next to each other.
  Answer: 4
COMPLEXITY
  Time  O(n)  one pass, one XOR per element
  Space O(1)  only the single int res
PATH TO OPTIMAL
  Nested loops, count each value - O(n^2) / O(1) - simplest start.
  Sort, then check pairs - O(n log n) / O(1) - no inner loop.
  HashSet, add or remove - O(n) / O(n) - one pass, but needs memory.
  XOR all values (optimal.cs) - O(n) / O(1) - one pass and no memory.
KEYWORDS
  XOR, bit manipulation, pairs cancel, single number, constant space
WATCH OUT
  - res must start at 0. If it starts at nums[0], the loop XORs nums[0] a
    second time, so it cancels and the answer is wrong.
  - The trick needs every other value to appear an EVEN number of times.
    If a value appears 3 times, it does not cancel and res is wrong.
  - Do not use sum tricks like 2*sum(set) - sum(nums). They need a set
    (O(n) space) and can overflow int.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Every value appears three times, except one. (Single Number II)
     -> Count the 1-bits at each of the 32 positions and take each count mod
        3. The leftover bits form the answer. O(32n) time, O(1) space.
  2. Two values appear once, all others twice. (Single Number III)
     -> XOR all to get x = a ^ b. Pick a set bit with x & -x, split nums into
        two groups by that bit, and XOR each group. O(n) time, O(1) space.
  3. Why does the order of nums not matter?
     -> XOR is commutative and associative, so the whole loop equals XOR of
        each pair first, which is 0, then the single value.
TRIGGER
  Reach for XOR when every value is "paired" and you must find the odd one out
  in O(1) space.
================================================================================
*/
