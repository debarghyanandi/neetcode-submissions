// ##########################################################################
// #  optimal.cs            O(n) time / O(1) space
// #  Two-pointer, fast and slow   [two-pointer-linked-list]
// #  the only solution in this folder
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  Two passes over the list with pointers spaced n apart position slow
// #  one node before removal point.
// ##########################################################################

//My solution
public class Solution
{
    public ListNode RemoveNthFromEnd(ListNode head, int n)
    {
        ListNode dummy = new ListNode(0, head);
        ListNode slow = dummy;
        ListNode fast = head;

        while (n > 0)
        {
            fast = fast.next;
            n--;
        }

        while (fast != null)
        {
            slow = slow.next;
            fast = fast.next;
        }

        //slow is now n+1 th node from the end.
        slow.next = slow.next.next;
        return dummy.next;
    }
}

/*
================================================================================
 PATTERN : Two Pointers - fixed gap, with a dummy head node
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-1.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  dummy   extra node placed before head; dummy.next is always the real head
  slow    stops on the node just before the one to remove
  fast    starts n nodes ahead of head; the loop ends when it passes the tail
WHY THIS PATTERN
  The problem counts "n from the end", but a singly linked list can only be
  walked forward. You cannot see the end until you reach it. So you move fast
  forward first, then move slow and fast together, one step each. The gap
  between them never changes. When fast falls off the end, slow is exactly the
  right distance from the end, and you only walk the list once.
BRUTE FORCE
  First walk the whole list to count its length L. Then walk again for L - n
  steps from dummy and unlink the next node. This is also correct, with O(n)
  time and O(1) space. It loses only because it walks the list twice and needs
  two separate loops. An interviewer usually asks for the one-pass version.
INVARIANT
  After the first loop, fast is n nodes ahead of head. slow starts at dummy,
  which is one node before head, so slow is n+1 steps behind fast. The second
  loop moves both pointers together, so that gap stays n+1 the whole time. When
  fast is null (one step past the last node), slow is n+1 steps back from null.
  That makes slow the node just before the nth node from the end, so slow.next
  is the node to remove. The code comment "slow is now n+1 th node from the end"
  matches this.
DUMMY NODE MAKES HEAD REMOVAL NORMAL
  When n equals the list length, the node to remove is head. It has no node
  before it. Because slow starts at dummy, slow simply stays on dummy, and
  slow.next = slow.next.next skips over head. Returning dummy.next instead of
  head then gives the new head, so there is no special case.
WATCH OUT
  There is no guard on n. If n is larger than the list length, fast becomes null
  inside the first loop, and the next fast.next throws a NullReferenceException.
  If n is 0 or less, the first loop does nothing, slow ends on the last node,
  and slow.next.next throws. An empty list (head == null) with n >= 1 also
  throws in the first loop. The loop also counts n down to 0, so the original
  value of n is gone after the first loop. Any later code that needs n will read
  0.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you do it recursively?
     Recurse to the end. On the way back, return a count of nodes from the end,
     and unlink the next node when the count reaches n. The code is short, but
     it uses O(n) call stack space instead of O(1), and a very long list can
     overflow the stack.
  2. How do you find the middle node in one pass?
     Use the same two pointers with different speeds, not a fixed gap. fast
     moves 2 steps and slow moves 1 step. When fast reaches the end, slow is at
     the middle.
  3. You get only a pointer to the node to delete, not head. How do you delete
  it?
     Copy node.next.val into node, then set node.next = node.next.next. This
     needs O(1) time, but it cannot delete the tail node, and it changes which
     node object holds each value.
TRIGGER
  The problem asks for the kth node from the END of a singly linked list, and
  you want one pass.
C# NOTE
  new ListNode(0, head) uses the two-argument constructor, so the dummy node and
  its link to head are made in one line. int is a value type, so decrementing n
  changes only the method's local copy, not the caller's variable.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
