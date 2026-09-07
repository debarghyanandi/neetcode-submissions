// ##########################################################################
// #  optimal.cs            O(n) time / O(n) space
// #  explicit stack fold with operator dispatch table   [stack-eval]
// #  ties with optimal-variant.cs on O(n) time / O(n) space
// #
// #  YOU SOLVED THIS YOURSELF (was optimal-variant.cs)
// #
// #  single pass pushing operands and popping two per operator via a
// #  dictionary of lambdas, worst-case stack depth O(n)
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
 PATTERN : Stack - operator pops the two most recent values
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-1.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  Postfix notation has no parentheses and no precedence rules, because position
  already encodes grouping: an operator always applies to the two most recently
  completed values to its left. "Most recently completed, taken first" is the
  definition of a stack, so a single left-to-right pass over tokens is enough.
  No tokenizer pass, no precedence table, no recursion.
INVARIANT
  After processing any prefix of tokens, stack holds - bottom to top, in
  left-to-right order - the value of every complete subexpression seen so far.

  An operand token pushes one value (+1 depth). An operator token pops two and
  pushes one (net -1 depth). A well-formed RPN string guarantees depth is at
  least 2 whenever an operator arrives and is exactly 1 after the last token.
  That guarantee is why the code never inspects stack.Count and why the trailing
  stack.Pop() is the answer rather than a leftover.
POP ORDER IS THE WHOLE BUG SURFACE
  right is popped first and left second, because the top of the stack is the
  operand that appeared later in the input. Only then is operations[token](left,
  right) applied.

  Swap those two Pop lines and "+" and "*" still pass every test while "-" and
  "/" quietly compute left-right reversed. This is the single most likely place
  to lose the problem.

  The comment "Prev result is this" on left is loose - left is whatever
  subexpression finished immediately before right, which is often a raw operand.
  In ["2","1","+"], left is 2, not a prior result.
WHY THE CONTAINSKEY TEST COMES FIRST
  Dispatch is exact string equality against the four keys "+", "-", "*", "/". A
  token like "-11" is not one of those keys, so it falls through to int.Parse,
  which consumes the leading minus itself.

  That ordering is the only thing separating an operator from a negative
  operand. Classifying by first character instead (char.IsDigit(token[0]), or
  token == "-" checked after a digit test) is where negative literals break.
WATCH OUT
  1. int division in C# truncates toward zero: -7 / 2 is -3, not -4. That
  matches what the problem asks for, so do not reach for Math.Floor or double.
  2. operations[token] is a second hash lookup after ContainsKey already did
  one; TryGetValue collapses them into one. Correctness is unchanged - this is a
  tidiness note.
  3. The Dictionary of Func<int,int,int> is constructed fresh on every EvalRPN
  call even though the lambdas capture nothing. Hoisting it to a static readonly
  field is the natural cleanup.
  4. There is no guard for division by zero, a malformed token, or an empty
  stack; each would throw (DivideByZeroException, FormatException,
  InvalidOperationException). Acceptable under the problem's validity guarantee,
  but say that out loud rather than letting an interviewer find it.
TRIGGER
  Reach for this shape whenever the answer depends on the most recent unresolved
  item: postfix or prefix evaluation, bracket matching, undo histories,
  nested-structure decoding.

  The natural follow-up is infix input, where position no longer encodes
  grouping - that needs shunting-yard, or two stacks (values and operators) with
  a precedence comparison on push. A second follow-up is operands or
  intermediate results exceeding 32 bits, which is a Stack<long> and long
  arithmetic, nothing structural.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
