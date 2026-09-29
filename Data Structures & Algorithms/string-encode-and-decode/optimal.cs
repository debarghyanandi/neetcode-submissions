// --------------------------------------------------------------------------
// -  optimal.cs            O(n + m) time / O(1) space
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
 PROBLEM : Design Encode(list of strings) -> one string, and Decode(string) ->
           the original list. Strings may contain any character, including
           '#', and may be empty. Decode(Encode(x)) must equal x exactly.
           Example: ["ab#", "", "c"] -> "3#ab#0#1#c" -> ["ab#", "", "c"].
 PATTERN : Length-prefix encoding (length + delimiter + payload)
================================================================================
IDEA
  Encode writes each string as its length, then '#', then the raw string.
  Decode reads digits from i until j hits '#', parses length, then copies
  exactly length chars starting at j + 1. It jumps i past them.
  It is correct because the length tells Decode where the payload ends.
  So a '#' inside a string is never read as a separator.
EXAMPLE
  Input ["ab#", "", "c"] -> Encode gives "3#ab#0#1#c".
  i=0: '#' at j=1, length=3, take "ab#", i=5.
  i=5: '#' at j=6, length=0, take "", i=7.
  i=7: '#' at j=8, length=1, take "c", i=10 = end -> ["ab#", "", "c"].
COMPLEXITY
  Time  O(n + m)  each char is scanned or copied a constant number of times
  Space O(1)      only i, j, length besides the output
PATH TO OPTIMAL
  Join with a delimiter - O(n+m) - fails when a string has the delimiter.
  Escape delimiters - O(n+m) - works, but the escape logic is error-prone.
  Header of all lengths, then data (suboptimal.cs) - O(m) extra space.
  Inline length prefix per string (this file) - O(1) extra, one pass.
KEYWORDS
  serialization, length prefix, delimiter, string parsing, encode decode
WATCH OUT
  - Search for '#' only in the length part. Never split on '#' globally.
    "ab#" would break into two strings.
  - Empty strings must survive: "0#" gives length 0 and Substring(i, 0).
  - Parse the full number: length 10 is "10#", not one digit.
  - Bad input with no '#' makes s[j] throw IndexOutOfRange.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Why not escape special characters instead?
     -> It works, but encode and decode both get harder. Mistakes with an
        escaped escape char are easy. Length prefix needs no escaping.
  2. Can you avoid variable-length number parsing?
     -> Write each length as a fixed 4-byte binary header. Time is still
        O(n+m) and parsing is simpler, but output is not human-readable.
  3. How do you decode from a stream, chunk by chunk?
     -> Keep a small state: reading length or reading payload, plus remaining
        count. Emit a string when remaining hits 0. Memory is O(one string).
  4. Does empty list vs [""] work?
     -> Yes. [] encodes to "" and decodes to []. [""] encodes to "0#".
TRIGGER
  When you must pack items with arbitrary content into one stream and split
  them back safely, prefix each item with its length.
================================================================================
*/
