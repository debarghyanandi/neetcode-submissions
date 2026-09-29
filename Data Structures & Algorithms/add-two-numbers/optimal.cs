// ##########################################################################
// #  optimal.cs            O(n + m) time / O(1) space
// ##########################################################################

public class Solution
{
    public ListNode AddTwoNumbers(ListNode l1, ListNode l2)
    {
        // my solution
        ListNode dummy = new ListNode(0);
        ListNode res = dummy;

        int carry = 0;
        while (l1 != null || l2 != null || carry != 0)
        {
            int x = l1 != null ? l1.val : 0;
            int y = l2 != null ? l2.val : 0;

            int sum = x + y + carry;
            carry = (sum) / 10;

            res.next = new ListNode(sum % 10);
            res = res.next;

            if (l1 != null)
                l1 = l1.next;

            if (l2 != null)
                l2 = l2.next;

        }

        return dummy.next;
    }
}

/*
================================================================================
 PROBLEM : Two non-empty linked lists hold two non-negative numbers, digits in
           reverse order (head is the ones digit). Return their sum as a
           linked list in the same reverse order. Example: [2,4,3] + [5,6,4]
           -> [7,0,8].
 PATTERN : Linked List traversal + carry (grade-school addition)
================================================================================
IDEA
  Walk l1 and l2 together and add digit by digit, like addition on paper.
  A missing digit counts as 0 (x, y), so the lists can differ in length.
  New digit is sum % 10 and carry becomes sum / 10. Append via res.
  The loop also runs while carry != 0, so a last carry adds a new node.
  dummy removes the empty-head special case; return dummy.next.
EXAMPLE
  l1 = [9,9], l2 = [1] (99 + 1, lengths differ, final carry)
  9+1+0=10 -> 0,c=1 | 9+0+1=10 -> 0,c=1 | 0+0+1=1 -> 1,c=0
  Answer: [0,0,1]
COMPLEXITY
  Time  O(n + m)  each node of l1 and l2 is read once
  Space O(1)      only carry and pointers; output list is not counted
PATH TO OPTIMAL
  Convert to integers, add, rebuild - O(n+m) - overflows long.
  Copy digits to arrays, add, rebuild - O(n+m) space - see suboptimal.cs.
  One pass with carry - O(1) extra space - no copies, no overflow.
KEYWORDS
  linked list, carry, dummy node, digit addition, big integer, simulation
WATCH OUT
  - Dropping "|| carry != 0" loses the last digit: [5]+[5] gives [0].
  - Stopping when one list ends; use 0 for the null side instead.
  - Returning dummy instead of dummy.next adds a leading 0.
  - "O(1) space" means extra space; the result still has max(n,m)+1 nodes.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Digits are stored in forward order (Add Two Numbers II)?
     -> Push both lists on two stacks, pop and add, prepend each node. O(n+m)
        time and space. Or reverse both lists first for O(1) space.
  2. Can you do it without allocating new nodes?
     -> Write sums into l1's nodes and link l2's tail if l1 is short. Still
        O(n+m) time, O(1) space, but it mutates the input.
  3. Why not just convert to int or long?
     -> Lists can be longer than any built-in integer, so it overflows.
        Digit-by-digit addition works for any length.
  4. Can you write it recursively?
     -> Recurse on (l1.next, l2.next, carry). Same time, O(max(n,m)) stack.
TRIGGER
  Numbers given as digit lists or strings that may be too big for an int.
================================================================================
*/
