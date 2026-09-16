// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(n) space
// -  Fast/slow midpoint, reverse recursively, merge alternately
// -  [fast-slow-reverse-merge]
// -  the only solution in this folder
// -
// -  Reference solution - not one you solved yourself
// -
// -  Recursive reversal creates O(n) call stack depth; three linear passes
// -  produce O(n) time.
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
 PATTERN : Fast/Slow Split + Reverse Second Half + Weave Merge
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  The target order 1, n, 2, n-1, ... pairs the front of the list with the back,
  but a singly linked list gives no way to walk backwards. Reversing the tail
  turns "walk from the back" into "walk forward", so two forward pointers, first
  and second, can be zipped together. The fast/slow scan finds the split point
  without knowing the length in advance: fast moves two nodes per step, slow
  one, so slow lands at the end of the first half when fast runs off the end.
BRUTE FORCE
  The obvious first attempt is to copy every node reference into a
  List<ListNode>, then walk two indexes i from the front and j from the back and
  rewire node[i].next = node[j]. That is also linear time but needs an explicit
  array of n references, and it throws away the linked structure you were asked
  to work with. A worse version repeatedly scans to find the current last node,
  which gives O(n^2) time.
INVARIANT
  After the split, first points at a chain of ceil(n/2) nodes and second at the
  reversed chain of floor(n/2) nodes, so the first chain is never shorter than
  the second. Each pass of the merge loop takes exactly one node from each
  chain, links first -> second -> firstNext, and advances both cursors; the
  nodes already placed behind first are in final order and the two remaining
  chains still satisfy the length relation. When second becomes null every node
  from the back half has been inserted, and the last node of the front half
  still carries the terminating null that slow.next = null installed.
THE MERGE LOOP ONLY TESTS SECOND
  The loop condition is while (second != null) with no null check on first, and
  that is safe only because of the length relation above. For odd n the front
  chain has one extra node, which correctly ends up last; for even n both chains
  are equal and both empty out together. If the split were done the other way,
  so the back half could be longer, first would be null in the middle of the
  body and first.next would throw.
WATCH OUT
  head == null crashes: the while loop never runs, slow stays null, and second =
  slow.next throws a NullReferenceException. There is no guard for it, so an
  empty list must never reach this method. The comment "slow is at middle" is
  loose - for even n slow is the last node of the first half, which is one past
  the true midpoint - but the code is correct as written, the comment just
  describes it imprecisely. Single node and two nodes are fine: second becomes
  null and the merge loop is skipped.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. ReverseList is recursive. What breaks on a very long list, and how do you
  fix it?
     Recursion depth equals the length of the second half, so a long list can
     overflow the call stack. Rewrite it as an iterative three-pointer loop
     (prev, curr, next); that also drops the extra space to O(1) and keeps the
     same linear time.
  2. The method returns void. Is the caller's head still valid afterwards?
     Yes - the original head node stays the first node of the result, because
     the weave only rewires next pointers and never promotes a different node to
     the front. If the problem instead asked you to return a new head, you would
     have to return it explicitly, since C# passes the reference by value.
  3. What if the list were doubly linked?
     You could drop the reverse entirely and walk one pointer forward from head
     and one backward from tail using prev, meeting in the middle. That removes
     the ReverseList call, though you must fix both next and prev on every
     rewire.
  4. How would you produce the reordered sequence without modifying the input
  list?
     Push node values into a List<int> or an array, then emit them by
     alternating indexes from the two ends to build fresh nodes. Same linear
     time, but you pay O(n) for the buffer and lose the in-place property.
TRIGGER
  A linked-list task that needs to pair the front with the back, or reach nodes
  in reverse order, without an index or a length.
C# NOTE
  The three-line rewire in the merge could be written with tuple assignment,
  (first.next, second.next) = (second, firstNext), because C# fully evaluates
  the right side before assigning; here the explicit firstNext and secondNext
  temporaries are clearer and are needed anyway as the next cursors.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
