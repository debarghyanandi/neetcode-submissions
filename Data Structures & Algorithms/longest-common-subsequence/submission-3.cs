public class Solution {
    public int LongestCommonSubsequence(string text1, string text2) {
        //My solution
        //Tabulation one right shift
        int n = text1.Length;
        int m = text2.Length;
        
        //int[,] dp = new int[n+1, m+1];
        int[] prev = new int[m+1];
        int[] curr = new int[m+1];
        
        //base cases
        for(int j = 0; j < m+1 ; j++) prev [j] = 0;//optional as array already have 0

        for (int i = 1; i <= n; i++){
            for (int j = 1; j <= m; j++)
            {
                if(text1[i-1] == text2[j-1])
                    curr[j] = 1 + prev[j-1];
                else 
                    curr[j] = Math.Max(prev[j], curr[j-1]);
            }
            //I know swapping is not necessary
            //we only want prev to become curr
            //but also we need to preserve the array of curr
            //for next iteration.
            (prev, curr) = (curr, prev);
        }
        //as we are swapping curr and prev at end of loop
        //The Current value is in Prev
        return prev[m];
    }

}
