public class Solution {
    public int Trap(int[] height) {
        int n = height.Length;
        int rainTotal = 0;

        //prefixMax
        int [] lMax = new int[n];
        lMax[0] = height[0];
        for(int i = 1; i < n; i++)
        {
            lMax[i] = Math.Max(lMax[i - 1] , height[i]);
        }

        //suffixMax
        int [] rMax = new int[n];
        rMax[n - 1] = height[n - 1];
        for(int i = n - 2; i >= 0; i--)
        {
            rMax[i] = Math.Max(rMax[i + 1] , height[i]);
        }
        
        for(int i = 0; i < n; i++)
        {
            ///if(height[i] < lMax[i] && height[i] < rMax[i])

            // No if needed because lMax[i] and rMax[i] include height[i] itself.
            // So min(lMax[i], rMax[i]) is always >= height[i].
            // Therefore trapped water is never negative.
            rainTotal += Math.Min (lMax[i], rMax[i]) - height[i];
        
        }
        return rainTotal;
    }
}
