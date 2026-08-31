// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
// -  length-prefix encoding (interleaved length#string)
// -  [length-prefix-encoding]
// -  ranks above suboptimal.cs (O(n) time / O(n) space)
// -
// -  Reference solution - not one you solved yourself (from submission-6)
// -
// -  Streams each string's length and content directly into the
// -  output/parses directly from input with only constant extra scalars, no
// -  intermediate collections.
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

/*
================================================================================
 PATTERN : Length-Prefixed Framing - count, '#', then raw payload
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-6.cs when it was first processed
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  The whole problem is that the alphabet of the payload is unrestricted: any
  separator you pick can also appear inside a string, so a pure delimiter scheme
  (join on '#', split on '#') is ambiguous. Encode sidesteps ambiguity instead
  of trying to escape it - it writes s.Length, then '#', then s, so the decoder
  is told in advance exactly how many characters to copy and never has to
  interpret them. The '#' is not a separator between strings; it is a terminator
  for the length header only.
THE CORRECTNESS ARGUMENT
  Two facts do all the work.

  1. When the outer while body starts, i always points at the first digit of a
  length header. Decimal digits never contain '#', so the first '#' the inner
  scan finds at or after i is guaranteed to be that header's terminator - not a
  '#' living inside somebody's payload.

  2. After parsing, the payload is copied by count, not by content:
  s.Substring(i, length). The decoder never looks at those characters, so '#'
  inside a string is skipped over, and i = j lands on the first digit of the
  next header, restoring fact 1.

  That is the induction. Strings containing '#', digits, "3#", or the entire
  encoding of another list all decode correctly.
THE POINTER DANCE
  i and j are reused for two different jobs in one iteration, which is what
  makes this easy to misread weeks later:

  - Entry: i = start of record (first digit).
  - Inner while: j walks to the '#'. Header digits are the half-open range [i,
  j), hence the length j - i passed to Substring.
  - i = j + 1: step over the '#'; i is now the payload start.
  - j = i + length: one past the payload end.
  - res.Add(s.Substring(i, length)); i = j: consume and advance to the next
  record.

  j carries nothing across iterations - it is reassigned to i at the top every
  time. The line j = i + length followed by i = j is just bookkeeping; i +=
  length after the Add is the same thing.
EDGE CASES THAT PASS FOR FREE
  Empty list: Encode's foreach appends nothing, res.ToString() is "", and
  Decode's while (i < s.Length) never runs, returning an empty list. No special
  case needed.

  Empty string: encodes to "0#". Decode parses length 0, sets i = j + 1, then j
  = i + 0, and Substring(i, 0) returns "". A list of three empty strings is
  "0#0#0#" and round-trips. Interviewers reach for this one first, so name it
  before they do.

  Unicode: s.Length counts UTF-16 code units and Substring slices by the same
  unit, so surrogate pairs are cut consistently on both sides. The scheme does
  not require ASCII payloads. It would break only if you switched one side to a
  byte count.
WATCH OUT
  The inner while (s[j] != '#') has no j < s.Length guard. It is safe only under
  the contract that Decode is fed Encode's output; on hand-written or truncated
  input it throws IndexOutOfRangeException rather than looping. Say that out
  loud rather than letting the interviewer find it - the fix is a bounds test in
  the loop condition plus a validity check after.

  Do not "simplify" the '#' away. Without a terminator the header length is
  unknowable: "12abc" could be one string of length 1 ("2ab") or one of length
  12. A fixed-width header (say 4 digits, zero-padded) is the alternative that
  also works, and it removes the scan entirely - but it caps string length.

  int.Parse(s.Substring(i, j - i)) allocates a throwaway string for the header.
  Accumulating digits by hand (length = length * 10 + (s[j] - '0')) inside the
  scan gives the same result in one pass with no header allocation. Worth
  mentioning as a refinement; it does not change the asymptotics.
TRIGGER
  Reach for length prefixing whenever you must serialize a sequence whose
  elements can contain arbitrary bytes or characters - joining files, framing
  messages on a socket, flattening nested structures. The tell is "any delimiter
  I choose could appear in the data." The alternatives are escaping (rewrite '#'
  as an escape sequence on the way in, undo it on the way out - correct but
  touches every character and is easy to get wrong on the escape character
  itself) and fixed-width headers. Length prefixing is the one that leaves the
  payload untouched.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
