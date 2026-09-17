// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
// -  Iterative three-pointer reversal   [iterative-pointer-reversal]
// -  ranks above suboptimal.cs (O(n) time / O(n) space)
// -
// -  Reference solution - not one you solved yourself
// -
// -  Single forward pass with constant variables rewiring each node's next
// -  pointer in place.
// --------------------------------------------------------------------------

public class Solution
{
    public ListNode ReverseList(ListNode head)
    {
        ListNode prev = null;
        ListNode current = head;

        while (current != null)
        {
            ListNode nextNode = current.next;
            current.next = prev;
            prev = current;
            current = nextNode;
        }
        return prev;
    }
}

/*
================================================================================
 PATTERN : Linked List - iterative three-pointer reversal
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  The task is to flip the direction of every next link in a singly linked list.
  Each node only knows its successor, so once you overwrite current.next you
  lose the rest of the list unless you saved it first. The three pointers prev,
  current and nextNode give exactly that: nextNode holds the tail before the
  cut, prev holds the already reversed part, and current is the node being
  re-pointed. One pass is enough because each link needs to be flipped exactly
  once.
BRUTE FORCE
  The simplest correct first attempt is to walk the list, push every node (or
  every value) onto a stack or List, then walk back and rebuild the links in
  reverse order. That is still O(n) time but costs O(n) extra memory. Recursion
  is the other common first answer: it is short, but it uses a call stack that
  is as deep as the list, so a long list can throw StackOverflowException. This
  loop replaces that memory with three local references.
INVARIANT
  At the top of every iteration, prev points at the head of the fully reversed
  prefix, and current points at the head of the untouched suffix; the two pieces
  together contain all original nodes exactly once. The body cuts one node off
  the suffix and pushes it onto the front of the prefix, so the invariant holds
  again. When current becomes null the suffix is empty, so prev is the head of
  the reversed whole list, which is why prev is returned and not current.
WATCH OUT
  The returned value must be prev, not head - head still refers to the original
  first node, which is now the tail and whose next is null. Return current by
  mistake and you always return null. An empty list (head == null) is handled by
  luck rather than a special case: the loop body never runs and prev is still
  null, which is the right answer. Also note the original head's next is set to
  null on the first iteration, so the caller's old head reference no longer
  reaches any other node; if the caller needed the old order, it is gone because
  this reverses in place.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Write the recursive version and say what it costs.
     Recurse to the end, get the new head back, then set current.next.next =
     current and current.next = null on the way up. Same O(n) time but O(n)
     stack depth, so it risks a stack overflow on a long list where this loop
     does not.
  2. Reverse only the nodes between positions left and right, leaving the rest
  attached.
     Walk to the node before left and keep it as a fixed anchor, run this same
     three-pointer loop for right - left + 1 steps, then reconnect: anchor.next
     becomes the new sub-head, and the old sub-head's next becomes the node
     after right. A dummy node in front of head removes the special case when
     left is 1.
  3. How do you detect that the list has a cycle before reversing it?
     Run Floyd's fast and slow pointer first; if they meet, there is a cycle.
     Without that check this loop never terminates on a cyclic list, because
     current never reaches null.
  4. Reverse the list in groups of k nodes.
     Count k nodes ahead; if fewer than k remain, leave them as they are.
     Otherwise reverse that block with this loop, join the previous block's tail
     to the new block head, and continue from the block's original first node,
     which is now its tail. Still O(n) time and O(1) space.
TRIGGER
  When a singly linked list must be re-pointed in place and you need the
  successor saved before overwriting next, reach for the prev / current /
  nextNode trio.
C# NOTE
  ListNode is a class, so prev, current and nextNode are references: assigning
  them copies a pointer, not a node, which is what makes the O(1) space claim
  real. Note the field is lowercase next, matching LeetCode's provided class
  rather than normal C# PascalCase property style - keep that in mind if you
  retype the node class yourself.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
