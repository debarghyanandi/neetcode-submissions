public class Solution {
    public int UniquePaths(int m, int n) {
        //space optimized tabulation
        int [] curr = new int [n + 1];
        int [] next = new int [n + 1];
        
        next[n - 1] = 1;
        
        for (int i = m - 1; i >= 0; i--){
            for (int j = n - 1; j >= 0; j--)
            {
                curr[j] = next[j] + curr[j + 1];
            }
            (curr, next) = (next, curr);
        }
        return next[0];
    }
}
