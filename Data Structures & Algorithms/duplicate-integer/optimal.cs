// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(n) space
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
 PROBLEM : Given an integer array nums, return true if any value appears at
           least twice. Return false if every value is distinct. Example: [1,
           2, 3, 3] -> true, [1, 2, 3] -> false.
 PATTERN : Hashing (HashSet for seen values)
================================================================================
IDEA
  Walk nums once and keep every value met so far in the HashSet seen.
  seen.Add(number) returns false when number is already in the set. That
  means we met it before, so return true at once. If the loop ends, no
  value repeated, so return false. It is correct because the set always
  holds exactly the values before the current index.
EXAMPLE
  nums = [3, 1, 4, 1]
  Add(3) true, Add(1) true, Add(4) true, seen = {3, 1, 4}
  Add(1) false, since 1 is already in seen -> return true (4 is never read)
COMPLEXITY
  Time  O(n)  one pass, each Add is O(1) on average
  Space O(n)  seen may hold all n values when there is no duplicate
PATH TO OPTIMAL
  Compare every pair i < j - O(n^2) time, O(1) space - no extra memory.
  Sort, then compare neighbors - O(n log n), O(1) or O(n) - no pair loop.
  HashSet lookup, then insert - O(n) time, O(n) space - uses memory for speed.
  One Add call does both steps - this file. optimal-variant.cs is a sibling.
KEYWORDS
  contains duplicate, hash set, hashing, frequency, early exit, array
WATCH OUT
  - Do not flip the test. Add returns TRUE for a new value, so the
    duplicate case is !seen.Add(number).
  - A null nums throws in the foreach. Say so, or guard it if asked.
  - Sorting to save memory changes the caller's array. Ask before you do it.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you use O(1) extra space?
     -> Sort nums, then check nums[i] == nums[i-1]. Time is O(n log n), and it
        changes the input or needs a copy.
  2. Duplicate within distance k (Contains Duplicate II)?
     -> Keep only the last k values in the set, a sliding window. Remove
        nums[i-k] as you move. O(n) time, O(k) space.
  3. What if values are small, like 0..m?
     -> Use a bool array of size m+1 in place of the HashSet. O(n) time, O(m)
        space, and no hashing cost.
  4. The data is a stream too big for memory?
     -> A Bloom filter says "maybe seen" in little memory, but it can give
        false positives. Or sort chunks on disk and merge them.
TRIGGER
  When you must know "have I seen this value before?" in one pass, use a
  hash set.
================================================================================
*/
