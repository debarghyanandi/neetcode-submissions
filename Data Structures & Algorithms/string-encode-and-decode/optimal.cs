// --------------------------------------------------------------------------
// -  optimal.cs            O(n + m) time / O(1) space
// -  Length-prefix inline encoding   [length-prefix-inline]
// -  ranks above suboptimal.cs (O(n + m) time / O(m) space)
// -
// -  Reference solution - not one you solved yourself
// -
// -  Single pass encodes length and content together; decode parses
// -  on-demand without auxiliary storage.
// --------------------------------------------------------------------------

public class Solution
{
    public string Encode(IList<string> strs)
    {
        StringBuilder res = new StringBuilder();
        foreach (string s in strs)
        {
            res.Append(s.Length).Append('#').Append(s);
        }
        return res.ToString();
    }

    public List<string> Decode(string s)
    {
        List<string> res = new List<string>();
        int i = 0;
        while (i < s.Length)
        {
            int j = i;
            while (s[j] != '#')
            {
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

/*
================================================================================
 PATTERN : String Encoding - length prefix plus delimiter
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-6.cs when it was first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  res      Encode: the growing encoded text. Decode: the list of decoded strings
  i        index where the current "length#" header starts
  j        first scans to the '#' after i, then marks the end of the current string
  length   the number of characters in the current string, read from the header
WHY THIS PATTERN
  The problem asks you to pack any list of strings into one string and unpack it
  later. The strings can hold any character, including '#'. That means no single
  delimiter (a separator character) is safe by itself. If you write s.Length
  before each string, the decoder knows exactly how many characters to take. So
  it never has to guess where a string ends.
BRUTE FORCE
  The first correct idea most people write is escaping. You double every '#'
  inside a string, then join the strings with a separator such as "#,". Decoding
  it is also linear, but you must look at every character and handle the escape
  rules. That makes it easy to get wrong. The encoded text also grows when the
  input has many '#'. The length prefix never needs to look inside the strings,
  so it is simpler and harder to break.
INVARIANT
  At the top of the outer while loop, i always points at the first digit of a
  header "length#". A header holds only digits, so the first '#' that j finds is
  always the end of the header, even if the string content has '#' in it. The
  code then takes exactly length characters and sets i to the next header. Each
  step reads exactly one record that Encode wrote, so the output list matches
  the input list.
WATCH OUT
  Decode trusts its input. If there is no '#', the inner loop s[j] runs past the
  end and throws IndexOutOfRangeException. If a header says more characters than
  are left, Substring throws ArgumentOutOfRangeException. Encode throws
  NullReferenceException if strs is null or holds a null string, because it
  calls s.Length. The lines "j = i + length; ... i = j;" are only a long way to
  write i += length. The file also has no "using System.Text;", so StringBuilder
  compiles only because the judge adds that using for you.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How can you remove the '#' and the scan for it?
     Write a fixed-width header, for example 4 characters or 4 bytes for each
     length. The decoder reads the header in one step and never scans for a
     delimiter. The trade-off is that the width limits the largest length, and
     short strings waste header space.
  2. What changes if the encoded text is sent over a network as bytes?
     s.Length counts UTF-16 code units, not bytes. You must encode each string
     to bytes first, for example UTF-8, and write the byte count. Otherwise the
     decoder cuts through the middle of a multi-byte character.
  3. What if the encoded text is too big to decode into one list?
     Make Decode an iterator with yield return, so it gives back one string at a
     time. Memory then holds only the current string, but the caller can read
     the results only once, in order.
TRIGGER
  When you must turn a list of items that can contain any character into one
  flat string or stream and get it back exactly, put each item's length in front
  of it.
C# NOTE
  s.Substring(i, j - i) creates a new string just so int.Parse can read the
  header. You can use int.Parse(s.AsSpan(i, j - i)) to read the digits in place,
  without that extra string.
COMPLEXITY
  Time  : O(n + m)
  Space : O(1)
================================================================================
*/
