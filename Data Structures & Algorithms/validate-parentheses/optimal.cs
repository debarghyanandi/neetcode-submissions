// ##########################################################################
// #  optimal.cs            O(n) time / O(n) space
// ##########################################################################

public class Solution
{
    public bool IsValid(string s)
    {
        var stack = new Stack<char>();

        // closer -> the opener it must be matched against
        var enums = new Dictionary<char, char> { { ')', '(' }, { '}', '{' }, { ']', '[' } };

        foreach (char c in s)
        {
            if (enums.ContainsKey(c))
            {
                // A closer is only legal if the most recent unmatched opener
                // is its exact partner.
                if (stack.Count > 0 && enums[c] == stack.Peek())
                {
                    stack.Pop();
                }
                else
                    return false;
            }
            else
                stack.Push(c);
        }

        // Anything left over is an opener that was never closed.
        return stack.Count == 0;
    }
}

/*
================================================================================
 PROBLEM : Given a string s of only the chars ( ) { } [ ], return true if it
           is valid. Valid means every opener is closed by the same type, in
           the right order, and every closer has an opener before it. "([]{})"
           -> true, "(]" -> false.
 PATTERN : Stack (matching pairs, last opened first closed)
================================================================================
IDEA
  Push every opener on stack. For a closer c, look up its partner enums[c].
  The top of stack must be that partner; if yes pop, else return false.
  At the end, stack must be empty, or some opener was never closed.
  It is correct because the most recent unclosed opener is always the next
  one that must close, and a stack gives exactly that element.
EXAMPLE
  s = "{[]}(": push '{', push '[', ']' matches '[' -> pop,
  '}' matches '{' -> pop, push '(' -> stack = ['(']
  end: stack.Count == 1 -> false. (For "]" alone: stack empty -> false.)
COMPLEXITY
  Time  O(n)  each char is pushed and popped at most once
  Space O(n)  stack can hold all n chars, e.g. "(((((("
PATH TO OPTIMAL
  Repeatedly delete "()", "[]", "{}" until nothing changes - O(n^2) - simple.
  Stack of openers - O(n) - one pass instead of many rescans (this file).
KEYWORDS
  stack, balanced brackets, valid parentheses, matching pairs, LIFO, hash map
WATCH OUT
  - Dropping the stack.Count > 0 check: Peek() on an empty stack throws
    InvalidOperationException for input like "]".
  - Returning true after the loop without checking stack.Count == 0:
    "((" would wrongly pass.
  - Any char not in enums is pushed as an opener; "a" returns false only
    because it stays on the stack. Fine here, but not a real input check.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. What if there is only one bracket type, "(" and ")"?
     -> Replace the stack with an int counter; +1 for "(", -1 for ")", fail if
        it goes below 0. Time O(n), space O(1).
  2. What if "*" can be "(", ")" or empty (Valid Parenthesis String)?
     -> Track a range [lo, hi] of possible open counts in one greedy pass.
        O(n) time, O(1) space; a stack of indices also works with O(n) space.
  3. Minimum removals to make the string valid?
     -> Use the same stack but store indices; unmatched ")" and leftover "("
        indices are the ones to remove. O(n) time and space.
  4. Can you exit early?
     -> If s.Length is odd, return false at once; still O(n) worst case.
TRIGGER
  Nested pairs that must close in reverse order of opening mean use a stack.
================================================================================
*/
