// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
// -  iterative pointer reversal   [iterative-reverse]
// -  ranks above suboptimal.cs (O(n) time / O(n) space)
// -
// -  Reference solution - not one you solved yourself
// -
// -  Single pass iterating through the linked list with constant auxiliary
// -  pointers.
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
 PATTERN : Iterative Pointer Reversal - three-pointer walk
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  prev        head of the already-reversed part, null at the start
  current     node being re-pointed right now
  nextNode    saved successor of current, before the link is overwritten
WHY THIS PATTERN
  A singly linked list can only be walked forward, and reversing it means every
  next pointer must end up aimed at the node that came before it. Each node
  needs to know its predecessor, which the list does not store, so we carry it
  ourselves in prev. One forward pass is enough because at the moment we visit
  current we already hold both the node behind it (prev) and, after the save,
  the node ahead of it (nextNode).
BRUTE FORCE
  The simplest first attempt is to walk the list, push every value into a List
  or array, then walk again and write the values back in reverse order. That is
  O(n) time but O(n) extra space, and it only works because we are allowed to
  touch the values; it does not reverse the structure. Recursion is the other
  common first try - it is O(n) time but uses O(n) stack frames, which can
  overflow on a long list.
INVARIANT
  At the top of each loop pass, prev points to the reversed chain of all nodes
  seen so far, and current points to the first untouched node of the original
  list. The body keeps that true: it saves nextNode, flips current.next to prev,
  then slides both pointers forward one step. When current is null every node
  has been flipped exactly once, so prev is the new head and the return is
  correct.
WHY NEXTNODE MUST BE SAVED FIRST
  The line current.next = prev destroys the only reference to the rest of the
  list. Saving nextNode before that write is what keeps the walk alive; swap the
  two lines and the loop would follow the link it just rewrote and spin between
  two nodes. This is the whole reason the reversal needs three pointers instead
  of two.
WATCH OUT
  Return prev, not current or head - current is always null when the loop exits,
  and head has become the tail. An empty list (head is null) never enters the
  loop and correctly returns null, so no special case is needed. The original
  head node is left with next = null and is now the last node; any caller still
  holding the old head reference is holding the tail, which surprises people who
  reverse a list in place and keep using the old variable.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Write it recursively - what changes?
     Recurse to the end, then on the way back set head.next.next = head and
     head.next = null, returning the deepest node as the new head. Same O(n)
     time, but O(n) stack space, and a very long list can throw
     StackOverflowException, which .NET does not let you catch.
  2. Reverse only the nodes between positions left and right?
     Walk to the node before left, keep it in a pointer, run this same
     three-pointer loop for right - left + 1 steps, then stitch: the node before
     left points at prev, and the node that was at left points at current. A
     dummy head node in front removes the special case where left is 1.
  3. Reverse the list in groups of k, leaving a short tail as is?
     First count ahead k nodes to confirm a full group exists; if not, stop and
     leave that part alone. Otherwise reverse the group with this loop and
     connect the previous group's tail to the new group head. Still O(n) time
     and O(1) space, just more bookkeeping pointers.
  4. How would you check if the list is a palindrome using this?
     Find the middle with slow and fast pointers, reverse the second half with
     this exact loop, compare the halves node by node, then reverse the second
     half back to restore the input. O(n) time, O(1) space.
TRIGGER
  A singly linked list problem where you need a node's predecessor, or need the
  links themselves flipped, and extra space is not allowed.
C# NOTE
  ListNode is a class, so prev, current and nextNode are references - assigning
  them copies a pointer, not a node, which is what makes the O(1) space claim
  real. Declaring ListNode nextNode inside the loop body costs nothing extra
  here; it just limits the name to the one pass where it is meaningful.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
