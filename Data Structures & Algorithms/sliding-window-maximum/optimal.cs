// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(k) space
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
 PATTERN : Monotonic Deque - sliding window maximum
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Optimal
================================================================================
VARIABLES
  result   result[j] = max of nums[j .. j+k-1]
  deque    indices of the window; nums at those indices never increase from front to back
  i - k + 1   the left edge (start index) of the window that ends at i
WHY THIS PATTERN
  The problem asks for the maximum of every window of size k as the window moves
  one step at a time. A heap can find the max, but removing old items from it is
  costly. In this code, deque keeps only the indices that could still become a
  window maximum. So the answer for each window is at deque.First, and nothing
  has to be searched.
BRUTE FORCE
  For each of the n - k + 1 window starts, scan all k elements and take the max.
  This is O(n*k) time and O(1) extra space. It loses because neighbouring
  windows share k - 1 elements, and the scan does that shared work again for
  every window. When k is close to n / 2, this becomes quadratic.
INVARIANT
  Before the emit step, every index in deque is inside the window [i - k + 1,
  i], and the values nums[...] never increase from front to back. An index is
  removed only if it has left the window (step 1) or if a newer index with a
  larger value exists (step 2). A removed index can never be the maximum of this
  window or of any later one. So the front of deque is the largest value still
  in the window, and result[i - k + 1] gets the right answer.
STORE INDICES, NOT VALUES
  With values alone you cannot tell if the front has left the window. Step 1
  compares deque.First.Value with i - k + 1, and that check only works because
  the deque holds positions. The value is always read back as nums[index].
WATCH OUT
  The comment says the values are "strictly decreasing". The code does not do
  that. Step 2 uses a strict <, so an equal value is kept, and the values are
  only non-increasing. The result is still correct, but the comment is wrong.
  Also, the code never checks for k > nums.Length. If k = n + 1, the result is
  an empty array. If k is larger than that, new int[n - k + 1] gets a negative
  size and throws an exception.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How would you return the sliding window minimum instead?
     Flip the comparison in step 2 to nums[last] > nums[i], so the values never
     decrease from front to back. Everything else stays the same.
  2. What if the numbers arrive as a stream and you cannot store all of nums?
     The same loop works one element at a time. You only need to keep the last k
     values (for example, in a ring buffer indexed by i % k) plus the deque.
     Each answer is emitted as soon as its window is full.
  3. Is there an O(n) method without a deque?
     Yes. Split nums into blocks of size k. Build a prefix-max array and a
     suffix-max array inside each block. The max of a window is then
     max(suffix[start], prefix[end]). The logic is simpler, but it needs two
     extra arrays of size n.
  4. What if the window sizes differ from query to query?
     The deque only fits a fixed window that moves in one direction. For any
     range [l, r], build a sparse table (a table of the max for every
     power-of-two length). It answers each query in O(1) after O(n log n) setup.
TRIGGER
  Reach for this pattern when a problem asks for the max or min of every
  fixed-size window, or of a window whose edges only move forward.
C# NOTE
  Each LinkedList<int>.AddLast call creates a new node object. Each index is
  added at most once, so a plain int[n] array with head and tail indices works
  as the deque and creates no nodes.
COMPLEXITY
  Time  : O(n)
  Space : O(k)
================================================================================
*/
