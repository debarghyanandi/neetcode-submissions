// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
// -  Two-pass prefix-suffix products   [prefix-suffix-product]
// -  the only solution in this folder
// -
// -  Reference solution - not one you solved yourself
// -
// -  First pass accumulates prefix products left-to-right, second pass
// -  multiplies suffix products right-to-left in a single pass each.
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
 PATTERN : Prefix / Suffix Products - two passes, one output array
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Optimal
================================================================================
VARIABLES
  result          after pass 1: result[i] = nums[0] * ... * nums[i-1]; after pass 2: the final answer
  prefixProduct   product of nums[0..i-1], the elements to the left of i
  suffixProduct   product of nums[i+1..length-1], the elements to the right of i
WHY THIS PATTERN
  The problem asks, for each index, for the product of "everything except me".
  That splits into two parts: everything to my left and everything to my right.
  A running product from each side gives both parts in one sweep each. Pass 1
  stores the left part in result[i], and pass 2 multiplies in the right part
  from suffixProduct. So there is no need for a separate suffix array.
BRUTE FORCE
  For each i, loop over every j != i and multiply nums[j]. This is clearly
  correct, but it is O(n^2) time, because it computes the same partial products
  again and again. The other obvious idea is to take the total product and
  divide by nums[i]. That breaks when there is a zero, and the problem usually
  forbids division anyway.
INVARIANT
  In pass 1, when result[i] is written, prefixProduct equals the product of
  nums[0..i-1] exactly. In pass 2, when result[i] *= suffixProduct runs,
  suffixProduct equals the product of nums[i+1..length-1] exactly. Each running
  product is used first and updated second, so nums[i] is never in either
  factor. After both passes, result[i] = (left product) * (right product), which
  is exactly what the problem asks for.
ZEROS NEED NO SPECIAL CASE
  There is no division, so a zero in nums is just another factor. It turns every
  product that includes it into 0. With one zero, only the zero's own index gets
  a nonzero answer. With two or more zeros, every entry is 0. The code gets both
  cases right without any branch.
WATCH OUT
  The order inside each loop matters. If you update prefixProduct or
  suffixProduct before you use it, nums[i] gets counted in its own answer. All
  the math is int. C# arithmetic is unchecked by default, so an overflow wraps
  around silently and does not throw. The last update in each loop
  (prefixProduct *= nums[length-1] and suffixProduct *= nums[0]) builds the
  product of the whole array, and nothing ever reads it. That product can
  overflow even when every answer fits, and it would throw if this code ran in a
  checked context.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. What if division were allowed?
     Count the zeros and take the product of the nonzero values. With zero
     zeros, the answer is total / nums[i]. With exactly one zero, only the
     zero's index gets the product and every other index gets 0. With two or
     more zeros, every entry is 0. It is still one or two passes, but it needs
     more branches, and it is only safe when that product fits in the type.
  2. Why does this count as O(1) extra space when result has n entries?
     The output array does not count by convention. The simpler version keeps
     separate prefix[] and suffix[] arrays, which is O(n) extra. Here result is
     reused as the prefix array, and a single scalar replaces the suffix array.
  3. What if the answers could be larger than int?
     Change result, prefixProduct and suffixProduct to long, or to BigInteger if
     the values have no bound. long costs nothing in the algorithm. BigInteger
     makes each multiplication slower as the numbers grow.
  4. How about a 2D version: each cell gets the product of every other cell in
  the grid?
     Flatten the grid in row-major order (one row after another) and run the
     same two passes on the flat index. The left and right idea does not depend
     on the shape.
TRIGGER
  Look for "for each index, combine everything except this one" with no division
  or no inverse operation allowed. Build it from a left running value and a
  right running value.
C# NOTE
  new int[length] fills the array with zeros, but pass 1 writes every slot
  before anything reads it, so that fill does not matter here. Returning result
  directly avoids an extra copy, such as a List<int> converted with ToArray().
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
