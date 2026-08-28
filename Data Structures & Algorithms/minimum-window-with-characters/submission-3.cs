public class Solution
{
    public string MinWindow(string s, string t)
    {
        if (s.Length < t.Length)
            return string.Empty;

        var need = new Dictionary<char, int>();
        foreach(char c in t){
            need[c] = need.GetValueOrDefault(c) + 1;
        }

        var window = new Dictionary<char, int>();
        int have = 0, required = need.Count;
        int left = 0;
        int minLength = int.MaxValue;
        int resultStart = 0;

        for (int right = 0; right < s.Length; right++)
        {
            char c = s[right];
            
            window[c] = window.GetValueOrDefault(c) + 1;
            if (need.ContainsKey(c) && window[c] ==  need[c])
                have++;
            
            while (have == required)
            {
                if (right - left + 1 < minLength)
                {
                    minLength = right - left + 1;
                    resultStart = left;
                }
                
                char lc = s[left];
                window[lc]--;
                
                if (need.ContainsKey(lc) && window[lc] < need[lc])
                    have--;
                left++;    
            }

        }

        return minLength == int.MaxValue ? string.Empty : s.Substring(resultStart, minLength);
    }
}