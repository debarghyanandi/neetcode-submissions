// --------------------------------------------------------------------------
// -  optimal-variant.cs    O(n) time / O(n) space
// -  HashSet size comparison   [hashset-size-compare]
// -  ties with optimal.cs on O(n) time / O(n) space
// -
// -  Reference solution - not one you solved yourself
// -
// -  Constructs complete HashSet from array, then compares count against
// -  original length.
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
 PATTERN : Hash Set - compare distinct count to total count
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Optimal variant - ties the best complexity by another route
================================================================================
VARIABLES
  nums                    the input array; nums.Length is the total count, repeats included
  new HashSet<int>(nums)  unnamed set; holds each distinct value of nums once
  Count                   Count = number of distinct values in nums
WHY THIS PATTERN
  The question asks only whether any value appears twice. It does not ask which
  value or where. A set keeps each value once, so all duplicates show up as one
  number: the gap between nums.Length and the set's Count. If the set is smaller
  than the array, at least one value was repeated.
BRUTE FORCE
  The first correct idea is to compare every pair i < j and return true when
  nums[i] == nums[j]. That takes O(n^2) time and O(1) extra space. It loses
  because the number of pairs grows with the square of n. The set does each
  lookup in expected O(1) time, so it only needs one pass over the array.
INVARIANT
  As the constructor reads nums from left to right, the set always holds exactly
  the distinct values seen so far. When a value is already in the set, adding it
  again does nothing and Count does not grow. So when the constructor finishes,
  Count equals the number of distinct values in nums. Count < nums.Length is
  then true exactly when some value was dropped, which means a duplicate exists.
WATCH OUT
  The code has no early exit. Even if nums[0] == nums[1], it still hashes the
  whole array before it compares the sizes. If nums is null, the HashSet
  constructor throws ArgumentNullException. The code does not return false for
  null. An empty array gives 0 < 0, which is false. That is the correct answer.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you do it with O(1) extra memory?
     Sort nums in place, then check neighbours: nums[i] == nums[i-1]. This takes
     O(n log n) time, and it changes the caller's array.
  2. What if the values are guaranteed to be in 1..n and the array has n+1
  items?
     Treat each value as a pointer to an index and use Floyd's cycle detection
     (a slow pointer and a fast pointer). This takes O(n) time and O(1) space,
     and the array is not changed. It only works when that value range is
     guaranteed.
  3. Return true only if two equal values are at most k indices apart.
     Use a sliding window. Keep a set of only the last k values. Remove
     nums[i-k] as the window moves forward. Space drops to O(k).
  4. What if the data is a stream too large to fit in memory?
     Use a Bloom filter, a compact bit array that answers "possibly seen" or
     "definitely not seen". It can report false positives, so each "possibly
     seen" needs a check against real storage. Another option is an external
     sort on disk followed by a neighbour scan.
TRIGGER
  Reach for this pattern when the question is only "is anything repeated?" or
  "how many are unique?" and the order and positions do not matter.
C# NOTE
  HashSet<int> is generic, so it stores and compares int values directly with no
  boxing. The old non-generic Hashtable would wrap each int in an object. Count
  is a stored property, so reading it does not walk the set again.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
