// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(n) space
// -  monotonic deque, index-based sliding window max
// -  [monotonic-deque-sliding-max]
// -  ranks above optimal-variant.cs (O(n) time / O(n) space)
// -
// -  Reference solution - not one you solved yourself
// -
// -  each index enters and leaves the deque at most once so total work is
// -  amortised O(n); deque size is bounded by k which is O(n) worst case
// -  per the allowed enum
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
 PATTERN : Monotonic deque of indices - front is the window max
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  Every window shares k-1 elements with the previous one, so recomputing the max
  from scratch throws away almost everything you just learned. The only thing
  that can invalidate the current max is it sliding out of the window; the only
  thing that can create a new max is the element arriving on the right. A
  structure that answers "max" at the front and lets you evict from both ends is
  exactly the shape of that problem, hence a deque.

  The key decision is storing INDICES, not values. The deque has to answer two
  different questions - "is the max still inside the window?" (needs position)
  and "is the max bigger than nums[i]?" (needs value). An index answers both,
  because nums[deque.First.Value] recovers the value; a value alone cannot
  recover the position.
BRUTE FORCE
  For each of the n-k+1 window starts, scan k elements and take the max.
  Correct, trivially, and it is what you should state first in an interview
  before improving it.

  The intermediate step people reach for is a max-heap of (value, index). Push
  nums[i], then peek and lazily pop any top whose index has fallen out of the
  window. That is a genuine improvement and worth mentioning, but the heap can
  grow to hold all n elements and each push costs log n. The deque beats it
  because domination lets you DELETE dominated elements permanently instead of
  merely ignoring them later.
INVARIANT
  Held at the top of every iteration, and the whole correctness argument rests
  on it:

  1. Indices in the deque are strictly increasing front to back (guaranteed for
  free - i is appended in increasing order and pops only remove, never reorder).
  2. Their values are non-increasing front to back.
  3. The deque contains exactly the indices j in the current window such that no
  later index in the window holds a value greater than nums[j].

  Given (2), the front holds the largest value present. Given (1), the front is
  also the oldest index present, so it is the only one that can have expired -
  which is why step 1 checks First and not a scan. Given (3), the discarded
  elements can be discarded: an index k popped in step 2 satisfies k < i and
  nums[k] < nums[i], so every window that still contains k also contains i and
  has a max of at least nums[i] > nums[k]. Element k can never be the answer
  again.
WALKTHROUGH
  nums = [1,3,-1,-3,5,3,6,7], k = 3. Deque shown as indices, with values in
  parentheses.

  i=0: add -> [0(1)]. No emit, i < k-1.
  i=1: 3 dominates index 0, pop it -> [1(3)]. No emit.
  i=2: nothing expires (front 1 >= 0), -1 does not dominate -> [1(3),2(-1)].
  Emit result[0] = 3.
  i=3: window starts at 1, front is 1, stays -> [1(3),2(-1),3(-3)]. Emit
  result[1] = 3.
  i=4: window starts at 2, front 1 < 2 so RemoveFirst -> [2,3]. Then 5 dominates
  both, pop from the back -> []. Add -> [4(5)]. Emit result[2] = 5.
  i=5: 3 does not dominate 5 -> [4(5),5(3)]. Emit result[3] = 5.
  i=6: 6 dominates both -> [6(6)]. Emit result[4] = 6.
  i=7: 7 dominates -> [7(7)]. Emit result[5] = 7.

  Result [3,3,5,5,6,7], length n-k+1 = 6. Note i=4 is the one iteration that
  does both an expiry and multiple dominations - that is the case to trace by
  hand if the code ever misbehaves.
WHY THE NESTED LOOPS ARE NOT QUADRATIC
  The standard follow-up. Two while loops inside a for loop looks like O(nk),
  but count the work by element rather than by iteration: each index i is added
  by AddLast exactly once, and once removed - by RemoveFirst in step 1 or
  RemoveLast in step 2 - it is never re-added. So across the entire run, the two
  while loops execute at most n removals total. The loop body is O(1) amortized,
  not O(k).

  Make the argument about the deque, not the loop nesting; that is the whole
  point of the amortization.
WATCH OUT
  Ties are kept, despite the comment. Step 2 pops on nums[last] < nums[i],
  strictly less, so an equal value survives and the deque is non-increasing
  rather than strictly decreasing. That is still correct: with nums = [2,2] and
  k = 2 the deque is [0,1], the front emits 2, and when index 0 expires index 1
  is still there holding the same value. Popping on <= would also be correct
  (the newer index dominates an equal older one and outlives it) and would keep
  the deque smaller. The invariant to state out loud is non-increasing; do not
  claim strict.

  Order within the iteration: expiry must precede the emit in step 3, because a
  stale front would be read as the answer. Domination and expiry can be swapped
  relative to each other without changing the result - they remove from opposite
  ends - but never move either below step 3.

  Off-by-ones cluster around i-k+1, which is both the window start index in step
  1 and the output slot in step 3. Same expression, two meanings; if one is
  wrong they are probably both wrong.

  The guard rejects k <= 0 but not k > nums.Length. In that case new int[n - k +
  1] gets a negative length and throws OverflowException before the loop ever
  runs. The problem constraints promise k <= n, so this is defensible, but say
  so rather than letting the interviewer find it.
TRIGGER
  Reach for a monotonic deque when a fixed-size or two-pointer window needs its
  min or max at every step, and the elements leaving the window leave from the
  opposite end from the ones arriving. The tell is the domination argument: if a
  newly arrived element is both better and younger than an older candidate, the
  older one is dead forever - and that is what lets you delete instead of
  buffer.

  The same skeleton solves shortest subarray with sum at least k (deque of
  prefix sums, both ends popped) and constrained-subsequence-style DP where the
  transition is a max over a sliding range. Flip the comparison in step 2 to
  nums[last] > nums[i] and the front becomes the window minimum.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
