// --------------------------------------------------------------------------
//  Reference solution - from NeetCode / other resource (submission-0,
//  marked '// need to practice more')
//  Not one you solved yourself.
// --------------------------------------------------------------------------

public class Solution
{
    public int[] MaxSlidingWindow(int[] nums, int k)
    {
        if (nums == null || nums.Length == 0 || k <= 0)
            return Array.Empty<int>();

        int n = nums.Length;
        int[] result = new int[n - k + 1];

        // Holds INDICES, not values, and their values are strictly
        // decreasing from front to back. The front is always the maximum of
        // the current window.
        var deque = new LinkedList<int>();

        for (int i = 0; i < n; i++)
        {
            // 1. Expiry: the front may have fallen out of the window.
            while (deque.Count > 0 && deque.First!.Value < i - k + 1)
            {
                deque.RemoveFirst();
            }

            // 2. Domination: anything smaller than nums[i] and older than i
            //    can never be a maximum again - i outlives it and beats it.
            while (deque.Count > 0 && nums[deque.Last!.Value] < nums[i])
            {
                deque.RemoveLast();
            }

            deque.AddLast(i);

            // 3. Emit, once the first full window exists.
            if (i >= k - 1)
            {
                result[i - k + 1] = nums[deque.First!.Value];
            }
        }

        return result;
    }
}

/*
================================================================================
 PATTERN : Sliding Window - Fixed Size, with a MONOTONIC DEQUE
 SOURCE  : NeetCode / other resource (submission-0, marked
           '// need to practice more')
 STATUS  : Optimal - O(n) time, O(k) space
================================================================================

WHY THIS PATTERN
  The window's maximum is not maintainable by a single variable, and the
  reason is worth stating precisely: a running max survives ADMISSION
  (max(old, new)) but not EVICTION - when the maximum itself slides out of
  the window, there is no way to recover the second-largest without looking
  at the window again. Every "maintain an aggregate incrementally" problem
  hits this wall the moment the aggregate is not invertible. Sums are
  invertible (subtract); maxima are not.

  So keep more than one candidate - but not all of them. The key observation:

      if i < j and nums[i] <= nums[j], then index i is DEAD FOREVER.

  Any future window containing i also contains j (j is newer, so it expires
  later), and j is at least as large. i can never be an answer again.

  Discard every dead index and what remains is a strictly decreasing sequence
  of live candidates - the monotonic deque. Its front is the current maximum
  by construction, and the back is where new candidates arrive.

BRUTE FORCE (and why it fails)
  Scan each window for its max: O(n * k), which is 10^10 at the stated
  limits. A max-heap of (value, index) with lazy deletion is O(n log k) and
  is the natural second answer - correct, and the right thing to mention
  before producing this one, because the deque's advantage is that expiry is
  O(1) at the front rather than a lazy pop loop.

INVARIANT
  Front to back, the deque holds indices in increasing order whose values are
  strictly decreasing, and every index in it lies inside the current window.
  Therefore nums[deque.First] is the maximum of the window.

WHY THIS IS O(n) DESPITE TWO NESTED WHILE LOOPS
  Each index is added exactly once and removed at most once, across the
  entire run. The inner loops are therefore amortised O(1) - total work is
  bounded by 2n, not by n * k. Saying this out loud is the difference
  between "I memorised this" and "I understand it".

ALGORITHM (NeetCode: "Deque")
  1. Empty deque of indices; result sized n - k + 1.
  2. For each i:
       a. Pop the FRONT while it is older than i - k + 1 (expired).
       b. Pop the BACK while its value is < nums[i] (dominated).
       c. Push i at the back.
       d. If i >= k - 1, write nums[front] to result[i - k + 1].
  3. Return result.

COMPLEXITY
  Time  : O(n) - amortised, per the argument above.
  Space : O(k) - the deque never holds more than k indices.

TRIGGER
  "Max / min of every window of size k", or any window aggregate that is not
  invertible. The wider trigger is the domination test itself: whenever a
  newer element makes an older one permanently irrelevant, a monotonic
  stack or deque is the structure.

C# NOTES
  - There is no System.Collections.Generic.Deque in .NET. LinkedList<int> is
    the usual stand-in and is O(1) at both ends, but every node is a separate
    heap allocation with two pointers - poor cache behaviour on hot paths.
    An int[] ring buffer with head/tail indices is the version to write if
    performance matters, and it is a good answer to "how would you make this
    faster in C# specifically?".
  - The `!` in deque.First!.Value is the null-forgiving operator: the
    Count > 0 test already guarantees the node exists, and this silences the
    nullable-reference warning. It suppresses the compiler, not a runtime
    check.
  - Array.Empty<int>() returns a cached singleton - no allocation for the
    empty case.

WATCH OUT
  - The deque holds INDICES. Storing values loses the ability to test
    expiry, which is the whole reason indices are used.
  - `<` rather than `<=` on the domination test keeps equal values in the
    deque. Both are correct for the maximum, but keeping duplicates is safer:
    with `<=`, an equal element evicts one that may still be needed if the
    two are in different windows - fine for max, and a real bug in variants
    that also report the index.
  - Expire first, then dominate. Reversed, an expired index can evict a live
    candidate before being discarded itself.
  - result is sized n - k + 1; writing at index i - k + 1 only starts once
    i >= k - 1.
================================================================================
*/
