// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(n) space
// -  find middle (slow/fast), reverse second half, merge alternately
// -  [middle-reverse-merge]
// -  the only solution in this folder
// -
// -  Reference solution - not one you solved yourself (from submission-0)
// -
// -  linear pass to find middle and merge, but ReverseList is implemented
// -  recursively giving O(n) call-stack depth instead of O(1) iterative
// -  reversal
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
        //slow is at middle

        //cut the list
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
 PATTERN : Fast/slow midpoint, reverse tail, weave merge
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  The target order 0, n-1, 1, n-2, ... pairs the i-th node from the front with
  the i-th node from the back. A singly linked list gives you no way to walk
  backward, so you manufacture backward traversal by physically reversing the
  tail. Once the back half runs in reverse order, "walk both halves forward in
  lockstep and splice" produces exactly the required interleaving. Three linear
  passes, all pointer rewiring, no node values touched and no new nodes
  allocated.
BRUTE FORCE
  Dump every node into a List<ListNode>, then use two indices i=0 and j=count-1
  walking toward each other and relink node[i].next = node[j]. Same asymptotics
  as this file and much easier to get right, but it materializes an array of n
  references and hides the pointer work the question is actually testing. The
  reason to prefer the code here is that the reversal makes the back-to-front
  walk implicit.
WHERE SLOW LANDS
  slow and fast both start at head and slow steps once per two fast steps, so
  slow ends at 0-based index floor(n/2). That means the first half keeps
  floor(n/2)+1 nodes and second gets ceil(n/2)-1. Concretely: n=6 splits 4 and 2
  (1,2,3,4 | 6,5 after reversal, giving 1,6,2,5,3,4); n=5 splits 3 and 2 (giving
  1,5,2,4,3); n=4 splits 3 and 1 (giving 1,4,2,3). The front list is longer by 1
  on odd n and by 2 on even n - it is never shorter.
INVARIANT
  The merge loop is driven by second != null, and each iteration consumes
  exactly one node from each side. Because the front side started strictly
  longer, first is guaranteed non-null every time the body executes first.next =
  second - that is the whole correctness argument for the merge, and it is why
  no null guard on first is needed. When second runs out, whatever remains
  hanging off first is already the correct tail (the 3,4 in the n=6 example),
  and it is already null-terminated because the cut did that. No final fixup
  line is required.
ALGORITHM
  1. Advance slow/fast until fast or fast.next is null; slow is now the last
  node of the front half.
  2. second = slow.next, then slow.next = null to sever the two halves.
  3. second = ReverseList(second).
  4. Walk first from head and second from the reversed head; cache firstNext and
  secondNext, wire first.next = second and second.next = firstNext, then advance
  both to the cached pointers.
  5. Stop when second is null.
WHY THE CUT IS LOAD-BEARING
  Dropping slow.next = null does not merely leave a stray pointer, it builds a
  cycle. On 1..6, slow is node 4 and still points at node 5; after reversing,
  node 5 is the tail of the reversed segment, so the merge produces 1,6,2,5,3,4
  with node 5 still reachable from 4 and pointing back at 3 - the loop 3 -> 4 ->
  5 -> 3. Sever first, reverse second.
THE RECURSIVE REVERSE
  ReverseList recurses to the tail, returns that tail as newHead unchanged all
  the way back up, and on the way out does head.next.next = head to flip the
  single edge in front of the current node, then head.next = null so the old
  forward pointer does not survive. The head.next = null is what terminates the
  reversed list at its new tail; without it the original head would still point
  at its successor and close a two-node cycle.
WATCH OUT
  head == null throws: slow stays null and slow.next dereferences it. The
  problem constraints guarantee at least one node, so it passes, but an
  interviewer poking at edge cases will ask. n=1 and n=2 are safe - both leave
  second as null and skip the merge loop entirely. The other likely follow-up is
  the recursion: ReverseList goes n/2 frames deep, and rewriting it as the
  three-pointer iterative loop (prev, curr, next) makes the extra space constant
  with no other change to this file.
TRIGGER
  Reach for this shape whenever a singly linked list problem needs access to
  nodes from the back in order - reorder, palindrome check, fold-in-half. The
  recipe is always the same three moves: find the midpoint with fast/slow, cut,
  reverse the tail, then consume both halves in a single forward walk.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
