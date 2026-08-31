// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
// -  two pointers converging inward, skipping non-alphanumerics
// -  [two-pointer-inplace-palindrome]
// -  the only solution in this folder
// -
// -  Reference solution - not one you solved yourself
// -
// -  left/right indices advance toward each other over the original string,
// -  skipping non-alphanumeric chars and case-folding, using only O(1)
// -  extra scalars.
// --------------------------------------------------------------------------

public class Solution
{
    public bool IsPalindrome(string s)
    {
        int left = 0;
        int right = s.Length - 1;

        while (left < right)
        {
            // Skip anything that is not part of the comparison.
            while (left < right && !char.IsLetterOrDigit(s[left]))
                left++;

            while (right > left && !char.IsLetterOrDigit(s[right]))
                right--;

            if (char.ToLower(s[left]) != char.ToLower(s[right]))
                return false;

            left++;
            right--;
        }

        return true;
    }
}

/*
================================================================================
 PATTERN : Two pointers converging, skipping non-alphanumerics
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Optimal
================================================================================
WHY THIS SHAPE
  A palindrome is a statement about mirrored pairs: position i must match
  position (n-1-i) once the junk is removed. Two indices walking toward each
  other evaluate exactly that pairing, one pair per outer iteration, without
  ever materializing the cleaned string. The filtering is folded into the walk -
  left and right skip forward over anything char.IsLetterOrDigit rejects, so the
  pointers only ever meet on characters that actually participate in the
  comparison.
INVARIANT
  At the top of every outer iteration: every character at an index below left or
  above right has already been either matched against its mirror or discarded as
  punctuation. So the remaining question is always the same question on the
  smaller slice s[left..right]. Each pass either confirms one pair and shrinks
  that slice by at least two, or returns false immediately. Reaching left >=
  right means nothing is left to disprove, hence the unconditional return true.
WHY THE COMPARE IS ALWAYS IN BOUNDS
  This is the detail an interviewer probes. Both skip loops are guarded by the
  crossing condition (left < right, and right > left - the same test written two
  ways), never by s.Length. That is sufficient because left and right start
  inside the array and each skip loop stops the moment the pointers touch. So at
  the char.ToLower comparison, both indices are still within [0, s.Length-1].
  There is no need for a separate bounds check, and no need to re-test left <
  right before comparing.

  The case that looks broken but is not: a skip loop can exit on the guard
  rather than on an alphanumeric, leaving left == right on a punctuation
  character. Then the line compares s[left] against s[right] - the same index -
  which is trivially equal, the pointers cross, and the loop ends.
  Wrong-looking, harmless.
EDGE CASES THIS QUIETLY HANDLES
  1. Empty string: right = -1, the outer condition 0 < -1 fails, returns true.
  2. All punctuation, e.g. ",.": left skips to meet right, the degenerate
  self-comparison succeeds, returns true - which is the required answer, since
  the filtered string is empty.
  3. Odd-length center, e.g. "aba": left and right both land on 'b' or cross
  past it; the middle character is never compared against a different index,
  which is correct since it is its own mirror.
  4. Digits: char.IsLetterOrDigit keeps them, so "0P" correctly returns false
  rather than being treated as two skippable characters.
WATCH OUT
  char.ToLower uses the current culture. Under a Turkish culture, 'I' lowercases
  to dotless 'i' while 'i' stays 'i', so "Ii" would report false.
  char.ToLowerInvariant removes that dependency and is the safer default here,
  since the comparison is meant to be a plain ASCII case fold, not a
  locale-aware one.

  Relatedly, char.IsLetterOrDigit is Unicode-wide, not [a-zA-Z0-9] - accented
  letters and non-Latin digits are kept, not skipped. Fine for the judge's ASCII
  inputs; state the assumption if asked.
THE ALTERNATIVE AND WHY IT LOSES
  The obvious first attempt: build a filtered lowercase string, then compare it
  to its reverse (or run two pointers over it). Same asymptotic time, but it
  allocates a second buffer proportional to the input. This version keeps only
  left and right, which is the whole reason for interleaving the skip loops with
  the comparison instead of doing a clean pass first.
TRIGGER
  Reach for this when a predicate over a sequence is symmetric about the center
  and the elements can be judged pairwise, and when some elements are noise to
  be ignored rather than data. The generalization is: outer loop advances the
  answer, inner loops advance past irrelevant input, all pointers move
  monotonically so the walk always terminates.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
