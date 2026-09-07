// --------------------------------------------------------------------------
// -  optimal-variant.cs    O(n) time / O(n) space
// -  doubly linked list splicing as implicit stack
// -  [linked-list-simulated-stack]
// -  ties with optimal.cs on O(n) time / O(n) space
// -
// -  Reference solution - not one you solved yourself (was optimal.cs)
// -
// -  walks tokens as linked nodes, recycling operator nodes into results
// -  and relinking prev pointers to pop two operands and push one, in a
// -  single forward sweep
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
 PATTERN : Stack as a prev-chain, folded in place over the tokens
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Optimal variant - ties the best complexity by another route
================================================================================
MENTAL MODEL
  Read this as a stack machine where the stack is spelled with prev pointers
  instead of a Stack<int>.

  First loop just copies tokens into a doubly linked chain: head is tokens[0],
  curr walks forward, every new node gets prev = curr. Nothing is evaluated yet.

  Second loop is the evaluator. Walk forward with head. At any moment, head.prev
  is the top of the operand stack, head.prev.prev is the one below it, and head
  itself is the token being read. Pushing is free - it already happened when the
  chain was built - so the only real work is popping, which is a prev rewire.
INVARIANT
  When the loop reaches node head, every node reachable by following prev from
  head is an already-evaluated operand, in stack order, top first. Operators
  that have been processed are no longer in that prev chain; they were rewritten
  into their own numeric result and spliced in as the new top.

  The forward chain (next) is never rewritten in a way the loop reads. It stays
  the original token order and is purely the input cursor.
ALGORITHM
  1. Build the chain, head = tokens[0], each later node linked both ways.
  2. Walk head forward. If head.val is not an operator, it is an operand and
  needs no action - it is already on the stack by virtue of being in the prev
  chain.
  3. If head.val is an operator, pop two: left = head.prev.prev.val, right =
  head.prev.val.
  4. Compute, then overwrite head.val with result.ToString(). The operator node
  becomes the result node.
  5. Splice the two operands out: head.prev = head.prev.prev.prev, and if that
  is not null, patch its next.
  6. ans = int.Parse(head.val) on every node; advance head = head.next.
WHY THE ANSWER IS THE LAST NODE
  ans is reassigned at every node, so what is returned is whatever the final
  node parsed to. That is correct for two reasons.

  A valid RPN expression of length > 1 ends in an operator, and by step 4 that
  operator node holds the fully folded result by the time ans reads it. For the
  single-token input like ["5"], the loop runs once, the branch is skipped, and
  ans is that number - the same code path covers it with no special case.

  The int.Parse on operand nodes is pure waste: those values get consumed later
  and their parse result is thrown away. It is harmless only because every node
  holds a valid integer string at the moment it is visited - operators are
  rewritten one statement earlier.
WATCH OUT
  Operand order. left is head.prev.prev, right is head.prev - the deeper node is
  the left operand. Swap them and + and * still pass while - and / silently
  invert. This is the first thing to check when the tests fail on a mixed
  expression.

  Integer division. left / right on int truncates toward zero in C#, which is
  exactly what RPN evaluation asks for. Do not "fix" it with Math.Floor - that
  turns -7 / 2 from -3 into -4.

  The null guard. head.prev.prev.prev is null precisely when the two operands
  consumed were the bottom of the stack. head.prev = null is then correct (empty
  stack below the new top) and the if is there only to avoid dereferencing null
  for the next patch.

  "+-* /".Contains(head.val) is substring matching, not set membership. It
  happens to work because no numeric token in valid input is a substring of "+-*
  /", but the empty string would match, and a two-char token like "* /" would
  too. head.val.Length == 1 && "+-* /".Contains(head.val[0]) says what you
  meant.

  No guard on tokens.Length before tokens[0], and no guard that head.prev.prev
  exists before an operator. Malformed input throws rather than reporting.
THE DEAD WRITE
  head.prev.next = head is never read by this algorithm. The node it patches
  sits strictly earlier in the original token order, so the loop already visited
  it and already consumed its next on the way here. The only forward read
  remaining is head.next on the current node, which that line does not touch.

  It is there to keep the doubly linked list honest, not because the evaluation
  needs it. Notice what that implies: the next pointers are only ever the input
  array read in order. Drop them and you are left with a for loop over tokens
  plus a prev-chain stack - which is the plain Stack<int> solution.

  That is the honest cost comparison for this file. Same work, but every result
  makes an int -> string -> int round trip through result.ToString() and
  int.Parse, and each node carries two references the algorithm does not need.
INTERVIEWER FOLLOW-UP
  "Your stack is a linked list - what breaks if I hand you a huge expression?"
  Nothing about correctness; the recursion-free forward walk is fine. The answer
  they want is that this allocates a node per token up front whether or not the
  token is ever on the stack, whereas a stack only ever holds unconsumed
  operands.

  "Why is one pass enough?" Because RPN needs no lookahead: an operator's
  operands are always the two most recently completed values, and step 5 keeps
  that guarantee by making the result node the new top before moving on.

  "Make it handle overflow / doubles." The weak point is storing values back as
  strings in head.val. Committing to a typed stack removes the parse round trip
  and makes the element type a one-line change.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
