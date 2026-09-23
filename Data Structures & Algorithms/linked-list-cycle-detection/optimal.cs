// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
// -  Two-pointer cycle detection   [two-pointer-cycle-detection]
// -  the only solution in this folder
// -
// -  Reference solution - not one you solved yourself
// -
// -  Fast pointer moves at 2x speed; if cycle exists, pointers meet within
// -  n iterations; if no cycle, fast reaches null in O(n) time.
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
 PATTERN : Floyd's Cycle Detection - slow and fast pointers
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-1.cs when it was first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  slow     walks one node per step
  fast     walks two nodes per step; null means a clean end
WHY THIS PATTERN
  The problem asks only whether the list ever loops back, and it wants no extra
  memory. A hash set of visited nodes answers it but stores a node per step. Two
  pointers at different speeds solve it with two references: if there is a loop,
  fast keeps circling it and closes the gap on slow one node per iteration until
  fast == slow; if there is no loop, fast or fast.next hits null and the loop
  ends.
BRUTE FORCE
  Walk the list and put every node reference into a HashSet<ListNode>; return
  true the first time Add fails. That is O(n) time but O(n) space, and it needs
  reference identity, not value equality, so it breaks if ListNode ever
  overrides Equals. This file gets the same time with two local variables.
INVARIANT
  At the top of every iteration, fast is exactly as many steps ahead of slow as
  the number of iterations done so far, and both are on real nodes. If a cycle
  exists, once both pointers are inside it the distance from fast to slow around
  the loop shrinks by one each iteration, so it must reach zero and trigger fast
  == slow. If no cycle exists, the list is finite, so fast reaches the last node
  or past it and the while condition fails.
WHY THE GUARD TESTS TWO THINGS
  fast takes two hops per iteration, so both fast and fast.next must exist
  before fast.next.next is read. Checking only fast would throw
  NullReferenceException on a list with an even number of nodes and no cycle,
  where fast lands on the final node. The order matters too: fast != null is
  evaluated first, and && short-circuits, so fast.next is never read on a null
  reference.
WATCH OUT
  The equality check sits after both moves, so the pointers are never compared
  while they are both still at head; that is what keeps a one-node list without
  a cycle from reporting true. head == null falls straight through the while
  condition and returns false, which is correct for an empty list. This returns
  only a yes or no - it does not leave the pointers anywhere useful for finding
  where the cycle starts, since slow stops at a meeting point inside the loop,
  not at its entrance. The method mutates nothing, so calling it twice on the
  same list is safe.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Return the node where the cycle begins, not just true or false.
     After the meeting, reset one pointer to head and advance both one step at a
     time; they meet at the cycle entrance. Same O(1) space, one more pass.
  2. Report the length of the cycle.
     From the meeting point, keep one pointer fixed and walk the other until it
     comes back; count the steps. Adds one loop traversal, still no extra
     memory.
  3. What if the list is huge and stored across a network or disk, so each node
  read is expensive?
     Floyd reads roughly three nodes per step and revisits them, so a
     visited-set pass with one read per node is cheaper on I/O even though it
     costs O(n) memory. The trade flips from space to read count.
  4. Could you use Brent's algorithm instead?
     Yes - keep slow still and move fast in doubling-length bursts. Same O(1)
     space, fewer pointer moves in practice, but the code is longer and the
     stopping rule is easier to get wrong.
TRIGGER
  A linked structure where you must detect repetition or find a meeting point
  with no extra memory allowed.
C# NOTE
  fast == slow uses the default reference comparison for a class, which is
  identity - exactly what is wanted here. If ListNode were ever changed to a
  struct or given an == overload, this line would silently start comparing
  values and could report a cycle that does not exist.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
