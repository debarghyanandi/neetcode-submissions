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
 PATTERN : Stack eval of RPN - linked list used as the stack
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Optimal variant - ties the best complexity by another route
================================================================================
VARIABLES
  head    moving cursor over the token nodes, not a fixed list head; also the write target for each result
  curr    tail pointer used only while building the node chain from tokens
  l       left operand, taken from head.prev.prev.val
  r       right operand, taken from head.prev.val
  res     result of one operation, written back into head.val as a string
  ans     value of the last node visited; after the loop this is the answer
WHY THIS PATTERN
  In reverse Polish notation an operator always applies to the two most recent
  unconsumed values to its left, so the natural structure is last-in-first-out.
  This file makes the prev chain play that role: when head.val is one of "+-*
  /", the two pending operands are exactly head.prev and head.prev.prev. Writing
  res back into head.val and then setting head.prev to head.prev.prev.prev is a
  pop-pop-push done by pointer surgery instead of a container.
BRUTE FORCE
  The first thing most people write without a stack is a repeated scan: find the
  leftmost operator, evaluate it with the two tokens before it, and splice those
  three entries into one inside a List<string>. That is correct but each splice
  shifts the tail, so it is O(n^2) time for O(n) rewrites. The stack idea gets
  the same answer in one pass because each token is touched once.
INVARIANT
  When the loop reaches a node, every node still reachable backwards through
  prev holds a plain number, and those numbers are exactly the operands not yet
  consumed, in left-to-right order. So head.prev is always the second operand
  and head.prev.prev the first. Each operator node replaces those two entries
  with one, which keeps the property true for the next node, and after the final
  token exactly one number remains - the value the last iteration stored in ans.
THE OPERATOR NODE BECOMES THE RESULT
  No node is ever deleted or allocated during evaluation. The operator node is
  reused as the result cell (head.val = res.ToString()), and the two operand
  nodes simply stop being reachable through prev. That is why head.prev =
  head.prev.prev.prev jumps three links: past the two operands, to whatever was
  pending before them.
THE NEXT REPAIR IS DEAD WORK
  Forward movement always uses head = head.next, and no node's next is ever
  changed for the node being visited, so traversal only ever follows the
  original chain. The block "if (head.prev != null) head.prev.next = head;"
  repairs a pointer that is never read again. The algorithm only needs prev to
  be correct, so the list could have been built singly linked in the prev
  direction.
WATCH OUT
  tokens[0] is read before any length check, so an empty array throws
  IndexOutOfRangeException. "+-* /".Contains(head.val) is a substring test, not
  an equality test; it happens to work only because no number token is a
  substring of "+-* /", but a stray token such as "+-" would be treated as an
  operator and then fall into the else branch and be evaluated as division.
  Malformed input where an operator appears before two numbers exist
  dereferences head.prev.prev and throws NullReferenceException. int.MinValue /
  -1 overflows silently in this unchecked arithmetic, and a "0" divisor throws
  DivideByZeroException. Every node value is parsed with int.Parse on each visit
  and results are converted back with ToString, so numbers make a full string
  round trip even though they are only ever used as ints.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How would you rewrite this with less memory?
     Use Stack<int>: push parsed numbers, and on an operator pop r then l and
     push the result. It drops n node allocations and the string round trip, and
     the stack holds only the pending operands rather than every token.
  2. Can you get O(1) extra space?
     Yes, if you may modify tokens. Keep a write index top into the same array
     and use tokens[0..top) as the stack; each operator writes its result at
     top-2 and decreases top by one. The trade-off is that the caller's array is
     destroyed.
  3. What if the expression arrives in normal infix form with parentheses?
     Run shunting-yard first to convert infix to RPN with an operator stack and
     precedence rules, then feed the output into this same evaluation loop
     unchanged.
  4. What if values can exceed int range or be decimal?
     Switch the operand type to long or decimal and parse accordingly. With
     decimal or double you lose the truncating division that RPN specifies, so
     you must apply the rounding rule explicitly.
TRIGGER
  A token sequence where each operator or action refers to the most recent
  unconsumed items before it - that "most recent first" wording means a stack.
C# NOTE
  int / int in C# truncates toward zero, which is exactly the RPN rule, so the
  else branch needs no Math.Truncate or cast - but the same expression would
  silently change meaning if l and r ever became double. Replacing the if/else
  chain with a switch on head.val is both clearer and closer to the equality
  test the code actually intends.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
