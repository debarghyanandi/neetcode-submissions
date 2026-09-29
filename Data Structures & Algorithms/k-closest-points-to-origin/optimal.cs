// --------------------------------------------------------------------------
// -  optimal.cs            O(n log k) time / O(k) space
// -  Max heap, maintain K closest   [max-heap-k-closest]
// -  the only solution in this folder
// -
// -  Reference solution - not one you solved yourself
// -
// -  Heap operations on K elements cost O(log K) per point; draining K
// -  elements adds O(K log K), dominated by O(n log K).
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
 PATTERN : Top-K with a Bounded Max-Heap - evict the farthest point
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  maxHeap   holds at most K points; the priority is -distance, so the farthest point comes out first
  distance  squared distance from the origin, x*x + y*y (no square root)
WHY THIS PATTERN
  The problem asks for the K closest points, not a full ordering. That "only K
  of n" wording points to a heap of fixed size K. maxHeap keeps the K best
  points seen so far. The worst of those K sits at the top, so each new point
  only has to be checked against that one point. Squared distance is enough
  because the square root does not change which point is closer.
BRUTE FORCE
  Compute the distance for every point, sort all n points by that distance, and
  take the first K. This is correct and O(n log n) time. It loses when K is much
  smaller than n, because it orders all n points when only K matter. It also
  needs O(n) extra space unless you sort the input in place.
INVARIANT
  After each point is handled, maxHeap holds the K closest points among the
  points seen so far, or all of them if fewer than K have been seen. When a new
  point makes the count K+1, Dequeue removes the point with the smallest
  priority, which is the largest distance. That point cannot be in the final
  answer, because at least K points are closer to the origin. When the loop
  ends, every point has been checked, so the heap holds the true K closest.
WATCH OUT
  The result is not sorted from nearest to farthest. The drain loop pops the
  farthest point first, so result comes out in order of decreasing distance. If
  the caller needs nearest first, reverse it. point[0] * point[0] + point[1] *
  point[1] is done in int. With large coordinates it can overflow and turn
  negative, and then -distance gives the wrong order. Use long if the bounds are
  not known. Ties at the K-th distance are broken in no fixed order, so which
  tied point is kept can change from run to run.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you beat O(n log k) on average?
     Yes. Use Quickselect on the squared distances to split the array around the
     K-th smallest, then take the first K. It is O(n) on average but O(n^2) in
     the worst case. It also reorders the input and does not work on a stream.
  2. What if the points arrive as an endless stream and you must answer at any
  time?
     Keep this same bounded heap. It already uses only O(k) memory and never
     needs to see old points again. Quickselect and sorting both need all points
     at once.
  3. What if K is close to n?
     Then log k is about log n, so the heap gives no gain over sorting. Another
     option is to keep a heap of the n-K farthest points and return everything
     else.
TRIGGER
  The problem asks for the K smallest, largest, closest or most frequent items,
  and you do not need the full order.
C# NOTE
  PriorityQueue in .NET is a min-heap, and this code turns it into a max-heap by
  negating the priority. Another way is to pass Comparer<int>.Create((a, b) =>
  b.CompareTo(a)) to the constructor. Once the heap has K items,
  EnqueueDequeue(point, -distance) does the push and the pop in one call. It
  also returns the new point at once, without adding it, if the new point is the
  worst one.
COMPLEXITY
  Time  : O(n log k)
  Space : O(k)
================================================================================
*/
