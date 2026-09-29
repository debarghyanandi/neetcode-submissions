// ##########################################################################
// #  optimal.cs            O(n) time / O(1) space
// ##########################################################################

public class Solution
{
    public void MoveZeroes(int[] nums)
    {
        // Everything before `writeIndex` is a non-zero, in original order.
        int writeIndex = 0;

        for (int readIndex = 0; readIndex < nums.Length; readIndex++)
        {
            if (nums[readIndex] != 0)
            {
                // Swap rather than assign: this carries the zero that was at
                // writeIndex out to readIndex, so the tail fills with zeroes
                // automatically and needs no second pass.
                (nums[writeIndex], nums[readIndex]) = (nums[readIndex], nums[writeIndex]);
                writeIndex++;
            }
        }
    }
}

/*
================================================================================
 PROBLEM : You get an integer array nums. Move all 0s to the end, in place.
           The non-zero values must keep their original relative order. Return
           nothing; you change nums itself. Example: [0,1,0,3,12] ->
           [1,3,12,0,0].
 PATTERN : Two Pointers (same direction, read/write)
================================================================================
IDEA
  readIndex scans every element. writeIndex marks where the next non-zero
  goes.
  When nums[readIndex] is non-zero, swap it into nums[writeIndex] and move
  writeIndex.
  The zeros get pushed right by the swaps, so the tail ends up all zeros.
  It is correct because everything before writeIndex is always the non-zeros
  seen so far, in the order they were read.
EXAMPLE
  nums=[0,1,0,3,12]; r=0 is zero, skip; r=1 swap(0,1) -> [1,0,0,3,12], w=1
  r=2 is zero, skip; r=3 swap(1,3) -> [1,3,0,0,12], w=2
  r=4 swap(2,4) -> [1,3,12,0,0], w=3. Answer: [1,3,12,0,0]
COMPLEXITY
  Time  O(n)  readIndex visits each index once; each swap is O(1)
  Space O(1)  only two int indices; the swap is in place
PATH TO OPTIMAL
  Copy non-zeros to a new array, pad with zeros - O(n) / O(n) - simple, uses
  extra memory.
  Bubble each zero to the end by repeated swaps - O(n^2) / O(1) - no memory,
  but slow.
  Read/write pointers with swap (this file) - O(n) / O(1) - one pass, no extra
  array.
KEYWORDS
  two pointers, in-place, stable partition, read/write pointer, array, swap
WATCH OUT
  - The comment says the swap "carries the zero" out, but while no zero is
    seen yet, readIndex == writeIndex and it just swaps a value with itself.
  - Plain assign (nums[w]=nums[r]) without a second pass that fills zeros
    leaves old values in the tail: [0,1] would become [1,1].
  - Increment writeIndex only inside the if; moving it every step breaks
    order.
  - Do not return a new array; the caller reads nums after the call.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you do fewer writes when zeros are rare?
     -> Skip the swap when readIndex == writeIndex. Same O(n) / O(1), fewer
        writes.
  2. What if order of non-zeros did not matter?
     -> Use converging pointers: swap a zero from the left with a non-zero
        from the right. Still O(n) / O(1), fewer moves, but it is not stable.
  3. Move all copies of a value val instead of 0 (Remove Element)?
     -> Same loop with nums[readIndex] != val; writeIndex is the new length.
  4. Why is the relative order kept?
     -> Non-zeros are written to writeIndex in the order readIndex meets them,
        and writeIndex never passes readIndex, so none is overwritten early.
TRIGGER
  When you must keep or remove some elements in place and preserve their
  order,
  use a slow write pointer behind a fast read pointer.
================================================================================
*/
