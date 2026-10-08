public class Solution {
    public bool ValidPalindrome(string s) {
      
        bool IsPalindrome(int l, int r) {
            while (l < r) 
            {
                if (s[l] != s[r])
                return false;
                l++;
                r--;
            }
            return true;
        }
      
      
        int l = 0;
        int r = s.Length - 1;

        while(l < r)
        {
            if(s[l] != s[r])
            {
                return IsPalindrome(l + 1, r) || IsPalindrome(l, r - 1);
            }
            l++;
            r--;
        }
        return true;
    }
}