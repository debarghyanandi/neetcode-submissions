// ##########################################################################
// #  YOU SOLVED THIS YOURSELF  (submission-0, marked '//My solution')
// #  the standard answer, reached first try - nothing to trade up to
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
                else return false;
            }
            else stack.Push(c);
        }

        // Anything left over is an opener that was never closed.
        return stack.Count == 0;
    }
}

/*
================================================================================
 PATTERN : Stack - LIFO matching of nested pairs
 SOURCE  : YOUR OWN SOLUTION (submission-0, marked '//My solution')
 STATUS  : Optimal - O(n) time, O(n) space
================================================================================

WHY THIS PATTERN
  Brackets nest. That single word decides the data structure: the bracket a
  closer belongs to is always the MOST RECENTLY OPENED unclosed one, never
  an older one. "Most recent, and remove it" is the definition of a stack,
  so the problem is not really about brackets - it is about recognising a
  LIFO dependency when it is described in other words.

  The same recognition drives expression parsing, undo history, call stacks,
  and the monotonic-stack family (next greater element, largest rectangle).
  This is the entry point to all of them.

BRUTE FORCE (and why it fails)
  Repeatedly find and delete an adjacent "()" / "[]" / "{}" pair until the
  string stops changing, then check it is empty. Correct, and O(n^2) - each
  pass is O(n) and there can be O(n) passes on "((((...))))". A counter per
  bracket type is worse: it is not merely slow but WRONG, because it accepts
  "([)]" - counts cannot see order, and order is the entire problem.

INVARIANT
  The stack holds exactly the openers seen so far that are still unmatched,
  in the order they were opened. Every prefix processed without returning
  false is a valid prefix of some valid string.

ALGORITHM (NeetCode: "Stack")
  1. Empty stack; a map from each closer to its opener.
  2. For each character:
       - closer -> if the stack is empty or its top is not the partner,
         return false; otherwise pop.
       - opener -> push.
  3. Return true only if the stack is empty at the end.

COMPLEXITY
  Time  : O(n) - each character is pushed at most once and popped at most
          once.
  Space : O(n) - worst case "(((((((" pushes everything. O(1) is impossible
          for arbitrary nesting depth; the stack IS the answer.

TRIGGER
  "Balanced / valid / properly nested", or any rule where the thing you are
  closing must be the most recent thing you opened. If a problem statement
  contains the word "nested", start with a stack and justify moving away
  from it, not the other way round.

C# NOTES
  - Stack<char> is array-backed; Push/Pop amortise to O(1).
  - The `stack.Count > 0 &&` guard must come first - && short-circuits, so
    Peek() is never reached on an empty stack. Swapping the operands throws
    InvalidOperationException on input ")".
  - ContainsKey followed by enums[c] is two hash lookups. TryGetValue does
    it in one: `if (enums.TryGetValue(c, out char open))`. Negligible at
    n <= 10^4, but it is the idiom an interviewer expects to see in C#.
  - A `switch` expression on the character avoids the dictionary allocation
    entirely and is faster for three fixed pairs; the dictionary is the more
    extensible shape. Say which trade-off you are making.
  - An odd-length string can be rejected immediately: `if ((s.Length & 1) != 0)
    return false;` - a real early exit on half the random inputs.

WATCH OUT
  - `else stack.Push(c)` pushes ANY non-closer, including letters. That is
    correct for this problem's constraints (brackets only) and quietly wrong
    if the input can contain other characters - the guarantee is doing work,
    so name it rather than assume it.
  - Returning true on an empty stack mid-loop is wrong; the check belongs
    after the loop, which is where it is.
  - The map direction matters: closer -> opener lets the lookup itself
    classify the character. Opener -> closer would need a second membership
    test.
================================================================================
*/
