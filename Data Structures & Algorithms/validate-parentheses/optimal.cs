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
 PATTERN : Stack - match each closer with the most recent opener
 SOURCE  : YOUR OWN SOLUTION - your own annotation at c76939d
 STATUS  : Optimal
================================================================================
VARIABLES
  stack    unmatched openers seen so far, newest on top
  enums    enums[closer] = the opener that closer must match
WHY THIS PATTERN
  The rule "brackets must close in the reverse order they were opened" is
  exactly last-in-first-out, which is what a stack gives you. Every opener is
  pushed and waits; when a closer c arrives, only the newest waiting opener can
  legally answer it, so stack.Peek() is the only candidate to check. The enums
  map turns "is this the right partner" into one dictionary lookup instead of a
  chain of if statements.
BRUTE FORCE
  The first idea most people write is repeated string replacement: delete every
  "()", "{}" and "[]" from s, loop until the string stops changing, and return
  true if it is empty. That is correct but costs O(n) per pass with up to O(n)
  passes, so O(n^2) time plus a new string each pass. The stack does the same
  collapsing in a single scan.
INVARIANT
  At every point in the loop, stack holds exactly the openers seen so far that
  have not yet been matched, in the order they were opened, newest on top. A
  closer either cancels the top (valid so far) or the function returns false
  immediately, so the invariant is never broken. When the loop finishes, the
  string is valid if and only if nothing is still waiting, which is the final
  stack.Count == 0 check.
WATCH OUT
  The else branch pushes anything that is not a key of enums, so a letter or a
  space in s is treated as an opener and lands on the stack, which makes the
  result false at the end. If the input can contain characters outside the six
  brackets, add an explicit check that c is one of '(', '{', '[' before pushing.
  The stack.Count > 0 guard must stay before stack.Peek(), otherwise a leading
  closer like ")" throws InvalidOperationException. Note that the code calls
  enums[c] after ContainsKey(c), so it hashes c twice.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you do this with O(1) extra memory?
     Not in general. Nesting depth can be as large as n, and the stack must
     remember every open bracket, so the space is inherent. Only a single
     bracket type would let you replace the stack with one counter.
  2. How do you return the index of the first bad character instead of a bool?
     Switch the foreach to an indexed for loop and return that index on the
     mismatch branch; for the leftover case, push the index alongside the char
     (Stack<(char, int)>) so you can report the position of the unclosed opener.
  3. The input is a huge file that does not fit in memory. What changes?
     The algorithm is already streaming: read char by char and keep the same
     stack, since each character is touched once and never revisited. Only the
     stack needs memory, and it grows with nesting depth, not file length.
  4. What if a new pair is added, like angle brackets?
     Add one entry to enums. The loop body does not change, which is the reason
     for using the map rather than hard-coded comparisons.
TRIGGER
  When the problem says items must close, undo, or resolve in reverse order of
  appearance, reach for a stack.
C# NOTE
  Replacing the ContainsKey plus indexer pair with enums.TryGetValue(c, out char
  opener) does one lookup instead of two and reads cleaner; the Dictionary
  itself is also allocated on every call, so making it a static readonly field
  would avoid rebuilding it per invocation.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
