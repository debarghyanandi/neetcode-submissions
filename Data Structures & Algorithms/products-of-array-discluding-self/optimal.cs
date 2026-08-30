// --------------------------------------------------------------------------
//  Reference solution - from NeetCode / other resource (submission-0)
//  Not one you solved yourself.
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
 PATTERN : Prefix & Suffix Products (two-pass accumulation)
 SOURCE  : NeetCode / other resource (submission-0, refactored: named
           prefix/suffix accumulators, no Array.Fill)
 STATUS  : Optimal
================================================================================

WHY THIS PATTERN
  "Everything except me" decomposes into "everything to my left" times
  "everything to my right". Both halves are running accumulations, and a
  running accumulation is one linear sweep. Two sweeps, no division, O(n).

BRUTE FORCE (and why it fails)
  For each i, loop the array multiplying all j != i: O(n^2).
  The redundancy is obvious once named - the prefix for index 5 recomputes
  everything the prefix for index 4 already knew.

WHY NOT TOTAL PRODUCT / nums[i]?
  It is O(n) and one line, and the problem BANS division precisely because it
  breaks on zeros: one zero makes every other answer 0/0, two zeros make the
  whole array zero. Handling that needs a zero-count special case. The
  prefix/suffix version has no special cases at all - zeros flow through
  naturally. That robustness is the real reason this is the taught solution.

INVARIANT
  After pass 1: result[i] = nums[0] * ... * nums[i-1]   (1 when i = 0)
  After pass 2: result[i] = that, times nums[i+1] * ... * nums[n-1]

THE TRICK
  Assign result[i] BEFORE folding nums[i] into the accumulator. That one
  ordering is what excludes the element itself, in both directions.

ALGORITHM (NeetCode: "Prefix & Suffix")
  1. result array of length n.
  2. Sweep left to right carrying prefixProduct; write, then multiply.
  3. Sweep right to left carrying suffixProduct; multiply into result, then
     fold nums[i] in.

COMPLEXITY
  Time  : O(n) - exactly two passes.
  Space : O(1) extra - the output array does not count as auxiliary space by
          the problem's own definition; only the two int accumulators do.
          The naive version of this keeps separate prefix[] and suffix[]
          arrays for O(n) extra - reusing `result` as the prefix array is
          what gets it to O(1).

TRIGGER
  "For each index, compute something over ALL OTHER elements", or any range
  query answerable as (accumulate from left) combined with (accumulate from
  right). Same family as prefix sums, range-sum queries, and trapping rain
  water.

C# NOTES
  - int overflow is silent in C# by default (unchecked). The problem
    constrains the product to fit in int; in production use long or wrap in
    a `checked` block so it throws instead of corrupting quietly.
  - Array.Fill(result, 1) is unnecessary here: pass 1 writes every slot
    before it is ever read.
  - Two plain for-loops beat any LINQ formulation on both clarity and speed
    for this shape.

WATCH OUT
  - Reversing the write/multiply order in either pass silently includes
    nums[i] in its own product. It still runs, still returns numbers, and is
    wrong - the classic bug in this problem.
================================================================================
*/
