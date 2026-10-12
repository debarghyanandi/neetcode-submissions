public class Solution {
    public int LargestRectangleArea(int[] heights) {
        
        Stack <int> stack = new Stack <int> ();

        int maxArea = 0;

        int n = heights.Length;

        for (int i = 0; i < n ; i++)
        {
            while (stack.Count > 0 && heights[stack.Peek()] > heights[i]) 
            {
                int element = heights[stack.Pop()];
                
                int nse = i;
                int pse = stack.Count == 0 ? -1 : stack.Peek();

                maxArea = Math.Max(maxArea, element * (nse - pse - 1));
                
            }
            stack.Push(i);

        }

        while (stack.Count > 0)
        {
            int nse = n;
            int currelement = heights[stack.Peek()];
            stack.Pop();
            int pse = stack.Count == 0 ? -1 : stack.Peek();
            maxArea = Math.Max(maxArea, currelement * (nse - pse - 1));

        }
    return maxArea;
    }
}
