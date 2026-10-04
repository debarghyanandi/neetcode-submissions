public class Solution {
    public int[][] Insert(int[][] intervals, int[] newInterval) {
        
        int n = intervals.Length;
        var res = new List<int[]>();
        int i = 0;
        
        // Left NonOverlapping part
        while(i < n && (intervals[i][1] < newInterval[0]))
        {
            res.Add(new int [] {intervals[i][0], intervals[i][1]});
            i = i + 1;
        }

        // Merge overlapping compartments and store in newInterval
        while(i < n && intervals[i][0] <= newInterval[1])
        {
            newInterval[0] = Math.Min(newInterval[0], intervals[i][0]);
            newInterval[1] = Math.Max(newInterval[1], intervals[i][1]);
            
            i = i + 1;
        }

        res.Add(newInterval);

        // Right NonOverlapping part
        while(i < n)
        {
            res.Add(new int []{intervals[i][0], intervals[i][1]});
            i = i + 1; 
        }

        return res.ToArray();
    }
}
