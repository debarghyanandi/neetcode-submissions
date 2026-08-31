// --------------------------------------------------------------------------
// -  optimal-variant.cs    O(n) time / O(n) space
// -  hash set membership, build full set then compare sizes
// -  [hashset-membership]
// -  ties with optimal.cs on O(n) time / O(n) space
// -
// -  Reference solution - not one you solved yourself (was suboptimal.cs)
// -
// -  Constructs a HashSet from the entire array (deduplicating) and
// -  compares its Count to nums.Length, so it always does n insertions even
// -  if a duplicate appears immediately, unlike the early-exit variant.
// --------------------------------------------------------------------------

public class Solution
{
    public bool hasDuplicate(int[] nums)
    {
        // Build the whole set first, then compare sizes.
        // A HashSet silently drops repeats, so a smaller Count means duplicates existed.
        return new HashSet<int>(nums).Count < nums.Length;
    }
}

/*
================================================================================
 PATTERN : Set cardinality - distinct count vs array length
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Optimal variant - ties the best complexity by another route
================================================================================
CORE IDEA
  Reduce "does a repeat exist?" to a counting question. Feeding nums into new
  HashSet<int>(nums) is a deduplication pass: every value that has been seen
  before is silently absorbed. So the resulting Count is exactly the number of
  DISTINCT values in nums, and nums.Length is the number of SLOTS. If those two
  numbers disagree, at least one slot was absorbed, which is precisely a
  duplicate. No index bookkeeping, no comparison logic - the set does all of it.
CORRECTNESS ARGUMENT
  Two halves, and an interviewer will want both.

  1. Count <= nums.Length always. The set is built only from elements of nums,
  so it can never hold more entries than there were insertions. This is what
  makes the strict < a safe test rather than a guess - the > case is
  unreachable, so < and != are equivalent here, and < states the direction of
  the bound.

  2. Count == nums.Length if and only if all values are distinct. Each insertion
  either grows Count by 1 (new value) or by 0 (value already present). Equality
  means every insertion grew it, i.e. no value was ever seen twice. A strictly
  smaller Count means some insertion was a no-op, i.e. a value repeated.

  Edge cases fall out for free: an empty array gives 0 < 0, false; a single
  element gives 1 < 1, false. Neither needs a special branch.
VS THE EARLY-EXIT ROUTE
  The sibling solution loops and returns true the moment set.Add(n) returns
  false. Same worst case, different profile:

  - This file always consumes all n elements and always allocates storage for
  every distinct value, even when nums is {1, 1, 5, 9, ...} and the answer was
  decidable after two reads.
  - The Add loop stops at the first repeat, so on duplicate-heavy input it
  touches far fewer elements and holds a smaller set.

  They tie only on the all-distinct input, where the loop has to reach the end
  anyway. Pick this version for its one-line readability; pick the loop when you
  expect duplicates early or memory is tight. Do not claim this one is faster -
  it cannot be, it does a superset of the loop's work.
WATCH OUT
  - The whole result hinges on HashSet dropping repeats silently rather than
  throwing or returning a signal. If you rewrote this with a structure that
  permits duplicates (a List<int>, a multiset), Count would equal nums.Length
  unconditionally and the method would return false forever. The bug would be
  invisible - it compiles and passes on any all-distinct test.
  - Do not "optimize" by comparing against a captured length taken after some
  mutation of nums; nums.Length must be the same array the set was built from,
  or the comparison is meaningless.
  - Correctness here rests on int's default equality. Swap in a reference type
  without a value-based Equals/GetHashCode and the set stops collapsing
  equal-but-distinct instances, so real duplicates go unreported.
FOLLOW-UPS TO EXPECT
  "Can you do it without the extra allocation?" Sort nums first and scan
  adjacent pairs for nums[i] == nums[i-1]: O(n log n) time, O(1) extra space,
  but it destroys the caller's array order - say so before you write it.

  "What if the values are bounded, say 1..n?" Then pigeonhole applies and you
  can mark visited slots in place by negating nums[Math.Abs(v) - 1], recovering
  O(1) extra space at O(n) time.

  "What if nums does not fit in memory?" Neither this nor the Add loop survives;
  that becomes external sort or a Bloom filter as a probabilistic pre-filter.
TRIGGER
  Reach for the count-vs-length comparison whenever a problem asks a yes/no
  existence question about repeats and you do not need to know WHICH value
  repeated or where. The moment the problem wants the offending value, its
  index, or a count of repeats, this collapses back to an explicit Add loop or a
  frequency dictionary - Count throws away exactly the information those
  variants need.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
