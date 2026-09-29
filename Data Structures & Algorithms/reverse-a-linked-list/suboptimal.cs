// --------------------------------------------------------------------------
// -  suboptimal.cs         O(n) time / O(n) space
// --------------------------------------------------------------------------

public class Solution
{
    public ListNode ReverseList(ListNode head)
    {
        if (head == null)
        {
            return null;
        }
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
 PROBLEM : You get the head of a singly linked list. Reverse the list in place
           and return the new head. An empty list returns null. Example: 1 ->
           2 -> 3 -> null -> 3 -> 2 -> 1 -> null.
 PATTERN : Recursion (reverse the rest, then fix one link)
================================================================================
IDEA
  ReverseList(head.next) reverses everything after head and returns newHead,
  the old last node. That node is passed back up unchanged through every call.
  After the call, head.next is the tail of the reversed part, so
  head.next.next = head hooks head onto the end. Then head.next = null.
  It is correct by induction: each call leaves its sublist fully reversed.
  Unlike optimal.cs, it uses the call stack instead of a prev/curr loop.
EXAMPLE
  Input 1 -> 2 -> 3. Call(3): next is null, so newHead = 3, 3.next = null.
  Call(2): 2.next.next = 2 gives 3 -> 2, then 2.next = null.
  Call(1): 2.next = 1 gives 3 -> 2 -> 1, then 1.next = null.
  Answer: 3 -> 2 -> 1, and newHead = 3 is returned from every level.
COMPLEXITY
  Time  O(n)  each node is entered once and does O(1) link work
  Space O(n)  recursion depth equals list length, one stack frame per node
WATCH OUT
  - A very long list can cause a StackOverflowException. C# has no tail-call
    guarantee, and this call is not a tail call anyway.
  - Skip head.next = null and the old head keeps pointing at node 2, so you
    get a 1 <-> 2 cycle. Printing the list then loops forever.
  - Return newHead, not head. Returning head gives back the new tail.
  - Save nothing before the call: head.next still points to the old next
    node after the call. That is why head.next.next = head works.
================================================================================
*/
