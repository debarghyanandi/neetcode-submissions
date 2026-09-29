// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
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
 PATTERN : Fast and Slow Pointers - Floyd cycle detection
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-1.cs when it was first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  slow   moves one node per loop step
  fast   moves two nodes per loop step; reaches null first if there is no cycle
WHY THIS PATTERN
  The problem asks one thing: does following next ever lead back to a node you
  already visited? If there is no cycle, fast reaches the end (null). If there
  is a cycle, fast can never leave it. Inside the loop, fast gains one node on
  slow at every step, so it must land on slow in the end. The two pointers find
  the cycle without storing any visited nodes.
BRUTE FORCE
  Walk the list and put every node reference in a HashSet<ListNode>. Return true
  the first time Add fails, and false when you reach null. This is correct and
  also O(n) time, but it uses O(n) extra space. That space is exactly what the
  two-pointer version removes.
INVARIANT
  fast is always at least as far along the walk as slow, so if fast has not hit
  null, then slow has not hit null either. Once both pointers are inside the
  cycle, the gap between them (counted along the cycle) shrinks by exactly 1
  each step. A gap that shrinks by 1 cannot jump over 0, so the pointers must
  meet. The meeting happens within one lap of slow, which is why the time stays
  linear. If there is no cycle, the loop condition fails and the method returns
  false.
MOVE FIRST, THEN COMPARE
  slow and fast both start at head, so they are equal before any move. The check
  fast == slow sits after the two moves. If the check came first, every
  non-empty list would return true on the first step.
WATCH OUT
  The loop condition checks fast != null before fast.next != null, and this
  order matters. It is what makes fast.next.next safe. If you swap the two
  checks, an empty list (head == null) throws a NullReferenceException.
  slow.next needs no null check, because slow never passes fast. If a change in
  the code breaks that rule, slow.next can fail.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Return the node where the cycle starts, not just true or false.
     After the pointers meet, put one pointer back at head and leave the other
     at the meeting point. Move both one step at a time. They meet at the cycle
     start. This works because the distance from head to the start equals the
     distance from the meeting point to the start, modulo the cycle length. Time
     is still O(n), with no extra space.
  2. How long is the cycle?
     From the meeting point, keep slow still and step a second pointer around
     the loop until it comes back to slow. Count the steps. This adds one lap of
     work.
  3. Where else does this idea work when there is no linked list?
     Any function that maps a value to a next value, such as Find the Duplicate
     Number (i -> nums[i]) or Happy Number (n -> sum of the squares of its
     digits). The "next" step becomes a function call instead of .next.
TRIGGER
  Reach for this when you must detect a loop, or find its entry, in a sequence
  where each item points to exactly one next item, and you are asked for O(1)
  extra space.
C# NOTE
  fast == slow compares references only because ListNode does not overload the
  == operator. If a class defines value-based ==, use ReferenceEquals(fast,
  slow) to be sure you are comparing node identity.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
