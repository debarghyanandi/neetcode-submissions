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
 PATTERN : Stack - evaluate Reverse Polish Notation
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-1.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  stack        operands seen so far, top is the most recent value
  operations   maps token "+","-","*","/" to a Func<int,int,int>
  b            right operand, popped first
  a            left operand, popped second
WHY THIS PATTERN
  In postfix notation an operator always applies to the two values that came
  right before it, so the most recent operands are the first ones needed. That
  is exactly last-in-first-out, which is what a stack gives. Each token in
  tokens either pushes a number or replaces the top two entries with one result,
  so one pass is enough. The dictionary operations turns the four-way branch on
  the token into a single lookup plus a call.
BRUTE FORCE
  Without a stack you would scan the array repeatedly: find the first operator
  that has two numbers in front of it, compute it, then build a new shorter
  array and start again. That is a fresh scan and a fresh copy per operator, so
  O(n^2) time. It is correct but it redoes work the stack keeps for free.
INVARIANT
  After processing any prefix of tokens, stack holds exactly the values of the
  complete sub-expressions in that prefix, in left-to-right order. A number
  pushes a new one-token sub-expression; an operator consumes the last two
  finished sub-expressions and pushes the single value they combine into, so the
  invariant survives. For a valid RPN input the whole array is one expression,
  so at the end the stack holds exactly one value, and that final Pop is the
  answer.
POP ORDER IS THE WHOLE CORRECTNESS OF MINUS AND DIVIDE
  The first Pop gives the right operand and the second gives the left, because
  the right operand was pushed later. Calling operations[c](a, b) with a first
  preserves that. Swapping them still passes every test built only from "+" and
  "*" and then silently gives wrong answers for "-" and "/", which is the
  classic bug in this problem.
WATCH OUT
  The comments on the two pops are wrong and misleading: b is not "current num"
  and a is not "Prev result" - both are just the two most recent operand values,
  and either can be a raw number or an earlier result. int division in C#
  truncates toward zero, which happens to match the usual requirement for this
  problem, but it is a behavior to state out loud rather than assume. Division
  by zero throws DivideByZeroException; there is no guard. Any malformed input -
  an operator with fewer than two values below it, or leftover values at the end
  - is not detected: Pop throws on the first case and the second case silently
  returns the wrong element.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. The values can overflow int during intermediate multiplication. What
  changes?
     Switch stack to Stack<long>, parse with long.Parse, and change the Func to
     Func<long,long,long>. The structure is identical; only the numeric type
     widens, at the cost of more memory per entry.
  2. Can you avoid the Dictionary of delegates?
     Yes - a switch on c with the arithmetic inline. You lose the neat table and
     gain a direct branch with no delegate invocation and no dictionary
     allocation per call to EvalRPN.
  3. The input arrives as a stream of tokens instead of an array. Does the code
  still work?
     Yes, unchanged in spirit - the foreach never looks ahead or back, so it can
     read from an IEnumerable<string> one token at a time. Only the stack has to
     be kept in memory.
  4. How would you handle infix input like "3 + 4 * 2" instead?
     Convert to RPN first with the shunting-yard algorithm, which uses a second
     stack for operators and pops by precedence, then feed the result into this
     exact evaluator.
TRIGGER
  When each new item must be combined with the most recently produced items and
  then collapses into one, reach for a stack.
C# NOTE
  ContainsKey followed by operations[c] hashes the token twice; TryGetValue(c,
  out var fn) does it once and returns the delegate in the same call.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
