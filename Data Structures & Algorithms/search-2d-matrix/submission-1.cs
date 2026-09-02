public class Solution {
    public bool SearchMatrix(int[][] matrix, int target) {
        int ROWS = matrix.Length, COLS = matrix[0].Length;

        int l = 0, r = ROWS * COLS -1;
        while(l <= r){
            int mid = l + (r-l)/2 ;
            int row = mid / COLS; //convert virtual index back to row
            int col = mid % COLS; //convert virtual index back to col

            if(target > matrix[row][col]){
                l = mid + 1;
            }
            else if (target < matrix[row][col]){
                r = mid - 1;
            }
            else return true;
        }
        return false;
    }
}

