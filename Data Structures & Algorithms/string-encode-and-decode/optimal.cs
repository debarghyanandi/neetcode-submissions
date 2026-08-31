// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
// -  per-string length-prefix encoding (len#str repeated)
// -  [length-prefix-per-string]
// -  ranks above suboptimal.cs (O(n) time / O(n) space)
// -
// -  Reference solution - not one you solved yourself (from submission-3)
// -
// -  encode appends length+'#'+string for each element and decode reads a
// -  length, then immediately slices out that many characters and advances,
// -  needing no auxiliary storage beyond the output list itself; all
// -  operations are linear in total character count.
// --------------------------------------------------------------------------

public class Solution {
    public string Encode(IList<string> strs) {
        StringBuilder res = new StringBuilder();
        foreach (string s in strs) {
            res.Append(s.Length).Append('#').Append(s);
        }
        return res.ToString();
    }

    public List<string> Decode(string s) {
        List<string> res = new List<string>();
        int i = 0;
        while (i < s.Length) {
            int j = i;
            while (s[j] != '#') {
                j++;
            }
            int length = int.Parse(s.Substring(i, j - i));
            i = j + 1;
            j = i + length;
            res.Add(s.Substring(i, length));
            i = j;
        }
        return res;
    }
}