// ##########################################################################
// #  optimal.cs            O(n log k) time / O(n) space
// #  min-heap of size k via PriorityQueue   [min-heap-size-k]
// #  the only solution in this folder
// #
// #  YOU SOLVED THIS YOURSELF (from submission-0)
// #
// #  maintains a min-heap capped at k elements, popping the smallest
// #  whenever it exceeds k so the top is always the kth largest
// ##########################################################################

public class KthLargest {
    //My solution
    private PriorityQueue<int, int> pq;
    private int k;

    public KthLargest(int k, int[] nums) {
        this.k = k;
        this.pq = new PriorityQueue<int, int>();
        foreach(int v in nums){
            pq.Enqueue(v, v);
            
            if(pq.Count > k)
                pq.Dequeue();
        }
    }
    
    public int Add(int val) 
    {
        pq.Enqueue(val, val);
            
        if(pq.Count > k)
        pq.Dequeue();

        return pq.Peek();
    }
    
}

/*
================================================================================
 PATTERN : Top-K min-heap - size-k heap root is the kth largest
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-0.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  The question asked at every step is not "sort the stream" but "what is the
  single value sitting at rank k from the top right now?" That means everything
  below rank k is dead weight and can be thrown away permanently - a value that
  is already smaller than k other values can never become the kth largest,
  because the stream only ever adds more competitors above it.

  So keep exactly the k largest values seen so far, and keep them in a container
  whose cheapest-to-reach element is the SMALLEST of them. That smallest of the
  top k is, by definition, the kth largest overall. pq.Peek() answers the query
  in one step.

  Note what is never stored: the heap holds at most k+1 elements at any instant.
  The nums array is walked once and discarded; no history of the stream is
  retained.
INVARIANT
  After every completed call to the constructor or to Add, pq contains exactly
  min(k, number_of_values_seen) elements, and they are precisely the k largest
  values seen so far (counting duplicates as distinct occupants).

  The invariant is restored by the same two lines in both places: Enqueue
  unconditionally, then drop the root if Count exceeded k. The drop is correct
  because after the enqueue the heap holds the k+1 largest candidates, and the
  root is the smallest of those k+1 - it is the one and only element that cannot
  belong to the top k. Enqueue then Dequeue is a legal transient violation, not
  a bug: nothing reads the heap between those two statements.
TRACE
  k = 3, nums = [4, 5, 8, 2].

  Constructor: 4 -> {4}; 5 -> {4,5}; 8 -> {4,5,8}; 2 -> {2,4,5,8}, Count 4 > 3,
  dequeue root 2 -> {4,5,8}. The 2 is gone forever.

  Add(3): {3,4,5,8}, dequeue 3, heap {4,5,8}, Peek returns 4. The new value
  evicted ITSELF - that is the normal case for a small arrival, and it is why no
  "is val big enough" branch is required for correctness.

  Add(5): {4,5,5,8}, dequeue 4, heap {5,5,8}, Peek returns 5. Two fives coexist;
  the third largest is 5, not 4.
THE .NET API DETAIL
  PriorityQueue<TElement, TPriority> in the BCL is a MIN-heap by default -
  Dequeue and Peek both go to the lowest priority. That default is the entire
  reason this file is four lines of logic. Here TElement and TPriority are both
  int and the value is passed twice, v as element and v as priority, so priority
  order is value order.

  The queue is not documented as stable for equal priorities, which is
  irrelevant here: two entries with equal priority also have equal value, so it
  does not matter which one comes out.

  A tightening worth remembering: PriorityQueue exposes EnqueueDequeue(element,
  priority), which expresses exactly this push-then-drop-the-root step as one
  call and never lets the heap grow to k+1.
WATCH OUT
  Do not dedupe. Duplicates are separate ranks - with k=3 and values 5,5,8 the
  answer is 5. A HashSet or a distinct-values structure gives the kth largest
  DISTINCT value, a different problem.

  Do not invert the comparer. Handing the constructor a reversed Comparer to get
  a max-heap makes Peek return the largest of the retained set, and the element
  Dequeue evicts becomes the biggest one - the code would then discard exactly
  the values it needs to keep. There is no way to peek the far end of a .NET
  PriorityQueue, so the min-heap orientation must be the one that survives.

  The constructor can finish with Count < k when nums is short. Peek in Add is
  safe only because the problem guarantees at least k values exist by the time
  Add is queried; if k exceeds nums.Length + 1 the first Add returns the
  smallest retained value, which is not the kth largest. Worth stating out loud
  in an interview rather than being caught by it. Note also that Peek on an
  empty queue throws - it does not return a default.

  The two if statements are not brace-aligned identically (the Add one has its
  Dequeue on an unindented line). It is one statement and it is correct, but it
  reads like a bug at a glance.
INTERVIEW FOLLOW-UPS
  "Why not sort the array each Add?" - resorting redoes work already paid for;
  the heap keeps the answer amortized across arrivals and touches only a
  root-to-leaf path.

  "Why not a max-heap of everything and pop k-1 times?" - it retains all n
  values instead of k, and each query costs k pops plus k re-inserts to restore
  the structure, so cost grows with k on every single Add rather than staying at
  one sift.

  "Can you support a kth SMALLEST stream too?" - same shape, flipped: keep a
  size-k max-heap and peek its root.

  "Any duplication to clean up?" - the constructor body and Add share identical
  logic. The constructor loop could call Add(v) and ignore the return value,
  leaving one copy of the invariant-restoring code. Cosmetic, but it removes the
  risk of fixing one copy and not the other.
COMPLEXITY
  Time  : O(n log k)
  Space : O(n)
================================================================================
*/
