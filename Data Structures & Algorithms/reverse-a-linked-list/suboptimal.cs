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
 PATTERN : Linked List Recursion - reverse on the way back up
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-1.cs when it was first processed
 STATUS  : Suboptimal
================================================================================
VARIABLES
  newHead  the old tail node; it becomes the head of the reversed list and is passed up unchanged from the deepest call
WHY THIS PATTERN
  Reversing a singly linked list means that every next pointer must point back
  to the node before it. A node cannot reach the node before it by itself.
  Recursion fixes this: each call keeps its own head on the call stack while
  ReverseList(head.next) reverses the rest of the list. When that call returns,
  head.next is now the tail of the reversed part, so head.next.next = head adds
  head to the end.
BETTER APPROACH
  The better approach is iterative, with three pointers: prev, curr and next.
  Walk the list once and flip curr.next to prev at each step. It is also O(n)
  time, but it uses O(1) extra space. This file loses on space: it opens one
  stack frame per node, so memory grows to O(n) just to remember the path back.
INVARIANT
  When ReverseList(head.next) returns, the list that started at head.next is
  fully reversed. Its head is newHead, and its tail is the node head.next. So
  head.next.next = head adds head as the new tail, and head.next = null ends the
  list there. By induction on list length, each call returns a correctly
  reversed list, and newHead is the original last node from the base case up.
HEAD.NEXT STILL POINTS FORWARD
  After the recursive call, head.next was not changed. It still points at the
  node that is now the tail of the reversed part. That is the only way to reach
  the tail in O(1), and it is why the code does not need a separate tail
  variable.
WATCH OUT
  The recursion depth equals the list length. A very long list can throw a
  StackOverflowException, and in .NET you cannot catch that exception, so the
  process dies. Keep head.next = null outside the if block. It must also run for
  the original head, or the first and second nodes will point at each other and
  make a cycle. Order matters: head.next.next = head must run before head.next =
  null, or you lose the link to the tail.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Reverse only the nodes from position left to right.
     Walk to the node before left, then reverse right-left+1 nodes with the
     three-pointer loop, and reconnect both ends. It is one pass with O(1)
     space, but the edge case where left is 1 needs a dummy head node.
  2. Reverse the list in groups of k nodes.
     First check that k nodes remain, then reverse that group and link it to the
     previous group's tail. Leave a last group with fewer than k nodes as it is.
     Time is still O(n).
  3. Can you do it recursively but with tail recursion?
     Pass (prev, curr) as parameters and return the result of Helper(curr,
     next). The logic is the same as the loop, but C# does not promise tail-call
     optimization, so the stack still grows.
TRIGGER
  The problem asks you to flip or reorder links in a singly linked list, and you
  need access to a node's predecessor that you cannot store in the node itself.
C# NOTE
  The first check, if (head == null) return null, only handles an empty input.
  Every recursive call is guarded by head.next != null, so the null check never
  fires during recursion. You could fold it into one guard, if (head?.next ==
  null) return head, and the code would be shorter.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
