/**
 * Definition for singly-linked list.
 * public class ListNode
 * {
 *     public int val;
 *     public ListNode next;
 *
 *     public ListNode(int val = 0, ListNode next = null)
 *     {
 *         this.val = val;
 *         this.next = next;
 *     }
 * }
 */

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