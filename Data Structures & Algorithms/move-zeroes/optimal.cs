// ##########################################################################
// #  YOU SOLVED THIS YOURSELF  (submission-2, marked '//my solution.')
// #  merged with submission-3 - same partition, fewer edge cases
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
 PATTERN : Two Pointers - Fast/Slow (in-place partition, order preserving)
 SOURCE  : YOUR OWN SOLUTION (submission-2, marked '//my solution.'), merged
           with submission-3 - your pre-scanning loops that hunted for the
           first zero and the first non-zero are unnecessary; a single
           read/write pair does the same job with no edge cases
 STATUS  : Optimal
================================================================================

WHY THIS PATTERN
  Two pointers do not always converge from opposite ends. Here both move in
  the SAME direction at DIFFERENT SPEEDS: a fast reader scans every element,
  a slow writer marks where the next kept element belongs. That is the
  read/write (fast/slow) form, and it is the tool for every in-place
  "remove / compact / partition, keep relative order" problem.

BRUTE FORCE (and why it fails)
  Build a new array of non-zeroes and pad with zeroes: O(n) time but O(n)
  extra space, and the problem demands in place.
  Or repeatedly shift on each zero found: O(n^2) in the worst case
  ([0,0,0,...,1]).

INVARIANT
  nums[0 .. writeIndex-1]        = all non-zeroes seen so far, original order
  nums[writeIndex .. readIndex]  = the zeroes seen so far
  Both hold after every iteration, which is why one pass is enough.

WHY SWAP AND NOT ASSIGN
  Assigning (nums[writeIndex] = nums[readIndex]) also compacts correctly, but
  leaves stale values in the tail and needs a SECOND loop to zero it out.
  Swapping moves the displaced zero to readIndex, which the reader has
  already passed - so the tail is zero-filled for free. Same complexity,
  half the code, no second pass to get wrong.

  When writeIndex == readIndex the swap is a no-op on itself. Harmless, and
  removing the redundant self-swap with an extra `if` buys nothing readable.

ALGORITHM (NeetCode: "Two Pointers")
  1. writeIndex = 0.
  2. Scan with readIndex over the whole array.
  3. On a non-zero: swap it into writeIndex and advance writeIndex.
  4. Zeroes are simply skipped by the reader.

COMPLEXITY
  Time  : O(n) - one pass, each element inspected once.
  Space : O(1) - two indices, mutation in place.

TRIGGER
  "In place", "keep relative order", "remove all X", "move all X to the end",
  "compact / partition an array". Same skeleton as remove-duplicates and
  remove-element - change only the predicate on the fast pointer.

C# NOTES
  - (a, b) = (b, a) is C# 7 tuple deconstruction: a genuine swap with no
    temp variable and no allocation - the compiler emits plain loads/stores.
  - The method returns void and MUTATES the caller's array. int[] is a
    reference type, so the caller sees the change. Do not reassign the
    parameter (nums = new int[...]) - that rebinds the local only.
  - Span<T> has no built-in Swap; the tuple form is the idiomatic answer.
  - Worth knowing: (a[i], a[j]) = (a[j], a[i]) is guaranteed correct by the
    C# spec - the right-hand tuple is fully evaluated BEFORE any assignment
    happens. Roslyn (.NET 8, and the NeetCode judge) gets this right. The
    legacy Mono mcs compiler miscompiles it into a plain copy, so if you ever
    see this "swap" silently not swapping, suspect the toolchain, not the
    logic. The explicit temp-variable swap is immune either way.

WATCH OUT
  - The original version pre-scanned for the first zero and the first
    non-zero before the main loop. It works, but each pre-scan needs its own
    bounds guard (`if (index >= nums.Length) return;`) and those guards are
    exactly where the off-by-one lives. Fewer moving parts is not a style
    preference here - it is the correctness argument.
================================================================================
*/
