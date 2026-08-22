public class Solution {
    public int[][] Merge(int[][] intervals) {
        Array.Sort(intervals, (a, b) => a[0].CompareTo(b[0]));
        var output = new List<int[]>();
        output.Add(intervals[0]);

        for (int j = 1; j < intervals.Length; j++) {
            int start = intervals[j][0];
            int end = intervals[j][1];
            var current = output[output.Count - 1];

            if (start <= current[1]) {
                current[1] = Math.Max(current[1], end);
            } else {
                output.Add(intervals[j]);
            }
        }
        return output.ToArray();
    }
}