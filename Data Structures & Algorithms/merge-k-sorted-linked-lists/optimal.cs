// --------------------------------------------------------------------------
// -  optimal.cs            O(n log k) time / O(log k) space
// --------------------------------------------------------------------------

public class Solution
{
    public ListNode MergeKLists(ListNode[] lists)
    {
        //Not solved..
        if (lists == null || lists.Length == 0)
        {
            return null;
        }
        return Divide(lists, 0, lists.Length - 1);
    }

    private ListNode Divide(ListNode[] lists, int l, int r)
    {
        if (l > r)
        {
            return null;
        }
        if (l == r)
        {
            return lists[l];
        }

        int mid = l + (r - l) / 2;
        ListNode left = Divide(lists, l, mid);
        ListNode right = Divide(lists, mid + 1, r);

        return Conquer(left, right);
    }

    private ListNode Conquer(ListNode l1, ListNode l2)
    {
        ListNode dummy = new ListNode(0);
        ListNode curr = dummy;

        while (l1 != null && l2 != null)
        {
            if (l1.val <= l2.val)
            {
                curr.next = l1;
                l1 = l1.next;
            }
            else
            {
                curr.next = l2;
                l2 = l2.next;
            }
            curr = curr.next;
        }

        if (l1 != null)
        {
            curr.next = l1;
        }
        else
        {
            curr.next = l2;
        }

        return dummy.next;
    }
}

/*
================================================================================
 PROBLEM : You get an array of k linked lists. Each list is sorted in
           ascending order. Merge them all into one sorted linked list and
           return its head. Reuse the existing nodes. Some lists may be empty.
           Example: [[1,4],[1,3],[2,6]] -> [1,1,2,3,4,6]
 PATTERN : Divide and Conquer (merge sort on lists)
================================================================================
IDEA
  Divide splits the index range [l, r] at mid, just like merge sort.
  It merges the lists in each half recursively, then joins the two results.
  Conquer is the classic two-list merge: a dummy head, and curr always takes
  the smaller of l1 and l2. At the end it attaches the leftover tail.
  It is correct because merging two sorted lists gives a sorted list.
EXAMPLE
  lists=[[1,4],[1,3],[2,6]]; Divide(0,2) mid=1; Divide(0,1) mid=0
  Conquer([1,4],[1,3]) -> [1,1,3,4]; Divide(2,2) -> [2,6]
  Conquer([1,1,3,4],[2,6]) -> [1,1,2,3,4,6]
COMPLEXITY
  Time  O(n log k)  log k merge levels, and each level touches all n nodes
                    once
  Space O(log k)    recursion depth is log k; merges relink nodes, no new
                    copies
PATH TO OPTIMAL
  Copy all values, sort, rebuild - O(n log n) time, O(n) space - baseline.
  Merge lists one by one into a result - O(n*k) time - no extra array.
  Min-heap of k list heads (suboptimal.cs) - O(n log k), O(k) - fewer passes.
  Pairwise merge by halves (this file) - O(n log k), O(log k) - no heap.
KEYWORDS
  merge k sorted lists, divide and conquer, min-heap, priority queue, dummy
  node
WATCH OUT
  - The comment "//Not solved.." is stale. The code below it works.
  - Merging left to right (result = merge(result, lists[i])) looks the same
    but costs O(n*k). You must merge in pairs by halves.
  - Empty list entries are null. Conquer handles them: the loop is skipped.
  - Forget "curr = curr.next" and the merge loses nodes.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you do it with O(1) extra space?
     -> Merge bottom-up. Use interval = 1, 2, 4... and set lists[i] =
        merge(lists[i], lists[i+interval]). Same O(n log k) time, no recursion.
  2. The lists arrive as streams and you cannot see them all at once?
     -> Use a min-heap holding the current head of each stream. Pop the
        smallest, then push its next node. O(log k) per node, O(k) memory.
  3. Why is this O(n log k) and not O(n k)?
     -> Each node is copied through one merge per level. There are log k
        levels, because the number of lists halves each round.
TRIGGER
  You must combine many already sorted sequences into one sorted output.
================================================================================
*/
