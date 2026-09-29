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
 PATTERN : Two Pointers (read/write) - in-place stable partition
 SOURCE  : YOUR OWN SOLUTION - your own annotation at c76939d
 STATUS  : Optimal
================================================================================
VARIABLES
  writeIndex  the next slot for a non-zero; nums[0..writeIndex) holds all non-zeros seen so far
  readIndex   the scan position; each element is looked at exactly once
WHY THIS PATTERN
  The problem asks you to move the zeroes to the end, keep the order of the
  non-zeroes, and do it in place. "In place" rules out a second array. "Keep the
  order" means a stable partition: the elements are split into two groups, and
  each group stays in its original order. A read/write pointer pair does exactly
  this. readIndex finds each non-zero, and writeIndex says where it goes.
  Because non-zeroes are placed left to right in the order they are found, their
  order is kept.
BRUTE FORCE
  The first correct idea is to copy every non-zero into a new array, fill the
  rest with 0, and copy the result back into nums. This is O(n) time but needs
  O(n) extra space, so it breaks the in-place rule. Another in-place idea keeps
  its O(1) space: each time you find a zero, shift everything after it one step
  left and put the zero at the end. That costs O(n^2) time.
INVARIANT
  Before each loop step, two things are true. nums[0..writeIndex) holds every
  non-zero seen so far, in original order. nums[writeIndex..readIndex) holds
  only zeroes. When nums[readIndex] is a non-zero, the swap moves it to the
  front of the zero block and sends one zero to the back of that block, so both
  statements stay true. When readIndex reaches nums.Length, the zero block
  reaches the end of the array, and that is the answer.
WATCH OUT
  The swap comment says it carries "the zero that was at writeIndex". That is
  only true when writeIndex < readIndex. Until the first zero appears, the two
  indices are equal, so the code swaps a non-zero with itself. The result is
  still correct, but every element is written for no reason. A null nums throws
  a NullReferenceException at nums.Length. There is no guard for it.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How would you cut down the number of writes to the array?
     Add "if (writeIndex != readIndex)" before the swap, so self-swaps are
     skipped. Or assign nums[writeIndex] = nums[readIndex] with no swap, then
     fill nums[writeIndex..] with 0 at the end. The assign version writes each
     slot about once, but it needs that second pass.
  2. Remove every copy of a value val and return the new length (Remove
  Element).
     Use the same loop, but test nums[readIndex] != val and assign instead of
     swapping. Return writeIndex. The tail does not matter, so you need neither
     the swap nor the fill.
  3. What if the zeroes must go to the front instead?
     Run the same idea from right to left. Start writeIndex at nums.Length - 1
     and move it down each time you place a non-zero. The order of the
     non-zeroes is still kept.
  4. What if the order of the non-zeroes does not matter?
     Use two pointers from both ends. Swap a zero on the left with a non-zero on
     the right. This does fewer swaps, but it gives up stability.
TRIGGER
  Reach for this pattern when a problem says to rearrange an array in place so
  that the elements that pass a test come first, in their original order.
C# NOTE
  The tuple line (nums[writeIndex], nums[readIndex]) = (nums[readIndex],
  nums[writeIndex]) swaps two values without a temp variable. The method returns
  void and still works, because an int[] is a reference type: the caller sees
  the changes made to nums.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
