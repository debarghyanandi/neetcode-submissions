public class Solution {
    public int NumDecodings(string s)
    {
        // Memoization
        int [] dp = new int [s.Length + 1];
        Array.Fill(dp, -1);
        return  Dfs(0, s, dp); 
    }

    private int Dfs(int i, string s, int [] dp)
    {
        if(i == s.Length) return 1;
        if(s[i]  == '0') return 0;

        if(dp[i] != -1) return dp[i];

        //pick
        int res = Dfs(i + 1, s, dp);
        
        //pick 2
        if(i < s.Length - 1){
            if(s[i] == '1' || (s[i] == '2' && s[i + 1] < '7' )){
                res += Dfs(i + 2, s, dp);
            }
        }

        dp[i] = res;
        return res;
    }
}
