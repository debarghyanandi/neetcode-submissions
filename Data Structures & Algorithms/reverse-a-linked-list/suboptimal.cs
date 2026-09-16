// --------------------------------------------------------------------------
// -  suboptimal.cs         O(n) time / O(n) space
// -  Recursive reversal, fix on unwind   [recursive-reversal]
// -  ranks below optimal.cs (O(n) time / O(1) space)
// -
// -  Reference solution - not one you solved yourself
// -
// -  Recursion depth equals linked list length; call stack accumulates one
// -  frame per node.
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
 PATTERN : Recursive Linked List Reversal - unwind then relink
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-1.cs when it was first processed
 STATUS  : Suboptimal
================================================================================
WHY THIS PATTERN
  The problem asks for the same list with all next pointers flipped, and a
  linked list is a naturally recursive structure: head plus a smaller list
  starting at head.next. The code reverses the tail by calling
  ReverseList(head.next), which hands back newHead, the last node of the
  original list and the new front. Once the tail is reversed, head.next still
  points at the node that is now the tail end of that reversed part, so
  head.next.next = head hooks the current node on behind it. Setting head.next =
  null closes the chain so the old first node becomes the new last node.
BETTER APPROACH
  The better approach is the iterative three-pointer walk: keep prev = null and
  curr = head, and in a while loop save next = curr.next, set curr.next = prev,
  then advance prev = curr and curr = next, returning prev. It does the same
  number of pointer writes but uses O(1) space instead of O(n). This file loses
  because every node adds a frame to the call stack, so a long list can throw
  StackOverflowException, which no amount of tuning fixes since C# does not
  guarantee tail call elimination here (and the call is not in tail position
  anyway - work happens after it returns).
INVARIANT
  At the point ReverseList(head.next) returns, everything from head.next to the
  end has been fully reversed, and head.next is now the last node of that
  reversed block, while head itself is still untouched and still points into it.
  That single unchanged pointer is what makes head.next.next = head legal - it
  is the only handle back into the reversed part. Because the base cases (head
  == null, or head.next == null so newHead stays head) trivially satisfy the
  invariant, induction gives a correct full reversal, and newHead is passed up
  unchanged from the deepest call.
WATCH OUT
  head.next = null runs on every frame, including the deepest one where
  head.next was already null - harmless, but it means the assignment is not
  guarded by the if, which is easy to misread as being inside it. The order
  matters: if you wrote head.next = null before head.next.next = head, you would
  destroy the link you need and get a NullReferenceException. The null check at
  the top only protects the very first call with an empty list; after that
  head.next != null guarantees the recursive call never receives null, so the
  check is dead weight in every frame below the first. Depth equals list length
  exactly, with no early exit, so failure on a long input is a crash, not a
  slowdown.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How do you make this O(1) space without changing the language or data
  structure?
     Switch to the iterative prev/curr/next loop. Trade-off: you lose the short
     inductive proof and have to reason about three moving pointers by hand, but
     you remove the stack depth risk entirely.
  2. Reverse only the sublist between positions m and n, leaving the rest
  intact.
     Walk to the node before position m and keep it as a fixed anchor, reverse
     exactly n - m + 1 nodes iteratively, then reattach the anchor to the new
     front and the old front to the node after position n. A dummy head node in
     front of the list removes the special case where m is 1.
  3. What if the list might contain a cycle?
     This code would recurse forever and overflow. Detect the cycle first with
     Floyd's fast/slow pointers, or reject the input; reversal is only well
     defined on an acyclic list.
  4. Reverse the list in groups of k, leaving any leftover tail as is.
     Count k nodes ahead first; if fewer than k remain, return that segment
     untouched. Otherwise reverse the k nodes and set the segment's original
     head's next to the result of recursing on the remainder - same shape as
     this file, but with a counted loop inside each frame.
TRIGGER
  When a problem gives you a structure that is "one node plus a smaller version
  of itself" and the answer for the whole depends on the answer for the tail,
  reach for this unwind-then-relink recursion - then ask whether an iterative
  version removes the stack cost.
C# NOTE
  The field is written as next, lowercase, which does not match normal C#
  property naming - LeetCode's ListNode definition uses that spelling, so the
  code is correct here but would not compile against a class declaring Next.
  Since ListNode is a reference type, newHead, head and head.next are all just
  references into the same objects, so no node is ever copied; the whole method
  is pure pointer rewriting.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
