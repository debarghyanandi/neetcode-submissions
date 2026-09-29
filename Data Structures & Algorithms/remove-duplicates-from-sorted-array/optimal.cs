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
 PROBLEM : You get an integer array nums sorted in non-decreasing order.
           Remove the duplicates in place so each value appears once, keeping
           the original order. Return k, the number of unique values. The
           first k slots must hold them. Example: [1,1,2] -> 2, and nums
           starts with [1,2].
 PATTERN : Two Pointers (fast reader r, slow writer l)
================================================================================
IDEA
  r reads every element. l is the next free slot for a new unique value.
  Because nums is sorted, a new value shows up exactly when nums[r] differs
  from nums[r - 1]. Then we copy it to nums[l] and move l forward.
  It is correct because l never passes r, so each value is read before its
  slot can be overwritten, and nums[0..l-1] always holds each value once.
EXAMPLE
  nums = [1,1,2,3,3], l = 1
  r=1 same, skip | r=2 2!=1 nums[1]=2 l=2 | r=3 3!=2 nums[2]=3 l=3 | r=4 same
  return 3, nums = [1,2,3,3,3] (the part after index k-1 does not matter)
COMPLEXITY
  Time  O(n)  one pass, r visits each index once, and each write is O(1)
  Space O(1)  only two ints l and r, all writes happen inside nums
PATH TO OPTIMAL
  Shift the tail left on each duplicate - O(n^2) - simple, but slow.
  Copy unique values to a new list or HashSet, then copy back - O(n) time,
  O(n) space - one pass, but uses extra memory.
  Reader/writer pointers in place - O(n) time, O(1) space - this file.
  (No sibling file: optimal.cs is the only one here.)
KEYWORDS
  two pointers, in-place, sorted array, deduplication, read/write pointer
WATCH OUT
  - Empty array: l starts at 1, so this code returns 1 for []. Add
    "if (nums.Length == 0) return 0;" if empty input is possible.
  - Comparing with nums[r - 1] only works because l never passes r. The
    safer check is nums[r] != nums[l - 1], which compares with the kept
    values.
  - Write first, then do l++. Doing l++ first leaves a gap at the old slot.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Allow each value at most twice (Remove Duplicates II)?
     -> Start l at 2 and keep nums[r] when nums[r] != nums[l - 2]. For at most
        k copies, compare with nums[l - k]. Still O(n) time and O(1) space.
  2. What if the array is not sorted?
     -> Keep a HashSet of seen values and write only unseen ones to nums[l].
        O(n) time but O(n) space. Or sort first: O(n log n), and order is lost.
  3. Why does l never overwrite a value we still need?
     -> l <= r always holds, so nums[l] was already read. When l == r the
        write copies a value onto itself.
  4. Same idea on a sorted linked list?
     -> Walk with one pointer and skip next nodes that have the same value.
        O(n) time, O(1) space, and no copying of values.
TRIGGER
  Sorted input plus "modify in place, O(1) extra space" means reach for a
  slow write pointer and a fast read pointer.
================================================================================
*/
