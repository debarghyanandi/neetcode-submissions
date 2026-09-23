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
 PATTERN : Prefix/Suffix Products - two passes into the output array
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Optimal
================================================================================
VARIABLES
  result           result[i] = product of all nums except nums[i]
  prefixProduct    product of nums[0..i-1], carried left to right
  suffixProduct    product of nums[i+1..end], carried right to left
WHY THIS PATTERN
  The answer for each index is the product of two independent halves: everything
  left of i and everything right of i. That split means you never need nums[i]
  itself, so no division is required and zeros cause no special case. One
  left-to-right pass fills result[i] with the left half via prefixProduct, and
  one right-to-left pass multiplies in the right half via suffixProduct.
BRUTE FORCE
  For each i, loop over all other indices and multiply them: two nested loops,
  O(n^2) time. It is correct but recomputes the same partial products n times.
  The other common first idea, total product divided by nums[i], is O(n) but
  breaks when any element is 0 (and breaks worse with two zeros), and division
  is what this pattern is designed to avoid.
INVARIANT
  Before the write in pass 1, prefixProduct equals the product of nums[0..i-1];
  the write happens first and the update happens after, so result[i] never
  contains nums[i]. Entering pass 2 at index i, suffixProduct equals the product
  of nums[i+1..length-1], for the same reason: multiply first, then update. So
  after both passes result[i] = (product left of i) * (product right of i),
  which is exactly the required answer.
THE ORDER OF THE TWO LINES IS THE ALGORITHM
  In both loops the assignment comes before the accumulator update. Swap those
  two lines in either loop and result[i] silently includes nums[i], which is
  wrong for every index and still returns an array of the right size. The
  initial value 1 for both accumulators is what makes index 0 and index length-1
  work with no extra branch, since the empty product is 1.
WATCH OUT
  Products are held in int, so a long input of large values overflows and wraps
  silently; ask the interviewer for the value range, or use long for
  prefixProduct and suffixProduct if the answer still fits in int. The O(1)
  space claim counts only the two scalars - the returned result array is not
  counted, and this code needs no other buffer because pass 2 mutates result in
  place. nums is null-dereferenced at nums.Length with no guard; length 0
  returns an empty array, which is fine.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. What if division were allowed and no element is zero?
     One pass for the total product, then result[i] = total / nums[i]. Same O(n)
     time and fewer multiplications, but it fails on any zero and division is
     slower per element than multiplication.
  2. Can you do it in one pass instead of two?
     Yes: walk i forward and length-1-i backward in the same loop, doing
     result[i] *= prefixProduct and result[length-1-i] *= suffixProduct after
     initializing result to all 1s. Same work, just interleaved; the current
     two-pass form is easier to read and to prove.
  3. How would you answer many queries of "product except index i" on an array
  that changes?
     Prefix/suffix arrays go stale on every update, so switch to a segment tree
     or Fenwick-style structure over products, giving O(log n) per update and
     per query at the cost of O(n) extra memory and handling zeros separately.
TRIGGER
  Every index needs an aggregate of "all the other elements", and the combine
  operation splits cleanly into a left part and a right part.
C# NOTE
  new int[length] is already zero-filled by the runtime, and this code never
  relies on that - it overwrites every slot in pass 1 - so there is no need for
  Array.Fill(result, 1) as the one-pass variant would require.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
