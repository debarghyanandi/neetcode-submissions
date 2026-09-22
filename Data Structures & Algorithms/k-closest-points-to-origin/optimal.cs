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
 PATTERN : Bounded max-heap - keep the K smallest seen so far
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  maxHeap   holds at most K points; priority is -distance, so the farthest point sits on top
  distance  squared distance from origin, point[0]^2 + point[1]^2
  result    the surviving heap contents, drained farthest-first
WHY THIS PATTERN
  The question asks for the K points nearest the origin, not a ranking of all of
  them, so full sorting does work the answer never uses. A heap capped at K lets
  each point be tested against only the current worst keeper: if maxHeap grows
  past K, the single Dequeue removes the farthest one. Each point costs one push
  and maybe one pop on a structure of size K, and nothing about a point is
  remembered once it is evicted.
BRUTE FORCE
  The natural first try is to compute every squared distance, sort the whole
  array by it, and take the first K. That is O(n log n) time and it touches the
  order of elements you will throw away. It loses when K is far smaller than n,
  because the sort pays log n per element while this heap pays only log K.
INVARIANT
  After every loop iteration, maxHeap contains exactly the K closest points
  among the ones seen so far (or all of them, if fewer than K have been seen).
  The eviction is safe because the element removed has the largest distance in
  the heap, and a point that is the worst of the current best K can never be one
  of the best K of a larger set. When the loop ends, the set seen so far is the
  whole input, so the heap is the answer.
NO SQUARE ROOT
  distance is the squared distance and the real Euclidean distance is never
  computed. Square root is monotonic for non-negative values, so ordering by
  x^2+y^2 gives the same ordering as by sqrt(x^2+y^2). This avoids floating
  point entirely, so two points at the same distance can never compare wrongly
  because of rounding.
WATCH OUT
  The name maxHeap is a description of behaviour, not of the type: .NET's
  PriorityQueue is a min-heap, and the max behaviour comes only from the minus
  sign in Enqueue(point, -distance). If someone later adds a reversing comparer
  and leaves the negation in, the code silently keeps the K farthest points. The
  distance expression is int arithmetic, so large coordinates overflow and can
  wrap negative, making a far point look close; long would remove the risk.
  Also, if K is bigger than points.Length the method returns every point instead
  of K, and the returned order is farthest-to-nearest among the keepers, which
  only works if the problem accepts any order.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you get below O(n log K)?
     Quickselect on the squared distances partitions around a pivot and recurses
     into one side only, giving O(n) average time and O(1) extra space, but
     worst case is O(n^2) and it must own and reorder the input array.
  2. What if K is close to n?
     Flip the roles: keep a min-heap of the n-K farthest points and return
     whatever the input has that is not in it, or just sort, since log K is then
     no better than log n.
  3. The points arrive as an endless stream and you cannot store them all.
     This code already handles it - the heap never exceeds K entries and each
     point is processed once, so only the loop body and a running heap of size K
     are needed.
  4. The output must be sorted nearest first.
     Reverse result after the drain, or drain into a fixed int[K][] filling from
     index K-1 down to 0; either adds no extra asymptotic cost.
TRIGGER
  The problem asks for the top or bottom K items by a score you can compute per
  item, and does not ask for the rest to be ordered.
C# NOTE
  Once the heap is full, Enqueue followed by Dequeue can be replaced by the
  single call maxHeap.EnqueueDequeue(point, -distance), which sifts once instead
  of twice and never lets the heap grow to K+1. Also, since the final size is
  known, filling a pre-sized int[][] avoids the List plus ToArray copy.
COMPLEXITY
  Time  : O(n log k)
  Space : O(k)
================================================================================
*/
