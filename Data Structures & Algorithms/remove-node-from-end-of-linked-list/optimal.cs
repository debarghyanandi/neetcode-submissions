// ##########################################################################
// #  optimal.cs            O(n) time / O(1) space
// #  two-pointer gap technique   [two-pointer-gap]
// #  the only solution in this folder
// #
// #  YOU SOLVED THIS YOURSELF (from submission-1)
// #
// #  advances fast pointer n steps ahead then moves both pointers together
// #  until fast reaches the end, giving a single pass with constant extra
// #  space
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
 PATTERN : Two Pointers - fixed n-gap window plus dummy head
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-1.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  "nth from the end" is a position you cannot name until you know the length,
  and a singly linked list will not tell you its length without walking it.
  Holding two pointers a fixed distance apart converts an offset-from-the-tail
  into an offset-from-the-head: when fast runs off the end, the gap has already
  carried slow to the right place. Nothing is measured, nothing is stored.
BRUTE FORCE
  Two passes: walk once counting nodes into L, then walk L-n-1 steps from dummy
  and unlink. Same asymptotics and arguably easier to argue about, but it
  traverses twice and needs L held between the passes. It also dies if the input
  is a stream you only get to see once. This file does the job in a single
  traversal.
WHY THE DUMMY EXISTS
  dummy = new ListNode(0, head) with slow = dummy makes "delete the head" an
  ordinary case: slow always has a predecessor to write through, so there is no
  if (target == head) return head.next branch. That is also why the return is
  dummy.next and not head - head is the very node that may have just been
  unlinked, so returning it would hand back the deleted node.
INVARIANT
  Index dummy as -1 and head as 0. slow starts at -1 and fast at 0, so fast -
  slow == 1. The first loop advances only fast, n times, making fast - slow ==
  n+1; the second loop steps both, so that distance is preserved to exit. The
  loop ends when fast == null, i.e. fast is conceptually at index L for a list
  of length L. Therefore slow is at L-n-1, which is exactly the predecessor of
  the nth-from-end node at index L-n. That is what the comment on the line above
  the splice is claiming.
WHY THE UNGUARDED SPLICE IS SAFE
  slow.next.next has no null check and does not need one. slow sits at L-n-1, so
  slow.next is index L-n, and since n >= 1 that is at most L-1, a real node -
  the dereference is always valid. slow.next.next may legitimately be null
  (removing the tail), but that is a read of a field on a live node and
  assigning null into slow.next is the correct result. The one genuinely
  unguarded spot is fast = fast.next in the first loop: if n exceeded the list
  length it would throw. Correctness there rests on the problem constraint n <=
  size, not on anything in the code.
WATCH OUT
  fast is initialized to head, not to dummy. That one-node head start is what
  makes n advances produce a gap of n+1; start fast at dummy and you must
  advance it n+1 times instead. The mirror-image bug is starting slow at head,
  which leaves slow standing on the target node with no way to unlink it. Also
  note n is consumed - the first loop decrements it to 0, so the parameter is
  unusable afterward; save it first if you ever need it again.
TRIGGER
  Reach for this whenever the question says "kth from the end" of a singly
  linked list under a one-pass or no-length constraint. More generally:
  constant-gap two pointers for any tail-relative index, and a dummy node for
  any deletion that could touch the head.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
