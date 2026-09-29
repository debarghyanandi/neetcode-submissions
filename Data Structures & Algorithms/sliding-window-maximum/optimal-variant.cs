// --------------------------------------------------------------------------
// -  optimal-variant.cs    O(n) time / O(k) space
// -  Monotonic deque, sliding window   [monotonic-deque]
// -  ties with optimal.cs on O(n) time / O(k) space
// -
// -  Reference solution - not one you solved yourself
// -
// -  Each element is added and removed from the deque at most once; expiry
// -  uses if instead of while since l advances by at most 1 per iteration.
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
 PATTERN : Monotonic Deque - keep indices with non-increasing values
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Optimal variant - ties the best complexity by another route
================================================================================
VARIABLES
  output   output[l] = max of nums[l..l+k-1]
  q        indices of the window; their nums values never go up from front to back
  l        left edge of the current window, and also the output slot to fill next
  r        right edge; the index being added in this step
WHY THIS PATTERN
  The problem asks for the max of every window of size k while the window slides
  by one. Each step adds one element on the right and drops one on the left. A
  monotonic deque is a queue that stays sorted and can be changed at both ends.
  The deque q keeps only the indices that could still become a window max, so
  the answer is always nums[q.First.Value]. Each index goes into q once and
  leaves once.
BRUTE FORCE
  For every start l from 0 to n-k, scan nums[l..l+k-1] and keep the largest
  value. This takes O(n*k) time and O(1) extra space. It loses because windows
  next to each other share k-1 elements, and it scans all of them again every
  time.
INVARIANT
  Before the answer is written in each step, q holds indices in increasing
  order, all inside [l, r], and their values never go up from front to back. An
  index is dropped from the back only when a newer, larger nums[r] arrives. The
  dropped index can never be the max again, because r is larger and stays in the
  window longer. So the front index is the largest value still in the window,
  and output[l] = nums[q.First.Value] is correct.
WHY POPPING THE BACK FIRST IS SAFE
  The code pops from the back before it checks the front for a stale index (an
  index that has left the window). If the back loop removes the stale front too,
  then q.First is r, and l > r is false, so nothing breaks. If the back loop
  does not reach the stale front, the if removes it. The new index r is never
  stale, so the order of the two steps does not change the result.
WATCH OUT
  The comment says "See the note below", but there is no note below it in this
  file. The explanation lives in optimal.cs or nowhere, so you will not find it
  here. If k > n, new int[n - k + 1] gets a negative size and throws an
  exception. If k = 0, the output size and the window meaning are both wrong,
  and nothing in the code checks the input. The plain if (not while) is correct
  only because l at the check always equals r-k+1 (or 0 before the first full
  window). So at most one index, l-1, can be stale. If you change when l is
  incremented, that guarantee breaks.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How do you get the sliding window minimum instead?
     Flip the comparison to nums[q.Last.Value] > nums[r], so the values in q
     never go down. Nothing else changes.
  2. Can you do it in O(n) without a deque?
     Split nums into blocks of size k. Build prefix-max and suffix-max arrays
     for the blocks. Then each window max is max(suffixMax[l],
     prefixMax[l+k-1]). This uses O(n) extra memory instead of O(k), but it is
     simple array work and easy to run in parallel.
  3. What if the numbers arrive as a stream that is too big to store?
     The deque only needs the last k indices. Keep a running counter for r,
     store values next to indices in q, and emit each max as soon as it is ready
     instead of filling output.
  4. Where else does this deque idea show up?
     Shortest subarray with sum at least K (a deque over prefix sums), and DP
     where dp[i] needs the max of the last k dp values. In both, the deque turns
     an O(k) scan into O(1) amortized, meaning O(1) on average per step.
TRIGGER
  When you need the max or min over a fixed-size window that moves one step at a
  time, and a nested scan is too slow, use a monotonic deque of indices.
C# NOTE
  LinkedList<int> allocates a new node object on every AddLast. An int[] of size
  n with head and tail pointers does the same deque job with no allocation per
  element. Since each index is pushed only once, the tail never goes past n.
COMPLEXITY
  Time  : O(n)
  Space : O(k)
================================================================================
*/
