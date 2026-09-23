// --------------------------------------------------------------------------
// -  suboptimal.cs         O(n) time / O(n) space
// -  recursive pointer reversal   [recursive-reverse]
// -  ranks below optimal.cs (O(n) time / O(1) space)
// -
// -  Reference solution - not one you solved yourself
// -
// -  Recursion depth equals the length of the list, requiring O(n)
// -  call-stack space.
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
 PATTERN : Recursion on a linked list - reverse pointers on unwind
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-1.cs when it was first processed
 STATUS  : Suboptimal
================================================================================
VARIABLES
  newHead  the last node of the original list; it stays the return value through every frame
WHY THIS PATTERN
  The task is to flip every next pointer so the old tail becomes the head. A
  list is defined recursively (a node plus a shorter list), so
  ReverseList(head.next) reverses everything after head and hands back the final
  node, which never changes as the stack unwinds. That leaves only one local job
  per frame: make head.next point back at head. The returned newHead is passed
  up unchanged from the deepest frame to the caller.
BETTER APPROACH
  The better approach is the iterative three-pointer walk: keep prev, curr and a
  saved next, set curr.next = prev, then slide all three forward, and return
  prev. It does the same number of pointer writes but uses O(1) extra space.
  This file loses because every node holds a stack frame until the recursion
  bottoms out, so a long list can throw StackOverflowException, which .NET does
  not let you catch.
INVARIANT
  When ReverseList(head.next) returns, everything from head.next onward is
  already reversed, head.next is now the tail of that reversed part, and newHead
  is its head. So head.next.next = head appends head to the end of the reversed
  part, and head.next = null makes head the new tail. Each frame keeps the same
  list contents and only extends the reversed prefix by one node, so the top
  frame returns a fully reversed list.
WATCH OUT
  Order matters: head.next.next = head must run before head.next = null,
  otherwise the link to the rest of the list is gone and you cannot reach the
  node you need to fix. The initialization newHead = head is what makes the
  single-node case work - when head.next is null the if body is skipped and the
  node returns itself. head.next = null sits outside the if so it runs in both
  branches, which is correct but easy to break by moving it inside. An input
  list with a cycle never reaches the null base case and recurses forever.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Rewrite it without recursion.
     Use prev = null, curr = head; inside the loop save next = curr.next, set
     curr.next = prev, prev = curr, curr = next; return prev. Same time, O(1)
     space, and no stack depth limit.
  2. Could the compiler turn this into a loop by itself?
     No - the recursive call is not the last thing the method does;
     head.next.next = head and head.next = null run after it returns, so the
     frame must stay alive.
  3. Reverse only the sublist between positions left and right.
     Walk to the node before left, keep it as a pointer, reverse exactly
     right-left+1 nodes with the iterative loop, then reattach both cut ends.
     Tracking the node before left and the node after right is the whole
     difficulty.
  4. Reverse the list in groups of k.
     Count k nodes ahead first; if fewer than k remain, leave that tail as is.
     Reverse the group, then recurse or loop on the rest and join the old group
     head to the next group's new head.
TRIGGER
  The problem asks you to rebuild the next pointers of a singly linked list and
  each node's final link depends on what comes after it.
C# NOTE
  The two null checks can collapse into one base case with the null-conditional
  operator: if (head?.next == null) return head; - that returns null for an
  empty list and the node itself for a single node, removing the need for the
  newHead = head seed.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
