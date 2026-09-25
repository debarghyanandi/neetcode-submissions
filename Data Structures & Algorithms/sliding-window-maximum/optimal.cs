// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(k) space
// -  Monotonic deque, sliding window   [monotonic-deque]
// -  ties with optimal-variant.cs on O(n) time / O(k) space
// -
// -  Reference solution - not one you solved yourself
// -
// -  Each element is added and removed from the deque at most once, giving
// -  amortized linear time.
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
 PATTERN : Monotonic Deque - sliding window maximum in one pass
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Optimal
================================================================================
VARIABLES
  result   result[i - k + 1] = max of the window ending at index i
  deque    indices into nums, kept in order so nums[...] never increases front to back
WHY THIS PATTERN
  The problem asks for the maximum of every window of fixed width k as the
  window slides one step at a time. Consecutive windows share k-1 elements, so
  rescanning them is wasted work; what changes each step is one index entering
  and one index leaving. A monotonic deque (a double-ended queue whose stored
  values stay ordered) lets you drop, at the moment i arrives, every older index
  that nums[i] beats, because those can never win again. What survives at
  deque.First is exactly the current window's maximum, so result[i - k + 1] is
  one read.
BRUTE FORCE
  The first thing most people write is two nested loops: for each start
  position, scan the k values and keep the largest. That is O(n*k) time and O(1)
  extra space beyond the output. It loses because every window re-reads the k-1
  elements it shares with the previous window; when k is large this repeated
  scanning dominates.
INVARIANT
  At the top of each iteration, after the two while loops and the AddLast, deque
  holds exactly the indices in [i-k+1, i] that are not beaten by any later index
  in that range, ordered by index and with non-increasing nums values. Step 1
  guarantees no index older than the window survives; step 2 guarantees no index
  whose value is smaller than a newer one survives. So deque.First is the
  newest-oldest surviving candidate with the largest value, i.e. the true
  maximum of the window ending at i, which is what gets written once i >= k - 1.
INDICES, NOT VALUES
  The deque stores i, not nums[i], because expiry is a question about position:
  deque.First!.Value < i - k + 1 can only be tested if the position is
  available. Values alone would make it impossible to know whether the front has
  slid out of the window. Everywhere a value is needed, the code dereferences
  through nums[deque.Last!.Value] or nums[deque.First!.Value].
ONE PUSH AND ONE POP PER INDEX
  The two while loops look like they could be expensive, but each index is added
  by AddLast exactly once and removed at most once, by either RemoveFirst or
  RemoveLast. Total deque operations over the whole run are therefore bounded by
  2n, which is why the inner loops do not multiply the cost. This amortized
  argument is the standard interview answer for why the nested whiles are still
  linear.
WATCH OUT
  The comment says values are "strictly decreasing from front to back", but the
  pop condition is nums[deque.Last!.Value] < nums[i], which keeps duplicates;
  the real invariant is non-increasing. That choice is correct and deliberate -
  popping on <= would still work here, but keeping equals is safer if you later
  need the oldest maximum - yet the comment is wrong as written. The guard does
  not check k > nums.Length, so new int[n - k + 1] gets a negative length and
  throws instead of returning an empty array. The expiry loop runs before the
  current index is pushed, so it can never remove i itself; reordering steps 1
  and 2 after AddLast would break that.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How do you get the sliding window minimum instead?
     Flip the comparison in step 2 to nums[deque.Last!.Value] > nums[i] so the
     deque becomes non-decreasing; everything else is unchanged. Running both at
     once needs two independent deques.
  2. The input arrives as a stream and you cannot hold nums in memory. What
  changes?
     Store the pair (index, value) in the deque instead of the index alone,
     since nums[...] is no longer indexable, and emit each maximum as soon as
     the counter reaches k - 1. Memory stays bounded by the deque, which never
     holds more than k entries.
  3. What if k changes between queries on a fixed array?
     The deque pass is tied to one k, so you would rebuild per query. For many
     queries, precompute a sparse table for range maximum: O(n log n) build,
     O(1) per query, at the cost of O(n log n) memory.
  4. Can you avoid the deque entirely?
     Yes - the block prefix/suffix maximum trick. Split nums into blocks of size
     k, compute prefix maxima left to right and suffix maxima right to left
     inside each block, then each window's answer is max(suffix[start],
     prefix[end]). Same linear time but two extra n-sized arrays instead of a
     k-sized deque.
TRIGGER
  A fixed-width window slides by one and you need an extreme value (max or min)
  of each window - reach for the monotonic deque.
C# NOTE
  C# has no built-in deque - Queue<T> only removes from the front - so
  LinkedList<int> is used here, and it allocates a LinkedListNode<int> object
  per push. An int[] of length k with head and tail indices used as a ring
  buffer gives the same operations with one allocation, since the deque provably
  never exceeds k entries.
COMPLEXITY
  Time  : O(n)
  Space : O(k)
================================================================================
*/
