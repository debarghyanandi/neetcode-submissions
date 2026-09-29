// --------------------------------------------------------------------------
// -  optimal.cs            O(n log k) time / O(k) space
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
 PROBLEM : Given an array of points [x, y] on a plane and an integer k, return
           the k points closest to the origin (0, 0). Distance is Euclidean.
           The answer may be in any order. Example: points = [[1,3],[-2,2]], k
           = 1 -> [[-2,2]].
 PATTERN : Heap (max-heap of size k, Top-K)
================================================================================
IDEA
  Keep maxHeap holding the k closest points seen so far, with the farthest on
  top.
  C# PriorityQueue is a min-heap, so the priority is -distance to make it act
  as
  a max-heap. After each Enqueue, if Count > K, Dequeue drops the farthest
  point.
  A point that is dropped is farther than K kept points, so it can never be in
  the answer. distance skips sqrt because squaring keeps the same order.
EXAMPLE
  points = [[3,3],[5,-1],[-2,4]], K = 2 -> distances 18, 26, 20
  add [3,3] (-18), add [5,-1] (-26), add [-2,4] (-20) -> Count 3 > 2,
  Dequeue removes -26 = [5,-1]. Drain: [-2,4] then [3,3].
  Answer: [[-2,4],[3,3]] (farthest of the k comes first)
COMPLEXITY
  Time  O(n log k)  n pushes and pops on a heap that never holds more than k+1
                    items
  Space O(k)        the heap and result hold at most k+1 points
PATH TO OPTIMAL
  Sort all points by distance, take first k - O(n log n) - simple baseline.
  Min-heap of all n, pop k - O(n + k log n) - heapify is O(n), but O(n) space.
  Max-heap capped at k (this file) - O(n log k) - O(k) space, works on a
  stream.
KEYWORDS
  top k, k closest, max-heap, priority queue, quickselect, squared distance
WATCH OUT
  - Forgetting the minus sign: priority distance keeps the k FARTHEST points,
    because Dequeue removes the smallest priority.
  - x*x + y*y is int. Very large coordinates would overflow; use long then.
  - Do not use Math.Sqrt: it is slower and adds floating point error.
  - The output comes out farthest-first; sort it if the caller wants order.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you do better than O(n log k)?
     -> Quickselect on distance, partitioning until index k-1 is placed. O(n)
        average, O(n^2) worst, O(1) extra space, but it changes the input array.
  2. Points arrive as a stream and are too many to store?
     -> This heap already fits: keep only k points, O(log k) per new point,
        O(k) memory. Sorting or quickselect need all points in memory.
  3. What if many queries ask for different k?
     -> Sort once in O(n log n); then each query just takes a prefix in O(k).
  4. Why max-heap and not min-heap?
     -> The max-heap top is the worst of the kept points, the only one that a
        new closer point can replace, so the check stays O(log k).
TRIGGER
  When a problem asks for the k smallest/largest/closest items and k is much
  smaller than n, keep a heap of size k.
================================================================================
*/
