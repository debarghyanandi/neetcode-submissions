public class Solution {
    public int[] FindMissingAndRepeatedValues(int[][] grid) {
        int N = grid.Length;
        HashSet<int> seen = new HashSet<int>();
        int doubleVal = 0, missing = 0;

        for (int i = 0; i < N; i++) {
            for (int j = 0; j < N; j++) {
                if (seen.Contains(grid[i][j])) {
                    doubleVal = grid[i][j];
                }
                seen.Add(grid[i][j]);
            }
        }

        for (int num = 1; num <= N * N; num++) {
            if (!seen.Contains(num)) {
                missing = num;
                break;
            }
        }

        return new int[] { doubleVal, missing };
    }
}