// --------------------------------------------------------------------------
// -  suboptimal.cs         O(n) time / O(n) space
// --------------------------------------------------------------------------

public class Solution
{
    public int Trap(int[] height)
    {
        int n = height.Length;
        int rainTotal = 0;

        //prefixMax
        int[] lMax = new int[n];
        lMax[0] = height[0];
        for (int i = 1; i < n; i++)
        {
            lMax[i] = Math.Max(lMax[i - 1], height[i]);
        }

        //suffixMax
        int[] rMax = new int[n];
        rMax[n - 1] = height[n - 1];
        for (int i = n - 2; i >= 0; i--)
        {
            rMax[i] = Math.Max(rMax[i + 1], height[i]);
        }

        for (int i = 0; i < n; i++)
        {
            ///if(height[i] < lMax[i] && height[i] < rMax[i])

            // No if needed because lMax[i] and rMax[i] include height[i] itself.
            // So min(lMax[i], rMax[i]) is always >= height[i].
            // Therefore trapped water is never negative.
            rainTotal += Math.Min(lMax[i], rMax[i]) - height[i];

        }
        return rainTotal;
    }
}

/*
================================================================================
 PROBLEM : You get an array height of non-negative bar heights. Each bar is 1
           wide. Return the total units of rain water trapped between the
           bars. Example: [0,1,0,2,1,0,1,3,2,1,2,1] -> 6.
 PATTERN : Prefix max + suffix max arrays
================================================================================
IDEA
  Water above bar i is set by the lower of the two tallest walls around it.
  lMax[i] is the tallest bar in 0..i, and rMax[i] is the tallest in i..n-1.
  Add min(lMax[i], rMax[i]) - height[i] for every i into rainTotal.
  This is correct because water at i rises until it spills over the lower
  side. Unlike optimal.cs, both maxima are stored in arrays, not two pointers.
EXAMPLE
  height = [3,0,2,0,4]
  lMax = [3,3,3,3,4], rMax = [4,4,4,4,4], min = [3,3,3,3,4]
  water per i = [0,3,1,3,0] -> rainTotal = 7
COMPLEXITY
  Time  O(n)  three separate single passes over the array
  Space O(n)  the two extra arrays lMax and rMax of size n
WATCH OUT
  - Empty input crashes: lMax[0] = height[0] throws when n == 0.
    Add "if (n == 0) return 0;" at the top.
  - If lMax/rMax exclude height[i], the value can go negative.
    Then you need the commented-out if, or Math.Max(0, ...).
  - Use Math.Min of the two walls, not Math.Max. The lower wall decides.
  - Fill rMax from the right, starting at rMax[n-1], and go down to i = 0.
================================================================================
*/
