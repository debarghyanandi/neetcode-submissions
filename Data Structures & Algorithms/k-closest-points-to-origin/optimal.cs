// --------------------------------------------------------------------------
// -  optimal.cs            O(n log k) time / O(k) space
// -  bounded max-heap of size K, evict farthest   [max-heap-size-k]
// -  the only solution in this folder
// -
// -  Reference solution - not one you solved yourself
// -
// -  heap capped at K elements by negated squared distance, popping the
// -  farthest whenever size exceeds K
// --------------------------------------------------------------------------

public class Solution
{
    public int[][] KClosest(int[][] points, int K)
    {
        PriorityQueue<int[], int> maxHeap = new();

        foreach (var point in points)
        {
            int distance = point[0] * point[0] + point[1] * point[1];
            maxHeap.Enqueue(point, -distance);
            if (maxHeap.Count > K)
            {
                maxHeap.Dequeue();
            }
        }

        var result = new List<int[]>();
        while (maxHeap.Count > 0)
        {
            result.Add(maxHeap.Dequeue());
        }

        return result.ToArray();
    }
}

/*
================================================================================
 PATTERN : Bounded max-heap of size K - evict the farthest
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  You never need the points sorted - you need a membership decision: is this
  point among the K closest. The only thing that can disqualify a candidate you
  are holding is the arrival of a closer one, and the candidate it displaces is
  always the worst one you hold. That is exactly the query a heap answers in one
  look at the root, so the heap is sized to the answer (K) rather than to the
  input (points.Length).
INVARIANT
  After the foreach body finishes for the i-th point, maxHeap holds exactly
  min(i, K) points, and those are the min(i, K) smallest-distance points among
  the first i seen. The root is the largest distance inside the heap - the
  current worst survivor. The invariant is re-established by the
  Enqueue/if-Count>K/Dequeue trio: Enqueue makes the set size min(i, K+1) and
  correct-by-superset, the Dequeue trims the one element that cannot belong.
WHY THE MINUS SIGN
  PriorityQueue<int[], int> in .NET dequeues the SMALLEST priority - it is a
  min-heap, there is no max-heap type. Enqueue(point, -distance) inverts the
  order so the point with the largest distance sits at the root and is what
  Dequeue removes. Drop the minus and the code silently computes the K farthest
  points. The alternative is passing a reversing IComparer<int> to the
  constructor; negation is fine here because distance is a non-negative sum of
  squares, so -distance never touches the int.MinValue negation edge case.
WHY NO SQRT
  distance is point[0]*point[0] + point[1]*point[1], the squared Euclidean
  distance. Square root is monotonic on non-negative reals, so ordering by d^2
  and ordering by d give the same ranking - and the same heap root. Skipping it
  keeps the priority an exact int instead of a double, which removes
  floating-point tie behavior from the comparison entirely.
CORRECTNESS OF THE EVICTION
  The step to defend is the Dequeue: why is it safe to throw a point away
  forever. When Count hits K+1 you are holding K+1 candidates and the root is
  the maximum of them. Any final answer of size K drawn from this prefix can
  exclude that maximum - there are K others at least as close - so discarding it
  loses no achievable optimum. And the discarded point can never become eligible
  later, because the heap's set of distances only improves: every future
  replacement swaps the root for something strictly not larger. So exclusion is
  permanent and the invariant carries to the end of the loop.
WATCH OUT
  1. The heap transiently holds K+1 entries, not K - the Enqueue happens before
  the size check. Correct, but if you were asked to hold a hard K-element bound,
  use EnqueueDequeue after the heap is full (one sift instead of an insert plus
  an extract), or compare against Peek first.
  2. Overflow lives in distance. With coordinates bounded by 1e4 the sum tops
  out near 2e8 and int is safe; loosen the constraint and this needs long, which
  also changes the TPriority type argument.
  3. The drain loop emits farthest-first among the K survivors, and .NET's
  PriorityQueue gives no tie-break guarantee for equal priorities. Both are fine
  here only because the problem accepts the K closest in any order - do not
  reuse this drain if the caller expects nearest-first.
  4. result is a List<int[]> grown by default; its final length is min(K,
  points.Length), so a short points array silently returns fewer than K rather
  than throwing.
INTERVIEW FOLLOW-UP
  Expect "can you beat n log k". Quickselect on the squared distances partitions
  around the K-th smallest in O(n) average with O(1) extra space, but degrades
  to O(n^2) on adversarial pivots and mutates the input array. Full sort is O(n
  log n) and needs all n distances resident. The pin for this heap version is
  streaming: it makes one pass, touches each point once, and never needs
  points.Length to be known or finite - which is the answer when the follow-up
  becomes "the points arrive on a socket".
TRIGGER
  "K best out of n" with K much smaller than n, a scorable item, and no
  requirement that the output be ordered. Reach for a heap of size K ordered
  OPPOSITE to what you want to keep - max-heap to keep the smallest, min-heap to
  keep the largest - so the root is always the element you are willing to lose.
COMPLEXITY
  Time  : O(n log k)
  Space : O(k)
================================================================================
*/
