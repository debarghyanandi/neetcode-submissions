// --------------------------------------------------------------------------
// -  suboptimal.cs         O(n + m) time / O(m) space
// -  Two-phase encoding with collected metadata   [two-phase-encoding]
// -  ranks below optimal.cs (O(n + m) time / O(1) space)
// -
// -  Reference solution - not one you solved yourself
// -
// -  Collects all sizes into auxiliary list before encoding, requiring
// -  extra space and passes.
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
 PATTERN : Length-prefixed serialization - size header then payload
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-5.cs when it was first processed
 STATUS  : Suboptimal
================================================================================
VARIABLES
  sizes    in Encode: one entry per input string, sizes[k] = strs[k].Length; in Decode: the same list rebuilt from the header
  res      in Encode: the growing encoded string; in Decode: the decoded list being filled
  i        cursor into s - first scans the header, then walks the payload
  j        end index of the current number in the header, stops on the ','
WHY THIS PATTERN
  The strings may contain any character, so no separator alone can be trusted -
  a '#' or ',' inside a string would be read as a boundary. Recording each
  length up front removes the need to search the payload at all: Decode reads
  sizes, then cuts the payload into blocks of exactly those lengths with
  s.Substring(i, sz). The single '#' is only needed to mark where the header
  ends, and it is safe there because the header holds nothing but digits and
  commas.
BETTER APPROACH
  The better version interleaves: for each string append its length, then '#',
  then the string itself, so the encoding is "5#hello3#abc". Decode keeps one
  cursor, scans digits to the next '#', parses the length, takes that many
  characters, and repeats. That needs no sizes list and no second pass over
  strs, so the extra memory beyond the output disappears; this file pays for a
  List<int> of m lengths in both directions and walks strs twice in Encode.
INVARIANT
  In Decode's second loop, i always points at the first character of the next
  string that has not been decoded yet, and sizes still describes the remaining
  blocks in order. Each step takes exactly sz characters and advances i by sz,
  so no character is read twice and none is skipped. Because Encode wrote the
  lengths in the same order it wrote the strings, the k-th block consumed is
  exactly strs[k].
WHY THE PAYLOAD NEEDS NO ESCAPING
  Nothing in the payload is ever inspected. The delimiters ',' and '#' are
  searched for only while i is inside the header, and the loop stops the moment
  s[i] == '#'. So a string that is itself "3#abc" or ",,,," round-trips
  unchanged, which is the whole point of this problem.
WATCH OUT
  The two empty guards are a matched pair: Encode returns "" for an empty list
  and Decode returns an empty list for "". Remove only one and the pair breaks -
  without the Encode guard an empty list would encode to "#", which Decode still
  handles, but "" would then never be produced. Decode trusts its input
  completely: while (s[i] != '#') and while (s[j] != ',') both run past the end
  and throw IndexOutOfRangeException on any malformed string, and Substring(i,
  sz) throws if a length is larger than the bytes left. Also note strs is
  IList<string>, so a null element inside it makes s.Length throw a
  NullReferenceException in the first foreach.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How would you cut the memory this uses beyond the output?
     Drop the sizes list and write length + '#' + string per item, decoding with
     a single cursor. The header then costs nothing extra, at the price of
     losing the ability to read all the lengths before touching the payload.
  2. Does this still work for non-ASCII text or emoji?
     Yes, because s.Length and Substring both count UTF-16 code units, so a
     surrogate pair is counted as 2 and cut back out as 2. It would break only
     if you encoded with char counts and decoded with byte counts.
  3. The input is far too large to hold one encoded string in memory. What
  changes?
     Stream it - write each record to a Stream or TextWriter as it is produced
     and read it back record by record. That forces the interleaved layout,
     since the header-first form needs all lengths before any payload can be
     written.
  4. Could you return the decoded parts without copying the characters?
     Return ReadOnlyMemory<char> slices over the original s instead of new
     strings from Substring. That removes m allocations but keeps the whole
     encoded string alive as long as any slice is held.
TRIGGER
  Reach for a length prefix whenever you must pack variable-length data that can
  contain any character, so no delimiter is safe.
C# NOTE
  res.Append(sz) uses the StringBuilder int overload, which formats the number
  straight into the buffer instead of allocating sz.ToString() first. On the
  decode side, int.Parse(s.Substring(i, j - i)) does allocate a throwaway string
  per length - int.Parse(s.AsSpan(i, j - i)) parses the same characters in
  place.
COMPLEXITY
  Time  : O(n + m)
  Space : O(m)
================================================================================
*/
