// ##########################################################################
// #  optimal.cs            O(n) time / O(1) space
// #  iterative digit-by-digit addition with carry using dummy head
// #  [iterative-carry-addition]
// #  ranks above suboptimal.cs (O(n) time / O(n) space)
// #
// #  YOU SOLVED THIS YOURSELF (from submission-1)
// #
// #  single pass with constant extra variables (carry, pointers), building
// #  only the required output list
// ##########################################################################

public class Solution {
    public ListNode AddTwoNumbers(ListNode l1, ListNode l2)
    {
        // my solution
        ListNode dummy = new ListNode(0);
        ListNode current = dummy;

        int carry = 0;
        while (l1 != null || l2 != null || carry != 0)
        {
            int digit1 = l1 != null ? l1.val : 0;
            int digit2 = l2 != null ? l2.val : 0;

            int sum = digit1 + digit2 + carry;
            carry = (sum) / 10;

            current.next = new ListNode(sum % 10);
            current = current.next;

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
 PATTERN : Little-endian digit add - one pass, carry as state
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-1.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  The digits are stored least-significant-first, which is exactly the order
  addition needs. Column addition propagates carries from the ones place upward,
  and a singly linked list can only be walked head to tail - here those two
  directions agree, so no reversal, no stack, no recursion is needed.
  Recognizing that the input representation already matches the dependency order
  of the algorithm is the whole insight; everything else in the method is
  bookkeeping.
INVARIANT
  At the top of each iteration: the nodes from dummy.next through current hold
  the correct digits for every column already processed, and carry holds the
  single value spilling out of those columns into the next one. carry is always
  0 or 1, never more - sum = digit1 + digit2 + carry is at most 9 + 9 + 1 = 19,
  so sum / 10 cannot reach 2 and sum % 10 is always a legal digit. That bound is
  why one int suffices as the entire carried state, and it is the correctness
  argument an interviewer is fishing for.
THE THREE-PART LOOP CONDITION
  l1 != null || l2 != null || carry != 0 - the third clause is the one people
  forget, and dropping it makes 5 + 5 return [0] instead of [0, 1]. It exists so
  the final carry gets its own node after both lists are exhausted. It also
  cannot loop forever: once l1 and l2 are both null, digit1 and digit2 are 0, so
  sum equals carry which is at most 1, making the new carry 0. The condition
  fails on the next check, so the tail can grow by at most one node.
WHY THE DUMMY NODE
  dummy = new ListNode(0) exists purely so that current.next = ... is a legal
  statement on the very first iteration. Without it the loop body needs an if
  (head == null) branch to special-case the first append, and current would have
  to be tracked as possibly-null. The price is one throwaway node. Return
  dummy.next, never dummy - returning dummy would prepend a spurious 0, and
  because the list is least-significant-first, that spurious 0 would sit in the
  ones place and multiply the answer by ten.
HOW UNEQUAL LENGTHS DISAPPEAR
  The digit1 and digit2 ternaries substitute 0 once a list runs out - virtual
  zero padding of the shorter number, which does not change its value. That is
  why there is no second drain loop copying the remainder of the longer list.
  Note that the advance is a separate guarded if, not folded into the ternary:
  an exhausted l1 stays null rather than dereferencing null.next, and the same
  iteration can still consume l2.
WATCH OUT
  The shortcut that seems obvious - decode both lists into a numeric type, add,
  re-encode - is wrong, not just inelegant. There is no length bound on the
  input, and anything past 19 digits overflows long; the linked-list-of-digits
  representation is the standard way to hold integers too big for a machine
  word, so undoing it defeats the point. This loop never overflows because it
  only ever adds three single-digit quantities.
FOLLOW-UPS TO EXPECT
  1. Digits stored most-significant-first instead. This loop breaks outright,
  because addition must start at the tail and forward pointers cannot get there
  cheaply. Real answers: reverse both lists and reverse the result, push both
  onto stacks and pop in lockstep, or recurse to the ends and add on the way
  back up.
  2. Can you avoid the allocations? Yes - overwrite l1's own nodes with sum % 10
  for as long as l1 lasts and only allocate past its end. This version
  deliberately does not: l1 and l2 are read-only here, so the caller's inputs
  survive the call.
  3. The space figure is auxiliary only. The result list is max(len(l1),
  len(l2)) + 1 nodes, but that is the required output, not scratch space; only
  dummy, current, carry, digit1, digit2 and sum are working storage.
TRIGGER
  Reach for this shape when two sequences are combined position by position,
  each position's output depends on the previous position only through a small
  bounded piece of state, and you are allowed to visit positions in the order
  that dependency flows. Big-integer addition and subtraction, base-b conversion
  with a running remainder, and binary string addition are all the same loop
  with a different carry rule.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
