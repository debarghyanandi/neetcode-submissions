// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
// -  iterative pointer reversal   [iterative-pointer-reversal]
// -  the only solution in this folder
// -
// -  Reference solution - not one you solved yourself (from submission-0)
// -
// -  single pass through the list reversing next pointers with three
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
 PATTERN : Iterative pointer reversal - prev/curr/temp walk
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Optimal
================================================================================
CORE IDEA
  Reversing a singly linked list is not about moving data, it is about flipping
  every next pointer to face backwards. The only obstacle is that once you
  overwrite curr.next you have destroyed your only route to the rest of the
  list. So you carry exactly one node of lookahead (temp) and one node of
  history (prev), and walk the list once flipping one arrow per step.
INVARIANT
  At the top of every iteration:
  - prev is the head of the already-reversed prefix (all nodes strictly before
  curr, in reverse order), and that sublist is properly null-terminated at its
  tail.
  - curr is the head of the untouched original suffix, still in forward order.
  - The two lists are disjoint and their union is every node of the input.

  The body restores this invariant by moving exactly one node from the front of
  the suffix to the front of the prefix. It holds trivially before the first
  iteration: prefix is empty (prev = null), suffix is the whole list (curr =
  head).
ALGORITHM
  1. prev = null, curr = head.
  2. While curr != null:
     a. temp = curr.next - save the suffix before you can lose it.
     b. curr.next = prev - flip this node's arrow to point at the reversed
     prefix.
     c. prev = curr - curr is now the head of the reversed prefix.
     d. curr = temp - advance into the saved suffix.
  3. Return prev.

  The order of a-d is rigid. Doing b before a orphans the rest of the list;
  doing d before c loses the node you just flipped.
WHY RETURN PREV
  The loop exits only when curr == null, i.e. the suffix is empty, so by the
  invariant prev heads a reversed list containing every node - it is the
  original tail. Returning curr would return null every time. Note also that
  returning prev is what makes the empty-list case free: if head is null the
  loop body never runs, prev is still null, and null is the correct answer.
WHY THE OLD HEAD GETS NULL-TERMINATED FOR FREE
  A common bug in hand-rolled reversals is leaving the original head pointing at
  the original second node, which produces a two-node cycle. Here it cannot
  happen: on the very first iteration curr is head and prev is null, so
  head.next = null is the first assignment made. Initializing prev to null
  rather than to head is doing double duty as the terminator.
EDGE CASES
  - head == null: loop skipped, returns null.
  - Single node: temp = null, node.next = null (already was), prev = node, curr
  = null, returns the same node.
  - Two nodes a -> b: after iteration one a.next = null, prev = a, curr = b;
  after iteration two b.next = a, prev = b; returns b -> a -> null.

  No dummy node and no length precomputation is needed - the null terminator is
  the entire stopping condition.
INTERVIEWER FOLLOW-UP
  Expect one of:
  - "Do it recursively." Recurse to the tail, then set head.next.next = head and
  head.next = null. Same linear work, but it costs stack depth proportional to
  the list length, which this version does not.
  - "Reverse only positions m..n." Same three-pointer core, but you stash the
  node before m and the node at m first, then re-stitch the four boundary
  pointers after the loop.
  - "Reverse in groups of k." Repeated application of this loop, run k steps at
  a time, with the previous group's tail re-linked to the next group's new head.
  - "Does the input list get mutated?" Yes - this is destructive and in place.
  The caller's head reference becomes the tail of the result.
WATCH OUT
  temp is declared inside the loop, so it is a fresh local each iteration - that
  is correctness-neutral here (it is written before every read) but do not
  conclude the variable persists across iterations. The real trap is muscle
  memory writing prev = curr before curr.next = prev, which points a node at
  itself and creates a self-loop; trace two nodes on paper if you are unsure of
  the order.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
