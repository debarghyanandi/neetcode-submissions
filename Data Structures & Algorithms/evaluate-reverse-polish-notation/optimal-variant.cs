// ##########################################################################
// #  optimal-variant.cs    O(n) time / O(n) space
// #  explicit Stack<int> with dictionary-dispatched operators
// #  [stack-eval]
// #  ties with optimal.cs on O(n) time / O(n) space
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  single pass pushing operands and popping two per operator via a Func
// #  lookup table, worst-case stack depth O(n)
// ##########################################################################

public class Solution {
    public int EvalRPN(string[] tokens) {
        // My Solution
        var stack = new Stack<int>();
        var operations = new Dictionary<string, Func<int, int, int>>
        {
            ["+"] = (left, right) => left + right,
            ["-"] = (left, right) => left - right,
            ["*"] = (left, right) => left * right,
            ["/"] = (left, right) => left / right
        };

        foreach (string token in tokens)
        {
            if (operations.ContainsKey(token))
            {
                int right = stack.Pop(); // current num
                int left = stack.Pop(); // Prev result is this.
                int result = operations[token](left, right);
                stack.Push(result);
            }
            else
                stack.Push(int.Parse(token));
        }

        return stack.Pop();
    }
}

/*
================================================================================
 PATTERN : Stack fold over postfix tokens - operator lookup table
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-1.cs when it was
           first processed
 STATUS  : Optimal variant - ties the best complexity by another route
================================================================================
WHY THIS WORKS
  Postfix notation encodes the parse tree without parentheses: an operator's two
  operands are always the two most recently completed subexpressions to its
  left. "Most recent, not yet consumed" is the definition of LIFO, so a
  Stack<int> is not a convenience here - it is the exact data structure the
  grammar asks for. Every token is touched once, and each operator both removes
  two entries and adds one, so the stack shrinks by exactly one per operator.
INVARIANT
  After processing any prefix of tokens, stack holds - bottom to top, in
  left-to-right source order - the integer values of every maximal complete
  subexpression seen so far. Nothing partially evaluated is ever on the stack;
  an int only gets pushed once its whole subtree is collapsed. For well-formed
  input the operand count exceeds the operator count by exactly one, so when the
  loop ends the stack has exactly one entry, which is why the bare stack.Pop()
  at the bottom is safe without a Count check.
THE POP ORDER TRAP
  right = stack.Pop() comes first, left = stack.Pop() second. This is the single
  most common bug in this problem and the thing to re-derive rather than
  memorize: the right operand was pushed last, so it comes off first. Swap those
  two lines and "+" and "*" still pass while "-" and "/" silently produce
  negated / reciprocal-ish garbage. The comments in the file ("current num" for
  right, "Prev result is this" for left) are the mnemonic - the accumulated left
  side has been sitting deeper in the stack.
WHY THE ELSE BRANCH IS SAFE
  The dispatch is operations.ContainsKey(token), matching the whole string, not
  a character scan. So the negative literal "-11" does not collide with the "-"
  operator key: it misses the dictionary, falls to the else, and int.Parse
  handles the leading minus. If you had instead tested token[0] == '-' or
  checked token.Length, negative operands would be misread as subtraction. Worth
  being able to say out loud when asked "what about negative numbers?"
TRUNCATION IS LOAD-BEARING
  C# int / int truncates toward zero, which is exactly the semantics RPN
  evaluation requires: 6 / -132 must be 0, not -1. The lambda ["/"] = (left,
  right) => left / right inherits that for free. Port this to Python and a // b
  floors toward negative infinity and quietly breaks on mixed signs - you would
  need int(a / b) or math.trunc. Same trap in any language whose division rounds
  down.
WHAT THE TABLE COSTS
  The Dictionary<string, Func<int,int,int>> replaces a switch on token. Two
  things follow from the code as written. First, ContainsKey(token) then
  operations[token] hashes the same string twice; a single TryGetValue(token,
  out var op) collapses that into one probe and reads cleaner. Second, the
  dictionary and its four lambdas are constructed inside EvalRPN, so they are
  rebuilt on every call - hoisting to a static readonly field makes it one-time.
  Neither changes the asymptotics, and the table pays for itself the moment you
  need to extend the operator set or make it data-driven.
WHEN IT BREAKS
  Every guarantee above rests on the input being valid RPN. Feed it "1 +" and
  stack.Pop() throws InvalidOperationException on an empty stack; feed it "1 2
  3" and the trailing Pop returns 3, silently ignoring the leftovers. Division
  by zero throws DivideByZeroException. If an interviewer asks you to harden it:
  check stack.Count >= 2 before each operator, use int.TryParse instead of Parse
  for the operand branch, and assert stack.Count == 1 at the end.
FOLLOW-UPS TO EXPECT
  1) Build the RPN from infix - that is shunting-yard, same operator table plus
  precedence. 2) Return the expression tree instead of the value - push TreeNode
  instead of int, identical control flow. 3) Overflow - all intermediates are
  stated to fit in a signed 32-bit int, so int suffices; otherwise switch to
  long. 4) Do it with O(1) extra space - reuse the tokens array itself as the
  stack by writing results back at a write index, since the stack never grows
  past the number of tokens already consumed.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
