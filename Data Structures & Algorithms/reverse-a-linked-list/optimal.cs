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
 PATTERN : Linked List / Iterative Pointer Reversal - three pointers
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  prev      the head of the part already reversed (null at the start)
  current   the node we are about to flip
  nextNode  saved link to the rest of the list, kept before current.next is overwritten
WHY THIS PATTERN
  The problem asks us to reverse a singly linked list, which means every next
  arrow must point the other way. A node only knows its next node, so we walk
  the list once and flip each arrow as we pass it. The loop moves prev and
  current forward together. When current becomes null, prev is the old tail, and
  the old tail is the new head.
BRUTE FORCE
  The simple first idea is to push every node onto a Stack<ListNode>, or copy
  the values into a List<int>. Then you pop the nodes and relink them, or write
  the values back in reverse order. This is also O(n) time, but it needs O(n)
  extra space for the stack or list. The in-place version does the same job with
  only three pointers.
INVARIANT
  At the top of each loop, prev is the head of a fully reversed list made of all
  the nodes already visited. current is the head of the untouched rest of the
  list. The two parts never share a node. Each loop step moves exactly one node
  from the front of the rest onto the front of the reversed part. So when
  current is null, the whole list has been reversed and prev is its head.
ORDER OF THE FOUR ASSIGNMENTS
  The four lines must run in exactly this order: save nextNode, flip
  current.next, move prev, move current. If you flip current.next before you
  save nextNode, you lose the only link to the rest of the list. A way to
  remember it: each line's left side is the right side of the line before it, so
  the names chain together in a diagonal (nextNode, current.next, prev,
  current).
WATCH OUT
  Return prev, not current or head. At the end current is null, and head is now
  the tail, whose next is null. An empty list (head == null) works: the loop
  never runs and prev is null. Do not start prev at head. If you do, the first
  node points to itself and the list becomes a cycle.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you write it recursively?
     Reverse head.next first, then set head.next.next = head and head.next =
     null, and return the new head from the deepest call. The code is shorter,
     but it uses O(n) call stack, and a very long list can overflow the stack.
  2. Reverse only the nodes from position left to right (Reverse Linked List
  II).
     Walk to the node just before left. Run this same loop for right - left + 1
     steps. Then reconnect the node before the range to the new front of the
     range, and the old front of the range to the node after it. A dummy node
     makes left = 1 work without a special case.
  3. Reverse the list in groups of k (Reverse Nodes in k-Group).
     First check that k nodes are left. If they are, reverse those k nodes with
     this loop and connect the group to the previous group's tail. A group with
     fewer than k nodes stays as it is. It is still O(n) time and O(1) extra
     space.
TRIGGER
  When a problem asks you to reverse, reorder, or check a palindrome on a singly
  linked list in O(1) extra space, reach for prev/current/next pointer flipping.
C# NOTE
  You can replace the four-line body with one tuple assignment: (current.next,
  prev, current) = (prev, current, current.next). C# evaluates every value on
  the right side before it assigns any of them, so no temporary nextNode is
  needed.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
