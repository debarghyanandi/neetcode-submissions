// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(n) space
// -  doubly linked list splicing as implicit stack
// -  [linked-list-simulated-stack]
// -  ranks above optimal-variant.cs (O(n) time / O(n) space)
// -
// -  Reference solution - not one you solved yourself
// -
// -  walks a linked list of tokens, rewriting operator nodes in place and
// -  splicing out consumed operand nodes to mimic pop-pop-push
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
                int left = int.Parse(head.prev.prev.val);
                int right = int.Parse(head.prev.val);
                int result = 0;
                if (head.val == "+")
                {
                    result = left + right;
                }
                else if (head.val == "-")
                {
                    result = left - right;
                }
                else if (head.val == "*")
                {
                    result = left * right;
                }
                else
                {
                    result = left / right;
                }

                head.val = result.ToString();
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
 PATTERN : Doubly linked list used as an in-place stack
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Optimal
================================================================================
MENTAL MODEL
  The prev chain hanging off the cursor IS the operand stack. Every token
  becomes a node up front, then a single forward sweep drives head through them.
  When head lands on an operator, head.prev is the top of stack (the right
  operand) and head.prev.prev is the one below it (the left operand) - exactly
  the two pops an RPN evaluator performs. Nothing is ever pushed as a new
  object: the operator node itself is recycled into the value it produces.
THE SPLICE
  Two lines do the whole stack transaction. head.val = result.ToString()
  converts the operator node into a number node - that is the push. head.prev =
  head.prev.prev.prev unlinks right operand, left operand, and lands on whatever
  was beneath them - that is the two pops, in one assignment. The invariant
  restored after every operator: walking prev from the cursor enumerates the
  current stack, top down. If the new head.prev is null the stack is empty below
  this node, which is why the back-link fix sits behind a guard.
WHY ANS IS THE ANSWER
  ans = int.Parse(head.val) runs on EVERY node, operand or operator, so it
  simply remembers the last node visited. That is the result only because a
  well-formed RPN expression ends in an operator (or is a lone number like
  ["18"], which the loop handles by falling straight through), and by the time
  the cursor leaves that final node its val has already been overwritten. There
  is no "exactly one value remains" check anywhere - the code trusts the input
  shape. tokens[0] also assumes a non-empty array.
OPERATOR TEST - THE TRAP
  "+-* /".Contains(head.val) is a SUBSTRING test on a string, not membership in
  a set of chars. It is correct here because "-3" is not a substring of "+-* /",
  so negative operands classify as numbers. But it would call the empty string
  an operator, and it is precisely why you cannot shortcut to "inspect the first
  character": "-" and "-3" share one.
INTEGER DIVISION
  The final else does left / right on int, and C# truncates toward zero: -7 / 2
  is -3, not -4. That is the semantics these problems ask for; a floor-dividing
  language (Python's //) needs an explicit truncation to match. No guard on
  right == 0.
DEAD BOOKKEEPING
  if (head.prev != null) head.prev.next = head; repairs the forward pointer
  behind the cursor - but no such pointer is ever read again. The sweep only
  ever does head = head.next from the current node forward, and those next
  fields were never modified. Delete the whole if and the answer is unchanged.
  Keep it only to leave the structure a genuinely valid doubly linked list; know
  that it is not load-bearing.
WHAT A PLAIN STACK BUYS
  Stack<int> matches this on both resource counts and does strictly less work:
  this version round-trips each computed value through a string
  (result.ToString(), then re-parsed on the next visit) and allocates a node per
  token. Nothing here is asymptotically worse - if an interviewer probes, the
  honest framing is that the doubly linked list is a stack written out by hand,
  with the operator node standing in for the push slot.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
