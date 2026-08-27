// --------------------------------------------------------------------------
//  Reference solution - from NeetCode / other resource (submission-1 + submission-2)
//  Not one you solved yourself.
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
 PATTERN : Hashing - Set Membership
 SOURCE  : NeetCode / other resource (submission-1 + submission-2, byte-identical)
 STATUS  : Sub-optimal (same Big-O as optimal, worse in practice)
================================================================================

WHY THIS PATTERN
  "Have I seen this value before?" is a membership question. A hash set answers
  it in O(1) average, which is the whole reason we do not sort or nest loops.

BRUTE FORCE (and why it fails)
  Compare every pair with two nested loops -> O(n^2). At n = 100,000 that is
  10^10 comparisons. Sorting first gives O(n log n) and destroys input order.

WHY THIS ONE IS SUB-OPTIMAL
  Identical Big-O to optimal.cs, but it ALWAYS inserts all n elements before
  answering. If nums = [1, 1, 2, 3, ..., 100000] the answer is known after two
  inserts; this version still does 100,000. Same worst case, much worse average.
  It also always peaks at O(n) memory, where the early-exit version may not.

ALGORITHM (NeetCode: "Hash Set Length")
  1. Construct a HashSet<int> from nums - the constructor de-duplicates.
  2. If the set is smaller than the array, at least one value repeated.

COMPLEXITY
  Time  : O(n)  - n insertions, each O(1) average.
  Space : O(n)  - worst case every element is distinct and stored.

  Caveat on "O(1) average": adversarial inputs that all collide degrade to
  O(n) per insert -> O(n^2) total. Not a concern on LeetCode, worth knowing
  when a hash key is attacker-controlled in production.

TRIGGER
  "Does this collection contain a duplicate / has this been seen before?"
  with no requirement to report WHERE or HOW MANY.

C# NOTES
  - new HashSet<int>(nums) accepts any IEnumerable<T> and de-duplicates for you.
  - Prefer this one-liner ONLY when you need the count of distinct values
    anyway; otherwise the early-exit version reads just as clearly.
  - HashSet<int> uses the default EqualityComparer<int>, no boxing.

WATCH OUT
  - Method name is lower-case hasDuplicate to match the NeetCode judge stub.
    Not C# convention - do not "fix" it here.
================================================================================
*/
