public class Solution
{
    public bool CheckInclusion(string s1, string s2)
    {
        //My solution
        if (s1.Length > s2.Length)
        {
            return false;
        }

        var s1Counts = new Dictionary<char, int>();
        var windowCounts = new Dictionary<char, int>();

        for (int i = 0; i < s1.Length; i++)
        {
            if (s1Counts.ContainsKey(s1[i]))
            {
                s1Counts[s1[i]]++;
            }
            else
            {
                s1Counts.Add(s1[i], 1);
            }

            if (windowCounts.ContainsKey(s2[i]))
            {
                windowCounts[s2[i]]++;
            }
            else
            {
                windowCounts.Add(s2[i], 1);
            }
        }

        int left = 0;
        int right = s1.Length - 1;

        while (right < s2.Length)
        {
            if (s1Counts.Count == windowCounts.Count &&
                s1Counts.All(pair =>
                    windowCounts.TryGetValue(pair.Key, out int value) &&
                    value == pair.Value))
            {
                return true;
            }

            if (windowCounts[s2[left]] > 1)
            {
                windowCounts[s2[left]]--;
            }
            else
            {
                windowCounts.Remove(s2[left]);
            }

            left++;
            right++;

            if (right == s2.Length)
            {
                return false;
            }

            if (windowCounts.ContainsKey(s2[right]))
            {
                windowCounts[s2[right]]++;
            }
            else
            {
                windowCounts.Add(s2[right], 1);
            }
        }

        return false;
    }
}