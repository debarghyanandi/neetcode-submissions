// ##########################################################################
// #  optimal.cs            O(n) time / O(n) space
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
 PROBLEM : You get tokens, a string array holding an expression in Reverse
           Polish Notation: each operator comes after its two operands.
           Operators are +, -, *, /. Integer division truncates toward zero.
           Return the int value. Example: ["2","1","+","3","*"] -> 9, because
           (2 + 1) * 3.
 PATTERN : Stack (operand stack for postfix evaluation)
================================================================================
IDEA
  Read tokens left to right. A number is pushed onto stack. An operator
  pops b (the top) and then a, computes operations[c](a, b), and pushes
  the result. At the end, the only value left on stack is the answer.
  This is correct because in postfix, an operator always applies to the
  two most recent values that are not yet used, and those sit on top.
EXAMPLE
  tokens = ["4","-13","5","/","+"] ("-13" is a number, not the "-" key)
  push 4, push -13, push 5 -> "/": b=5, a=-13, -13/5 = -2 (toward zero)
  "+": b=-2, a=4 -> 2. stack = [2], so the answer is 2.
COMPLEXITY
  Time  O(n)  each token is pushed or popped a constant number of times
  Space O(n)  the stack can hold up to about n/2 numbers
PATH TO OPTIMAL
  Rescan the list for the first operator, then replace it and its two
  operands with the result - O(n^2) - every rescan and shift costs O(n).
  One pass with a stack - O(n) - each token is handled only once.
  optimal-variant.cs uses the same stack idea with other operator dispatch.
KEYWORDS
  reverse polish notation, postfix, stack, expression evaluation, parsing
WATCH OUT
  - Pop order matters: the first pop is b, the right operand. If you swap
    a and b, then "-" and "/" give wrong answers.
  - The comment on a ("Prev result") is wrong. a is just the value below
    the top, and it can be a plain number, as with 3 in ["3","4","-"].
  - Division by zero throws DivideByZeroException. * and + can overflow
    int without an error. Bad input makes stack.Pop() throw.
  - C# / truncates toward zero, which is correct here. Python's // floors.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How do you evaluate a normal infix string with parentheses?
     -> Convert it to postfix with the shunting-yard algorithm, using an
        operator stack and precedence, then evaluate it as here. O(n) / O(n).
  2. Can you do it without an extra stack?
     -> Reuse tokens as the stack, with a write index that you overwrite. O(1)
        extra space, but it destroys the input and is harder to read.
  3. Can you solve it recursively?
     -> Evaluate from the end. An operator recursively evaluates its right
        operand, then its left. Still O(n), but the call stack can get deep.
  4. How would you add unary minus or other operators like ^?
     -> Add an entry to the operations dictionary. Unary operators pop one
        value. The loop stays the same.
TRIGGER
  Reach for a stack when each operator must act on the latest values that
  have not been used yet, as in postfix or nested expressions.
================================================================================
*/
