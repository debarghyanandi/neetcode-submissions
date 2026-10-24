// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
// --------------------------------------------------------------------------

public class Solution
{
    public void SortColors(int[] nums)
    {
        int i = 0;
        int l = 0;
        int r = nums.Length - 1;

        while (i <= r)
        {
            if (nums[i] == 0)
            {
                Swap(nums, i, l);
                l++;
            }
            if (nums[i] == 2)
            {
                Swap(nums, i, r);
                r--;
                i--;
            }
            i++;
        }
    }

    private void Swap(int[] nums, int i, int j)
    {
        int temp = nums[i];
        nums[i] = nums[j];
        nums[j] = temp;
    }
}

/*
================================================================================
 PROBLEM : Given an array nums of only 0, 1 and 2 (red, white, blue), sort it
           in place so all 0s come first, then 1s, then 2s. Return nothing;
           change nums. You may not use the library sort. Example:
           [2,0,2,1,1,0] -> [0,0,1,1,2,2]
 PATTERN : Three Pointers (Dutch National Flag partition)
================================================================================
IDEA
  Keep three zones: [0, l) holds 0s, [l, i) holds 1s and (r, end] holds 2s.
  The zone [i, r] is not seen yet. If nums[i] is 0, swap it to l and grow
  both zones. If nums[i] is 2, swap it to r and shrink r. Then i-- cancels
  the i++, because the value that came from r has not been checked yet.
  It is correct because every step keeps the three zones valid, and the loop
  stops when the unseen zone is empty.
EXAMPLE
  [2,0,2,1,1,0] i=0: 2 -> swap(0,5) [0,0,2,1,1,2] r=4, recheck i=0
  i=0: 0 -> l=1; i=1: 0 -> l=2; i=2: 2 -> swap(2,4) [0,0,1,1,2,2] r=3
  i=2: 1, i=3: 1, then i=4 > r=3, so stop. Answer [0,0,1,1,2,2]
COMPLEXITY
  Time  O(n)  each step moves i up or r down, so at most n steps
  Space O(1)  only i, l, r and one temp for the swap
PATH TO OPTIMAL
  Built-in sort - O(n log n) - simple, but slower and not allowed here.
  Counting sort - O(n), two passes - count 0/1/2, then rewrite nums.
  One-pass three pointers - O(n), one pass, O(1) - this file (no sibling).
KEYWORDS
  Dutch national flag, three pointers, in-place partition, counting sort,
  one pass, 3-way partition
WATCH OUT
  - The loop must be i <= r, not i < r. With i < r the value at r is never
    checked: [1,0] stops at once and stays unsorted.
  - After a swap with r, do not move i forward. The new nums[i] came from
    the unseen zone. Here i-- then i++ does this.
  - The two separate ifs only work in this order. A 0 swap brings back a 1
    from l (or the same 0), so the 2 check cannot fire. Use else-if to be
    safe.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. What if there are k colors instead of 3?
     -> Use counting sort: O(n + k) time, O(k) space. Or partition by color
        again and again (rainbow sort), O(n log k) time, O(1) extra space.
  2. Is this sort stable?
     -> No. Swaps with r can reorder equal keys. With records attached, use a
        counting sort into an output array: stable, but O(n) extra space.
  3. Why not just count the colors?
     -> Counting needs two passes and works only for plain values. The swap
        method moves real objects in one pass, like quicksort's 3-way partition.
  4. Where else is this used?
     -> In 3-way quicksort, to group keys less than, equal to, and greater
        than the pivot. It stays fast when the array has many duplicate keys.
TRIGGER
  When an array with only two or three kinds of values must be grouped in
  place in one pass, think Dutch National Flag with pointers l, i, r.
================================================================================
*/
