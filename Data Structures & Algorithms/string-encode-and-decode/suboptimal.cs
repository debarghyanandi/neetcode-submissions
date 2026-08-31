// --------------------------------------------------------------------------
// -  suboptimal.cs         O(n) time / O(n) space
// -  length-prefix encoding (sizes header + concatenated payload)
// -  [length-prefix-encoding]
// -  ranks below optimal.cs (O(n) time / O(1) space)
// -
// -  Reference solution - not one you solved yourself (from submission-5)
// -
// -  Builds an auxiliary List<int> of all string lengths (size proportional
// -  to string count, up to O(n) with many short strings) before/while
// -  scanning the O(n) total characters.
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

/*
================================================================================
 PATTERN : Length-prefixed header - size table, '#', raw payload
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-5.cs when it was first processed
 STATUS  : Suboptimal
================================================================================
WHY THIS ENCODING IS UNAMBIGUOUS
  The only thing that makes any encode/decode scheme correct is that the decoder
  never has to guess where a string ends. Here the decoder learns every length
  before it touches a single payload byte: it reads the comma-separated size
  table, hits '#', and from then on it only does arithmetic. It never searches
  the payload for a delimiter. That is why a string containing ',' or '#' or the
  whole encoded blob of another test case is still decoded correctly - those
  characters live past the '#' and are never compared against anything. The
  delimiters are only meaningful inside the header region, which by construction
  contains nothing but digits and commas.
THE INVARIANT TO STATE OUT LOUD
  Two phases, one cursor i.

  Header phase: at the top of the outer while, i is the index of the first digit
  of an unread size. The inner while walks j to the terminating ',', so
  s.Substring(i, j - i) is exactly one decimal number, and i = j + 1 restores
  the invariant. The loop ends with i on '#'.

  Payload phase: after i++, i is the start of the next undecoded string. Each
  iteration takes exactly sz characters and advances i by sz, so i is always a
  string boundary. The final i equals s.Length precisely because sum(sizes)
  equals the payload length - which Encode guaranteed by appending each s.Length
  and then each s in the same iteration order over strs.
WHY THIS LOSES TO THE INLINE-PREFIX VERSION
  The better encoding interleaves instead of segregating: for each s append
  s.Length, then '#', then s, giving "4#neet3#for". Decode reads digits up to
  the next '#', slices that many characters, and repeats.

  What this file pays for the split-header layout:
  - Encode walks strs three times (build sizes, append sizes, append payload)
  and materializes a List<int> that exists only to be drained by the very next
  loop. Even keeping this format, the first two loops collapse into one - append
  s.Length and ',' directly.
  - Decode builds a second List<int> sizes holding every length at once before
  emitting anything. The inline format needs no size table at all; it decodes as
  a stream with one live integer.
  - int.Parse(s.Substring(i, j - i)) allocates a throwaway string per element
  just to parse a number. int.Parse(s.AsSpan(i, j - i)) parses the same
  characters in place. The payload Substring calls are unavoidable - those
  strings are the return value - but the header ones are pure garbage.

  Same asymptotics, strictly more passes and strictly more allocation, and a
  format that is harder to explain than the one-liner alternative.
THE EDGE CASE EVERYONE GETS WRONG
  Distinguishing an empty list from a list holding one empty string. Trace both:

  strs = [] -> Encode short-circuits to "", Decode sees s.Length == 0 and
  returns an empty list. Round trip holds.

  strs = [""] -> sizes is [0], so the result is "0,#", which is not empty.
  Decode parses 0, steps past '#' to i = 3, and calls s.Substring(3, 0), which
  is legal at exactly index == Length and yields "". Result is [""]. Round trip
  holds.

  Note that the two guards are load-bearing as a pair. The Encode guard is what
  makes the Decode guard safe: without it Encode would emit "#" for the empty
  list, and any format where a non-empty output can also mean "no strings" is
  where these solutions break.
WHY THE UNBOUNDED INNER LOOPS ARE SAFE HERE
  Both scanning loops - while (s[i] != '#') and while (s[j] != ',') - run with
  no length check. They terminate only because Encode always emits a ',' after
  every size and exactly one '#' after the table, and because the empty-input
  case was already returned. Feed Decode a hand-written string with a missing
  comma and it walks off the end with IndexOutOfRangeException rather than
  throwing something meaningful.

  That is fine for the judge's contract, where Decode only ever receives
  Encode's output. Say so explicitly if asked - the answer is "it is a closed
  protocol, not a parser" - because the interviewer is usually probing whether
  you noticed, not asking you to harden it.
TRIGGER
  Reach for length prefixing the moment the alphabet of the payload is
  unrestricted - any character, including whatever you wanted to use as a
  separator. Escaping schemes and "pick a rare delimiter" both fail the
  adversarial input; counting characters cannot. The same reflex covers
  serializing over a socket and length-delimited binary framing.

  Recall hook for this specific file: sizes first, '#', then everything glued
  together - and the note that folding the size table inline is the version to
  actually write.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
