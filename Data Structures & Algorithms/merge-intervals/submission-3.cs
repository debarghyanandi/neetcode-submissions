public class Solution
{
    public int[][] Merge(int[][] intervals)
    {
        // Sorting by START is what makes a single pass sufficient: any
        // interval that can overlap the one being built must come next.
        Array.Sort(intervals, (a, b) => a[0].CompareTo(b[0]));

        var merged = new List<int[]>();

        // Copy rather than reference, so the caller's array is never mutated.
        merged.Add(new int[] { intervals[0][0], intervals[0][1] });

        for (int i = 1; i < intervals.Length; i++)
        {
            int currentStart = intervals[i][0];
            int currentEnd = intervals[i][1];

            int[] lastMerged = merged[merged.Count - 1];

            if (currentStart <= lastMerged[1])
            {
                // Overlap: absorb it by stretching the end.
                // Math.Max matters - the current interval may be fully
                // CONTAINED, e.g. [1,10] then [2,3] must stay [1,10].
                lastMerged[1] = Math.Max(lastMerged[1], currentEnd);
            }
            else
            {
                // Gap: the previous block is final, start a new one.
                merged.Add(new int[] { currentStart, currentEnd });
            }
        }

        return merged.ToArray();
    }
}
