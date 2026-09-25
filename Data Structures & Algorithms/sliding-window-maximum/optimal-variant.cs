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
 PATTERN : Monotonic Deque - decreasing indices, front is window max
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Optimal variant - ties the best complexity by another route
================================================================================
VARIABLES
  n        nums.Length
  output   output[l] = maximum of the window that starts at index l
  q        indices into nums, kept so nums[q] is non-increasing front to back
  l        left edge of the current window, and also the next slot to write in output
  r        right edge of the current window, the index being added this step
WHY THIS PATTERN
  The problem asks for the maximum of every window of fixed width k, and
  consecutive windows overlap in k-1 elements, so rescanning each window throws
  away work. A value can only ever be the answer if no later value inside the
  window is larger than it, so q drops any index whose nums value is beaten by
  the new nums[r] - those indices can never win again. What is left in q is a
  non-increasing chain of candidates, and its front is the current answer.
BRUTE FORCE
  The first thing most people write is two loops: for each start l from 0 to
  n-k, scan nums[l..l+k-1] and keep the largest. It is correct and needs no
  extra structure, but it costs O(n*k) time because every element is re-read up
  to k times. A max-heap of (value, index) with lazy popping of out-of-window
  tops is the middle ground at O(n log n); the deque wins because each index is
  pushed once and popped once.
INVARIANT
  At the end of every iteration: all indices in q lie in [l, r], they increase
  from front to back, and nums of those indices never increases from front to
  back. So q.First.Value is the index of the maximum over [l, r], and once r+1
  >= k that range is exactly a full window of width k, which is what gets stored
  in output[l]. Indices removed from the back were strictly smaller than
  nums[r], which stays in the window at least as long as they would, so removing
  them cannot lose an answer.
WHY THE SWAPPED ORDER IS STILL CORRECT
  The usual write-up evicts the stale front first and then does the domination
  pops; this file does domination, then AddLast(r), then the front check. It
  still holds because the two operations touch opposite ends: domination only
  ever removes indices larger than the front, and if the single stale index is
  also the only element, domination removes it and r takes its place - and l > r
  is never true, so the front check simply does nothing. Doing AddLast before
  the front check also guarantees q is non-empty, so q.First is never null.
WATCH OUT
  new int[n - k + 1] runs before anything is validated, so k > n gives a
  negative length and throws, and a null nums throws on nums.Length. With k = 0
  and empty nums the array has length 1 and the loop body never runs, so it
  returns a silent 0 instead of failing. The `if (l > q.First.Value)` is only
  safe in this position: if you ever move it above q.AddLast(r), q can be empty
  and q.First is null, giving a NullReferenceException on the very first
  iteration. The domination test uses strict `<`, so equal values are all kept -
  that is deliberate, since the later duplicate outlives the earlier one and
  must stay as a candidate.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How do you get the minimum of every window instead?
     Flip one comparison to nums[q.Last.Value] > nums[r]; q then becomes
     non-decreasing and the front is the minimum. Nothing else changes.
  2. The stream is endless and you cannot allocate output up front - what
  changes?
     Turn the method into IEnumerable<int> and yield nums[q.First.Value] where
     output[l] is assigned; drop output and use a counter in place of l for the
     eviction test, which needs the running window start anyway. Memory stays at
     the deque only.
  3. Can you drop the LinkedList and still keep the same bounds?
     Yes - an int[] of capacity k (or n) with head and tail indices gives the
     same push-back, pop-back, pop-front operations by index arithmetic, so no
     per-element node objects are created.
  4. What if k changes between queries over the same fixed array?
     A deque pass is tied to one k, so re-running it per k costs O(n) each time;
     if there are many queries, precompute a sparse table (O(n log n) build,
     O(1) per range max) instead.
TRIGGER
  A fixed-width window slides over a sequence and you need an extreme value (max
  or min) for every position.
C# NOTE
  LinkedList<int> is chosen because it is the only built-in list that removes
  from both ends cheaply - Queue<int> cannot pop from the back - but note that
  q.Last.Value is the node's payload, which here is an index, so the real
  comparison is nums[q.Last.Value] and never q.Last.Value itself.
COMPLEXITY
  Time  : O(n)
  Space : O(k)
================================================================================
*/
