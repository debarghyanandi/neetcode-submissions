public class Solution {
    public string LongestCommonPrefix(string[] strs) {
        string ans = "";
        bool isMatch = true;

        for (int i = 0; i < strs[0].Length; i++)
        {
            char prefix = strs[0][i];
            
            for (int j = 1; j < strs.Length; j++){

                //Not Match
                if (strs[j].Length <= i || prefix != strs[j][i])
                {
                    isMatch = false;
                    break;
                }
            }
            
            if (isMatch == false)
                break;
            else 
                ans += prefix;
        }
        return ans;
    }
}