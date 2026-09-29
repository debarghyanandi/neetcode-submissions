// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
// --------------------------------------------------------------------------

public class Solution
{
    public int[] ProductExceptSelf(int[] nums)
    {
        int length = nums.Length;
        int[] result = new int[length];

        // PASS 1 (left to right): result[i] = product of everything BEFORE i.
        int prefixProduct = 1;

        for (int i = 0; i < length; i++)
        {
            result[i] = prefixProduct;        // written before updating: excludes nums[i]
            prefixProduct *= nums[i];
        }

        // PASS 2 (right to left): multiply in the product of everything AFTER i.
        int suffixProduct = 1;

        for (int i = length - 1; i >= 0; i--)
        {
            result[i] *= suffixProduct;
            suffixProduct *= nums[i];
        }

        return result;
    }
}

/*
================================================================================
 PROBLEM : Given an int array nums, return an array where output[i] is the
           product of every element except nums[i]. You may not use division,
           and it must run in O(n). Example: [1,2,3,4] -> [24,12,8,6].
 PATTERN : Prefix Product + Suffix Product (two passes)
================================================================================
IDEA
  The answer at i is (product of everything left of i) times (product of
  everything right of i). Pass 1 goes left to right. It writes prefixProduct
  into result[i] before it multiplies in nums[i]. Pass 2 goes right to left
  and multiplies result[i] by suffixProduct. It is correct because each
  nums[j] with j != i joins exactly one of the two products, and nums[i]
  joins neither.
EXAMPLE
  nums = [2,3,0,4] (a zero is the tricky case)
  Pass 1: result = [1,2,6,0], prefixProduct ends at 0
  Pass 2: i=3 -> 0*1=0, i=2 -> 6*4=24, i=1 -> 2*0=0, i=0 -> 1*0=0
  Answer: [0,0,24,0]
COMPLEXITY
  Time  O(n)  two separate linear passes over nums
  Space O(1)  only two scalars; the output array is not counted
PATH TO OPTIMAL
  Brute force: nested loop per i - O(n^2) - the baseline.
  Total product / nums[i] - O(n) - but division is banned and zeros break it.
  Separate prefix[] and suffix[] arrays - O(n) time, O(n) extra space.
  This file: reuse result as prefix, keep suffix in one var - O(1) extra.
KEYWORDS
  prefix product, suffix product, no division, two passes, in-place output
WATCH OUT
  - Write result[i] BEFORE updating prefixProduct. Swapping the two lines
    includes nums[i] in its own answer.
  - Never divide by the total: one zero gives divide-by-zero, two zeros
    make every answer 0.
  - Products are int. If values can be large, prefixProduct overflows with
    no error; use long if the product might not fit.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Why is this O(1) space when result has n slots?
     -> The output array is required, so by convention it is not counted. The
        only extra memory is prefixProduct and suffixProduct.
  2. What if division is allowed?
     -> Count the zeros. Two or more: all 0. One: only the zero index gets the
        product of the non-zeros. None: total / nums[i]. Still O(n), O(1).
  3. Many queries "product of nums[l..r]" on a fixed array?
     -> Build a prefix product array once in O(n). With division and no zeros,
        each query is prefix[r+1] / prefix[l] in O(1). Otherwise use a segment
        tree for O(log n) per query.
TRIGGER
  When each answer needs "everything except me" or "all on my left and all
  on my right", combine a prefix pass and a suffix pass.
================================================================================
*/
