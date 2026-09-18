// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
// -  Two-pointer, overwrite duplicates   [two-pointer-duplicates]
// -  the only solution in this folder
// -
// -  Reference solution - not one you solved yourself
// -
// -  Single pass with two pointers; duplicate elements overwritten in-place
// -  with no extra storage.
// --------------------------------------------------------------------------

public class Solution
{
    public int RemoveDuplicates(int[] nums)
    {
        int l = 1;
        for (int r = 1; r < nums.Length; r++)
        {
            if (nums[r] != nums[r - 1])
            {
                nums[l] = nums[r];
                l++;
            }
        }
        return l;
    }
}

/*
================================================================================
 PATTERN : Two Pointers - slow write index, fast read index
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  l    next slot to write into; also the count of unique values kept so far
  r    scan index; nums[r] is the candidate being examined
WHY THIS PATTERN
  The input is sorted, so every group of equal values sits in one continuous
  block, and a duplicate can only be the element right before it. The problem
  also asks to edit the array in place and return a length, not to build a new
  array. That is exactly a two-pointer split: nums[0..l-1] is the clean result
  being written, and r walks ahead over the raw data. Because l never passes r,
  the write never destroys a value that has not been read yet.
BRUTE FORCE
  The first idea most people write is to copy nums into a HashSet or a List,
  keep only values not seen before, sort if needed, then copy back. That is O(n)
  time but O(n) extra memory, and it ignores the fact that the array is already
  sorted. A second common try is to remove duplicates by shifting the tail left
  each time a repeat is found, which costs O(n^2) in the worst case when the
  array is all one value.
INVARIANT
  Before each iteration, nums[0..l-1] holds the distinct values of nums[0..r-1]
  in their original sorted order. The test nums[r] != nums[r-1] is enough to
  detect a new value, because sorting means any earlier occurrence of nums[r]
  would have to be adjacent to it. When the loop ends, r has covered the whole
  array, so nums[0..l-1] is the full distinct list and l is its length.
WHY BOTH POINTERS START AT 1
  Index 0 is always unique by definition, so it is counted up front by setting l
  = 1 and never compared. Starting r at 1 also makes nums[r - 1] safe on the
  very first read. Note nums[r - 1] refers to the original array only while l ==
  r; after the first duplicate, l lags behind r, but the comparison still uses r
  - 1, the unmodified neighbour in the read region, which is what correctness
  needs.
WATCH OUT
  If nums is empty, the loop body never runs and the method returns 1, claiming
  one valid element in a zero-length array; a caller that then reads nums[0]
  throws. If nums is null, nums.Length throws a NullReferenceException before
  anything else. The method mutates the caller's array, so any code holding the
  same reference sees the rewritten contents and the stale tail beyond index l -
  1 left untouched.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Allow each value to appear at most twice instead of once.
     Compare against nums[l - 2] instead of nums[r - 1], and start l at 2 with a
     guard for arrays shorter than 3. The comparison must move to the written
     region so the count of kept copies is what limits you.
  2. What if the array is not sorted but you must keep the first occurrence of
  each value?
     You need a HashSet<int> of seen values and the same write pointer l, which
     costs O(n) extra space. Without sorting there is no way to detect a repeat
     by looking only at the neighbour.
  3. The array is huge and lives on disk or arrives as a stream.
     Turn it into a single pass that yields a value only when it differs from
     the previous one, keeping just the last emitted value in memory. You lose
     the in-place return-a-length contract and return a sequence instead.
  4. How would you also report how many elements were removed?
     Return nums.Length - l, computed at the end from the same l; no extra
     counter is needed since l already counts survivors.
TRIGGER
  Sorted input plus a requirement to compact or filter in place and return a new
  length.
C# NOTE
  This works on int[] directly, so nums[l] = nums[r] is a plain array store with
  no bounds surprises beyond the loop condition; reaching for
  List<int>.Distinct() or a new array here would allocate and break the in-place
  contract the signature implies.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
