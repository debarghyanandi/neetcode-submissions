// ##########################################################################
// #  optimal.cs            O(n) time / O(1) space
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
 PROBLEM : You get the head of a singly linked list and an integer n. Remove
           the n-th node counted from the END, and return the new head. n = 1
           means the last node. The head itself may be the node removed.
           Example: [1,2,3,4,5], n = 2 -> [1,2,3,5]
 PATTERN : Two Pointers (fast/slow with a fixed gap) + dummy node
================================================================================
IDEA
  A dummy node points to head, so removing the head is not a special case.
  slow starts at dummy and fast starts at head, so fast is one step ahead.
  fast moves n more steps, which makes the gap between them n+1 nodes.
  Then both move together until fast is null (past the last node).
  slow is now just before the target, so slow.next = slow.next.next skips it.
EXAMPLE
  [1,2,3,4,5], n=2: after the first loop fast=3. Pairs (slow,fast):
  (1,4) (2,5) (3,null). slow=3, so 4 is removed -> [1,2,3,5].
  Tricky: [1,2], n=2: fast reaches null in the first loop, slow stays dummy,
  dummy.next = 2 -> [2] (the head was removed).
COMPLEXITY
  Time  O(n)  fast walks the list once and slow follows it, one pass in total
  Space O(1)  only dummy, slow and fast, no copy of the list
PATH TO OPTIMAL
  Copy the nodes into an array, then relink index len-n - O(n)/O(n) - simple.
  Two passes: count the length, then walk to node len-n - O(n)/O(1) - no
  array.
  Fast/slow gap, this file - O(n)/O(1) - one pass, no length needed.
KEYWORDS
  linked list, two pointers, fast and slow pointer, dummy node, one pass, nth
  from end
WATCH OUT
  - Off-by-one gap: slow starts at dummy and fast at head, so n steps make
    the gap right. If fast also started at dummy, it would need n+1 steps.
  - Return dummy.next, not head. For [1], n=1 the head is removed -> [].
  - If n > list length, fast = fast.next hits null and throws. The problem
    guarantees a valid n, so say that out loud or add a null check.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you do it in one pass? Why is slow in the right place?
     -> Yes, this code does. The gap stays n+1 nodes, so when fast is null
        slow is n+1 nodes from the end. That is the node before the target.
  2. How would you return the n-th node from the end instead of removing it?
     -> Start both at head and move fast n steps, then move both until fast is
        null. slow is the answer. Still O(n) time, O(1) space.
  3. How would you find the middle node in one pass?
     -> Same fast/slow idea, but fast moves 2 steps per slow step. When fast
        ends, slow is in the middle. O(n) time, O(1) space.
  4. Could you solve it with recursion?
     -> Recurse to the end and count on the way back. Unlink the node when the
        count is n. O(n) time but O(n) stack space, so it is worse.
TRIGGER
  When you need a position counted from the end of a linked list in one
  pass, send a fast pointer ahead by a fixed gap and let a slow one follow.
================================================================================
*/
