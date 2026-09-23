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
 PATTERN : Length-prefixed encoding - "len#payload" framing
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-6.cs when it was first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  res      Encode: the growing buffer; Decode: the decoded list
  j        scan pointer - first finds the '#', then the end of the payload
  length   the integer parsed from the digits before '#'
WHY THIS PATTERN
  The strings may contain any characters, so no separator character is safe on
  its own - a plain join on '#' breaks the moment a string contains '#'. Writing
  the count of characters first removes the ambiguity: after reading length and
  the single '#', the decoder knows exactly how many characters to take,
  whatever they are. The '#' here is not a separator between strings, it only
  marks where the digits of length stop.
BRUTE FORCE
  The first idea is usually to join with some "unlikely" delimiter and Split on
  it, which is O(n) but simply wrong for any input containing that delimiter.
  The next fix is escaping: double every '#' in the payload and use a single '#'
  as the boundary. That is also O(n) but needs a character-by-character pass
  with state on both sides and is much easier to get wrong than counting.
INVARIANT
  At the top of the while loop, i always points at the first digit of the next
  record, and everything before i has been fully decoded into res. Inside the
  body, j walks to the '#', so s[i..j) is exactly the digit run, then i = j + 1
  puts i on the first payload character and j = i + length on the character
  after it. Since length was written by Encode as s.Length, the slice taken is
  byte-for-byte the original string, so the loop re-establishes the invariant
  and ends exactly at s.Length.
WATCH OUT
  The inner while (s[j] != '#') has no bound check - on any malformed or
  truncated input it runs past the end and throws IndexOutOfRangeException
  instead of a clear error. Same for s.Substring(i, length) if the stored length
  is longer than what remains. int.Parse will throw on a non-digit run rather
  than return a failure, so Decode trusts its input completely; this is fine
  when only Encode produces it, but not for anything coming off a network. An
  empty string encodes as "0#" and decodes correctly, since length 0 makes
  Substring return "" and i lands on the next record.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How would you avoid allocating a new string for every field in Decode?
     Scan with ReadOnlySpan<char>: parse the digits with int.Parse(s.AsSpan(i, j
     - i)) and build the payload once. It removes the temporary digit string,
     though the payload strings themselves must still be materialised because
     the return type is List<string>.
  2. What changes if the encoded form must be bytes on a wire, not a char
  string?
     Write a fixed 4-byte big-endian length instead of decimal digits plus '#'.
     Decoding then needs no scan for a delimiter at all - read 4 bytes, read
     that many bytes - at the cost of 4 bytes of overhead even for tiny strings.
  3. What if the whole encoded payload does not fit in memory?
     Turn Decode into an iterator (yield return) over a Stream, reading one
     length header and one payload at a time. Callers process strings as they
     arrive, but they lose random access and cannot go back to an earlier
     record.
  4. Could you drop the '#' entirely?
     Only with a fixed-width length field, for example always 10 digits
     zero-padded. The '#' exists purely because a decimal length has variable
     digit count and would otherwise run into a payload that begins with a
     digit.
TRIGGER
  When you must serialise a list of arbitrary strings into one string and no
  character can be reserved as a separator, prefix each item with its length.
C# NOTE
  res.Append(s.Length) uses the int overload of StringBuilder.Append, which
  writes the digits straight into the buffer - no s.Length.ToString()
  intermediate string is created; Append(string.Concat(...)) or string += in a
  loop would be the costly version here.
COMPLEXITY
  Time  : O(n + m)
  Space : O(1)
================================================================================
*/
