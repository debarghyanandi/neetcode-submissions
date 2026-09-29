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
 PROBLEM : You get the heads of two linked lists, each sorted in ascending
           order. Return the head of one sorted list made by splicing the
           existing nodes together, not by copying values. Either list may be
           empty. Example: [1,2,4] and [1,3,5] -> [1,1,2,3,4,5].
 PATTERN : Two Pointers (merge step) + dummy head node
================================================================================
IDEA
  list1 and list2 point at the smallest node not yet used in each list.
  Each step links the smaller of the two onto node.next, moves that
  pointer forward, then moves node to the new tail. When one list runs
  out, the other list is attached in one step, because it is already
  sorted. This is correct because the output tail is always the smallest
  unused value, and dummy removes the special case for the first node.
EXAMPLE
  list1 = 1->2->4, list2 = 1->3->5 (tie on 1: the else branch takes list2)
  take 1(list2), 1(list1), 2(list1), 3(list2), 4(list1); list1 is null
  the loop ends, so node.next = list2 attaches 5
  result: 1->1->2->3->4->5, returned as dummy.next
COMPLEXITY
  Time  O(n + m)  each node is linked once, and the leftover tail is linked in
                  O(1)
  Space O(1)      only dummy and a few pointers; the nodes are reused, not
                  copied
PATH TO OPTIMAL
  Copy all values to an array, sort, build a new list - O((n+m) log(n+m))
  time, O(n+m) space - simple, but it ignores that both inputs are sorted.
  Two-pointer merge with splicing (this file) - O(n+m) time, O(1) space -
  it uses the sorted order and reuses nodes. There is no sibling file.
KEYWORDS
  linked list, merge, two pointers, dummy node, sentinel, merge sort
WATCH OUT
  - Forgetting node = node.next overwrites node.next again and again,
    so the output list keeps only its last linked node.
  - Returning dummy instead of dummy.next adds a fake 0 at the head.
  - On ties, "<" takes list2 first. Use "<=" if equal keys must keep
    list1's order (a stable merge).
  - Do not loop over the leftover list. One assignment is enough.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you write it recursively?
     -> Pick the smaller head, set its next to merge(rest, other), and return
        it. Same O(n+m) time, but O(n+m) call stack, which risks overflow.
  2. Merge k sorted lists?
     -> Use a min-heap of the k heads and pop the smallest each step, for O(N
        log k) time and O(k) space. Pairwise divide and conquer also gives O(N
        log k).
  3. What if the inputs are arrays and the first one has spare room?
     -> Merge from the back, with pointers at the ends, writing the largest
        value first. That is O(n+m) time and O(1) extra space, with no shifting.
  4. Why is the tail attach safe?
     -> The remaining list is sorted, and all its values are >= the last node
        linked, so it can follow as it is.
TRIGGER
  Two or more already-sorted sequences must become one sorted sequence.
================================================================================
*/
