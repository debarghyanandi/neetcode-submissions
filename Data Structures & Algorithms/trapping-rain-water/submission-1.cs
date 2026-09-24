public class Solution {
    public int Trap(int[] height) {
        int l = 0;
        int r = height.Length - 1;
        
        int lMax = 0;
        int rMax = 0;
        int rainTotal = 0;
        
        while(l < r){
            
            if( height[l] <= height[r] )
            {
                if(height[l] < lMax)
                    rainTotal += (lMax - height[l]);
                else
                    lMax = height[l];

                l++;
            }
            else
            {
                if(height[r] < rMax)
                    rainTotal += (rMax - height[r]);
                else
                    rMax = height[r];

                r--;
            }
         
        }
        return rainTotal;
    }
}
