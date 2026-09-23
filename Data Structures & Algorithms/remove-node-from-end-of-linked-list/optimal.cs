// ##########################################################################
// #  optimal.cs            O(n) time / O(1) space
// #  Two-pointer, fast and slow   [two-pointer-linked-list]
// #  the only solution in this folder
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  Two passes over the list with pointers spaced n apart position slow
// #  one node before removal point.
// ##########################################################################

//My solution
public class Solution
{
    public ListNode RemoveNthFromEnd(ListNode head, int n)
    {
        ListNode dummy = new ListNode(0, head);
        ListNode slow = dummy;
        ListNode fast = head;

        while (n > 0)
        {
            fast = fast.next;
            n--;
        }

        while (fast != null)
        {
            slow = slow.next;
            fast = fast.next;
        }

        //slow is now n+1 th node from the end.
        slow.next = slow.next.next;
        return dummy.next;
    }
}

/*
================================================================================
 PATTERN : Two Pointers - fixed gap of n on a linked list
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-1.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  dummy   fake node placed before head so removing head needs no special case
  slow    walker that ends on the node just BEFORE the one to delete
  fast    lead walker, started n nodes ahead of slow
  n       reused as a countdown while building the gap; zero after the first loop
WHY THIS PATTERN
  The problem asks for a position counted from the end, but a singly linked list
  can only be walked forward. Putting a fixed gap of n between fast and slow
  turns "n from the end" into "fast ran off the end", which you can test
  locally. When fast becomes null, slow has been pushed exactly to the
  predecessor of the target, so one pointer assignment does the removal.
BRUTE FORCE
  The natural first attempt is two passes: walk the whole list once to get its
  length L, then walk again L-n-1 steps to reach the predecessor and unlink.
  That is still O(n) time but touches the list twice, and it needs the same
  dummy trick anyway when n equals L. This file does it in one forward sweep
  with no length variable.
INVARIANT
  After the first loop, fast is exactly n nodes ahead of slow, and the second
  loop moves both by one, so that gap never changes. Therefore at the moment
  fast is null (one past the last node), slow.next is the node with exactly n-1
  nodes after it - the nth from the end. Because slow starts at dummy, not head,
  the gap still holds when the target is the head itself, and dummy.next then
  carries the new head out.
WATCH OUT
  There is no guard for n larger than the list length: the first loop would call
  fast.next on null and throw a NullReferenceException. The same crash happens
  if head is null with n greater than zero. The final line slow.next =
  slow.next.next assumes slow.next is not null, which is only guaranteed because
  the gap invariant held - if you ever change the first loop to advance n+1
  steps, this line breaks. Also note the comment "slow is now n+1 th node from
  the end": that is true for the real list only when n is smaller than the
  length; when n equals the length, slow is dummy, which is not a list node at
  all.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Remove the nth node from the FRONT instead. How does the code change?
     You no longer need fast at all - walk slow forward n-1 steps from dummy and
     unlink. Still O(1) extra space, and dummy is still what saves you when n is
     1.
  2. What if you must also return the removed node, not just the new head?
     Save ListNode removed = slow.next before the unlink, set removed.next =
     null so it does not keep the tail alive, and return it. One extra
     reference, same complexity.
  3. The list is huge and stored on disk or streamed once - can you still do it?
     Yes, this is the reason the two-pointer form matters: it reads each node
     once in order, so it works on a forward-only stream, while the length-first
     version would need a second read.
  4. How would you handle n given as possibly invalid?
     Add a counter in the first loop and return head unchanged (or throw) if
     fast becomes null before the countdown finishes - a cheap check that
     removes the NullReferenceException path.
TRIGGER
  Any singly linked list question phrased as "kth from the end" or "middle" -
  set two pointers with a fixed gap or fixed speed ratio instead of measuring
  length first.
C# NOTE
  LeetCode's ListNode ships a two-argument constructor, so new ListNode(0, head)
  builds the sentinel in one line instead of assigning dummy.next afterwards.
  Unlike C++, the unlinked node needs no delete - once nothing points at it the
  garbage collector reclaims it.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
