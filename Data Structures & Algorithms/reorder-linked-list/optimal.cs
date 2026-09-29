// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(n) space
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
 PROBLEM : You get the head of a singly linked list L0 -> L1 -> ... -> Ln.
           Reorder it in place to L0 -> Ln -> L1 -> Ln-1 -> L2 -> ... Change
           the links, not the values. Return nothing. Example: 1->2->3->4->5
           -> 1->5->2->4->3
 PATTERN : Fast/slow pointers + reverse second half + merge
================================================================================
IDEA
  First, slow and fast walk the list until fast reaches the end.
  Now slow is at the middle. Cut the list after slow (slow.next = null).
  ReverseList turns the second half around.
  Then weave: first takes one node from the front half, second takes one from
  the reversed back half, and so on until second is null.
  It works because the reversed half gives Ln, Ln-1, ... in the order needed.
EXAMPLE
  1->2->3->4->5: slow/fast (2,3), (3,5); stop, slow=3.
  Cut: 1->2->3 and 4->5; reversed second = 5->4.
  Merge: 1->5->2, then 2->4->3; second=null. Result 1->5->2->4->3.
  Even 1->2->3->4: slow=3, halves 1->2->3 and 4 -> 1->4->2->3.
COMPLEXITY
  Time  O(n)  find middle, reverse and merge are each one pass over the list
  Space O(n)  recursive ReverseList uses one stack frame per node of second
              half
PATH TO OPTIMAL
  Find the tail again for each front node - O(n^2) time, O(1) space.
  Copy nodes into an array, relink with two pointers i, j - O(n) / O(n).
  This file: middle + reverse + merge - O(n) / O(n), with no array needed.
  An iterative reverse makes it O(n) / O(1), the true optimal.
KEYWORDS
  linked list, fast and slow pointers, find middle, reverse linked list, merge
WATCH OUT
  - Empty list: slow = head = null, so slow.next throws. Add: if head==null
    return.
  - Forgetting slow.next = null leaves a cycle, and the merge never ends.
  - In the merge, save firstNext and secondNext before you change any links.
  - Recursive ReverseList can overflow the stack on a very long list.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you do it in O(1) extra space?
     -> Reverse with an iterative loop using prev, curr and next. Time stays
        O(n) and space drops to O(1). The code is slightly longer but it has no
        stack risk.
  2. Why does the loop stop on second == null and not on first?
     -> The front half is the same size as the back half or one node longer.
        So second runs out first, and the last front node already points to null.
  3. What if you may not change the links, only the values?
     -> Copy the values into an array, then write them back in the order i, j,
        i+1, j-1. This is O(n) time and O(n) space, and it is simple.
  4. How do you check whether a list is a palindrome in O(1) space?
     -> Use the same steps: find the middle, reverse the second half, then
        compare the two halves node by node. You can restore the list after the
        compare.
TRIGGER
  When a list must be read from both ends at once but you have only next
  links, find the middle, reverse one half, and merge.
================================================================================
*/
