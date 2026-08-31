// --------------------------------------------------------------------------
// -  suboptimal.cs         O(n) time / O(n) space
// -  encode all string lengths as a comma-separated header block terminated
// -  by '#', then concatenate raw string data; decode by parsing the header
// -  list then slicing by size
// -  [length-header-block]
// -  ranks below optimal.cs (O(n) time / O(1) space)
// -
// -  Reference solution - not one you solved yourself (from submission-0)
// -
// -  single pass to build sizes list and header, single pass to concatenate
// -  data, decode does two linear passes; auxiliary sizes list scales with
// -  number of strings
// --------------------------------------------------------------------------

public class Solution {

    public string Encode(IList<string> strs) {
        if (strs.Count == 0) return "";
        List<int> sizes = new List<int>();
        StringBuilder res = new StringBuilder();
        foreach (string s in strs) {
            sizes.Add(s.Length);
        }
        foreach (int sz in sizes) {
            res.Append(sz).Append(',');
        }
        res.Append('#');
        foreach (string s in strs) {
            res.Append(s);
        }
        return res.ToString();
    }

    public List<string> Decode(string s) {
        if (s.Length == 0) {
            return new List<string>();
        }
        List<int> sizes = new List<int>();
        List<string> res = new List<string>();
        int i = 0;
        while (s[i] != '#') {
            int j = i;
            while (s[j] != ',') {
                j++;
            }
            sizes.Add(int.Parse(s.Substring(i, j - i)));
            i = j + 1;
        }
        i++;
        foreach (int sz in sizes) {
            res.Add(s.Substring(i, sz));
            i += sz;
        }
        return res;
    }
}