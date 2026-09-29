// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
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
 PATTERN : Two Pointers (read/write) - compact unique values in place
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  l  write index: nums[0..l-1] holds the unique values found so far
  r  read index: the next element to check
WHY THIS PATTERN
  The array is sorted, so equal values always sit next to each other. The task
  is to keep one copy of each value, in place. One pointer (r) reads every
  element. The other pointer (l) marks where the next new value goes. A value is
  new exactly when nums[r] differs from its left neighbor nums[r - 1].
BRUTE FORCE
  The first idea is to shift everything left by one each time you find a
  duplicate. That is correct but costs O(n^2) time, because each shift can move
  up to n elements. Another idea is to copy the unique values into a new list
  and write them back. That costs O(n) extra space, and the task wants the work
  done in place.
INVARIANT
  Before each step, nums[0..l-1] holds every distinct value from nums[0..r-1],
  each one once, in sorted order. Also, l <= r at all times, so a write never
  lands ahead of the read pointer. When nums[r] != nums[r - 1], nums[r] is a
  value not seen before, so placing it at nums[l] keeps the invariant true. When
  the loop ends, r = nums.Length, so the first l slots are exactly the answer.
COMPARING WITH NUMS[R - 1] IS SAFE
  The code compares nums[r] with nums[r - 1], and that slot might already have
  been written to. It is still safe. A write goes to index l, and l <= r at
  every step. So either index r - 1 was never written, or it was written with
  its own value when l was equal to r - 1. Comparing with nums[l - 1], the last
  value kept, also works and is easier to explain in an interview.
WATCH OUT
  If nums is empty, the loop never runs and the method returns 1, not 0. You
  need a guard: if (nums.Length == 0) return 0. The code also only works on
  sorted input. On unsorted input, like [1,2,1], equal values are not next to
  each other, so duplicates survive. Slots from index l to the end still hold
  old values, and callers must not read them.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. What if each value may appear at most twice?
     Start l at 2 and write nums[r] only when nums[r] != nums[l - 2]. For "at
     most k" copies, compare with nums[l - k]. It is still one pass with no
     extra memory.
  2. What if the array is not sorted?
     Keep a HashSet of values already seen, and write nums[r] at l only if the
     set did not have it. This uses O(n) extra space. Sorting first costs O(n
     log n) and changes the order of the elements.
  3. What if you must return the count and leave nums unchanged?
     Count the positions where nums[r] != nums[r - 1] and add 1 (when the array
     is not empty). The loop is the same, just without the writes.
TRIGGER
  A sorted array where you must remove or filter elements in place and return
  the new length.
C# NOTE
  nums.Distinct().ToArray() would be shorter, but it builds a new array. The
  caller only sees changes made to the original nums, so it does not meet the
  in-place contract. Writing through nums[l] changes the caller's array
  directly, because arrays are reference types (the method gets a reference to
  the same array, not a copy).
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
