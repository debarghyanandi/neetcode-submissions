// --------------------------------------------------------------------------
// -  optimal-variant.cs    O(n) time / O(n) space
// -  hash set, build full set then compare cardinality to length
// -  [hashset-membership]
// -  ties with optimal.cs on O(n) time / O(n) space
// -
// -  Reference solution - not one you solved yourself
// -
// -  always inserts all n elements into a HashSet and compares Count to
// -  nums.Length, never exiting early even on immediate duplicates
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
 PATTERN : Hash Set - dedupe then compare cardinality
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Optimal variant - ties the best complexity by another route
================================================================================
WHY THIS PATTERN
  The question "does any value repeat" is the question "is the number of
  distinct values smaller than the number of slots". A HashSet<int> answers the
  first half for free: feeding nums into its constructor collapses every repeat,
  so Count is exactly the count of distinct values. Nothing else has to be
  tracked - no counter, no flag, no explicit loop.
CORRECTNESS
  Two facts carry the whole return statement.
  1. Every element of nums is offered to the set, so Count can never exceed
  nums.Length.
  2. An insert of a value already present leaves Count unchanged; an insert of a
  new value raises it by one.
  So after all nums.Length inserts, Count == nums.Length exactly when every
  insert was new (all distinct), and Count < nums.Length exactly when at least
  one insert was absorbed - which is precisely a repeated value. The two cases
  are exhaustive, so the single comparison decides it.
  Degenerate inputs fall out without special-casing: length 0 gives 0 < 0 =
  false, length 1 gives 1 < 1 = false.
WHAT THIS VARIANT TRADES
  The other route is a loop that calls Add per element and returns true the
  moment Add reports false. That version bails at the first collision; this one
  always consumes all nums.Length elements before it looks at anything.
  On the worst case - all values distinct, answer false - the two do identical
  work, which is why this ties and does not lose. They diverge on duplicate-rich
  input: nums = [1,1,1,...,1] finishes after two elements in the early-exit loop
  and after the whole array here. Same asymptotics, strictly more work on the
  lucky inputs.
  What you buy for that is the absence of a loop body to get wrong - no index
  bounds, no inverted Add return value.
WATCH OUT
  Count < nums.Length and Count != nums.Length are interchangeable here, because
  fact 1 above rules out the greater-than side. Writing < is the honest version:
  it says which direction is possible. Writing > compiles and is dead - always
  false.
  The set is a separate allocation; nums is read, never reordered. That matters
  when you compare against the sort-then-scan-adjacent alternative, which is
  slower asymptotically and, done in place, destroys the caller's ordering.
  Type-generalization trap: for int the set's default comparer agrees with ==.
  If you lift this to double, EqualityComparer<double>.Default routes through
  double.Equals, which treats NaN as equal to NaN - so [NaN, NaN] reports a
  duplicate even though NaN == NaN is false. Know which notion of equality the
  interviewer wants before you generalize the element type.
TRIGGER
  Reach for this shape when the answer needed is a bare yes/no about existence
  and the input is small enough to copy. The moment the ask changes to "which
  value repeats" or "where" or "how many times", the cardinality comparison has
  thrown away the evidence - switch to the Add loop and return the offending
  element, or to a Dictionary<int,int> of counts.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
