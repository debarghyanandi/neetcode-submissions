// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
// -  iterative pointer reversal, prev/curr/temp walk
// -  [iterative-pointer-reversal]
// -  ranks above suboptimal.cs (O(n) time / O(n) space)
// -
// -  Reference solution - not one you solved yourself
// -
// -  single pass flipping each node's next pointer using constant extra
// -  tracking variables
// --------------------------------------------------------------------------

public class Solution {
    public ListNode ReverseList(ListNode head) {
        ListNode prev = null;
        ListNode curr = head;

        while(curr != null){
            ListNode temp = curr.next;
            curr.next = prev;
            prev = curr;
            curr = temp;
        }
        return prev;
    }
}

/*
================================================================================
 PATTERN : Three-pointer iterative reversal, rewiring next in place
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  A singly linked list gives you only forward references, so reversal means
  rewriting every next field. You cannot do that by walking and reading alone -
  the moment you overwrite curr.next you lose your way forward. The fix is to
  carry the piece of state the list no longer stores: prev, the node that should
  come after curr in the finished list. Three names (prev, curr, temp) are the
  minimum needed to hold the boundary node, the frontier, and the one link you
  are about to clobber.
INVARIANT
  At the top of every iteration:
  1. prev is the head of the already-reversed prefix; following its next chain
  walks backward through the nodes seen so far and terminates at null.
  2. curr is the head of the untouched suffix, still in original order.
  3. The two chains are disjoint and together contain every node.
  The body moves exactly one node (curr) across that boundary and re-establishes
  all three conditions. Initializing prev = null is not just a placeholder - it
  is the invariant already true for an empty prefix, and it supplies the null
  terminator that the original head needs once it becomes the tail. No special
  case is required for the last node.
LINE ORDER
  The four body lines are a fixed rotation: save, rewire, advance prev, advance
  curr. Two orderings are traps.
  If curr.next = prev runs before temp = curr.next, then temp reads back prev,
  so on the first iteration prev = curr = head and curr = temp = null. The loop
  exits after one pass and you return a single node whose next is null - the
  rest of the list is unreachable, and the bug looks like a truncation, not a
  crash.
  If curr = temp runs before prev = curr, then prev picks up the
  already-advanced pointer, skipping a node and leaving the chain broken. Fix
  the order in memory as: you may only overwrite curr.next after temp is holding
  it, and prev must claim curr before curr moves on.
WHY RETURN PREV
  The loop exits precisely when curr == null, so curr carries no information at
  the end - returning it always yields null. prev is the last node the loop
  advanced past, which is the original tail and therefore the new head. This
  also handles head == null for free: the while condition fails immediately,
  prev is still its initial null, and the empty list reverses to the empty list
  with no branch. A single-node list runs one iteration, sets head.next = null
  (already true) and returns the same node.
WATCH OUT
  The reversal is destructive and in place - no new ListNode is allocated, the
  existing nodes are relinked. After the call returns, any variable the caller
  still holds pointing at the old head now points at the tail of the reversed
  list, and traversing from it sees exactly one node. Anything that needed the
  original order must be captured before calling. This aliasing effect is what
  makes reverse useful as a subroutine (reverse the second half, compare,
  optionally reverse back) and what makes it dangerous when the list is shared.
TRIGGER
  Reilnk-in-place with three pointers is the answer whenever a linked-list
  problem needs nodes visited in an order the links do not provide, or needs the
  direction of the links themselves changed, under constant extra space. The
  tell is that you catch yourself wanting to index backward or push nodes onto a
  stack.
FOLLOW-UPS
  Expect an interviewer to push in one of these directions:
  1. Recursive version - same result, but the call stack costs O(n) space, so
  this iterative form is the one to state as optimal.
  2. Reverse only positions m through n - the same loop, plus a saved pointer to
  the node before position m and to the node at position m (which becomes the
  reversed segment's tail) so the three pieces can be stitched back together.
  3. Reverse in groups of k - run this loop k times per group, after first
  checking that k nodes remain.
  4. Palindrome check - reverse the second half found by slow/fast pointers,
  then compare; be ready to say whether you restore the list afterward.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
