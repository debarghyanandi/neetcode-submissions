// ##########################################################################
// #  optimal.cs            O(n log k) time / O(k) space
// ##########################################################################

public class KthLargest
{
    //My solution
    private PriorityQueue<int, int> pq;
    private int k;

    public KthLargest(int k, int[] nums)
    {
        this.k = k;
        this.pq = new PriorityQueue<int, int>();
        foreach (int v in nums)
        {
            pq.Enqueue(v, v);

            if (pq.Count > k)
                pq.Dequeue();
        }
    }

    public int Add(int val)
    {
        pq.Enqueue(val, val);

        if (pq.Count > k)
            pq.Dequeue();

        return pq.Peek();
    }

}

/*
================================================================================
 PATTERN : Top-K with a Min-Heap - keep only the k largest
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-0.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  pq    min-heap that holds the k largest values seen so far; pq.Peek() = kth largest
  k     heap size limit, saved in a field so Add can use it
WHY THIS PATTERN
  The problem asks for the kth largest value in a stream that only grows. We
  never need the whole order, only the border between the top k and the rest. A
  min-heap (a tree that always gives the smallest item first) of size k keeps
  that border at its root. So pq.Peek() is the answer, and each new val needs
  only one push and at most one pop.
BRUTE FORCE
  Keep every value in a sorted List<int>. On each Add, find the place with
  binary search, Insert there, and return the item at index Count - k. This is
  correct, but Insert shifts elements, so each Add costs O(n) time. It also uses
  O(n) memory, and that memory grows with the whole stream. The heap needs only
  O(k) memory.
INVARIANT
  After every constructor step and every Add, pq holds exactly the min(k, values
  seen) largest values. When pq.Count goes above k, Dequeue removes the smallest
  one. That value already has k larger values above it, and the stream only adds
  values, so it can never become the kth largest again. This means the root of
  pq is always the kth largest value.
WATCH OUT
  If k is 0, Add pushes val and then pops it at once. The heap is then empty,
  and pq.Peek() throws InvalidOperationException. If nums plus the added values
  give fewer than k items, Peek returns the smallest of what is there. That is
  not a true kth largest, and the code gives no warning. In the constructor, the
  parameter k hides the field k. The code is correct only because the field is
  set first with this.k = k.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How would you return the kth smallest instead?
     Keep the k smallest values in a max-heap. Pass Comparer<int>.Create((a, b)
     => b.CompareTo(a)) to the PriorityQueue, or use -val as the priority. The
     logic stays the same, only the heap order flips.
  2. What if values can also be removed from the stream?
     A heap cannot remove any item except its root. One option is an
     order-statistic tree (a balanced tree that also stores subtree sizes),
     which gives O(log n) insert, delete and kth lookup. The cost is O(n)
     memory, because a removal can bring an evicted value back into the top k.
  3. What if each query asks for a different k?
     A fixed-size heap no longer works. Keep all values in an order-statistic
     tree, or in two heaps split around the current rank. Memory grows to O(n).
TRIGGER
  The problem asks for the kth largest or smallest value in a stream that keeps
  growing, and you only need the top k items, not the full sorted order.
C# NOTE
  When pq.Count already equals k, you can call pq.EnqueueDequeue(val, val). It
  does the push and the pop in one call. If val is not larger than the root, it
  returns val and does not change the heap.
COMPLEXITY
  Time  : O(n log k)
  Space : O(k)
================================================================================
*/
