// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
// -  classic length-prefix delimiter: for each string append its length, a
// -  '#' delimiter, then the string itself inline; decode reads length up
// -  to '#' then slices exactly that many chars
// -  [length-prefix-inline]
// -  ranks above suboptimal.cs (O(n) time / O(n) space)
// -
// -  Reference solution - not one you solved yourself (from submission-1)
// -
// -  single linear pass over all characters for both encode and decode with
// -  only O(1) auxiliary bookkeeping (indices/length variable)
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