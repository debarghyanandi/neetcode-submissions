// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
// -  prefix/suffix product accumulation   [prefix-suffix-product]
// -  the only solution in this folder
// -
// -  Reference solution - not one you solved yourself
// -
// -  two linear passes accumulate running left and right products directly
// -  into the output array, avoiding division and using only O(1) auxiliary
// -  accumulators
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
 PATTERN : Prefix/Suffix Products - two passes, output as scratch
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Optimal
================================================================================
CORE IDENTITY
  Everything rests on one factorization: the product of all elements except
  nums[i] equals (product of nums[0..i-1]) * (product of nums[i+1..n-1]). Prefix
  times suffix. Once you see that, the code is just "compute the prefix for
  every i, then multiply in the suffix for every i" - and each of those is a
  single running accumulator, because prefix(i+1) = prefix(i) * nums[i].

  The obvious alternative - total product divided by nums[i] - is what the
  problem forbids, and it is genuinely broken, not just banned: any zero in nums
  makes the division undefined at that index, and two zeros make the total
  product 0 so you cannot recover anything.
THE TWO INVARIANTS
  Pass 1, at the top of iteration i: prefixProduct holds the product of
  nums[0..i-1] (empty product = 1 when i == 0). It is stored into result[i] and
  only then multiplied by nums[i], which re-establishes the invariant for i+1.

  Pass 2, at the top of iteration i: suffixProduct holds the product of
  nums[i+1..n-1] (1 when i == length-1). result[i] already holds the prefix, so
  result[i] *= suffixProduct completes the answer for i, and suffixProduct *=
  nums[i] re-establishes the invariant for i-1.

  At no point does either accumulator contain nums[i] while index i is being
  written. That is the whole correctness argument.
WRITE BEFORE UPDATE
  The trap is the line order inside each loop. In pass 1, result[i] =
  prefixProduct must come before prefixProduct *= nums[i]. Swap them and
  prefixProduct already includes nums[i], so you get the product of nums[0..i] -
  the inclusive prefix - and every answer is wrong by a factor of nums[i] (or
  silently zero). Same hazard in pass 2 with suffixProduct.

  Mnemonic: the accumulator is always one step behind the index. Read it, then
  feed it.
OUTPUT AS SCRATCH
  result is not just the return value; during pass 1 it is the storage for the
  prefix table, and pass 2 upgrades it in place rather than allocating a suffix
  array. The textbook version of this problem builds two arrays, prefix[] and
  suffix[], and combines them in a third loop. Collapsing to one array works
  only because pass 2 walks right to left: the suffix for index i depends on
  nums, never on result, so overwriting result[i] destroys nothing that a later
  iteration needs.

  Interviewer follow-up to expect: "can you do it without extra space?" The
  intended answer is exactly this - the output array is not counted, and no
  second array is needed.
ZEROS FALL OUT FREE
  Worth checking explicitly, because it is the case the division approach dies
  on and the case an interviewer will probe. With exactly one zero at index k:
  for i != k, either prefixProduct or suffixProduct spans across k, so result[i]
  is 0. For i == k, the prefix stops before k and the suffix starts after k, so
  neither ever multiplies in the zero, and result[k] is the product of
  everything else - correct. With two or more zeros, every index has a zero on
  one side or the other, so the whole array is 0. No special-casing, no zero
  counting.
WATCH OUT
  The accumulators are int and C# arithmetic here is unchecked, so an
  overflowing prefixProduct wraps silently instead of throwing. The code relies
  on the problem's guarantee that every prefix and suffix product fits in 32
  bits; if that guarantee is lifted, both accumulators and result need to become
  long.

  Edge cases that need no code: length 1 returns [1] (both accumulators stay at
  their identity value of 1), and length 0 returns an empty array since both
  loops have zero iterations.
TRIGGER
  Reach for prefix/suffix when the answer at index i is a fold over everything
  except i, and the fold operation has no safe inverse - product with possible
  zeros, min, max, GCD, bitwise OR. If the operation does have a clean inverse
  (sum, XOR), the one-pass total-minus-element trick is simpler and you should
  use that instead. The two-pass shape generalizes: replace * with the operation
  and 1 with its identity.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
