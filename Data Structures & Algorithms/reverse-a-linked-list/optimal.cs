// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
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
 PROBLEM : You get the head of a singly linked list. Reverse the list in place
           and return the new head (the old tail). An empty list returns null.
           Example: 1 -> 2 -> 3 -> null -> 3 -> 2 -> 1 -> null.
 PATTERN : Linked List In-Place Reversal (three pointers)
================================================================================
IDEA
  Walk the list once with current, keeping prev as the node behind it.
  At each node, save current.next in nextNode, point current.next back to
  prev, then move prev and current one step forward.
  It is correct because prev always heads a fully reversed prefix, and
  nextNode keeps the rest reachable. When current is null, prev is the head.
EXAMPLE
  Input 1 -> 2 -> 3. Start prev=null, current=1.
  Step 1: nextNode=2, 1.next=null, prev=1, current=2.
  Step 2: nextNode=3, 2.next=1, prev=2, current=3.
  Step 3: nextNode=null, 3.next=2, prev=3, current=null. Return 3 -> 2 -> 1.
COMPLEXITY
  Time  O(n)  each node is visited once, with O(1) work per node
  Space O(1)  only three pointers (prev, current, nextNode), no copies
PATH TO OPTIMAL
  Copy values to an array, rebuild or write back reversed - O(n) space.
  Stack or recursion (suboptimal.cs) - O(n) space, still one pass of nodes.
  Iterative pointer flip (this file) - O(1) space, no stack overflow risk.
KEYWORDS
  linked list, in-place reversal, three pointers, prev/current/next, O(1)
  space
WATCH OUT
  - Save nextNode BEFORE setting current.next = prev, or you lose the rest
    of the list.
  - Return prev, not current or head: current is null at the end, and head
    is now the tail.
  - Loop while current != null, not current.next != null, or the last node
    is never linked back.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you do it recursively?
     -> Reverse head.next, then set head.next.next = head and head.next =
        null. O(n) time but O(n) call stack; deep lists can overflow.
  2. Reverse only positions left..right (Reverse Linked List II)?
     -> Walk to the node before left, run the same flip right-left+1 times,
        then reconnect both ends. Still O(n) time, O(1) space.
  3. Reverse in groups of k?
     -> Check k nodes exist, flip that group with this loop, link the old
        group head to the next group. O(n) time, O(1) space iteratively.
  4. Check if a linked list is a palindrome in O(1) space?
     -> Find the middle with slow/fast pointers, reverse the second half with
        this code, compare halves, then reverse it back to restore the input.
TRIGGER
  When a problem asks to reverse or reorder links of a linked list without
  extra memory, reach for the prev/current/next pointer flip.
================================================================================
*/
