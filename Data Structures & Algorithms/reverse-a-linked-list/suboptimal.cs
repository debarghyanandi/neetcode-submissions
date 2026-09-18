// --------------------------------------------------------------------------
// -  suboptimal.cs         O(n) time / O(n) space
// -  Recursive reversal, fix on unwind   [recursive-reversal]
// -  ranks below optimal.cs (O(n) time / O(1) space)
// -
// -  Reference solution - not one you solved yourself
// -
// -  Recurses to list end then relinks nodes on unwind; call stack depth
// -  equals list length.
// --------------------------------------------------------------------------

public class Solution
{
    public ListNode ReverseList(ListNode head)
    {
        if (head == null)
        {
            return null;
        }
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
 PATTERN : Recursion on a linked list - reverse by rewiring on the way back
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-1.cs when it was first processed
 STATUS  : Suboptimal
================================================================================
WHY THIS PATTERN
  The task is to flip every `next` pointer in a singly linked list, and a list
  is a recursive structure: head plus a smaller list. ReverseList(head.next)
  hands back the head of the already reversed tail, stored in newHead, and
  that same node is passed unchanged all the way back to the caller. The only
  local work left is one rewire: head.next.next = head makes the old successor
  point back at head, and head.next = null cuts the old forward link.
BETTER APPROACH
  The better approach here is the iterative one: walk the list with prev, curr
  and a saved next, set curr.next = prev, then slide both forward. It does the
  same number of pointer writes but uses a fixed number of local variables, so
  O(1) extra space instead of O(n). This file loses because every node gets a
  stack frame, and the frames cannot be dropped early - head.next.next = head
  runs after the recursive call returns, so each frame must stay alive holding
  its own head.
INVARIANT
  At every return, newHead is the last node of the original list, and the
  sublist starting at head.next has already been fully reversed with head still
  pointing into it. That is why head.next.next = head is safe: head.next is the
  old successor, which after reversal is now the tail of the reversed sublist,
  so appending head there extends it by one. The final head.next = null keeps
  exactly one node with a null next, which is the new end of the list.
WATCH OUT
  The depth of recursion equals the length of the list, so a long input can
  overflow the stack - this is the concrete failure mode, not a slow runtime.
  The initial `ListNode newHead = head;` matters for the single node case:
  with head.next == null nothing is recursed and head itself is returned, so do
  not "simplify" it away. Also note head.next = null is executed on both paths,
  including the base case where head.next is already null - harmless, but it
  means the function always writes to the node it was given, so it cannot be
  used on a list you must not mutate.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Rewrite it without recursion.
     Iterative three-pointer walk: prev = null, curr = head, and in the loop
     save nxt = curr.next, set curr.next = prev, prev = curr, curr = nxt; return
     prev. Same time, constant space, and no stack depth limit.
  2. Reverse only the nodes between positions left and right.
     Walk to the node before left, keep it as a pivot, then reverse that segment
     and reattach both ends. You need the node before left and the node at
     right+1, so a dummy node in front of head removes the special case where
     left == 1.
  3. Reverse the list in groups of k, leaving a shorter tail alone.
     First count k nodes ahead; if fewer than k remain, return head untouched.
     Otherwise reverse that block iteratively and set the block's original
     head's next to the result of recursing on the rest - the same "recurse then
     rewire" shape as this file.
  4. The list may be very long and memory is tight.
     Use the iterative version. There is no way to make this code tail recursive
     as written, because the rewiring happens after the call, so the frame
     cannot be reused.
TRIGGER
  When the answer for a node is one pointer fix on top of the answer for the
  rest of the list, think "recurse to the end, rewire on the way back" - then
  ask if a loop can do it in constant space.
C# NOTE
  The two guards could collapse into one using the null-conditional operator:
  `if (head?.next == null) return head;` - `head?.next` yields null when head
  itself is null, covering both the empty list and the single node in one line
  with the same behavior.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
