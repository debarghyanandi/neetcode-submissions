// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
// -  two pointers converging inward, skip non-alphanumerics
// -  [two-pointer-inplace-palindrome]
// -  the only solution in this folder
// -
// -  Reference solution - not one you solved yourself
// -
// -  left/right indices walk toward each other over the original string,
// -  skipping non-alphanumeric chars and case-folding, using only O(1)
// -  extra scalars
// --------------------------------------------------------------------------

public class Solution
{
    public bool IsPalindrome(string text)
    {
        int left = 0;
        int right = text.Length - 1;

        while (left < right)
        {
            // Skip anything that is not part of the comparison.
            while (left < right && !char.IsLetterOrDigit(text[left]))
                left++;

            while (right > left && !char.IsLetterOrDigit(text[right]))
                right--;

            if (char.ToLower(text[left]) != char.ToLower(text[right]))
                return false;

            left++;
            right--;
        }

        return true;
    }
}

/*
================================================================================
 PATTERN : Two Pointers - converge inward, skip non-alphanumerics
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  A palindrome check is a statement about pairs: position i from the front must
  match position i from the back. Two pointers walk those pairs directly, so no
  copy of the input is needed. The obvious alternative - filter text into a new
  string of lowercase alphanumerics, then compare it to its reverse (or run two
  pointers over it) - is easier to write and just as fast asymptotically, but it
  allocates a second buffer proportional to the input. This version pays nothing
  extra by doing the filtering inline: the skip loops ARE the filter, applied
  lazily at the moment each pointer is consulted.
INVARIANT
  Everything strictly outside [left, right] has already been matched as an equal
  pair. The outer condition left < right is the termination rule: when the
  pointers meet or cross, every remaining pair is exhausted. Note that left ==
  right means one character is left in the middle, and a middle character is its
  own mirror - it never needs a comparison, which is why the outer loop uses <
  and not <=.

  The nested while loops do not make this quadratic. left only ever increases
  and right only ever decreases; between them they can take at most text.Length
  steps in total across the whole run, no matter how they are distributed
  between the skip loops and the trailing left++/right--.
ALGORITHM
  1. left = 0, right = text.Length - 1.
  2. While left < right:
  3. Advance left past any character where char.IsLetterOrDigit is false.
  4. Retreat right past any such character.
  5. Compare char.ToLower(text[left]) against char.ToLower(text[right]); return
  false on mismatch.
  6. Step both pointers inward past the pair just matched.
  7. Surviving the loop means no mismatching pair exists - return true.
WHY THE INNER GUARDS MATTER
  The skip loops re-test left < right (and right > left) rather than just left <
  text.Length. Drop that guard and the code crashes on any input with no
  alphanumerics at all: for text = ".,", left would run off the end and
  text[left] throws IndexOutOfRangeException. With the guard, left stops at 1,
  the second skip loop sees right > left is false and does nothing, and the
  comparison degenerates to text[1] against text[1] - a character compared with
  itself, trivially equal. So the guard converts the degenerate case into a
  harmless self-comparison instead of an out-of-range read.

  That self-comparison is also what saves the one-alphanumeric case such as
  "a.": correctness does not depend on avoiding it, only on it being safe.
THE TOLOWER TRAP
  char.ToLower(char) uses the current culture, not the invariant one. Under a
  Turkish culture, 'I' lowercases to dotless 'i' while 'i' stays 'i', so text =
  "Ii" would report false on a machine set to tr-TR and true everywhere else.
  char.ToLowerInvariant is the correct call here, since the comparison is about
  character identity, not about how a human reads the text.

  Related: char.IsLetterOrDigit is Unicode-aware, so accented letters, non-Latin
  scripts, and non-ASCII digits all count as comparable characters. If the
  intended definition is ASCII-only, this admits more characters than expected -
  worth stating out loud rather than leaving to the reader.
EDGE CASES TO REPLAY
  Empty string: right = -1, the outer loop never runs, returns true.
  Single character: left == right immediately, returns true without any
  comparison.
  All punctuation such as ",.": handled by the guard path described above,
  returns true.
  Digits: IsLetterOrDigit admits them and ToLower leaves them unchanged, so "0P"
  correctly returns false on the P/0 mismatch.
TRIGGER
  Reach for converging two pointers when the input is a linear sequence, the
  property is defined over symmetric pairs, and the problem asks you to ignore
  or filter certain elements. The filter belongs inside the pointer advance, not
  in a preprocessing pass - that is the move that turns an O(n) space solution
  into an O(1) space one without changing the shape of the loop.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
