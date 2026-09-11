// --------------------------------------------------------------------------
// -  optimal.cs            O(n log k) time / O(n) space
// -  max-heap of size K   [heap-size-k]
// -  the only solution in this folder
// -
// -  Reference solution - not one you solved yourself (from submission-0)
// -
// -  maintains a max-heap capped at K elements by distance, popping the
// -  farthest when it exceeds K
// --------------------------------------------------------------------------

public class Solution {
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
 PATTERN : Top-K via bounded max-heap: evict the current farthest
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  The question asks for a SET of K points, not a ranking of all n. So the whole
  ordering of the far-away points is wasted work. Keeping a heap capped at K
  means every point that is already beaten by K better points gets thrown away
  the moment it arrives, and the heap never grows past K regardless of how large
  points is.

  The counterintuitive part: to keep the K SMALLEST distances you need a
  MAX-heap, not a min-heap. The element you must be able to reach in O(1) is the
  worst one currently held, because that is the only one eviction can ever
  touch.
THE NEGATION TRICK
  C# PriorityQueue<TElement, TPriority> is a MIN-heap: Dequeue returns the item
  with the smallest priority. There is no max-heap in the BCL and no flag to
  flip it.

  That is the entire reason for Enqueue(point, -distance). Negating maps the
  largest distance to the smallest priority, so Dequeue pulls out the farthest
  point held. The element stored is still the unmodified point array - only the
  priority key is negated, so result never contains anything mangled.
INVARIANT
  After each iteration of the foreach, maxHeap contains exactly the min(K,
  points seen so far) closest points among those seen so far.

  Proof of the step: enqueue makes the heap hold the previous K closest plus the
  new point, i.e. K+1 candidates. The K closest of those K+1 are obtained by
  deleting the single farthest, which is precisely what the guarded Dequeue
  does. No point that belonged in the answer can be discarded, because a
  discarded point was strictly worse than K others still held.

  At the end of the loop the invariant instantiated at n gives the K closest of
  all points.
WHY SQUARED DISTANCE IS SAFE
  distance is point[0]*point[0] + point[1]*point[1] - no Math.Sqrt anywhere.
  Square root is monotonically increasing on non-negative inputs, so comparing
  squares orders the points identically to comparing true Euclidean distances.
  Skipping it also keeps the priority an int, so the comparisons are exact
  integer comparisons with no floating point tie weirdness.

  The real caveat is overflow: the multiply is int arithmetic, so a coordinate
  beyond about 46340 would wrap. The LeetCode constraint caps coordinates at
  10^4, giving a max distance of 2*10^8, comfortably inside int. If an
  interviewer widens the coordinate range, this is the line that breaks, and
  long is the fix.
WATCH OUT
  1. The guard is Count > K, not Count >= K. Enqueue first, then evict - the
  order matters. If you tested the size before inserting you would refuse to
  admit a point that is closer than something already held.

  2. The point just enqueued can be the one immediately dequeued. That is
  correct, not a bug: a point farther than all K incumbents belongs nowhere in
  the answer, and the heap round-trip is how the code discovers that.

  3. K is never compared against points.Length. If K >= points.Length the Count
  > K branch never fires and the method returns every point - which is the
  desired behaviour, so no special case is needed.
OUTPUT ORDER
  The drain loop empties a max-heap, so result comes out FARTHEST first and
  closest last among the K survivors. This problem accepts the answer in any
  order, which is the only reason that is acceptable.

  If a follow-up demands nearest-first, do not re-sort the output - reverse the
  list, or drain into the array back-to-front by index. Also note that ties in
  distance are broken arbitrarily by the heap's internal sift; with duplicate
  distances at the K boundary, which one survives is not deterministic, and any
  choice is a valid answer.
INTERVIEWER FOLLOW-UP
  Expect "can you beat n log K?" Yes: quickselect/partition on the distance
  array is O(n) average, partitioning around the K-th smallest distance and
  returning the left side. It costs O(n) extra space for the distances, has an
  O(n^2) adversarial worst case, and mutates the input order.

  The honest trade-off to state out loud: the heap version wins when points
  arrives as a stream or is too large to hold at once, since it only ever
  retains K elements and never needs random access. Quickselect wins on a fixed
  in-memory array. Plain Array.Sort by distance is the third option and is
  strictly worse than both at O(n log n).
COMPLEXITY
  Time  : O(n log k)
  Space : O(n)
================================================================================
*/
