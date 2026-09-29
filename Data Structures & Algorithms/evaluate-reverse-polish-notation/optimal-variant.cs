// --------------------------------------------------------------------------
// -  optimal-variant.cs    O(n) time / O(n) space
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
 PROBLEM : You get an array of string tokens for an expression in Reverse
           Polish Notation: each operator comes after its two operands. Tokens
           are integers or one of + - * /. Return the integer value. Division
           truncates toward zero. Example: ["2","1","+","3","*"] -> 9.
 PATTERN : Stack (simulated with a doubly linked list)
================================================================================
IDEA
  First copy all tokens into a doubly linked list. Then walk it with head.
  When head.val is an operator, head.prev.prev is the left operand l and
  head.prev is the right operand r. The result goes into the operator node.
  The two operand nodes are then unlinked, so the node becomes a number.
  It is correct because the prev chain acts as a stack: the two nearest
  unused values on the left are the operator's operands. The optimal.cs
  file uses a real stack instead.
EXAMPLE
  ["4","-7","2","/","-"]: at "/" l=-7, r=2, res=-3 (truncates toward zero).
  List is now 4 <-> -3 <-> "-". At "-" l=4, r=-3, res=7, prev becomes null.
  Answer: ans = 7 (the "-7" token is not an operator, because Contains is
  false for it).
COMPLEXITY
  Time  O(n)  one pass to build the list, one pass that splices in O(1) per
              token
  Space O(n)  one list node per token
WATCH OUT
  - Order matters for - and /: l is head.prev.prev and r is head.prev.
    If you swap them, "4 2 -" gives -2.
  - The operator test "+-* /".Contains(head.val) is a substring test. It is
    safe only because the tokens are valid. "" or "+-" would also match.
  - Every result goes through ToString and int.Parse again, so l * r can
    overflow int with no warning. The walk also moves head, so the name
    "head" does not mean the list start.
================================================================================
*/
