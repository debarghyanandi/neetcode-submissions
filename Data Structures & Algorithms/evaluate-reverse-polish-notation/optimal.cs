// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(n) space
// -  stack simulation via doubly linked list splicing
// -  [linked-list-simulated-stack]
// -  ranks above optimal-variant.cs (O(n) time / O(n) space)
// -
// -  Reference solution - not one you solved yourself (from submission-0)
// -
// -  builds a doubly linked list of all tokens, then walks it collapsing
// -  each operator with its two preceding nodes via pointer splicing,
// -  achieving the same effect as a stack in O(n) time and O(n) auxiliary
// -  space for the list.
// --------------------------------------------------------------------------

public class DoublyLinkedList {
    public string val;
    public DoublyLinkedList next;
    public DoublyLinkedList prev;

    public DoublyLinkedList(string val, DoublyLinkedList next = null,
                            DoublyLinkedList prev = null) {
        this.val = val;
        this.next = next;
        this.prev = prev;
    }
}

public class Solution {
    public int EvalRPN(string[] tokens) {
        DoublyLinkedList head = new DoublyLinkedList(tokens[0]);
        DoublyLinkedList curr = head;

        for (int i = 1; i < tokens.Length; i++) {
            curr.next = new DoublyLinkedList(tokens[i], null, curr);
            curr = curr.next;
        }

        int ans = 0;
        while (head != null) {
            if ("+-*/".Contains(head.val)) {
                int l = int.Parse(head.prev.prev.val);
                int r = int.Parse(head.prev.val);
                int res = 0;
                if (head.val == "+") {
                    res = l + r;
                } else if (head.val == "-") {
                    res = l - r;
                } else if (head.val == "*") {
                    res = l * r;
                } else {
                    res = l / r;
                }

                head.val = res.ToString();
                head.prev = head.prev.prev.prev;
                if (head.prev != null) {
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
 PATTERN : Stack discipline over a doubly linked list prev-chain
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Optimal
================================================================================
THE CORE TRICK
  There is no Stack<int> here. The tokens are strung into a doubly linked list,
  and the prev-chain behind the cursor IS the operand stack. When the cursor
  sits on a node, head.prev is the top of the stack, head.prev.prev is the one
  under it, and so on backwards. An operator node does not get removed and
  replaced; it is rewritten in place. head.val = res.ToString() turns the
  operator node itself into the pushed result, and head.prev =
  head.prev.prev.prev unlinks the two operands it just consumed. Pop-pop-push,
  expressed entirely as one field write plus one pointer hop.
INVARIANT
  When the cursor named head arrives at index i, every node reachable by
  following prev from head is a numeric string, in stack order: nearest = top.
  Operator nodes never survive the cursor passing over them, because the moment
  the cursor lands on one it is overwritten with its own result. That is why
  head.prev.prev.val can be parsed as an int with no check - the invariant
  guarantees no operator is ever sitting in the prev-chain.
ALGORITHM
  1. Build the list: head from tokens[0], then walk i from 1 setting curr.next =
  new node with prev = curr.
  2. Reuse head as a forward cursor (it stops meaning "first node" after the
  first advance).
  3. If head.val is an operator: l = int.Parse(head.prev.prev.val), r =
  int.Parse(head.prev.val), compute into res by the if/else chain, store
  res.ToString() into head.val.
  4. Splice: head.prev = head.prev.prev.prev, and if that is non-null,
  head.prev.next = head.
  5. Unconditionally ans = int.Parse(head.val), then head = head.next.
WHY THE RETURN IS CORRECT
  ans is reassigned on every single node, operand or operator, so it simply
  holds the value of the last node visited. In a well-formed RPN string the
  final token is either the outermost operator - by then rewritten to the whole
  expression's value - or, for a single-token input like ["5"], the number
  itself. Both cases fall out of the same line with no special casing. The loop
  ends when head walks off the tail into null.
WATCH OUT
  Operand order: l is head.prev.prev and r is head.prev, because the right
  operand was pushed last. Swapping them silently breaks - and only breaks - the
  "-" and "/" cases, which is exactly the bug an interviewer probes for. C# int
  division truncates toward zero, which is what this problem asks for; no
  Math.Floor.

  "+-* /".Contains(head.val) is a substring test, not a set-membership test. It
  is correct here only because no numeric token - not "5", not "-11" - happens
  to be a substring of "+-* /". It would misfire the instant a two-character
  operator or a token like "* /" entered the alphabet. head.val.Length == 1 &&
  "+-* /".Contains(head.val[0]) says what was actually meant.

  head.prev.next = head is dead maintenance. Forward traversal only ever uses
  head.next, which the splice never touches, and nothing walks next from a
  spliced-out node. Keeping the next pointers honest costs nothing but buys
  nothing either - be ready to say so rather than defend it as necessary.

  tokens[0] is dereferenced before any length check, so an empty array throws.
  Fine under the problem's guarantee, worth naming out loud.
IF ASKED FOR SOMETHING SIMPLER
  A Stack<int> doing push on numbers and pop-pop-compute-push on operators is
  the same algorithm with the scaffolding deleted: no node type, no prev/next
  bookkeeping, and no round-tripping ints through strings via res.ToString()
  followed by int.Parse on the very next visit. This version is worth keeping
  only as a demonstration that the stack discipline is what matters, not the
  container - the list is a stack you can also see.
TRIGGER
  Postfix input plus a fixed-arity operator means one left-to-right pass with a
  stack; no parsing, no precedence, no parentheses, because the token order
  already encodes the tree. Recognize it by the shape of the guarantee: every
  operator has exactly its operands immediately available behind it.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
