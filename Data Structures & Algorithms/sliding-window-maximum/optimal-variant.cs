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
 PATTERN : Monotonic deque of indices, values decreasing
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Optimal variant - ties the best complexity by another route
================================================================================
WHY THIS PATTERN
  A window max cannot be maintained by a running variable, because the element
  that leaves on the left may be the max, and there is no way to recover the new
  max without rescanning. The deque q fixes that by keeping, for the current
  window, exactly the indices that could still become the answer for some
  window: those whose value is not beaten by any later index inside the window.
  That set is decreasing in value by construction, so nums[q.First.Value] is the
  answer and no rescan is ever needed.
INVARIANT
  At the top of each iteration, l == max(0, r - k + 1). It is simultaneously (a)
  the left edge of the window that iteration r will complete and (b) the slot in
  output that gets written, which is why output[l] takes no offset arithmetic. q
  holds strictly increasing indices whose nums values are non-increasing from
  front to back, all drawn from [l - 1, r - 1]; the l - 1 is the one index that
  may have gone stale since the previous write and has not been evicted yet.
WHY REMOVING FROM THE BACK IS SAFE
  When nums[q.Last.Value] < nums[r] the popped index j satisfies j < r. Every
  window still to be written ends at some r' >= r, so any such window that
  contains j also contains r. Since nums[j] < nums[r], j is never the maximum of
  any remaining window and dropping it loses nothing. This is the whole
  correctness argument for the pop loop, and it is why the answer for a window
  is always at the front rather than found by searching.
WHY DOMINATION BEFORE EVICTION STILL WORKS
  The two maintenance steps commute here for a reason worth stating out loud.
  Doing the back-pops first can remove the stale index l - 1 (if it is the last
  node and its value is beaten by nums[r]) - harmless, it was going to be
  discarded anyway. If the back-pops do not reach it, the l > q.First.Value
  check that follows removes it. Either path, by the time output[l] is read the
  front index is >= l. What would NOT be correct is deferring the front eviction
  until after the write.
WHY A PLAIN IF, NOT A WHILE
  The front is the smallest index in q, and l grows by exactly one per completed
  window. So between two consecutive checks at most one index - namely the old l
  - can have fallen out of range. Any index below that was already evicted on an
  earlier pass. A single RemoveFirst therefore restores the invariant, and a
  while loop would be dead weight. This is the one place this file diverges
  structurally from the textbook shape, and it is the thing an interviewer will
  poke at.
WHY Q.FIRST IS NEVER NULL
  Both q.First dereferences run after q.AddLast(r), so the deque is non-empty on
  entry to each of them. The eviction can only fire when q.First.Value < l,
  which requires a front index other than r (since l <= r always), which
  requires Count >= 2. So RemoveFirst can never empty the deque before the
  output[l] read. The code depends on this ordering; hoisting AddLast below the
  eviction check would break it.
WATCH OUT
  1. The comparison is strict <, so equal values are both kept. That is fine -
  the earlier duplicate expires first and the later one remains as the max - but
  it makes q longer than necessary on runs of equal elements. Switching to <=
  would also be correct.
  2. new int[n - k + 1] silently assumes 1 <= k <= n. With k > n it throws on
  the allocation, not at a guarded check.
  3. LinkedList<int> allocates a node object per AddLast. An int[] of size n
  used as a ring buffer with head/tail integers holds the same indices with no
  per-push allocation and the same O(1) operations.
TRIGGER
  Fixed-width window plus an extremum (max or min) per position, with the window
  sliding one step at a time. Reach for the monotonic deque. If the window width
  varies, or you need the k-th largest rather than the largest, the deque
  argument collapses and you fall back to a heap with lazy deletion or a
  multiset, at O(n log n) time.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
