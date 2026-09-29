// ##########################################################################
// #  optimal.cs            O(n) time / O(n) space
// #  stack-based bracket matching   [stack-matching]
// #  the only solution in this folder
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  Each character is processed once with O(1) stack operations; stack
// #  size scales with input in worst case.
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
 PATTERN : Stack - match each closer to the latest open bracket
 SOURCE  : YOUR OWN SOLUTION - your own annotation at c76939d
 STATUS  : Optimal
================================================================================
VARIABLES
  stack    open brackets seen so far that are not matched yet; the top is the newest
  enums    closer -> its opener: ')'->'(', '}'->'{', ']'->'['
WHY THIS PATTERN
  Brackets must close in the reverse order they opened. So the last one opened
  must be the first one closed. That is last-in, first-out, which is exactly
  what a stack does. When a closer c arrives, only stack.Peek() can be its
  partner, so each check is one look at the top.
BRUTE FORCE
  Search the string for an adjacent pair "()", "[]" or "{}", remove it, and
  repeat until no pair is left. The string is valid if it ends up empty. Each
  pass is O(n), and there can be up to n/2 passes, so the total is O(n^2) time.
  It also builds a new string on every removal. The stack gets the same answer
  in one pass.
INVARIANT
  At every step, stack holds the unmatched openers of the prefix read so far, in
  the order they appeared. A closer is legal only if it matches stack.Peek(). If
  it does not match, no later character can fix the order, so the early return
  false is safe. At the end, the prefix is the whole string, and it is valid
  only when stack.Count == 0.
KEY THE MAP BY THE CLOSER
  enums maps closer -> opener, not opener -> closer. This way one call,
  enums.ContainsKey(c), tells you whether c is a closer, and enums[c] gives the
  opener to compare with stack.Peek(). If you keyed it by the opener, you would
  need a second set or a reverse lookup to spot closers.
WATCH OUT
  The else branch pushes every character that is not a closer, not only '(', '{'
  and '['. So the comment "Anything left over is an opener" is only true if the
  input has nothing but bracket characters. A letter like "a" gets pushed and
  makes the result false. If the input can hold other characters, skip them
  instead of pushing them. Also, the stack.Count > 0 check must come before
  Peek(). Without it, a leading closer such as ")" throws an exception instead
  of returning false. A null s also throws in the foreach.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. What if there is only one bracket type, like '(' and ')'?
     Replace the stack with an int counter. Add 1 on '(' and subtract 1 on ')'.
     Fail if it ever goes below 0, and require 0 at the end. This is O(1) space,
     but it only works for one type, because a counter cannot remember the order
     of different types.
  2. Return the minimum number of brackets to add to make the string valid.
     Keep the same scan, but do not return false on a bad closer. Count it as
     one needed insertion and go on. The answer is that count plus stack.Count
     at the end.
  3. Find the length of the longest valid substring.
     Push indexes instead of characters, and start with -1 as a base index. On a
     match, pop, then measure i minus the new top. On a bad closer, push i as
     the new base. This is still one pass.
  4. Can you reject some inputs before the loop?
     Yes. If s.Length is odd, return false right away, because every bracket
     needs a partner. This is a cheap early exit. It does not change the worst
     case.
TRIGGER
  Reach for a stack when items must be matched or undone in the reverse order
  they appeared, and only the newest open item can be closed next.
C# NOTE
  enums.ContainsKey(c) followed by enums[c] looks up the key twice.
  enums.TryGetValue(c, out char open) does both in one call and gives you the
  opener directly.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
