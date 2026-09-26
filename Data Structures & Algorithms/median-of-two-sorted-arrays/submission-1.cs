public class Solution
{
    public double FindMedianSortedArrays(int[] nums1, int[] nums2)
    {
        //Hard problem Have to Understand again
        int n1 = nums1.Length;
        int n2 = nums2.Length;

        // Always binary search the smaller array
        if (n1 > n2)
            return FindMedianSortedArrays(nums2, nums1);

        int low = 0;
        int high = n1;

        int left = (n1 + n2 + 1) / 2;
        int n = n1 + n2;

        while (low <= high)
        {
            int mid1 = (low + high) / 2;
            int mid2 = left - mid1;

            int l1 = int.MinValue;
            int l2 = int.MinValue;

            int r1 = int.MaxValue;
            int r2 = int.MaxValue;

            if (mid1 < n1)
                r1 = nums1[mid1];

            if (mid2 < n2)
                r2 = nums2[mid2];

            if (mid1 - 1 >= 0)
                l1 = nums1[mid1 - 1];

            if (mid2 - 1 >= 0)
                l2 = nums2[mid2 - 1];

            // Correct partition
            if (l1 <= r2 && l2 <= r1)
            {
                // Odd total length
                if (n % 2 == 1)
                    return Math.Max(l1, l2);

                // Even total length
                return ((double)Math.Max(l1, l2)
                        + Math.Min(r1, r2)) / 2.0;
            }

            // Took too many elements from nums1
            if (l1 > r2)
            {
                high = mid1 - 1;
            }
            else
            {
                // Took too few elements from nums1
                low = mid1 + 1;
            }
        }

        return 0;
    }
}