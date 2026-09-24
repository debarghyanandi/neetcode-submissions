// ##########################################################################
// #  suboptimal.cs         O(n log k) time / O(k) space
// #  Min-heap priority queue merge   [heap-priority-queue]
// #  ranks below optimal.cs (O(n log k) time / O(log k) space)
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  Priority queue maintains k list heads, extracting minimum and
// #  enqueueing successors n times.
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
 PATTERN : Min-Heap over k list heads - k-way merge
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-1.cs when it was
           first processed
 STATUS  : Suboptimal
================================================================================
VARIABLES
  dummy    fixed node before the output list, so no special case for the first append
  res      tail of the merged list built so far
  q        min-heap of one live node per input list, keyed on node.val
  n        lists.Length, only used to bound the seeding loop
WHY THIS PATTERN
  Every input list is already sorted, so the global smallest remaining value is
  always the head of one of the k lists. That makes the job "repeatedly pick the
  minimum of k candidates", which is exactly what a min-heap gives in log k per
  pick. q holds at most one node per list, and when a node is taken its
  successor node.next replaces it, keeping the candidate set complete.
BETTER APPROACH
  The better answer is pairwise divide and conquer: merge lists[0] with
  lists[1], lists[2] with lists[3], and so on, halving the count each round.
  Same time, but extra space drops from a heap of k nodes to O(1) for the
  iterative bottom-up version, or O(log k) stack if written recursively. This
  file loses only on that space term; it does not lose on time. A truly naive
  version - collect all values into a List, sort, rebuild - is O(n log n) and
  holds all n nodes at once.
INVARIANT
  At the top of the while loop, q contains exactly the first unmerged node of
  every list that still has nodes left, and res points at the last node already
  placed. So Dequeue returns the smallest value not yet output, which means the
  chain from dummy.next to res is sorted at all times. The loop ends only when
  no list has anything left, so every node is placed exactly once.
THE TAIL CLOSES ITSELF
  No line ever sets res.next = null, yet the result is not a cycle or a stray
  tail. A node is enqueued only when its next is non-null, so the queue is
  non-empty whenever any dequeued node had a successor. The loop can therefore
  only exit right after dequeuing a node whose next was already null, and that
  node is the tail.
WATCH OUT
  Nodes are relinked in place, not copied, so the input arrays' lists are
  destroyed - the caller cannot reuse lists afterwards. lists == null throws on
  lists.Length; an empty array or an array of all nulls is fine and returns null
  through dummy.next. Ties on val are broken arbitrarily by PriorityQueue, which
  is not a stable heap, so two nodes with equal val may come out in either order
  - harmless here, but wrong if the node carried extra payload whose original
  list order mattered. The comment "My Solution" says nothing about the code and
  is worth deleting.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Cut the extra space to O(1).
     Switch to bottom-up pairwise merging: merge lists in place two at a time
     over log k rounds with a single two-pointer merge helper. No heap, same O(n
     log k), but you lose the streaming property - the heap version can emit
     output while later lists are still unread.
  2. The runtime has no PriorityQueue (.NET 5 or earlier).
     Write a small array-backed binary heap of ListNode compared by val, or use
     SortedSet with a comparer that adds a tie-break key, since SortedSet drops
     duplicates when the comparer says equal.
  3. Same problem but k sorted arrays instead of linked lists.
     Push (arrayIndex, elementIndex) pairs with priority
     arr[arrayIndex][elementIndex]; after popping, push the next index in that
     same array. Identical shape, but you now need the index pair in the element
     type because arrays have no next pointer.
  4. Merge in descending order instead.
     Enqueue with priority -node.val, or pass a custom IComparer to the
     PriorityQueue constructor; the second is safer because negation overflows
     on int.MinValue.
TRIGGER
  Several already-sorted sequences must become one sorted sequence, and you only
  ever need the current smallest across all of them.
C# NOTE
  PriorityQueue<TElement, TPriority> keeps element and key apart, so node.val is
  copied into the heap as an int key and the heap never re-reads the node -
  clean here, but remember the key is a snapshot. The seeding loop could be one
  q.EnqueueRange(lists.Where(l => l != null).Select(l => (l, l.val))) call if
  you prefer it, at the cost of a LINQ allocation.
COMPLEXITY
  Time  : O(n log k)
  Space : O(k)
================================================================================
*/
