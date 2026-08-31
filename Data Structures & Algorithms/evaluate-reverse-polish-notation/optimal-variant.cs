// ##########################################################################
// #  optimal-variant.cs    O(n) time / O(n) space
// #  stack-based evaluation with operator dictionary   [stack-eval]
// #  ties with optimal.cs on O(n) time / O(n) space
// #
// #  YOU SOLVED THIS YOURSELF (from submission-1)
// #
// #  pushes operands onto a Stack<int> and pops two operands per operator
// #  using a Func lookup table, single pass O(n) time with O(n) worst-case
// #  stack space.
// ##########################################################################

public class Solution {
    public int EvalRPN(string[] tokens) {
        // My Solution
        var stack = new Stack<int>();
        var operations = new Dictionary<string, Func<int, int, int>>
        {
            ["+"] = (a, b) => a + b,
            ["-"] = (a, b) => a - b,
            ["*"] = (a, b) => a * b,
            ["/"] = (a, b) => a / b
        };

        foreach(string c in tokens)
        {
            if (operations.ContainsKey(c))
            {
                int b = stack.Pop(); // current num
                int a = stack.Pop(); // Prev result is this.
                int op = operations[c](a, b); 
                stack.Push(op);
            }
            else
            stack.Push( int.Parse(c));
        }

        return stack.Pop();
    }
}

/*
================================================================================
 PATTERN : Stack simulation - delegate table for operator dispatch
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-1.cs when it was
           first processed
 STATUS  : Optimal variant - ties the best complexity by another route
================================================================================
WHY THIS PATTERN
  RPN has no parentheses and no precedence: an operator always applies to the
  two most recently completed subexpressions immediately to its left. "Most
  recent two, then replace with one" is LIFO by definition, so a stack is not a
  clever choice here, it is the direct transcription of the notation. The
  Dictionary<string, Func<int,int,int>> named operations is just the dispatch
  half of that: it turns the token string into the binary function to apply, so
  the loop body has one shape for all four operators instead of a four-way
  branch.
INVARIANT
  After processing any prefix of tokens, stack holds the integer values of the
  maximal complete subexpressions of that prefix, ordered so the top is the
  rightmost one.

  Induction: a number token pushes a complete subexpression of size 1. An
  operator token pops the two rightmost complete subexpressions, and by the
  grammar of RPN those two are exactly its operands, so pushing their combined
  value keeps the property. Operands add 1 to the depth, operators net -1 (two
  pops, one push). A well-formed expression therefore ends at depth exactly 1,
  which is why the bare return stack.Pop() is safe with no emptiness check.
POP ORDER - THE TRAP
  b = stack.Pop() runs FIRST and is the RIGHT operand; a = stack.Pop() runs
  second and is the LEFT operand. The comments in the file say the same thing in
  different words ("current num" then "prev result"). Swapping the two lines
  silently passes every test built only from + and *, and breaks every - and /
  case.

  Concrete check: ["4","13","-"] pushes 4 then 13, so b=13, a=4, and
  operations["-"](4, 13) = -9. Reversed it would yield 9. This is the single
  most likely thing to get wrong from memory weeks later, and the first thing an
  interviewer probes.
WHY CONTAINSKEY IS THE RIGHT OPERATOR TEST
  The branch condition is operations.ContainsKey(c), not a character or length
  test. That matters because the token list can contain negative numbers: "-9"
  is an operand, "-" is an operator. A test like c[0] == '-' or
  !char.IsDigit(c[0]) classifies "-9" as subtraction and corrupts the stack.
  Membership in the four-key table is exact - only the one-character strings
  "+", "-", "*", "/" hit it - so "-9" falls through to int.Parse and pushes -9
  correctly. Keying on the same dictionary that supplies the operation is what
  makes the test and the dispatch impossible to drift apart.
INTEGER DIVISION SEMANTICS
  The problem requires division to truncate toward zero, and C# int division
  does exactly that: -7 / 2 is -3, not -4. So the plain a / b inside the ["/"]
  lambda already matches the spec with no Math.Truncate or cast to double
  wrapping it. Do not "fix" it into (int)((double)a / b) - that adds a
  rounding-mode question the int path never had. The problem statement also
  guarantees no division by zero, which is why there is no guard; say so out
  loud if asked rather than letting it look like an oversight.
WHAT THIS VARIANT COSTS
  Two things, both visible in the code and neither asymptotic.

  1. Double lookup: operations.ContainsKey(c) hashes c, then operations[c]
  hashes it again. operations.TryGetValue(c, out var fn) does it in one and
  reads cleaner.

  2. The dictionary and its four closures are constructed inside EvalRPN, so
  they are rebuilt on every call. A static readonly field would build them once;
  nothing in the lambdas captures per-call state, so the hoist is safe.

  The switch-on-string version avoids both and needs no delegate at all. This
  version's argument is uniformity, not speed: adding an operator is one
  dictionary entry rather than a new case arm.
TRIGGER
  Reach for this shape when input is a linear token stream where each element
  either produces a value or consumes a fixed number of the most recent values.
  RPN, but also: expression evaluation after shunting-yard, simplify-path, and
  basic-calculator style problems. The tell is postfix or "apply to the last k
  results" - if the notation needed parentheses to disambiguate, you are in a
  different problem and the plain stack is not enough.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
