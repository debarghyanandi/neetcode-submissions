// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(n) space
// -  HashSet with early exit   [hashset-early-exit]
// -  ties with optimal-variant.cs on O(n) time / O(n) space
// -
// -  Reference solution - not one you solved yourself
// -
// -  Single pass with immediate return when HashSet.Add() detects a
// -  duplicate.
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
 PATTERN : Hash Set - remember seen values, stop at first repeat
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Optimal
================================================================================
VARIABLES
  seen    every value from nums read so far in the loop
WHY THIS PATTERN
  The question is "does any value appear twice?" To answer it you only need to
  know whether you have met this value before. The order and the position do not
  matter. A hash set answers "have I seen this?" in constant time on average, so
  one pass over nums with seen is enough.
BRUTE FORCE
  The first idea is to compare every pair with two nested loops: for each i,
  check every j > i for nums[i] == nums[j]. This is correct and needs no extra
  memory. It runs in O(n^2) time, because the number of pairs grows with the
  square of n. So it is too slow for large input.
INVARIANT
  Before each loop step, seen holds exactly the values that came before number
  in nums, and none of them is repeated. If number is already in seen, it
  matched an earlier element, so returning true is correct. If the loop ends, no
  value was ever added twice, so all values are different and false is correct.
ADD AS LOOKUP AND INSERT
  seen.Add(number) returns false when the value is already present. So one call
  does both the check and the insert. This avoids the two-step pattern "if
  Contains then return, else Add", which does two hash lookups for each new
  value. The comment in the code describes this correctly.
WATCH OUT
  If nums is null, the foreach throws a NullReferenceException. There is no
  guard for it. An empty array is fine: the loop does not run and the method
  returns false. When no duplicate exists, seen grows to hold every element.
  That is the worst case for memory.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you do it with O(1) extra memory?
     Sort nums, then check each pair of neighbours for equal values. This takes
     O(n log n) time instead of O(n). Sorting in place also changes the caller's
     array. Copying the array first would bring back O(n) memory.
  2. What if a duplicate only counts when the two indices are at most k apart?
     Keep a sliding window. seen holds only the last k values. Remove nums[i -
     k] from seen when the window moves forward. Memory becomes O(k) instead of
     O(n).
  3. What if the values are known to be in a small range, for example 0..m?
     Use a bool[] or a BitArray of size m + 1 instead of a hash set. Checks
     become simple array index reads with no hashing. The cost is memory that
     grows with m, not with n.
TRIGGER
  When a problem asks "have I seen this value before?" and order does not
  matter, use a hash set.
C# NOTE
  You can write new HashSet<int>(nums.Length) to set the starting capacity. When
  there are no duplicates, the set will not need to resize and rehash while it
  grows to hold every element.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
