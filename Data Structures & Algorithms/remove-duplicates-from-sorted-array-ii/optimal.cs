// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
// --------------------------------------------------------------------------

public class Solution
{
    public int RemoveDuplicates(int[] nums)
    {
        int l = 0, r = 0;

        while (r < nums.Length)
        {

            int count = 1;

            while (r + 1 < nums.Length && nums[r] == nums[r + 1])
            {
                r++;
                count++;
            }

            for (int i = 0; i < Math.Min(2, count); i++)
            {
                nums[l] = nums[r];
                l++;
            }
            r++;
        }
        return l;
    }
}

/*
================================================================================
 PROBLEM : Given a sorted int array nums, remove extra duplicates in place so
           each value appears at most twice. Keep the relative order. Return
           k, the new length; the first k slots must hold the result. Example:
           [1,1,1,2,2,3] -> 5, nums starts with [1,1,2,2,3].
 PATTERN : Two Pointers (read r / write l) over runs of equal values
================================================================================
IDEA
  r walks the array one run of equal values at a time. The inner loop
  moves r to the last element of the run and counts its length in count.
  Then we write nums[r] into nums[l] Math.Min(2, count) times and move l.
  It is correct because the input is sorted, so all copies of a value
  form one run. Also l never passes r, so we never overwrite unread data.
EXAMPLE
  nums=[1,1,1,2,2,3]
  run 1s: r=2,count=3 -> write 2 copies, l=2 | run 2s: r=4,count=2 -> l=4
  run 3: r=5,count=1 -> write 1 copy, l=5 | r=6 ends loop
  return 5, nums=[1,1,2,2,3,3] (only first 5 count)
COMPLEXITY
  Time  O(n)  r moves forward only; each index is read once, at most n writes
  Space O(1)  only l, r, count and i; all edits happen in place
PATH TO OPTIMAL
  Shift the tail left on each extra copy - O(n^2) - simple but slow.
  Copy kept items into a new array, then back - O(n) time, O(n) space.
  In-place read/write pointers (optimal.cs) - O(n), O(1) - no extra array.
KEYWORDS
  two pointers, in-place, sorted array, at most k duplicates, runs, read/write
WATCH OUT
  - Forgetting the final r++ after a run causes an infinite loop.
  - Writing nums[l] = nums[r] is safe only because l <= r; check this
    if you change the write count.
  - In the shorter version (if nums[i] != nums[k-2]), compare with the
    written nums[k-2], not nums[i-2]. Old values may already be gone.
  - The values after index k are leftovers; do not return or test them.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Allow at most k copies instead of 2?
     -> Replace 2 with k in Math.Min(k, count). Or keep a write index w and
        copy nums[i] when w < k or nums[i] != nums[w-k]. Still O(n) / O(1).
  2. Is there a shorter one-loop version?
     -> Start w=2. For each i from 2, if nums[i] != nums[w-2], set nums[w++] =
        nums[i]. Same O(n) / O(1), no inner loop, fewer bugs.
  3. What if the array is not sorted?
     -> Use a hash map of counts and keep a value while its count <= 2. O(n)
        time, O(n) space. Or sort first, but that loses the order.
  4. Why does keeping order work in place?
     -> Writes only go to l <= r, and they move forward. So every kept item
        moves left or stays, in its original order.
TRIGGER
  A sorted array that must be filtered in place with O(1) space calls for
  a slow write pointer and a fast read pointer.
================================================================================
*/
