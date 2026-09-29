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
 PATTERN : String Encoding - length header, then the raw payload
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-5.cs when it was first processed
 STATUS  : Suboptimal
================================================================================
VARIABLES
  sizes    sizes[k] = length of the k-th string (built in both Encode and Decode)
  res      in Encode, the output being built; in Decode, the list of decoded strings
  i        in Decode, the read position in s (first in the header, then in the body)
  j        in Decode, scans forward from i to the next ',' to find one size number
  sz       the length of the current string, used to cut the next piece from the body
WHY THIS PATTERN
  The strings can hold any character, so no single separator is safe. The
  encoder cannot pick a delimiter that never shows up inside the data. This code
  writes all the lengths first as "len,len,...,#". Then it writes all the
  strings joined with nothing between them. The header has only digits and
  commas, so the first '#' always ends it. After that, Decode cuts the body
  using the numbers in sizes and never looks at the string content.
BETTER APPROACH
  The better way puts each length right before its own string, as
  "5#hello3#abc". This takes one pass to encode and one pass to decode. It needs
  no sizes list on either side. This file stores every length in sizes before it
  writes anything. It then loops over the data twice in Encode and twice in
  Decode. The big-O is the same. The loss is the extra list of lengths and the
  extra loops.
INVARIANT
  In the header loop, i always points to the first digit of a size number, or to
  the '#'. At the '#' the header is done. In the body loop, i always points to
  the first character of the next string. Adding sz moves i to the exact start
  of the string after it. Every string in the body has a matching entry in
  sizes, in the same order. So each Substring(i, sz) returns exactly one
  original string, even when that string contains ',' or '#'.
WATCH OUT
  Decode trusts its input completely. If there is no '#', or a size is larger
  than what is left, s[j] or Substring throws an exception. It does not return
  an error. The check strs.Count == 0 in Encode is not needed: without it,
  Encode would return "#", and Decode already turns "#" into an empty list. An
  input of [""] encodes to "0,#", not "", so it stays different from an empty
  list. Do not "simplify" the code in a way that returns "" for this case.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you decode the data as a stream, without reading the whole header
  first?
     Yes. Put each length in front of its own string ("len#str"). The reader can
     then return each string as soon as it arrives. The cost is that you lose
     the separate header, which is useful if you want to know the count up
     front.
  2. How do you remove the need to parse numbers?
     Write each length as a fixed 4-byte integer, or as a fixed-width field such
     as 8 digits. Decoding is then simple offset math with no need to look for
     ',' or '#'. The trade-off is a few wasted bytes for short strings and a
     hard limit on the maximum length.
  3. What if the interviewer asks for a delimiter approach instead of lengths?
     Use escaping. For example, write every '#' in the data as "##" and put " #
     " between strings. It works, but both sides must scan every character, and
     the escape rules are easy to get wrong.
TRIGGER
  When you must pack a list of strings that can contain any character into one
  string and get the exact list back, use a length prefix, not a delimiter.
C# NOTE
  int.Parse(s.Substring(i, j - i)) creates a throwaway string for each size.
  Calling int.Parse(s.AsSpan(i, j - i)) reads the digits in place without that
  extra allocation.
COMPLEXITY
  Time  : O(n + m)
  Space : O(m)
================================================================================
*/
