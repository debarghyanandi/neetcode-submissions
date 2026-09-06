// --------------------------------------------------------------------------
// -  suboptimal.cs         O(n) time / O(n) space
// -  recursive reversal, fix links on unwind   [recursive-reversal]
// -  ranks below optimal.cs (O(n) time / O(1) space)
// -
// -  Reference solution - not one you solved yourself (from submission-1)
// -
// -  recurses to the tail then relinks pointers on the way back up, costing
// -  O(n) call stack depth
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
 PATTERN : Recursion - reverse the tail, then splice head on
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-1.cs when it was first processed
 STATUS  : Suboptimal
================================================================================
WHY THIS PATTERN
  A singly linked list gives you no way back, so reversing means every node has
  to learn who pointed at it. Recursion supplies that for free: the call stack
  holds onto head while the rest of the list is being reversed, and on the way
  back up each frame owns exactly the one edge it needs to flip. No prev
  variable is needed because the frame IS the prev variable.
INVARIANT
  ReverseList(x) returns the head of the reversed sublist starting at x, and
  leaves x as the TAIL of that sublist with x.next == null.

  Both halves are load-bearing. The first half is what lets newHead be passed
  straight up through every frame untouched. The second half is what lets the
  caller's fix-up work: because the recursive call never rewrote head.next,
  after it returns head.next is still the old successor, and by the invariant
  that node is now the last node of the reversed tail - exactly the node that
  must point back at head.
ALGORITHM
  1. head == null returns null. This guards the empty-list call only; it is not
  the recursion's stopping point.
  2. newHead = head. For a one-node list this is already the answer, and the if
  (head.next != null) guard means a tail node falls straight through to the
  return. The real base case is a last node, not a null.
  3. Otherwise recurse on head.next. Everything from head.next onward comes back
  reversed. newHead now holds the original final node and never changes again as
  the frames unwind.
  4. head.next.next = head. head.next is the old successor, which is now the
  reversed tail's end; aim it back at head.
  5. head.next = null, unconditionally. In the recursive branch this severs the
  old forward edge that step 4 just turned into a two-node cycle. In the
  one-node branch it is a harmless no-op on an already-null field.
  6. return newHead.
WHY THIS LOSES
  The iterative three-pointer version - prev = null, walk curr, save nxt =
  curr.next, curr.next = prev, prev = curr, curr = nxt, return prev - does the
  same single pass with a fixed handful of locals and no call stack. This
  version pays one stack frame per node, so a long list can overflow before it
  can be wrong. Note the recursion is not in tail position: head.next.next =
  head runs after the call returns, so no loop rewrite comes for free from the
  language. Reach for the iterative form in an interview unless recursion was
  requested; keep this one as the proof of why the iterative form is correct.
WATCH OUT
  Order of steps 4 and 5 is forced. Null head.next first and you have destroyed
  the only handle on the node you were about to write through.

  Do not return head from the recursive branch. head became the new tail;
  newHead has to be threaded up unchanged through every frame.

  Between steps 4 and 5 the two nodes form a genuine cycle (head -> old
  successor -> head). If you step through in a debugger and try to print the
  list mid-frame, it will not terminate. Step 5 is what closes that window, and
  it must run on every non-null node, not just some.

  The caller's original head reference is now a one-node tail. The returned
  value is the only valid handle on the list.
TRIGGER
  Reach for reverse-the-tail-then-splice whenever a linked structure has to be
  inverted and you want the predecessor handed to you rather than tracked by
  hand. The same shape reappears in reverse-nodes-in-k-group,
  palindrome-linked-list (recurse to the end, compare while unwinding), and
  binary tree problems where the parent needs the result of the child before it
  can fix its own pointers.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
