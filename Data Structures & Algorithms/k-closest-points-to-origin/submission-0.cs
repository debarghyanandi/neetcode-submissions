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
