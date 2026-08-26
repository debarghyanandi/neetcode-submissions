public class Solution
{
    public int LengthOfLongestSubstring(string s)
    {
        var lastSeen = new Dictionary<char, int>();
        int maxLen = 0;
        int left = 0;

        for (int right = 0; right < s.Length; right++)
        {
            char c = s[right];
            if (lastSeen.ContainsKey(c))
            {
                //The duplicate should not be outside the window.
                //if yes then left should not jump.
                left = Math.Max(left, lastSeen[c] + 1);
            }
            lastSeen[c] = right;
            maxLen = Math.Max(maxLen, right - left + 1);
        }

        return maxLen;
    }
}