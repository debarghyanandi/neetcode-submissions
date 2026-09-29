// --------------------------------------------------------------------------
// -  suboptimal.cs         O(n + m) time / O(n + m) space
// --------------------------------------------------------------------------

public class Solution
{
    public ListNode Add(ListNode l1, ListNode l2, int carry)
    {
        if (l1 == null && l2 == null && carry == 0)
        {
            return null;
        }

        int v1 = 0;
        int v2 = 0;
        if (l1 != null)
        {
            v1 = l1.val;
        }
        if (l2 != null)
        {
            v2 = l2.val;
        }

        int sum = v1 + v2 + carry;
        int newCarry = sum / 10;
        int nodeValue = sum % 10;

        ListNode nextNode = Add(
            (l1 != null ? l1.next : null),
            (l2 != null ? l2.next : null),
            newCarry
        );

        return new ListNode(nodeValue) { next = nextNode };
    }

    public ListNode AddTwoNumbers(ListNode l1, ListNode l2)
    {
        return Add(l1, l2, 0);
    }
}

/*
================================================================================
 PROBLEM : Two non-empty linked lists hold two non-negative numbers. The
           digits are stored in reverse order, one digit per node (the head is
           the ones digit). Return their sum as a linked list in the same
           reverse format. Example: [2,4,3] + [5,6,4] -> [7,0,8] (342 + 465 =
           807).
 PATTERN : Linked List Traversal (recursive) + carry
================================================================================
IDEA
  Add(l1, l2, carry) handles one digit position per call. A missing node
  counts as 0, so sum = v1 + v2 + carry. The call keeps nodeValue = sum % 10
  and passes newCarry = sum / 10 to the recursive call on the next nodes.
  It stops only when both lists are null and carry is 0, so a final carry
  still becomes a node. This is correct because it is grade-school addition,
  done from the lowest digit up. Unlike optimal.cs it uses recursion, not a
  loop.
EXAMPLE
  l1 = [9,9], l2 = [1] (99 + 1)
  Add(9,1,0): sum=10 -> node 0, carry 1; Add(9,null,1): sum=10 -> node 0,
  carry 1
  Add(null,null,1): sum=1 -> node 1, carry 0; Add(null,null,0) -> null
  Answer: [0,0,1] (100)
COMPLEXITY
  Time  O(n + m)  one call per digit position, max(n, m) + 1 calls
  Space O(n + m)  the recursion stack holds one frame per digit position
WATCH OUT
  - The base case must also check carry == 0. If it stops when both lists
    are null, then 5 + 5 returns [0] instead of [0,1].
  - Very long lists can cause a stack overflow here. The iterative
    optimal.cs does not have this risk. Expect the interviewer to ask.
  - Advance with l1 != null ? l1.next : null. Calling l1.next directly
    throws when the lists have different lengths.
================================================================================
*/
