public class Solution {
    public void Merge(int[] nums1, int m, int[] nums2, int n) {
        
        int last = m + n - 1;
        int i = m - 1;
        int j = n - 1;

        while (j >= 0)
        {
            if (i >= 0 && nums1[i] > nums2[j]) 
            {
                nums1[last] = nums1[i];
                last--;
                i--;
            }
            else
            {
                nums1[last] = nums2[j];
                last--;
                j--;
            }
        }
    }
}