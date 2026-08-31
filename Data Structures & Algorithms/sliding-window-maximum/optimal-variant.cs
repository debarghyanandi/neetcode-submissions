// --------------------------------------------------------------------------
// -  optimal-variant.cs    O(n) time / O(n) space
// -  monotonic deque, index-based sliding window max with explicit l/r
// -  pointers
// -  [monotonic-deque-sliding-max]
// -  ties with optimal.cs on O(n) time / O(n) space
// -
// -  Reference solution - not one you solved yourself
// -
// -  identical mechanism to optimal.cs (dominate-then-expire order swapped,
// -  harmless since the two ops touch opposite deque ends), same amortised
// -  O(n) time and O(k) deque size
// --------------------------------------------------------------------------

public class Solution
{
    public int[] MaxSlidingWindow(int[] nums, int k)
    {
        int n = nums.Length;
        int[] output = new int[n - k + 1];
        var q = new LinkedList<int>();

        // Both edges are tracked explicitly. l doubles as the index of the
        // window whose answer is being written, which is why output[l] needs
        // no arithmetic.
        int l = 0, r = 0;

        while (r < n)
        {
            // Domination first here - the order is swapped relative to
            // optimal.cs, and it is still correct. See the note below.
            while (q.Count > 0 && nums[q.Last.Value] < nums[r])
            {
                q.RemoveLast();
            }
            q.AddLast(r);

            // A plain `if`, not a `while`: at most ONE index can go stale
            // per step, because l advances by at most one per iteration.
            if (l > q.First.Value)
            {
                q.RemoveFirst();
            }

            if ((r + 1) >= k)
            {
                output[l] = nums[q.First.Value];
                l++;
            }
            r++;
        }

        return output;
    }
}

/*
================================================================================
 PATTERN : Sliding Window - Fixed Size, MONOTONIC DEQUE with explicit l / r
 SOURCE  : NeetCode / other resource (submission-1) - a restatement of
           optimal.cs, not a different algorithm
 STATUS  : Optimal - O(n) time, O(k) space (identical complexity)
================================================================================

WHY KEEP BOTH
  Same algorithm, two different framings, and the differences are exactly
  the places where this problem is usually got wrong. Reading them side by
  side is worth more than either alone.

    optimal.cs                      this file
    ------------------------------  ------------------------------
    one index i                     explicit l and r
    left edge derived: i - k + 1    left edge tracked: l
    expire  ->  dominate  ->  push  dominate  ->  push  ->  expire
    `while` on the expiry           `if` on the expiry
    result[i - k + 1]               output[l], with l++ at write time

WHY THE SWAPPED ORDER IS STILL CORRECT
  optimal.cs expires the front before letting nums[r] evict from the back.
  This version pushes first and expires afterwards. Both are sound, and the
  reason is that the two operations touch OPPOSITE ENDS of the deque and
  cannot interfere - domination only ever pops the back, expiry only ever
  pops the front. The one case where they meet is a deque of a single
  element, and there the outcomes coincide: that element is either dominated
  by nums[r] (popped from the back, then r is pushed and is in-window) or it
  survives to be tested for expiry.

WHY `if` IS ENOUGH AND `while` IS NOT NEEDED
  l advances by at most one per iteration of the loop, so at most one index
  can newly fall outside [l, r] per step. The front is the only candidate.
  optimal.cs uses `while` because it is the safer default when the left edge
  is derived rather than stepped - and on this problem the `while` never
  executes more than once either. Knowing WHY the weaker form suffices is
  the point; defaulting to `while` in an interview costs nothing.

INVARIANT
  q holds in-window indices whose values strictly decrease front to back,
  and l is the start index of the next window whose answer is to be written.

ALGORITHM
  1. l = r = 0, empty deque, output sized n - k + 1.
  2. While r < n:
       a. Pop the back while nums[back] < nums[r]; push r.
       b. If l > front, pop the front (expired).
       c. If r + 1 >= k, write output[l] = nums[front], then l++.
       d. r++.
  3. Return output.

COMPLEXITY
  Time  : O(n) amortised - every index enters and leaves the deque once.
  Space : O(k).

TRIGGER
  Identical to optimal.cs. The l / r framing is worth adopting generally: it
  makes the window's two edges first-class, which reads better in problems
  where the left edge moves for its own reasons rather than trailing the
  right one by a fixed k.

C# NOTES
  - q.First.Value without the null-forgiving `!` compiles here because the
    file has no nullable-reference context enabled; under `#nullable enable`
    it warns exactly as optimal.cs does.
  - No null / empty guard on nums: `new int[n - k + 1]` throws
    OverflowException-shaped negative-size errors when k > n. optimal.cs
    guards; this one relies on the constraints. Worth adding.

WATCH OUT
  - l++ lives INSIDE the write branch. Moving it out advances the left edge
    during the fill-up phase and corrupts every subsequent window.
  - `(r + 1) >= k` is the same test as `r >= k - 1`; the window is full when
    it holds k elements, counting from zero.
  - Because l only advances after a write, the expiry test `l > q.First.Value`
    is checked against the NEXT window's left edge - which is precisely why
    it belongs after the push rather than before it.
================================================================================
*/
