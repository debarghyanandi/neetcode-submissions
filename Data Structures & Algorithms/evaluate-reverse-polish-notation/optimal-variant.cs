// --------------------------------------------------------------------------
// -  optimal-variant.cs    O(n) time / O(n) space
// -  Doubly linked list with pointer rewiring   [doubly-linked-list-rpn]
// -  ties with optimal.cs on O(n) time / O(n) space
// -
// -  Reference solution - not one you solved yourself
// -
// -  Single traversal through n nodes; operator nodes store results and
// -  update prev pointers to skip consumed operands.
// --------------------------------------------------------------------------

public class DoublyLinkedList
{
    public string val;
    public DoublyLinkedList next;
    public DoublyLinkedList prev;

    public DoublyLinkedList(string val, DoublyLinkedList next = null,
                            DoublyLinkedList prev = null)
    {
        this.val = val;
        this.next = next;
        this.prev = prev;
    }
}

public class Solution
{
    public int EvalRPN(string[] tokens)
    {
        DoublyLinkedList head = new DoublyLinkedList(tokens[0]);
        DoublyLinkedList curr = head;

        for (int i = 1; i < tokens.Length; i++)
        {
            curr.next = new DoublyLinkedList(tokens[i], null, curr);
            curr = curr.next;
        }

        int ans = 0;
        while (head != null)
        {
            if ("+-*/".Contains(head.val))
            {
                int l = int.Parse(head.prev.prev.val);
                int r = int.Parse(head.prev.val);
                int res = 0;
                if (head.val == "+")
                {
                    res = l + r;
                }
                else if (head.val == "-")
                {
                    res = l - r;
                }
                else if (head.val == "*")
                {
                    res = l * r;
                }
                else
                {
                    res = l / r;
                }

                head.val = res.ToString();
                head.prev = head.prev.prev.prev;
                if (head.prev != null)
                {
                    head.prev.next = head;
                }
            }

            ans = int.Parse(head.val);
            head = head.next;
        }

        return ans;
    }
}

/*
================================================================================
 PATTERN : Stack via linked list - fold each operator into its node
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Optimal variant - ties the best complexity by another route
================================================================================
VARIABLES
  head    the node being processed now; it walks forward and is not kept as the list start
  curr    the tail of the list while it is being built
  l       the left operand, read from head.prev.prev
  r       the right operand, read from head.prev
  res     the result of l (op) r; it replaces the operator text in head.val
  ans     the value of the last node processed, which is the final answer at the end
WHY THIS PATTERN
  In Reverse Polish Notation (RPN), every operator uses the two values that come
  just before it. This is last-in, first-out access, so a stack fits. Here the
  nodes behind head, reached through prev, are the stack. When head is an
  operator, the code reads the two nodes behind it, writes res into head.val,
  and unlinks the two operand nodes. The operator node then becomes the new top
  of the stack.
BRUTE FORCE
  The first idea most people have: scan the token list for the first operator,
  replace it and its two operands with the result, and repeat until one token is
  left. Each pass is O(n), and there can be up to about n/2 passes, so the total
  is O(n^2). It loses because it rescans from the start after every reduction.
  This file avoids the rescan: the result stays where the operator was, and head
  just moves on.
INVARIANT
  Before each step, the nodes reached by walking prev from head.prev hold the
  evaluated values of every token already processed, in order. Together they
  form a correct RPN stack, with the top at head.prev. An operator pops two
  values (l, r) and pushes res into its own node, so the stack stays correct.
  For valid input, exactly one value is left after the last token, and ans holds
  it.
OPERAND ORDER
  l comes from head.prev.prev (deeper in the stack) and r comes from head.prev
  (the top). This order matters for "-" and "/". If you swap them, "4 2 -" gives
  -2 instead of 2.
WATCH OUT
  "+-* /".Contains(head.val) checks for a substring, not an exact match. It
  works for negative numbers like "-11" only because "-11" is not inside "+-*
  /". An empty-string token would match and crash. tokens[0] throws if the array
  is empty. l * r and l + r can overflow int without any error. The class is
  named DoublyLinkedList, but it is really a single node. The code also never
  checks for malformed input: if an operator comes too early, head.prev.prev is
  null and the code throws NullReferenceException.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you do it with O(1) extra space?
     Yes. Reuse the tokens array as the stack with a top index: write each value
     to tokens[top] and pop by moving top back. The trade-off is that you
     destroy the input, and each value is stored as a string, so it is parsed
     again when it is read.
  2. Why not just use Stack<int>?
     It is shorter, it has no node allocations or pointer splicing, and each
     number is parsed only once. This linked-list version shows the same idea,
     but it holds strings and calls ToString and Parse again on every result.
  3. How would you evaluate a normal infix expression like "3 + 4 * 2"?
     Use two stacks, one for values and one for operators, and apply operators
     by precedence. Or convert to RPN first with the shunting-yard algorithm,
     then run this code. It is still O(n), but you must handle precedence and
     parentheses.
  4. How would you add more operators, such as "%" or "^"?
     Replace the if-chain with a Dictionary<string, Func<int,int,int>>. The
     operator check and the calculation then both become one lookup.
TRIGGER
  When each operator or closing symbol acts on the most recent unfinished items
  before it, think of a stack.
C# NOTE
  C# integer division truncates toward zero, so l / r already gives the rounding
  RPN problems usually ask for. In Python, // rounds down (floor) instead and
  would need int(l / r).
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
