public class Solution {
    public int[][] Merge(int[][] intervals) {
        Array.Sort(intervals,(a,b) => a[0].CompareTo(b[0]));
        List<int[]> output = new List<int[]>();
        output.Add(intervals[0]);
        
        int i = 0;
        foreach(var num in intervals){
            int start = num[0];
            int end = num[1];
            if (start <= output[i][1]){
                if(output[i][1] < end)
                output[i][1] = end;
            }
            else{
            output.Add(num);
            i++;
            }
            
        }
        return output.ToArray();
    }
}
