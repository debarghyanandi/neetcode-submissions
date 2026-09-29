// ##########################################################################
// #  suboptimal.cs         O(n log k) time / O(k) space
// ##########################################################################

public class Solution
{
    public ListNode MergeKLists(ListNode[] lists)
    {
        // My Solution
        ListNode dummy = new ListNode(0);
        ListNode res = dummy;

        PriorityQueue<ListNode, int> q = new PriorityQueue<ListNode, int>();

        int n = lists.Length;

        for (int i = 0; i < n; i++)
        {
            if (lists[i] != null)
                q.Enqueue(lists[i], lists[i].val);
        }

        while (q.Count > 0)
        {
            ListNode node = q.Dequeue();

            if (node.next != null)
                q.Enqueue(node.next, node.next.val);

            res.next = node;
            res = res.next;
        }

        return dummy.next;
    }
}

/*
================================================================================
 PROBLEM : You get an array of k linked lists. Each list is sorted in
           ascending order. Merge them all into one sorted linked list and
           return its head. Some lists may be empty, and the array itself may
           be empty. Example: [[1,4],[1,3],[]] -> 1->1->3->4
 PATTERN : K-way merge with a min-heap (priority queue)
================================================================================
IDEA
  Put the head of every non-null list into the min-heap q, keyed by val.
  Loop: Dequeue the smallest node and append it after res.
  Then Enqueue node.next, the next node from the same list.
  q always holds the current front of each list that is not finished.
  So the global minimum is always on top, and the output stays sorted.
  optimal.cs instead merges the lists in pairs (divide and conquer), no heap.
EXAMPLE
  lists = [[1,4],[1,3],[]]; the empty list is skipped, so q = {1a,1b}
  pop 1a, push 4 -> {1b,4}; pop 1b, push 3 -> {3,4}; pop 3 -> {4}; pop 4
  Result: 1->1->3->4 (if the two 1s pop in the other order, same answer)
COMPLEXITY
  Time  O(n log k)  each of the n nodes is pushed and popped once; the heap
                    size is <= k
  Space O(k)        q holds at most one node per list; the output reuses the
                    nodes
WATCH OUT
  - Skip null heads before Enqueue. lists[i].val on an empty list throws
    a NullReferenceException.
  - The priority is the int val, not the node itself. In Java or Python you
    must give a comparator or a tie-breaker, or ties on val will crash.
  - Push node.next only if it is not null. The last node you pop has
    next == null, so the merged list ends correctly and has no cycle.
  - If lists is empty or all its lists are null, the loop never runs and
    dummy.next is null. That is correct, so do not add a special case.
================================================================================
*/
