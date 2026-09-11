public class Solution {
    public int[][] KClosest(int[][] points, int K) {
        PriorityQueue<int[], int> maxHeap = new();

        foreach (var point in points) {
            int dist = point[0] * point[0] + point[1] * point[1];
            maxHeap.Enqueue(point, -dist);
            if (maxHeap.Count > K) {
                maxHeap.Dequeue();
            }
        }

        var res = new List<int[]>();
        while (maxHeap.Count > 0) {
            res.Add(maxHeap.Dequeue());
        }

        return res.ToArray();
    }
}