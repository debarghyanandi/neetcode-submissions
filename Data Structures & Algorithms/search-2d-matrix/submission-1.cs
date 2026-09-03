public class Solution {
    public bool SearchMatrix(int[][] matrix, int target)
    {
        int rows = matrix.Length, cols = matrix[0].Length;

        int left = 0, right = rows * cols - 1;
        while (left <= right)
        {
            int mid = left + (right - left) / 2;
            int row = mid / cols; // convert virtual index back to row
            int col = mid % cols; // convert virtual index back to col

            if (target > matrix[row][col])
            {
                left = mid + 1;
            }
            else if (target < matrix[row][col])
            {
                right = mid - 1;
            }
            else return true;
        }
        return false;
    }
}
