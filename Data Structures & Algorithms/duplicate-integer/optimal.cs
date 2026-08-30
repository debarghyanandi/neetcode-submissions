// --------------------------------------------------------------------------
//  Reference solution - from NeetCode / other resource (submission-0)
//  Not one you solved yourself.
// --------------------------------------------------------------------------

public class Solution
{
    public bool hasDuplicate(int[] nums)
    {
        var seen = new HashSet<int>();

        foreach (int number in nums)
        {
            // HashSet.Add returns FALSE when the value was already present.
            // One call does both the lookup and the insert.
            if (!seen.Add(number))
                return true;
        }

        return false;
    }
}

/*
================================================================================
 PATTERN : Hashing - Set Membership (with early exit)
 SOURCE  : NeetCode / other resource (submission-0)
 STATUS  : Optimal
================================================================================

WHY THIS PATTERN
  Detecting a repeat only needs to know whether a value has been seen, never
  where or how often. That is exactly a set membership test - O(1) average.

BRUTE FORCE (and why it fails)
  Nested loops comparing every pair: O(n^2) time, O(1) space. Trading O(n)
  memory for a hash set collapses that to O(n) time. This is the canonical
  "space for time" trade and the first instinct to build for the whole
  Arrays & Hashing chapter.

INVARIANT
  After processing index i, `seen` holds exactly the distinct values of
  nums[0..i]. So a failed Add at index i proves nums[i] appeared earlier.

ALGORITHM (NeetCode: "Hash Set")
  1. Create an empty HashSet<int>.
  2. Walk the array once.
  3. Add the current value. If Add returns false, it was already there -> true.
  4. Loop finished without a failed Add -> no duplicates -> false.

COMPLEXITY
  Time  : O(n) worst case, but returns as soon as the first repeat is found,
          so the average on duplicate-heavy input is far below n.
  Space : O(n) worst case (all distinct); grows only up to the first repeat.

TRIGGER
  "Contains duplicate", "are all elements unique", "does anything repeat".
  More generally: any single pass where you must remember what you have seen.

C# NOTES
  - `seen.Add(x)` returns bool. Using it as the test avoids the classic
    double hash lookup of `if (seen.Contains(x)) ... else seen.Add(x)`.
    Same idea as Dictionary.TryGetValue over ContainsKey + indexer.
  - `new HashSet<int>(nums.Length)` pre-sizes the buckets and skips the
    internal re-hashing as it grows. Worth it when n is large and known.

WATCH OUT
  - Method name is lower-case hasDuplicate to match the NeetCode judge stub.
  - If the question changes to "find the duplicate value" or "return its
    index", swap HashSet for Dictionary<int,int> - the shape stays the same.
================================================================================
*/
