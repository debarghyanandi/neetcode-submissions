// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(n) space
// -  slow-fast pointers, reverse recursively, interleave
// -  [slow-fast-recursive-reverse-interleave]
// -  the only solution in this folder
// -
// -  Reference solution - not one you solved yourself
// -
// -  Recursion depth for ReverseList equals second-half length, and all
// -  three phases (find middle, reverse, interleave) traverse the list
// -  once.
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
 PATTERN : Linked List - fast/slow split, reverse tail, then weave
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  slow        walker that ends on the last node of the first half
  fast        walker moving two steps per one of slow; drives the stop
  second      head of the detached tail, then head of that tail reversed
  first       cursor walking the first half during the merge
  firstNext   saved first.next before the pointer is overwritten
  secondNext  saved second.next before the pointer is overwritten
  newHead     last node reached by the recursion; the new head of the reversed part
WHY THIS PATTERN
  The target order 0, n-1, 1, n-2, ... pairs the front of the list with the
  back, but a singly linked list only moves forward, so you cannot walk
  backwards to get n-1. Reversing the second half turns "walk backwards from the
  end" into "walk forwards from second". After that the answer is a plain merge
  of two forward lists, first and second, taking one node from each. The
  fast/slow scan finds the split point in one pass without counting length
  first.
BRUTE FORCE
  Push every node pointer into a List<ListNode> in one pass, then rewire with
  two indices i from the front and j from the back until they meet. That is also
  O(n) time but needs an explicit array of n references, and it is the version
  most people write first. This file avoids that array, though its recursive
  reverse still spends O(n) stack, so the space win is only on paper.
INVARIANT
  When the fast/slow loop ends, slow is the last node of the first chunk and
  slow.next starts the second chunk, and the first chunk is never shorter than
  the second. After slow.next = null the two lists are disjoint, so reversing
  second cannot touch the first half. In the merge loop, everything before first
  is already in final order, and because the first half is at least as long,
  second runs out first, which is exactly why the loop condition only tests
  second.
WATCH OUT
  head == null crashes: the while loop never runs, slow stays null, and
  slow.next throws NullReferenceException. The comment "slow is at middle" is
  loose - for an even length like 1,2,3,4 slow lands on node 3, one past the
  true middle, which is what makes the first half longer and the merge safe, so
  do not "fix" it by starting fast at head.next. In the merge, first can become
  null on the final assignment; that is fine only because second becomes null in
  the same iteration. ReverseList recurses once per node, so a long list can
  overflow the stack.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Make the space O(1).
     Replace the recursive ReverseList with the iterative three-pointer loop
     (prev, curr, next). Same time, no stack growth, a few more lines.
  2. Can you do it without reversing anything?
     Yes - collect node references into a List<ListNode> and rewire with a head
     index and a tail index. Simpler to reason about, but it costs an n-sized
     array of references.
  3. The problem changes to weave in groups of two (0,1,n-1,n-2,...). What
  changes?
     The split and the reverse are identical; only the merge loop changes to
     move two nodes from first and two from second per round, with a guard for a
     leftover node.
  4. The list is doubly linked. Does the approach simplify?
     Yes - drop the reverse entirely, keep a head pointer and a tail pointer,
     and alternate while they have not met, fixing both next and prev on each
     link.
TRIGGER
  A singly linked list task that needs the node k steps from the end paired with
  the node k from the start.
C# NOTE
  A StackOverflowException from the recursive ReverseList cannot be caught in
  .NET - it kills the process immediately - so the iterative rewrite is a safety
  fix, not just a style choice.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
