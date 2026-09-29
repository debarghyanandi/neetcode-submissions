// ##########################################################################
// #  optimal.cs            O(n) time / O(n) space
// #  Stack-based RPN evaluation   [stack-rpn]
// #  ties with optimal-variant.cs on O(n) time / O(n) space
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  Single pass iterates through tokens; stack stores operands, each
// #  operation is O(1).
// ##########################################################################

public class Solution
{
    public int EvalRPN(string[] tokens)
    {
        // My Solution
        var stack = new Stack<int>();
        var operations = new Dictionary<string, Func<int, int, int>>
        {
            ["+"] = (a, b) => a + b,
            ["-"] = (a, b) => a - b,
            ["*"] = (a, b) => a * b,
            ["/"] = (a, b) => a / b
        };

        foreach (string c in tokens)
        {
            if (operations.ContainsKey(c))
            {
                int b = stack.Pop(); // current num
                int a = stack.Pop(); // Prev result is this.
                int op = operations[c](a, b);
                stack.Push(op);
            }
            else
                stack.Push(int.Parse(c));
        }

        return stack.Pop();
    }
}

/*
================================================================================
 PATTERN : Stack - evaluate postfix (RPN) expressions
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-1.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  stack       numbers not used yet, plus results of earlier operations
  operations  maps each operator string to a function (a, b) => result
  b           the right operand, popped first from the top of the stack
  a           the left operand, popped second
  op          the result of a applied to b (the value, not the operator)
WHY THIS PATTERN
  In Reverse Polish Notation (postfix), each operator comes after its two
  operands. The operator always acts on the two most recent values that have not
  been used yet. "Most recent first" is last-in-first-out, and that is what a
  stack gives. When the loop finds an operator in operations, it pops two
  values, computes op, and pushes op back so a later operator can use it.
BRUTE FORCE
  A simple approach without a stack scans the list for the first operator. It
  computes that operator with the two tokens just before it, replaces those
  three tokens with the result, and scans again from the start. This is correct,
  but each reduction costs O(n) for the scan and the list shift, so the total is
  O(n^2). The stack version does each reduction in O(1).
INVARIANT
  After each token is handled, stack holds the values of all complete
  sub-expressions seen so far, in order, with the newest on top. A number is
  already a complete sub-expression. An operator joins the top two complete
  sub-expressions into one, so the rule still holds. For a valid expression, the
  whole input reduces to exactly one value, and the final stack.Pop() returns
  it.
OPERAND ORDER
  The first Pop gives the right operand, so the code stores it in b and then
  pops a. It then calls operations[c](a, b). If you swap them, "+" and "*" still
  work, but "-" and "/" give wrong answers. For example, ["4","2","-"] must give
  2, not -2.
DIVISION TRUNCATES TOWARD ZERO
  The problem asks that division truncate toward zero, meaning the fraction is
  cut off. C# int division a / b already does this. So -7 / 2 gives -3, not -4.
  You need no special rounding code here. Other languages differ: in Python, //
  rounds down (toward minus infinity).
WATCH OUT
  The comment "Prev result is this." on a is not always true. The value in a can
  be a plain number that was pushed from the input, not a result. It is simply
  the left operand. The name op is also misleading, because it holds a result,
  not an operator. The arithmetic is unchecked, so a * b can overflow int
  without an error. Bad input breaks the code: Pop on an empty stack throws, and
  int.Parse throws on a token that is not a number.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you avoid the Stack class?
     Yes. Use an int[] of size tokens.Length and an index as the top pointer.
     The stack never holds more values than there are tokens. The cost is the
     same, but it has no collection overhead, and you must manage the index
     yourself.
  2. What if the input is infix with parentheses, like "(1 + 2) * 3"?
     Use two stacks, one for numbers and one for operators. Apply operators by
     precedence and by parentheses. Or use the shunting-yard algorithm to
     convert the input to RPN first, then run this code. The extra cost is the
     precedence logic.
  3. How would you add a unary operator such as negation or abs?
     The dictionary would need to store each operator's arity (how many operands
     it takes), and the loop would pop that many values. The current Func<int,
     int, int> type only fits binary operators.
TRIGGER
  When an operator or closing token must act on the most recent unfinished
  items, and its result feeds back in as a new item, use a stack.
C# NOTE
  ContainsKey followed by operations[c] looks up the key twice.
  operations.TryGetValue(c, out var f) does one lookup and gives you the
  function directly.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
