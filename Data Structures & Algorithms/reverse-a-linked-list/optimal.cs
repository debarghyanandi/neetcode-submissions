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
 PATTERN : Iterative Pointer Reversal - three-pointer walk
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  The task is to flip the direction of every next link in a singly linked list.
  Each node only needs to know its new successor, which is the node that came
  just before it, so one pass carrying that previous node is enough. Here prev
  holds the already-reversed part, current is the node being rewired, and
  nextNode saves the rest of the list before the link is destroyed. When current
  falls off the end, prev is sitting on the old tail, which is the new head.
BRUTE FORCE
  The simplest correct first attempt is to walk the list once and push every
  node value into a List, then walk again writing the values back in reverse
  order. That is still linear time but uses extra memory proportional to the
  list length. It also only moves values, not nodes, which fails if the
  interviewer asks for real pointer rewiring or if nodes carry identity that
  must move with them.
INVARIANT
  At the top of each loop turn, prev points to the head of a fully reversed
  chain made of all nodes already visited, and current points to the head of the
  untouched original remainder. The three assignments keep that true: nextNode
  saves the remainder, current.next = prev moves one node from the remainder
  onto the reversed chain, then both pointers shift forward one step. The loop
  exits when the remainder is empty, so prev is the reversed whole list.
WATCH OUT
  Order of the four lines inside the loop is fragile. If current.next = prev
  runs before nextNode = current.next, the link to the rest of the list is gone
  and the loop stops after one node. Also note the original head node ends up
  with head.next == null; any caller still holding head now holds a one-node
  tail, not the list. An empty list is fine: current starts null, the loop never
  runs, and prev is returned as null, which is the correct empty result.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you write this recursively?
     Yes - recurse to the end, then set head.next.next = head and head.next =
     null on the way back. Same linear time, but it uses stack space equal to
     the list length and can overflow on a long list, which the iterative
     version avoids.
  2. How would you reverse only the nodes between position m and n?
     Walk to the node before position m, keep a pointer to it, run this same
     three-pointer loop for n - m + 1 steps, then reconnect: the saved node's
     next gets the new sub-head prev, and the old sub-head's next gets current.
     Still one pass, but you need a dummy head node so that m == 1 does not need
     a special branch.
  3. What changes for a doubly linked list?
     Inside the loop you must also set current.prev = nextNode, and after the
     loop the list's tail pointer has to be updated to the old head. Same time,
     but every node now costs two writes instead of one.
  4. How would you reverse the list in groups of k?
     Reverse k nodes with this loop, stop, stitch the previous group's tail to
     the new sub-head, and repeat. Time stays linear since each node is rewired
     once, but you must first count ahead k nodes so a trailing partial group is
     left in original order.
TRIGGER
  Any singly linked list problem where the direction of next must change and you
  are not allowed extra memory - reach for the prev/current/nextNode trio.
C# NOTE
  The field is lowercase next, so this is LeetCode's ListNode definition, not a
  normal C# property; assigning to current.next is a direct field write.
  Declaring ListNode nextNode inside the loop body costs nothing extra - it is a
  reference variable on the stack, reassigned each turn, not a new allocation.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
