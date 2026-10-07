// --------------------------------------------------------------------------
// -  optimal-variant.cs    O(n) time / O(1) space
// --------------------------------------------------------------------------

public class Solution
{
    public int MissingNumber(int[] nums)
    {
        int n = nums.Length;
        int xorr = n;
        for (int i = 0; i < n; i++)
        {
            xorr ^= i ^ nums[i];
        }
        return xorr;
    }
}

/*
================================================================================
 PROBLEM : You get an array nums of n distinct numbers, all taken from the
           range 0..n. Exactly one number in that range is missing. Return the
           missing number. Example: [3,0,1] -> 2.
 PATTERN : Bit Manipulation (XOR cancellation)
================================================================================
IDEA
  XOR has two useful rules: a ^ a = 0 and a ^ 0 = a. The order does not
  matter.
  Start xorr with n. In one loop, XOR in every index i and every value
  nums[i].
  Together, n and the indices i cover the full range 0..n.
  Every number in nums appears twice (once as a value, once in the range) and
  cancels to 0. Only the missing number appears once, so it stays in xorr.
EXAMPLE
  nums = [3,0,1], n = 3, so xorr = 3 at the start.
  i=0: xorr ^= 0^3 -> 0; i=1: xorr ^= 1^0 -> 1; i=2: xorr ^= 2^1 -> 2
  Answer 2. Edge case [0,1]: xorr starts at 2, each step adds 0, answer 2 (=
  n).
COMPLEXITY
  Time  O(n)  one pass over nums, with O(1) XOR work per index i
  Space O(1)  only the ints n, xorr and i, whatever the input size
WATCH OUT
  - Starting xorr at 0 instead of n is wrong: the loop only covers 0..n-1, so
    n is never added. [3,0,1] would give 1 instead of 2.
  - Writing xorr = i ^ nums[i] instead of ^= drops all earlier steps.
  - The trick needs the input to be valid. If a value repeats or lies outside
    0..n, the pairs do not cancel and the result is garbage, with no error.
================================================================================
*/
