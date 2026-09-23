// ##########################################################################
// #  optimal.cs            O(n) time / O(1) space
// #  Two-pointer swap in-place   [two-pointer-swap]
// #  the only solution in this folder
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  Single pass with swap operations moves non-zeroes forward; zeroes
// #  naturally settle at the end.
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
 PATTERN : Two Pointers - read/write index with swap
 SOURCE  : YOUR OWN SOLUTION - your own annotation at c76939d
 STATUS  : Optimal
================================================================================
VARIABLES
  writeIndex   next slot for a non-zero; nums[0..writeIndex-1] are the non-zeroes seen so far
  readIndex    the element currently being inspected
WHY THIS PATTERN
  The task is to push every zero to the end while keeping the non-zero values in
  their original relative order, and to do it in place. That is a partition of
  the array into two groups where one group must stay stable, which a read
  pointer plus a write pointer solves in one sweep: readIndex scans, writeIndex
  marks the boundary of the kept prefix. Because writeIndex only advances when a
  non-zero is found, it can never pass readIndex, so no unread element is ever
  overwritten.
BRUTE FORCE
  The first thing most people write is a second array: copy all non-zeroes into
  it in order, then pad with zeroes, then copy back. That is O(n) time but O(n)
  extra space, and the problem asks for in-place. The other naive try is
  repeatedly finding a zero and shifting the rest of the array left by one,
  which is O(n^2) when the array starts with many zeroes.
INVARIANT
  At the top of each iteration, nums[0..writeIndex-1] holds exactly the non-zero
  elements of nums[0..readIndex-1] in their original order, and
  nums[writeIndex..readIndex-1] holds only zeroes. The swap keeps both halves of
  this true: it moves the new non-zero into the boundary slot and sends whatever
  sat there (necessarily a zero, or the same cell) out to readIndex. When the
  loop ends readIndex equals nums.Length, so the statement covers the whole
  array, which is the required answer.
WHY SWAP AND NOT ASSIGN
  Plain assignment nums[writeIndex] = nums[readIndex] would also build the
  correct prefix, but it duplicates values and leaves garbage in
  nums[writeIndex..n-1], so you then need a second loop to write zeroes over the
  tail. The swap keeps the multiset of values unchanged at every step, so the
  tail is already all zeroes when the loop ends. The cost is that when
  writeIndex == readIndex the code swaps a cell with itself, which is harmless
  but is a real write.
WATCH OUT
  The method returns void and mutates the caller's array, so any test that
  expects a fresh array back will look like it did nothing. An empty array works
  because the loop body never runs, but nums == null throws
  NullReferenceException at nums.Length - there is no guard. The comment says
  the prefix before writeIndex is "a non-zero", singular; it means all cells
  there are non-zero, not one cell.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Move all zeroes to the front instead, keeping the non-zeroes in order?
     Scan from the right with both pointers starting at nums.Length - 1 and
     decrement writeIndex on each non-zero; the mirror image of this loop
     preserves order the same way.
  2. Remove all instances of a given value val and return the new length, order
  of the rest not important?
     Same writeIndex, but assign instead of swap and return writeIndex; you no
     longer care what lands in the tail, so you save the extra write per
     non-zero.
  3. Minimise the number of writes, not just the number of passes?
     Guard the swap with if (writeIndex != readIndex); an array with no zeroes
     then does zero writes instead of n self-swaps, at the cost of one
     comparison per non-zero element.
  4. The array is huge and lives on disk or in a stream you can only read
  forward once?
     Count zeroes while streaming the non-zeroes straight to the output, then
     emit that many zeroes at the end; still O(1) extra memory but it needs a
     separate output sink instead of in-place mutation.
TRIGGER
  An in-place array rearrangement that splits elements into "keep" and "push
  aside" while the kept ones must stay in their original order.
C# NOTE
  The tuple form (nums[writeIndex], nums[readIndex]) = (nums[readIndex],
  nums[writeIndex]) is the idiomatic C# 7+ swap and removes the need for a temp
  variable; note that int[] is passed by reference so the caller sees the
  mutation even though the parameter itself is not ref.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
