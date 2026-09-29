// --------------------------------------------------------------------------
// -  optimal.cs            O(n + m) time / O(1) space
// --------------------------------------------------------------------------

public class Solution
{
    public ListNode MergeTwoLists(ListNode list1, ListNode list2)
    {
        ListNode dummy = new ListNode(0);
        ListNode node = dummy;

        while (list1 != null && list2 != null)
        {
            if (list1.val < list2.val)
            {
                node.next = list1;
                list1 = list1.next;
            }
            else
            {
                node.next = list2;
                list2 = list2.next;
            }

            node = node.next;
        }

        if (list1 != null)
        {
            node.next = list1;
        }
        else
        {
            node.next = list2;
        }

        return dummy.next;
    }
}

/*
================================================================================
 PATTERN : Two Pointers / Linked List Merge - dummy head splice
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  dummy    a fake node with value 0; the real merged list starts at dummy.next
  node     tail of the merged list so far; the next chosen node is attached here
WHY THIS PATTERN
  Both input lists are already sorted, so the next smallest value must be at the
  head of list1 or at the head of list2. Comparing only those two heads and
  moving one pointer forward is the same merge step used in merge sort. The
  dummy node gives node a place to attach to before any real node has been
  chosen, so the first step needs no special case.
BRUTE FORCE
  Walk both lists and copy every value into an array, sort the array, then build
  a new linked list from it. This takes O((n + m) log(n + m)) time and O(n + m)
  extra space. It loses because it does not use the fact that the inputs are
  already sorted, and it makes new nodes when the existing ones could be reused.
INVARIANT
  At the start of each loop step, the list from dummy.next to node holds the
  smallest values seen so far, in sorted order. Every value still in list1 or
  list2 is greater than or equal to node.val. Taking the smaller head keeps both
  facts true. When one list runs out, the other list is already sorted and every
  value in it is greater than or equal to node.val, so attaching it in one step
  completes the answer.
SPLICE THE REST, DO NOT LOOP
  After the while loop, at most one list still has nodes. The code sets
  node.next to that list in one assignment instead of copying it node by node.
  The else branch also covers the case where both lists are null, because then
  node.next = list2 sets it to null, which is correct.
WATCH OUT
  On a tie, the test list1.val < list2.val is false, so the node from list2 is
  taken first. The output is still sorted, but the merge is not stable. If equal
  keys must keep list1 first, change the test to <=. The input lists are also
  changed in place: after the call, list1 and list2 no longer point to their
  original chains, and the caller's original nodes are now part of the merged
  list.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How do you merge k sorted lists?
     Put the head of each list into a min-heap (a structure that always gives
     back its smallest item fast), keyed by val. Pop the smallest, attach it,
     and push its next node. This takes O(N log k) time and O(k) extra space.
     Another option is to merge the lists in pairs, round after round, which is
     also O(N log k) and needs no heap.
  2. Can you write it recursively?
     Yes. Return the smaller head and set its next to the merge of the rest. The
     code is shorter, but the call stack grows to O(n + m) deep, so very long
     lists can overflow the stack. The iterative version stays at O(1) extra
     space.
  3. What if the inputs must not be changed?
     Create a new ListNode for each value you take, instead of linking the
     original nodes. This uses O(n + m) extra space, but it keeps the caller's
     lists as they were.
TRIGGER
  Two or more inputs that are each already sorted, and you need one sorted
  output: compare the fronts and take the smaller one.
C# NOTE
  dummy is created with new ListNode(0), but its value is never read, and only
  dummy.next is returned. If the ListNode class has a parameterless constructor,
  new ListNode() makes it clearer that this node is only a placeholder.
COMPLEXITY
  Time  : O(n + m)
  Space : O(1)
================================================================================
*/
