// ##########################################################################
// #  suboptimal.cs         O(n log k) time / O(k) space
// #  Min-heap priority queue merge   [heap-merge-klists]
// #  ranks below optimal.cs (O(n log k) time / O(log m) space)
// #
// #  YOU SOLVED THIS YOURSELF (from submission-1)
// #
// #  Each node enqueued and dequeued once from k-sized heap with O(log k)
// #  per operation
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
 PATTERN : K-way merge - min-heap over the k list heads
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-1.cs when it was
           first processed
 STATUS  : Suboptimal
================================================================================
VARIABLES
  dummy    fake node before the output list, so no null check on the first append
  res      tail of the output list built so far
  q        min-heap holding one live node per list, keyed by node.val
  n        number of input lists (lists.Length)
WHY THIS PATTERN
  Each input list is already sorted, so the next smallest node overall is always
  the head of one of the lists. You only ever need to compare k candidates, not
  all nodes, so a min-heap q that holds one candidate per list gives you that
  minimum in log k. After popping a node you refill the heap with node.next,
  which is the new head of that list, and repeat until q is empty.
BETTER APPROACH
  The alternative that beats this file is divide and conquer: merge lists
  pairwise (lists[0] with lists[1], etc.) until one list remains, or merge them
  in a balanced recursion over the array. That also runs in O(n log k) time but
  needs no heap - only O(log k) stack, or O(1) if you do the pairwise passes
  iteratively. This file loses on the extra k-node heap and on the constant
  factor of sift-up/sift-down inside Enqueue and Dequeue versus a plain
  two-pointer merge.
INVARIANT
  At the top of every while iteration, q contains exactly the current head of
  each input list that still has unconsumed nodes, and res points at the last
  node already placed in sorted order. Because every list is sorted, the minimum
  over those heads is the global minimum of everything not yet placed, so
  Dequeue always returns the correct next node. Enqueueing node.next right after
  the pop restores the invariant for that one list.
THE OUTPUT TAIL TERMINATES ITSELF
  The code never sets res.next = null at the end, and it does not need to. A
  node is only left as the final tail if it had node.next == null, otherwise its
  successor would have been enqueued and the loop would have continued and
  overwritten res.next. So the last appended node already carries a null next
  from the original input.
WATCH OUT
  lists.Length throws a NullReferenceException if lists itself is null; an empty
  array is fine and returns null through dummy.next. The nodes are relinked in
  place, so the input lists are destroyed - callers holding lists[i] will see a
  spliced-together list afterwards. PriorityQueue in .NET is not stable: when
  two nodes have equal val, the one that comes out first is unspecified, so
  nodes with the same value may interleave across lists in an order you cannot
  predict. Values are used directly as int priorities, so negative vals work,
  but any change to a non-int key needs a comparer.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How do you get the extra space down from the heap?
     Replace it with repeated pairwise merges over the lists array (merge index
     i with index i + gap, doubling gap). Same O(n log k) time, only two
     pointers of extra state, but you write and debug a merge helper instead of
     calling a library heap.
  2. What if the input is k huge sorted streams instead of in-memory lists?
     Keep the same heap but store an IEnumerator per stream and push
     (enumerator.Current, enumerator) after MoveNext. Memory stays O(k) and
     nothing changes structurally, but you cannot restart or re-read a stream.
  3. What if you must not modify the input lists?
     Allocate a new ListNode on each Dequeue instead of relinking node. Time is
     unchanged, but you now allocate n nodes.
  4. k = 1 or k very small but n very large - is the heap still worth it?
     No; with k = 1 you can return lists[0] directly, and with k = 2 a single
     two-pointer merge avoids every heap operation.
TRIGGER
  Several already sorted sequences that must come out as one sorted sequence,
  and you only need the next smallest one at a time.
C# NOTE
  PriorityQueue<TElement, TPriority> is a min-heap by default, which is exactly
  what this needs, so no custom IComparer is required with an int priority. The
  seeding loop could be new PriorityQueue<ListNode, int>(n) or a single
  EnqueueRange call, which sizes the internal array once instead of letting it
  grow while the k heads go in.
COMPLEXITY
  Time  : O(n log k)
  Space : O(k)
================================================================================
*/
