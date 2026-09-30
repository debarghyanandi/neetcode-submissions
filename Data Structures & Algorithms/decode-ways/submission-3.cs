public class Solution {
    public int NumDecodings(string s)
    {
        // Tabulation Space Optimized
        int n = s.Length;
        int next = 1;
        int next2 = 1;
       
        for(int i = n-1; i >= 0; i--)
        {
            int curr = 0;

            if (s[i] != '0')
            {
                //pick 1
                curr = next;
            
                //pick 2
                if(i < n - 1)
                {
                    if(s[i] == '1' || (s[i] == '2' && s[i + 1] < '7' ))
                    {
                        curr += next2;
                    }
                }
            } 
            
            next2 = next;
            next = curr;
        }
        
        return next;
    }
}
