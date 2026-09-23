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
 PATTERN : Hash Set Membership - size drop reveals repeats
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Optimal variant - ties the best complexity by another route
================================================================================
VARIABLES
  nums                    the input array; nums.Length is the count with duplicates kept
  new HashSet<int>(nums)  unnamed temporary; holds each distinct value of nums exactly once
WHY THIS PATTERN
  The question only asks whether any value appears twice - it does not ask which
  value, or where. That turns it into a counting question: how many distinct
  values does nums hold? A hash set stores each value once and silently ignores
  a second insert of the same value, so its Count is exactly the number of
  distinct values. If that Count is below nums.Length, at least one insert was
  dropped, and a dropped insert is a duplicate.
BRUTE FORCE
  The first thing most people write is a double loop comparing nums[i] with
  nums[j] for every j > i and returning true on the first match. That is correct
  but quadratic in time, though it uses no extra memory. Sorting first and then
  checking neighbouring pairs is the middle option at n log n time. The hash set
  trades memory for a single pass, which is the better trade when the array is
  large.
INVARIANT
  After the set constructor has consumed the first k elements of nums, the set
  contains exactly the distinct values among those k elements, so set.Count <= k
  always, with equality only when those k were all different. At the end k is
  nums.Length, so Count < nums.Length holds if and only if at least one element
  was ever a repeat of an earlier one. That equivalence is the whole correctness
  argument - no separate scan is needed.
NO EARLY EXIT
  This version always reads the entire array, even if nums[0] and nums[1] are
  already equal. The common alternative loops and returns true the moment Add
  returns false, which can stop after two elements. Both are the same order of
  work in the worst case, when the answer is false and every element must be
  seen. The loop version only wins on inputs where a duplicate appears early,
  and it costs a few extra lines.
WATCH OUT
  The method name hasDuplicate starts with a lower-case letter, which breaks the
  usual C# convention and will not match an interface or override that spells it
  HasDuplicate - check what the judge expects. There is no null check on nums:
  passing null throws inside the HashSet constructor, not at the comparison, so
  the stack trace points at a line that looks innocent. An empty array returns
  false correctly, since 0 < 0 is false. The code also builds the full set even
  when the answer is obvious from the first two elements, so peak memory is the
  whole distinct set regardless of the answer.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. What if you are not allowed extra memory?
     Sort nums in place and compare each element with its neighbour. That drops
     extra space to constant but raises time to n log n and destroys the
     original order, which may matter to the caller.
  2. What changes if the values are known to lie in a fixed small range, say 0
  to 9?
     Replace the set with a fixed bool or int array indexed by the value, or a
     bitmask. Lookup becomes a plain array index with no hashing, and memory is
     fixed rather than growing with n.
  3. The array is too large to fit in memory and arrives as a stream. Now what?
     You cannot hold every distinct value, so exact answers need external
     sorting or partitioning by hash into chunks that do fit. If an approximate
     answer is acceptable, a Bloom filter gives false positives but never false
     negatives, at far less memory.
  4. Instead of true or false, return the first value that repeats.
     Switch to an explicit loop with a HashSet and return the current element
     the moment Add returns false. The set-size trick cannot do this, because
     Count tells you a duplicate exists but not which one.
TRIGGER
  A question that asks only "does any element repeat" - existence, not position
  or count - should make you reach for a hash set and compare distinct count
  against total count.
C# NOTE
  The HashSet<int> constructor that takes an IEnumerable<T> does the whole loop
  for you, and because nums is an int[] it can size the internal buckets up
  front from the array's known length. Using HashSet<int> rather than
  Dictionary<int,bool> also avoids storing a value you never read.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
