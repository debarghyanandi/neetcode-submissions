public class Solution {
    public string LongestCommonPrefix(string[] strs) {
        string prefix = strs[0];

        for (int i = 1; i < strs.Length; i++) {
            int j = 0;
            while (j < Math.Min(prefix.Length, strs[i].Length)) {
                if (prefix[j] != strs[i][j]) {
                    break;
                }
                j++;
            }
            prefix = prefix.Substring(0, j);
        }

        return prefix;
    }
}