// ##########################################################################
// #  optimal.cs            O(n log k) time / O(k) space
// #  Min-heap of size k   [min-heap-kth-largest]
// #  the only solution in this folder
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  Heap size stays at most k, so each operation is logarithmic in k, not
// #  n.
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
 PATTERN : Min-Heap of Size K - root is the kth largest
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-0.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  pq    min-heap holding the k largest values seen so far; pq.Peek() is the kth largest
  k     the rank asked for; also the fixed cap on pq.Count
WHY THIS PATTERN
  The class is queried over and over as new values arrive, so the answer must
  survive between calls; only a data structure kept as a field can do that.
  Nothing below the kth largest can ever become the answer again, so those
  values can be thrown away, and pq only has to remember k items. A min-heap is
  the right shape because the item most likely to be evicted next is the
  smallest one it holds, and that is exactly the one at its root.
BRUTE FORCE
  Store every value in a List, and on each Add sort it descending and return
  index k-1. That is O(n log n) per call and O(n) memory for the whole stream.
  It loses because it re-sorts values that were already known to be too small to
  matter, and it never releases them.
INVARIANT
  After each iteration of the constructor loop and after each Add, pq contains
  exactly the min(k, number of values seen) largest values, and its root is the
  smallest of those. The enqueue-then-check-Count pair restores this: adding one
  value can make Count k+1, and the only value that can now be outside the top k
  is the current minimum, which Dequeue removes. So when Count is k, Peek is the
  kth largest by definition.
VALUE USED AS ITS OWN PRIORITY
  Enqueue(v, v) passes the same int as element and priority, so ordering is
  plain numeric ordering with no comparer to write. Duplicates are fine and must
  be kept: with nums = [4,4,4] and k = 3 the answer is 4, so treating the heap
  as a set would be wrong.
WATCH OUT
  Add calls Peek with no guard. If nums is shorter than k-1, the heap still has
  fewer than k items after the first Add and Peek returns the smallest of too
  few values - a wrong answer, not an exception. It only throws if nums is empty
  and k is 2 or more on the very first Add, because Peek on an empty queue
  throws InvalidOperationException. The code leans on the usual guarantee that
  nums has at least k-1 elements; nothing in the class checks it. Also, k is
  never validated, and k <= 0 makes the constructor dequeue every value and
  leaves pq empty.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How would you return the kth smallest of the stream instead?
     Flip to a max-heap of size k by enqueuing with a negated priority,
     Enqueue(val, -val), or pass Comparer<int>.Create((a,b) => b.CompareTo(a))
     to the constructor; the eviction logic is unchanged.
  2. What if k is close to the total number of values seen?
     The heap stops saving memory, since it holds nearly everything. A sorted
     structure such as a balanced BST or a skip list gives the same per-add cost
     and also supports queries for other ranks, not just k.
  3. What if you must also support removing a value that was added earlier?
     PriorityQueue has no remove-by-value, so you would need an indexed heap or
     a sorted multiset; with this class you would have to rebuild pq from
     scratch.
  4. How would you find the running median instead of the kth largest?
     Keep two heaps, a max-heap for the lower half and a min-heap for the upper
     half, and rebalance so their sizes differ by at most one; the median is one
     or both roots.
TRIGGER
  A stream or repeated queries asking for the kth largest or smallest, where
  only a fixed rank matters and everything past it can be discarded.
C# NOTE
  PriorityQueue<TElement,TPriority> is a min-heap by default, which is why no
  comparer appears here. When Count is already k, pq.EnqueueDequeue(val, val)
  does the insert and removal in one sift instead of growing to k+1 and then
  shrinking back.
COMPLEXITY
  Time  : O(n log k)
  Space : O(k)
================================================================================
*/
