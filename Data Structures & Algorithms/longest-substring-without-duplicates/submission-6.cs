public class Solution
{
    public int LengthOfLongestSubstring(string s)
    {
        //My solution.
        var charCount = new Dictionary<char, int>();

        int maxSub = 0;
        int left = 0;

        for (int right = 0; right < s.Length; right++)
        {
            if (charCount.ContainsKey(s[right]) && charCount[s[right]]>=left)
            {
                left = charCount[s[right]] + 1;
            }
            
            charCount[s[right]] = right;
            maxSub = Math.Max(maxSub, right - left + 1);
            
        }

        return maxSub;
    }
}