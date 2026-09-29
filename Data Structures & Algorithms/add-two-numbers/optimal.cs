// ##########################################################################
// #  optimal.cs            O(n + m) time / O(1) space
// #  Iterative linked list traversal with carry
// #  [linked-list-carry-iteration]
// #  ranks above suboptimal.cs (O(n + m) time / O(n + m) space)
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  Single pass through both lists; carry propagates forward in constant
// #  space.
// ##########################################################################

public class Solution
{
    public ListNode AddTwoNumbers(ListNode l1, ListNode l2)
    {
        // my solution
        ListNode dummy = new ListNode(0);
        ListNode res = dummy;

        int carry = 0;
        while (l1 != null || l2 != null || carry != 0)
        {
            int x = l1 != null ? l1.val : 0;
            int y = l2 != null ? l2.val : 0;

            int sum = x + y + carry;
            carry = (sum) / 10;

            res.next = new ListNode(sum % 10);
            res = res.next;

            if (l1 != null)
                l1 = l1.next;

            if (l2 != null)
                l2 = l2.next;

        }

        return dummy.next;
    }
}

/*
================================================================================
 PATTERN : Linked List Traversal - digit-by-digit add with carry
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-1.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  dummy    fake head node; the real answer starts at dummy.next
  res      tail pointer: the last node built so far, not the result itself
  carry    0 or 1, the ten carried into the next digit
  x        current digit of l1, or 0 when l1 has run out
  y        current digit of l2, or 0 when l2 has run out
  sum      x + y + carry, a value from 0 to 19
WHY THIS PATTERN
  The digits are stored in reverse order, so the head of each list is the ones
  digit. That is the same order you use to add numbers by hand on paper. So one
  walk over both lists at the same time, pushing carry forward, builds the
  answer from the lowest digit up. Using dummy removes the special case of
  creating the first output node.
BRUTE FORCE
  The first idea is to read each list into a number, add the two numbers, and
  turn the result back into a list. With int or long this is wrong: a long list
  overflows the type and gives a wrong answer. With BigInteger it is correct and
  still linear, but it does extra conversion passes and needs extra memory for
  the big numbers. The digit walk does the same work in one pass with only carry
  as state.
INVARIANT
  After each loop pass, the list from dummy.next to res holds the lowest digits
  of the true sum, and carry holds the part of the sum that has not been written
  yet. Since sum is never more than 9 + 9 + 1 = 19, carry is always 0 or 1. The
  loop stops only when both lists are empty and carry is 0, so nothing is left
  to write and the list is the full sum.
CARRY IN THE LOOP CONDITION
  The check carry != 0 in the while condition is what writes the last extra
  digit, for example 5 + 5 = 10 gives 0 -> 1. Without it, you would need a
  separate "if (carry > 0)" after the loop. Treating a finished list as digit 0
  (x and y) means lists of different lengths need no extra loops.
WATCH OUT
  The name res sounds like "result", but it moves forward every pass and ends at
  the last node. Returning res by mistake gives back only one node, so the
  correct return is dummy.next. Reassigning l1 and l2 is safe: C# passes the
  reference by value, so the caller's lists and variables do not change. The
  code assumes every node holds a digit from 0 to 9. A value like 12 would still
  run, but the output would not be a valid digit list.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. What if the digits are stored most-significant first (Add Two Numbers II)?
     Push each list's values onto two stacks, or reverse both lists first. Then
     pop and add with carry, and put each new node at the front of the answer.
     Stacks use O(n + m) extra memory. Reversing in place uses O(1) but changes
     the input lists.
  2. Can you avoid creating new nodes?
     Yes. Write each sum digit into the nodes of the longer list and attach one
     new node only for a final carry. This saves memory, but it destroys the
     input, and the caller may not expect that.
  3. Can you write it recursively?
     Pass carry down as a parameter and build the node on the way back:
     node.next = Add(l1.next, l2.next, newCarry). It reads cleanly, but it uses
     O(n + m) stack depth, and a very long list can overflow the stack.
TRIGGER
  Two numbers given as lists or arrays of digits, lowest digit first, that must
  be added or combined without turning them into a single number.
C# NOTE
  The null checks can be shorter with null-conditional operators: int x =
  l1?.val ?? 0; and l1 = l1?.next;. This does the same thing in fewer lines.
COMPLEXITY
  Time  : O(n + m)
  Space : O(1)
================================================================================
*/
