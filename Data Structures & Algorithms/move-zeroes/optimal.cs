// ##########################################################################
// #  optimal.cs            O(n) time / O(1) space
// #  two pointers, fast/slow swap partition   [two-pointer-swap-partition]
// #  the only solution in this folder
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  single pass swaps each non-zero into the writeIndex slot, pushing
// #  zeroes rightward automatically without a second pass
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
 PATTERN : Two Pointers (read/write) - stable partition by swap
 SOURCE  : YOUR OWN SOLUTION - your own annotation at c76939d
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  The task is a stable partition: non-zeroes keep their relative order, zeroes
  get pushed to the tail, and it must happen inside nums with no auxiliary
  array. That is exactly the read/write two-pointer shape - readIndex scans
  every slot once and decides only "does this element belong to the kept
  prefix", while writeIndex marks where the next kept element goes. Any pattern
  that reorders (sorting by a key, or swapping from the far end like the classic
  Dutch-flag partition) breaks the required order of the non-zeroes.
INVARIANT
  After each iteration of the loop, two things hold:
  1. nums[0 .. writeIndex-1] holds every non-zero seen so far, in the order it
  was seen.
  2. nums[writeIndex .. readIndex-1] is all zeroes.
  writeIndex is incremented only inside the non-zero branch, so writeIndex <=
  readIndex always, and readIndex - writeIndex is precisely the number of zeroes
  seen so far. When the loop ends readIndex == nums.Length, so clause 2 covers
  the whole tail and the array is finished with no second pass.
WHY THE SWAP IS SAFE
  The worry with an in-place swap is clobbering a value you have not read yet.
  It cannot happen here. By invariant clause 2, the slot at writeIndex is either
  readIndex itself (when no zero has been seen, writeIndex == readIndex, and the
  swap is a self-swap no-op) or it sits strictly inside the zero band, so it
  holds a 0. So the value the swap sends backwards to readIndex is always a zero
  - a value carrying no information, already scanned, and destined for the tail
  anyway. Nothing that still needs to be placed is ever overwritten. That is the
  whole correctness argument, and it is the thing to say out loud in an
  interview.
WALK IT
  nums = [0,1,0,3,12], writeIndex starts at 0.
  r=0: nums[0] is 0, skip. w=0.
  r=1: 1 is non-zero, swap slots 0 and 1 -> [1,0,0,3,12], w=1. The zero that was
  at slot 0 rode out to slot 1.
  r=2: 0, skip. w=1.
  r=3: 3, swap slots 1 and 3 -> [1,3,0,0,12], w=2.
  r=4: 12, swap slots 2 and 4 -> [1,3,12,0,0], w=3.
  Notice the zeroes accumulate in the band [w, r) exactly as the invariant
  claims, and the final array needed no cleanup loop.
TRIGGER
  Reach for this shape whenever the ask is "remove / relocate all elements
  matching a predicate, in place, preserving the order of the survivors":
  remove-element, remove-duplicates-from-sorted-array, and this problem are the
  same skeleton with a different test in the if. The only variable is whether
  you assign or swap - swap when the displaced values themselves have a required
  destination (here, the zero tail); plain assign when the discarded values may
  be left as garbage.
WATCH OUT
  Two things an interviewer will poke at.
  First, the assign variant: nums[writeIndex++] = nums[readIndex] is also
  correct but leaves stale copies past writeIndex, so it needs a second loop
  filling writeIndex..Length-1 with 0. That version does at most n writes total;
  this swap version does 2 writes per non-zero, including a pointless self-swap
  on every leading non-zero while writeIndex == readIndex. If asked to minimize
  writes, guard the swap with if (writeIndex != readIndex), or switch to assign
  plus zero-fill. The trade is one branch versus a second pass - state the
  trade, do not guess which is faster.
  Second, stability only matters for the non-zeroes. Zeroes are
  indistinguishable from one another, so shuffling them among themselves - which
  the swap does - costs nothing.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
