public class Solution {
    public int CharacterReplacement(string s, int k) {
        Dictionary<char, int> count = new Dictionary<char, int>();
        int res = 0;

        int l = 0, maxf = 0;
        for (int r = 0; r < s.Length; r++) {
            if (count.ContainsKey(s[r])) {
                count[s[r]]++;
            } else {
                count[s[r]] = 1;
            }
            maxf = Math.Max(maxf, count[s[r]]);

            while ((r - l + 1) - maxf > k) {
                count[s[l]]--;
                l++;
            }
            res = Math.Max(res, r - l + 1);
        }

        return res;
    }
}