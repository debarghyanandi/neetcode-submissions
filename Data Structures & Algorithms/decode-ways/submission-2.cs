public class Solution {
    public int NumDecodings(string s)
    {
        // Tabulation
        int n = s.Length;
        int [] dp = new int [n + 1];
        
        dp [n] = 1;

        for(int i = n-1; i >= 0; i--)
        {
            //Pick 1
            if (s[i] == '0') 
            { 
                dp[i] = 0;
                continue;
            }
            
            dp[i] = dp[i + 1];
        
            //pick 2
            if(i < n - 1)
            {
                if(s[i] == '1' || (s[i] == '2' && s[i + 1] < '7' ))
                {
                    dp[i] += dp[i + 2];
                }
            }
        }
        return dp[0];
    }
}
