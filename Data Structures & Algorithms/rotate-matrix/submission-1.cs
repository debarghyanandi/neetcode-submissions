public class Solution {
    public void Rotate(int[][] matrix) {
    
        int n = matrix.Length;
        
        //transpose
        for (int i = 0; i < n; i++) {
            for (int j = 0; j < n; j++) {
                if (j >= i)
                    continue;
                int temp = matrix[i][j];
                matrix [i][j] = matrix [j][i];
                matrix[j][i] = temp;
            }
        }

        //Refactor use ---->for (int j = i + 1; j < n; j++)
        // then we dont need if (j >= i)

        foreach (int [] arr in matrix) {
            Array.Reverse(arr);
        }

    }
}
