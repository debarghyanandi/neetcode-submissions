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
 PATTERN : K-way Merge / Min-Heap - always take the smallest head
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-1.cs when it was
           first processed
 STATUS  : Suboptimal
================================================================================
VARIABLES
  dummy    fake start node, so the first real node needs no special case
  res      tail of the merged list built so far; new nodes are added after it
  q        min-heap of the current head node of each list, keyed by node.val
WHY THIS PATTERN
  The problem gives k lists that are each already sorted, and asks for one
  sorted list. The next node of the output must be the smallest of the k current
  heads, so you only ever compare heads. A min-heap (a structure that returns
  its smallest item quickly) holds those heads in q. Each Dequeue gives the next
  output node in O(log k) instead of scanning all k heads.
BETTER APPROACH
  The time here is already the best possible for this problem. The better
  approach is divide and conquer done bottom-up. Merge the lists in pairs (0
  with 1, 2 with 3, ...), then merge the results in pairs, and keep going until
  one list is left. That takes log k rounds of O(n) work each, so the time is
  the same. But it only relinks existing nodes, so it needs O(1) extra space.
  This file loses because q holds up to k nodes at once.
INVARIANT
  Before each loop pass, q holds exactly one node from each list that still has
  unused nodes: its smallest unused node. So the node that comes out of q is the
  smallest unused node overall. Adding it after res keeps the output sorted.
  After that, the code pushes node.next, so the invariant holds again. When q is
  empty, every node has been linked exactly once.
WATCH OUT
  If lists itself is null, lists.Length throws a NullReferenceException. There
  is no guard for this. Also, when res.next = node runs, node.next still points
  into its old list. That is only safe because a later pass overwrites it, and
  the last node taken is always the tail of its own list, so its next is already
  null. If you change the loop to stop early, you must set res.next = null
  yourself, or the output will carry the rest of an old list.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How would you merge k sorted arrays instead of linked lists?
     Arrays have no next pointer, so the heap must store (listIndex, position)
     and use the value as the priority. You also need a new output array of size
     n, so extra space becomes O(n + k).
  2. What if the lists are huge streams that do not fit in memory?
     Use the same heap, but store one buffered head per stream and write each
     output value straight to disk. Memory stays O(k) plus the buffers. This is
     the classic external merge sort step.
  3. What if you only need the first m nodes of the merged result?
     Stop after m Dequeue calls and set res.next = null. The time becomes O(k +
     m log k), which beats merging everything when m is small.
TRIGGER
  When you see several inputs that are each already sorted and must be combined,
  or you need the "next smallest across k sources", reach for a min-heap of the
  k heads.
C# NOTE
  The Count check plus Dequeue can be one call: while (q.TryDequeue(out ListNode
  node, out _)). PriorityQueue does not keep equal priorities in insertion
  order. That is fine here, because nodes with equal val can come out in any
  order and the list is still sorted.
COMPLEXITY
  Time  : O(n log k)
  Space : O(k)
================================================================================
*/
