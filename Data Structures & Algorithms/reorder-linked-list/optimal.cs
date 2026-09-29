// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(n) space
// --------------------------------------------------------------------------

public class Solution
{
    public void ReorderList(ListNode head)
    {
        ListNode slow = head;
        ListNode fast = head;

        while (fast != null && fast.next != null)
        {
            slow = slow.next;
            fast = fast.next.next;
        }
        // slow is at middle

        // cut the list
        ListNode second = slow.next;
        slow.next = null;
        second = ReverseList(second);

        ListNode first = head;

        while (second != null)
        {
            ListNode firstNext = first.next;
            ListNode secondNext = second.next;

            first.next = second;
            second.next = firstNext;

            first = firstNext;
            second = secondNext;
        }
    }

    private ListNode ReverseList(ListNode head)
    {
        if (head == null)
            return head;

        ListNode newHead = head;
        if (head.next != null)
        {
            newHead = ReverseList(head.next);
            head.next.next = head;
        }
        head.next = null;
        return newHead;
    }
}

/*
================================================================================
 PATTERN : Fast/Slow Pointers + Reverse + Merge: split, flip, weave
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  slow         after the first loop: last node of the first half
  fast         moves two steps for each step of slow; stops the loop at the end
  second       head of the second half, then head of the reversed second half
  first        current node in the first half during the weave
  firstNext    saved first.next, kept before first.next is changed
  secondNext   saved second.next, kept before second.next is changed
  newHead      old tail of the list, which becomes the head after reversal
WHY THIS PATTERN
  The problem wants the order L0, Ln, L1, Ln-1, and so on. You must take nodes
  from the end, but a singly linked list can only move forward. So the code
  finds the middle with slow and fast, cuts the list there, and reverses the
  back half. Now the "end" nodes sit at the front of second. After that, one
  forward pass weaves first and second together.
BRUTE FORCE
  The simplest correct version copies every node into a List<ListNode>. Then two
  indexes, one at each end, move toward each other and relink the nodes in the
  new order. This takes O(n) time and O(n) extra space. It loses because the
  split-reverse-merge method can run in O(1) extra space, and the array does not
  add anything useful.
INVARIANT
  After the cut, the first half is the same length as the second half or one
  node longer. Each weave step joins one node from first to one node from second
  and then moves both forward. So first is never null while second is still
  non-null. The loop ends when second runs out. The last first node already has
  next = null from the line slow.next = null, so the list ends correctly with no
  cycle.
SAVE BOTH NEXTS BEFORE RELINKING
  Each weave step changes first.next and second.next. So firstNext and
  secondNext must be saved before those two writes. If you skip either save, you
  lose the rest of that half. The order is: save both, link first to second,
  link second to firstNext, then move forward.
WATCH OUT
  If head is null, slow is null and slow.next throws a NullReferenceException.
  There is no guard at the top of ReorderList. The comment "slow is at middle"
  is only exact for odd lengths. For even lengths, slow stops at the second of
  the two middle nodes. For example, with 1,2,3,4 it stops on 3. The result is
  still correct because the first half may be one node longer, but do not trust
  the comment when you change the split.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you make it O(1) extra space?
     Yes. Replace the recursive ReverseList with an iterative one that uses prev
     and curr pointers. The recursion uses one stack frame per node of the
     second half, so it costs O(n) space and can overflow the stack on a very
     long list. The iterative loop removes that cost.
  2. Can you do this without changing the input list?
     Not in place. You would have to build a new list or copy the nodes, which
     costs O(n) space. The in-place version is cheaper but destroys the original
     order.
  3. How would you check whether a linked list is a palindrome using the same
  pieces?
     Find the middle with slow and fast, reverse the second half, then walk both
     halves and compare values instead of weaving them. Reverse the half again
     afterward if the caller needs the list unchanged.
TRIGGER
  The list must be rearranged by pairing nodes from the front with nodes from
  the back, and you only have forward next pointers.
C# NOTE
  ReverseList uses no instance fields, so you can mark it private static. That
  makes clear it only works on the nodes passed in.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
