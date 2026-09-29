// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
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
 PATTERN : Two Pointers - converge from both ends, skip non-alnum
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Optimal
================================================================================
VARIABLES
  left   index of the next character to check from the front
  right  index of the next character to check from the back
WHY THIS PATTERN
  A palindrome reads the same from both ends. So the first kept character must
  match the last one, the second must match the second-to-last, and so on.
  Moving left forward and right backward checks exactly these pairs. The two
  inner while loops skip the ignored characters, so nothing has to be removed
  from s first.
BRUTE FORCE
  First build a new string with only the letters and digits, all in lowercase.
  Then compare it with its reverse, or check it with two indexes. This is also
  O(n) time, but it uses O(n) extra memory for the cleaned copy, and the
  reversed copy adds more. It loses only on space. The logic is just as simple.
INVARIANT
  Before each pass of the outer loop, every letter or digit below left has
  already matched its partner above right. The inner loops keep the check left <
  right, so the two pointers never cross. If the pointers meet or cross without
  a mismatch, every pair has been checked, so returning true is correct. The
  first pair that differs proves s is not a palindrome, so the code can return
  false at once.
SKIPS CAN LAND ON THE SAME CHAR
  If only ignored characters remain between the pointers, the skip loops stop
  with left == right. The code then compares s[left] with itself, which always
  matches, and then left++ and right-- end the loop. This is why no extra check
  is needed after the skip loops.
WATCH OUT
  char.IsLetterOrDigit uses Unicode rules. It accepts letters like 'e' with an
  accent and digits from other scripts, not only a-z, A-Z and 0-9. If the
  problem means ASCII letters and digits only, this code can keep characters it
  should skip. The code also works on single UTF-16 units (char). A character
  stored as a surrogate pair, such as an emoji, is really two chars, so it is
  judged one half at a time.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. What if you may delete at most one character to make a palindrome (Valid
  Palindrome II)?
     Run the same two pointers. At the first mismatch, try two cases: skip
     s[left], or skip s[right]. Check if the rest is a palindrome in either
     case. It is still O(n) time and O(1) space, with at most one extra pass.
  2. What if the input is a singly linked list instead of a string?
     You cannot move backward in a singly linked list. Find the middle with slow
     and fast pointers, reverse the second half, then compare the two halves.
     This keeps O(1) space but changes the list, so you should reverse it back
     afterward.
  3. How would you find the longest palindromic substring instead?
     Grow two pointers outward from each center (2n-1 centers, counting the gaps
     between characters). This takes O(n^2) time and O(1) space. Manacher's
     algorithm gets O(n) time, but it is harder to write.
TRIGGER
  The problem asks you to compare a sequence with its own mirror image, often
  while ignoring some characters, and wants no extra memory.
C# NOTE
  char.ToLower(c) uses the current culture. On a Turkish system, for example,
  'I' does not become 'i'. char.ToLowerInvariant gives the same result on every
  machine.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
