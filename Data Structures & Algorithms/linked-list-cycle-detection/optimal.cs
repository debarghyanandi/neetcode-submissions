// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
// -  Floyd's slow/fast pointer cycle detection   [floyd-cycle-detection]
// -  the only solution in this folder
// -
// -  Reference solution - not one you solved yourself (from submission-1)
// -
// -  Fast pointer moves two steps and slow one step per iteration; they
// -  meet within one traversal if a cycle exists.
// --------------------------------------------------------------------------

public class Solution
{
    public bool HasCycle(ListNode head)
    {
        ListNode slow = head;
        ListNode fast = head;
        while (fast != null && fast.next != null)
        {
            slow = slow.next;
            fast = fast.next.next;
            if (fast == slow)
                return true;
        }
        return false;
    }
}

/*
================================================================================
 PATTERN : Fast and Slow Pointers - Floyd's cycle detection
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-1.cs when it was first processed
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  The obvious alternative walks the list once and puts every node into a
  HashSet<ListNode>, returning true on the first repeat. It works, but it holds
  a live reference to every node it has seen. Floyd replaces that set with two
  cursors, slow and fast, and uses their difference in speed as the only
  bookkeeping. Note also that nothing is written to the nodes - no visited flag,
  no rewiring of next - so the list the caller passed in is untouched when
  HasCycle returns.
INVARIANT
  Say the cycle has length L. slow reaches the cycle after some number of steps;
  fast is already inside it by then. From that point, let d be the forward
  distance from fast to slow measured around the cycle. Each iteration fast
  advances 2 and slow advances 1, so fast gains exactly 1 on slow and d
  decreases by 1 modulo L. A gain of exactly 1 cannot step over slow, so d must
  land on 0 rather than skip past it, and it does so within L iterations. That
  is why fast == slow is guaranteed, not merely likely - the usual interview
  challenge is "how do you know they do not leapfrog forever?" and this is the
  answer. With no cycle, fast reaches the null tail first and the loop exits
  false.
WHY THE LOOP GUARD HAS TWO TESTS
  The body dereferences fast.next.next, which needs two links to exist. fast !=
  null makes fast.next legal; fast.next != null makes the second hop legal. Drop
  either test and you get a NullReferenceException at the end of an acyclic list
  - dropping the first fails on even lengths, dropping the second on odd. slow
  is never null-checked and does not need to be: slow always trails fast along a
  path fast has already traversed and the guard has already validated.
WHY THE COMPARISON SITS AFTER THE ADVANCES
  slow and fast are both initialized to head, so they are equal before the loop
  ever runs. The if (fast == slow) is placed after both pointers move, so the
  first comparison happens at step 1, not step 0. Hoist that test to the top of
  the body and every non-null head returns true. Also, == on ListNode here is
  reference identity, not value comparison - two separate nodes holding the same
  val are correctly not a match, since what matters is revisiting the same
  object.
EDGE CASES
  head == null: the guard fails on the first test, returns false without
  dereferencing anything.
  Single node, next == null: guard fails on the second test, false.
  Single node pointing at itself: slow = head, fast = head.next.next = head,
  they match on the first iteration, true.
  Two nodes pointing at each other: slow = b, fast = a on iteration 1 (no
  match), then slow = a, fast = a on iteration 2, true.
TRIGGER
  Reach for this whenever the question is "does this linked structure revisit a
  node" and you are told not to modify the nodes or not to allocate proportional
  storage. The same two-cursor trick generalizes to any deterministic next-state
  function, not just linked lists - the happy-number problem and cycle detection
  in a functional graph f(x) are the same code with slow = f(slow), fast =
  f(f(fast)).
FOLLOW-UP
  The near-certain next question is "now return the node where the cycle
  begins." This method cannot be reused as-is because it throws away the meeting
  node and returns bool. Save the node where fast == slow, then reset one
  pointer to head and advance both one step at a time; they meet at the cycle
  entry. Cycle length is easier: park one pointer at the meeting node and walk
  the other until it comes back around, counting steps.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
