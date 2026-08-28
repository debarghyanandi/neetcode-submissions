public class Solution
{
    public string MinWindow(string s, string t)
    {
        //My Solution
        if (s.Length < t.Length)
            return string.Empty;

        var need = new Dictionary<char, int>();
        var window = new Dictionary<char, int>();

        int left = 0;
        int right = 0;
        int minLength = int.MaxValue;

        var minIndices = new List<int> { 0, 0 };

        for (int i = 0; i < t.Length; i++)
        {
            if (need.ContainsKey(t[i]))
                need[t[i]]++;
            else
                need.Add(t[i], 1);
        }

        bool IsMatch()
        {
            return need.All(pair =>
                window.TryGetValue(pair.Key, out int count) &&
                count >= pair.Value);
        }

        while (right < s.Length)
        {
            if (window.ContainsKey(s[right]))
                window[s[right]]++;
            else
                window.Add(s[right], 1);

            while (IsMatch())
            {
                int currentLength = right - left + 1;

                if (currentLength < minLength)
                {
                    minIndices[0] = left;
                    minIndices[1] = right;
                    minLength = currentLength;
                }

                if (window[s[left]] > 1)
                    window[s[left]]--;
                else
                    window.Remove(s[left]);

                left++;
            }

            right++;
        }

        if (minLength == int.MaxValue)
            return string.Empty;

        var result = new StringBuilder();

        for (int i = minIndices[0]; i <= minIndices[1]; i++)
        {
            result.Append(s[i]);
        }

        return result.ToString();
    }
}