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
 PROBLEM : Design a class built from k and an int array nums. Each call
           Add(val) adds val to the stream and returns the k-th largest value
           so far (k-th in sorted order, duplicates count, not k-th distinct).
           Example: k=3, nums=[4,5,8,2]; Add(3) -> 4, Add(5) -> 5, Add(10) ->
           5.
 PATTERN : Top-K with a min-heap of size k
================================================================================
IDEA
  Keep only the k largest values seen so far in a min-heap, pq.
  After every Enqueue, if pq.Count > k, Dequeue removes the smallest.
  The root, pq.Peek(), is the smallest of the top k, so it is the answer.
  It is correct because a value dropped from pq already has k larger or
  equal values in pq, so it can never be the k-th largest again.
EXAMPLE
  k=3, nums=[4,5,8,2] -> pq holds 4,5,8 and 2 is dequeued.
  Add(3): 3 in, 3 out -> 4 | Add(5): 4 out -> {5,5,8} -> 5
  Add(10): 5 out -> {5,8,10} -> 5 | Add(9): 5 out -> {8,9,10} -> 8
  Add(4): 4 in, 4 out -> 8. Outputs: 4, 5, 5, 8, 8.
COMPLEXITY
  Time  O(n log k)  each push/pop costs log k because pq never holds more than
                    k+1
  Space O(k)        pq keeps at most k+1 values, the rest of the stream is
                    dropped
PATH TO OPTIMAL
  Sort all values on every Add - O(n log n) per call - simple baseline.
  Sorted list, insert by position - O(n) per call - no full re-sort.
  Min-heap of size k (this file) - O(log k) per call - never shifts n items.
KEYWORDS
  heap, priority queue, min-heap, top k elements, data stream, design
WATCH OUT
  - C# PriorityQueue is a min-heap by priority. Using a max-heap here
    gives the largest value, not the k-th largest.
  - Pop when Count > k, not >= k. With >= the heap keeps k-1 values.
  - Priority must be the value itself (Enqueue(v, v)); a constant priority
    breaks the ordering.
  - Peek() throws if pq is empty. This relies on the guarantee that at
    least k values exist when Add is called.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Why a min-heap and not a max-heap for the k-th largest?
     -> We must throw away small values fast; the min-heap root is the
        smallest of the top k, which is exactly the answer and the thing to drop.
  2. What if values can also be removed from the stream?
     -> Use a balanced BST or SortedDictionary with counts, or two heaps with
        lazy deletion. O(log n) per operation, but memory grows to O(n).
  3. What if k changes between calls?
     -> A fixed heap of size k no longer works. Keep all values in an
        order-statistic tree: O(log n) per query and O(n) space.
  4. Only one query at the end, not a stream?
     -> Quickselect finds the k-th largest in O(n) average time and O(1) extra
        space, but O(n^2) in the worst case.
TRIGGER
  When you need the k-th largest, k-th smallest or top k of a growing
  stream, keep a heap of size k with the opposite order.
================================================================================
*/
