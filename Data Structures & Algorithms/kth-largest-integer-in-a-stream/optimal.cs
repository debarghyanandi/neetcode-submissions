// ##########################################################################
// #  optimal.cs            O(n log k) time / O(k) space
// #  size-capped min-heap   [min-heap-size-k]
// #  the only solution in this folder
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  maintains a min-heap capped at k elements so the root is always the
// #  kth largest; heap never holds more than k+1 elements regardless of n
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
        foreach (int num in nums)
        {
            pq.Enqueue(num, num);

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
 PATTERN : Bounded Min-Heap - root of a size-k heap is kth largest
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-0.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  The stream only ever has to answer one question: what is the kth largest so
  far. That means every element smaller than the current kth largest is dead
  weight - it can never be the answer again, because elements are only ever
  added, never removed. So the state you must carry is exactly the top k values,
  and the one you must report is the smallest of those. A min-heap capped at k
  is the data structure that holds that set and puts that specific element at
  the root.
INVARIANT
  After the constructor finishes and after every Add returns, pq contains
  exactly the min(k, elements seen) largest values seen so far, and nothing
  else. The two lines 'Enqueue then if (pq.Count > k) Dequeue' are what maintain
  it: enqueue makes the set the top k plus one candidate, the dequeue drops the
  minimum of that k+1 set, which is precisely the element that cannot be in the
  top k. Note this is the same four lines in the constructor and in Add - the
  constructor is just Add without the Peek, which is why nums of any length is
  handled with no special case.
WHY PEEK IS THE ANSWER
  pq.Peek() returns the minimum of a set that is exactly the k largest values.
  The minimum of the top k is the kth largest - that equivalence is the whole
  proof. Spell it out that way in an interview rather than saying 'the root is
  the answer'. Also worth saying: kth largest counts by position, not by
  distinct value. With k=3 and stream 5,5,5 the answer is 5, and this code gets
  it right for free because the heap stores duplicates as separate entries and
  never deduplicates.
BRUTE FORCE AND WHY IT LOSES
  The naive version keeps a List of everything and sorts it on each Add, or
  scans it k times to pull the kth max. Both re-derive information you already
  had; the sort pays for full ordering of the entire history when you only need
  one boundary element. The heap keeps the memory at k regardless of stream
  length, which also matters here because this is a class with a live object -
  an unbounded list grows forever across calls.
WATCH OUT
  1. C# PriorityQueue<TElement, TPriority> dequeues the LOWEST priority first -
  it is a min-heap by default. That is what this solution needs, so there is no
  comparer argument. If you ever reach for the max-heap version of a problem you
  must pass Comparer<int>.Create((a,b) => b - a) or negate the priority;
  forgetting is the classic bug.
  2. Enqueue(num, num) passes the value as both element and priority. Peek()
  returns the ELEMENT, not the priority. They are identical here, so it does not
  matter - but in problems where the priority is a derived key (distance,
  frequency) the two differ and Peek gives you back the payload.
  3. Peek() throws on an empty heap. This code is safe only because the problem
  guarantees Add is called when at least k elements exist; there is no Count
  check before the return. Say that out loud rather than letting an interviewer
  find it.
  4. pq.Count momentarily reaches k+1 inside both methods. That is intentional
  and harmless, but it means the heap is sized k+1 at peak, not k.
INTERVIEWER FOLLOW-UP
  'Can you avoid the transient k+1 element?' Yes - once pq.Count == k, the
  two-step Enqueue/Dequeue can be replaced by pq.EnqueueDequeue(val, val), which
  pushes and pops in one sift and skips the grow. Same asymptotics, one heap
  operation instead of two. 'What if k were huge, close to n?' Then the
  bounded-heap advantage shrinks toward just holding everything, and you would
  consider keeping a sorted structure or a max-heap of the complement instead.
  'What about kth SMALLEST in a stream?' Same skeleton, flipped comparer - a
  max-heap of size k whose root is the kth smallest.
TRIGGER
  Streaming or online input, a fixed k, and a query for a rank near one end. The
  reflex chain: want the k largest -> keep a MIN-heap of size k -> root is the
  kth largest. The heap is always the opposite polarity of the word in the
  problem statement, and that inversion is the single thing worth memorizing
  here.
COMPLEXITY
  Time  : O(n log k)
  Space : O(k)
================================================================================
*/
