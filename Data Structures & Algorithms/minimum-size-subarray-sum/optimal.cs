// ##########################################################################
// #  optimal.cs            O(n) time / O(1) space
// #  sliding window, shrink while valid, running sum
// #  [sliding-window-running-sum]
// #  ranks above suboptimal.cs (O(n) time / O(n) space)
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  maintains the window sum incrementally via add/subtract as pointers
// #  move, so left advances at most n times total with only scalar state
// ##########################################################################

public class Solution
{
    public int MinSubArrayLen(int target, int[] nums)
    {
        int sum = 0;
        int left = 0;
        int result = int.MaxValue;

        for (int right = 0; right < nums.Length; right++)
        {
            sum = sum + nums[right];   // admit the new right-hand element

            // Shrink while the window still qualifies - the first failure
            // means every shorter window ending here fails too.
            while (sum >= target)
            {
                result = Math.Min(result, right - left + 1);
                sum = sum - nums[left];  // evict, THEN move the edge
                left++;
            }
        }

        return result == int.MaxValue ? 0 : result;
    }
}

/*
================================================================================
 PATTERN : Variable-size sliding window - shrink while valid
 SOURCE  : YOUR OWN SOLUTION - your own annotation at c76939d
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  You want the shortest contiguous run whose sum reaches target. Because every
  entry of nums is a positive integer, the window sum is monotone in the window:
  extending right can only raise sum, shrinking from left can only lower it.
  That monotonicity is exactly what lets one pointer chase the other in a single
  pass instead of re-examining pairs. The moment a problem gives you
  "contiguous" plus "positive values" plus "minimize/maximize length under a
  threshold", this two-pointer window is the intended shape.
BRUTE FORCE
  Fix each start index, walk right accumulating until the running sum reaches
  target, record the length, restart from the next start. That is n^2 work, and
  its wasted effort is concrete: after finishing the window starting at left, it
  throws away the sum and recomputes almost the same numbers for left+1. The
  window keeps that sum alive across starts - sum - nums[left] is the answer for
  the next start, already computed.
INVARIANT
  At the top of every iteration of the outer for loop, sum equals the total of
  nums[left..right-1] and that segment is strictly below target (the inner while
  ran until it failed, or never qualified). result holds the smallest qualifying
  length seen among all windows ending at index right-1 or earlier. sum = sum +
  nums[right] restores sum to nums[left..right], and the while loop
  re-establishes the "below target" half of the invariant before the next
  iteration.
WHY SHRINKING IS SAFE
  The concern is that advancing left discards a window you might still need. It
  cannot: any window you drop, say nums[left..r] for some r > right, is strictly
  longer than nums[left..right], which you already measured and folded into
  result. So the discarded candidates are all worse than one you have already
  counted. Conversely you never miss a candidate you have not seen, because for
  each right the loop measures the shortest qualifying window ending at right -
  it stops the instant sum falls below target, and the comment in the code names
  the reason: once nums[left..right] is too small, every shorter window ending
  at right is too small too. Every right gets its best answer, and the true
  optimum ends at some right, so result is that optimum.
WHY WHILE, NOT IF
  One admitted element can enable several shrinks, so an if here is a real bug,
  not a stylistic choice. Take target = 4, nums = [1, 4, 4]. At right = 1, sum =
  5 >= 4: record length 2, evict nums[0] = 1, sum = 4, which still qualifies -
  record length 1. An if would have stopped after recording 2 and never found
  the single-element answer.
WATCH OUT
  Order inside the while body: result is updated before the eviction, because
  [left, right] is the window that currently qualifies - swap those two lines
  and you measure a window you have already broken. The eviction and the
  increment must also stay paired in that order, sum - nums[left] then left++;
  incrementing first subtracts the wrong element.

  result = int.MaxValue is a sentinel, not a length. The final ternary turns
  "the while loop never fired once" into 0, the required answer when no subarray
  reaches target. If you ever add arithmetic on result inside the loop, remember
  it may still be int.MaxValue.

  sum is an int accumulating a prefix of nums; it stays bounded by the largest
  qualifying window plus one element, but if the inputs are large enough that a
  single window can exceed int range, widen sum to long.
TRIGGER
  Reach for this when the array is all non-negative, the target is a lower bound
  on a sum, and you want the extreme length. If negatives are allowed, the
  monotonicity dies - adding an element can shrink sum, so a failed window may
  still succeed after extending, and shrinking from the left can raise sum. That
  variant (shortest subarray with sum at least k) needs prefix sums plus a
  monotonic deque instead; do not try to patch this loop into handling it.
FOLLOW-UP
  "Your loop is nested - isn't that quadratic?" No: left is initialized once
  outside the for loop and only ever increases, so across the whole run the
  inner while body executes at most nums.Length times total. Each index is
  admitted by right exactly once and evicted by left at most once. Amortized,
  not per-iteration - that is the argument to say out loud.

  A second likely follow-up: return the window itself, not just its length.
  Store the pair (left, right) alongside each improvement to result rather than
  trying to reconstruct it afterward - the pointers have moved past it by the
  time the loop ends.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
