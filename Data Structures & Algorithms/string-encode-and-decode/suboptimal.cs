// --------------------------------------------------------------------------
// -  suboptimal.cs         O(n + m) time / O(m) space
// --------------------------------------------------------------------------

public class Solution
{

    public string Encode(IList<string> strs)
    {
        if (strs.Count == 0)
            return "";
        List<int> sizes = new List<int>();
        StringBuilder res = new StringBuilder();
        foreach (string s in strs)
        {
            sizes.Add(s.Length);
        }
        foreach (int sz in sizes)
        {
            res.Append(sz).Append(',');
        }
        res.Append('#');
        foreach (string s in strs)
        {
            res.Append(s);
        }
        return res.ToString();
    }

    public List<string> Decode(string s)
    {
        if (s.Length == 0)
        {
            return new List<string>();
        }
        List<int> sizes = new List<int>();
        List<string> res = new List<string>();
        int i = 0;
        while (s[i] != '#')
        {
            int j = i;
            while (s[j] != ',')
            {
                j++;
            }
            sizes.Add(int.Parse(s.Substring(i, j - i)));
            i = j + 1;
        }
        i++;
        foreach (int sz in sizes)
        {
            res.Add(s.Substring(i, sz));
            i += sz;
        }
        return res;
    }
}

/*
================================================================================
 PROBLEM : Design Encode, which turns a list of strings into one string, and
           Decode, which turns that string back into the same list. Strings
           may hold any character, including '#' and ','. Example:
           ["neet","code"] -> one string -> ["neet","code"].
 PATTERN : Length-prefix header (all sizes first, then payload)
================================================================================
IDEA
  Encode writes every length as "len," into res, then one '#', then all the
  strings glued together. Decode reads numbers up to each ',' into sizes
  until it sees '#', skips it, then cuts s.Substring(i, sz) for each sz.
  The header holds only digits and commas, so its first '#' is the real end.
  After that, lengths alone decide the cuts, so payload characters never
  matter. Unlike optimal.cs, it stores all lengths in a sizes list first.
EXAMPLE
  ["ab", "", "#,"] -> Encode gives "2,0,2,#ab#,"
  Header: parse 2 (i=2), 0 (i=4), 2 (i=6); s[6]='#', so i=7
  Cuts: Substring(7,2)="ab", i=9; Substring(9,0)="", i=9; Substring(9,2)="#,"
  Result: ["ab", "", "#,"]
COMPLEXITY
  Time  O(n + m)  each header and payload char is scanned or copied once
  Space O(m)      sizes list keeps one int per string
WATCH OUT
  - Read the length up to the ',' and never as one char. A string of length
    12 must parse as 12, not as 1 and 2.
  - Forget the i++ after the header loop and every cut starts on '#'.
  - The inner while (s[j] != ',') has no bounds check. A malformed input
    with no ',' or '#' throws IndexOutOfRangeException.
  - The empty list [] encodes to "" and [""] encodes to "0,#". Keep these
    two cases apart, or [""] decodes to [].
================================================================================
*/
