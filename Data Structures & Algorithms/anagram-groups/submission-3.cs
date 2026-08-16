public class Solution
{
    public List<List<string>> GroupAnagrams(string[] strs)
    {
        // Key = character frequency pattern
        // Value = all strings having that same pattern
        var res = new Dictionary<string, List<string>>();

        foreach (var s in strs)
        {
            // Store the frequency of each character:
            // count[0] = number of 'a'
            // count[1] = number of 'b'
            // ...
            // count[25] = number of 'z'
            int[] count = new int[26];

            foreach (char c in s)
            {
                // Convert character to an index from 0 to 25
                // 'a' - 'a' = 0
                // 'b' - 'a' = 1
                // ...
                count[c - 'a']++;
            }

            // Arrays cannot directly be used as useful content-based
            // dictionary keys, so convert the frequency array into
            // a string. Anagrams will produce the same key.
            var keyBuilder = new StringBuilder();

            foreach (int c in count)
            {
                keyBuilder.Append(c);
                keyBuilder.Append(',');
            }

            string key = keyBuilder.ToString();

            // If this frequency pattern doesn't exist yet,
            // create a new group for it.
            if (!res.ContainsKey(key))
            {
                res[key] = new List<string>();
            }

            // Add the current string to its anagram group.
            res[key].Add(s);
        }

        // Dictionary values contain all the anagram groups.
        return res.Values.ToList();
    }
}