public class Solution {
    public bool SearchMatrix(int[][] matrix, int target) {
        //My solution
        //this is good but mLogn
        //we need log(m*n)
        foreach (int[] arr in matrix)
        {
            bool res = BinarySearch(0, arr.Length -1, target, arr);
            if(res == true)
            return res;
        }
        return false;
    }

    private bool BinarySearch(int l, int r, int target, int[] nums)
    {
        if (l > r)
            return false;

        int mid = l + (r - l) / 2;

        if (nums[mid] == target)
            return true;

        if (nums[mid] < target)
            return BinarySearch(mid + 1, r, target, nums);

        return BinarySearch(l, mid - 1, target, nums);
    }
}

