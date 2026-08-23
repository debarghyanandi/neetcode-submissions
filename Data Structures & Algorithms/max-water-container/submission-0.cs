public class Solution
{
    public int MaxArea(int[] heights)
    {
        //My Solution.
        int l = 0;
        int r = heights.Length - 1;
        int maxArea = 0;

        while (l < r)
        {
            int area = (r - l) * Math.Min(heights[l], heights[r]);
            maxArea = Math.Max(maxArea, area);

            if (heights[l] > heights[r])
                r--;
            else
                l++;
        }

        return maxArea;
    }
}