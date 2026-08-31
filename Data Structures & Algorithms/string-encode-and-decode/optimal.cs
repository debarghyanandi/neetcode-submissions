// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(n) space
// -  length-prefix encoding   [length-prefix-encode]
// -  the only solution in this folder
// -
// -  Reference solution - not one you solved yourself (from submission-4)
// -
// -  Encodes each string's length before a delimiter, then parses lengths
// -  back out during decode using a single linear scan, with an auxiliary
// -  sizes list sized to the number of strings.
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
 PATTERN : Length-prefixed header, then '#', then raw payload
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-4.cs when it was first processed
 STATUS  : Optimal
================================================================================
WHY THIS SHAPE
  The tempting encoding is a join on some sentinel character. It cannot work:
  strs holds arbitrary strings, so any sentinel you pick may appear inside an
  element, and fixing that means an escaping scheme you then have to unescape.
  Length-prefixing removes the ambiguity instead of patching it. Decode never
  inspects the payload at all - it only jumps by sz. The one place a delimiter
  is still needed is the boundary between header and payload, and that is safe
  here because the header is built exclusively from digits (Append(sz)) and
  commas, so the first '#' in the whole string is necessarily the terminator
  that Encode wrote.
INVARIANT
  After Encode's first loop, sizes[k] == strs[k].Length for every k, and the
  payload appended by the third loop has length equal to the sum of sizes, laid
  down in the same order. Decode restores that: i is always positioned at the
  first character of the next unread field - first the next size token, then
  after i++ past the '#', the first character of the next string. Every read
  advances i by exactly the width of what was consumed (j - i + 1 for a size
  token, sz for a string), which is why the walk never drifts.
TRACE THE ADVERSARIAL CASE
  Take strs = ["#,3", ""]. Encode writes sizes 3 and 0 as "3,0,", then '#', then
  the payload "#,3", giving "3,0,##,3". Decode's while loop stops at index 4,
  the first '#'; it parses "3" and "0" from "3,0,"; then i = 5 and it slices 3
  characters -> "#,3", then 0 characters -> "". The second '#' and the comma
  inside the payload are never examined by any comparison. That is the whole
  correctness argument in one example: only the header is scanned, and the
  header is guaranteed sentinel-free.
THE EMPTY CONTRACT
  Encode returns "" for an empty list and Decode returns an empty list for "".
  These two guards are not symmetric in importance. Decode's s.Length == 0 check
  is load-bearing: without it, s[i] at i = 0 indexes an empty string and throws.
  Encode's strs.Count == 0 check is redundant - it would otherwise emit "#", and
  Decode handles that correctly (the while loop body never runs, sizes stays
  empty, i becomes 1, the foreach yields nothing). Keep both anyway; they
  document the pair as one convention, and dropping only Encode's makes the ""
  branch in Decode look like dead code.
TRAILING COMMA, NOT SEPARATOR
  Append(sz).Append(',') puts a comma after every size including the last, so
  the header is "4,4,4,3," and not "4,4,4,3". That choice is what keeps Decode's
  parse loop free of special cases: the inner while (s[j] != ',') is guaranteed
  to find a terminator for every token, and i = j + 1 lands cleanly on either
  the next digit or the '#'. Switch to a separator-style join and the final size
  sits directly against '#', so the inner loop needs a second stop condition and
  a branch.
WATCH OUT
  1. Decode's inner while (s[j] != ',') has no bound on j. It assumes input
  produced by this Encode; a malformed string with no comma before the end runs
  off the string and throws. Fine for the judge, worth naming out loud in an
  interview.
  2. int.Parse(s.Substring(i, j - i)) allocates a throwaway string per size.
  Accumulating digits in place - n = n * 10 + (s[j] - '0') while scanning - gets
  the same value with no substring at all.
  3. The sizes list in Encode is dead weight. The first foreach could append
  s.Length straight into res, because the third foreach re-walks strs in the
  same order anyway. Two passes over strs are unavoidable (the whole header must
  precede the payload); the extra List is not.
FOLLOW-UP
  Non-ASCII round-trips correctly, and for a specific reason: s.Length and
  Substring both count UTF-16 code units, so a size measured at encode time is
  in exactly the unit the decoder slices with. Surrogate pairs and emoji cannot
  be split. If the interviewer moves the problem to a byte stream, drop the
  commas and the '#' entirely and write a fixed-width 4-byte big-endian length
  before each string - the header becomes self-delimiting, so no terminator
  character is needed.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
