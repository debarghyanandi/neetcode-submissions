public class Solution
{
    public bool SearchMatrix(int[][] matrix, int target)
    {
        // My solution
        // this is good but mLogn
        // we need log(m*n)
        foreach (int[] row in matrix)
        {
            bool found = BinarySearch(0, row.Length - 1, target, row);
            if (found == true)
                return found;
        }
        return false;
    }

    private bool BinarySearch(int left, int right, int target, int[] nums)
    {
        if (left > right)
            return false;

        int mid = left + (right - left) / 2;

        if (nums[mid] == target)
            return true;

        if (nums[mid] < target)
            return BinarySearch(mid + 1, right, target, nums);

        return BinarySearch(left, mid - 1, target, nums);
    }
}
