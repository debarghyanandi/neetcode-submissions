/**
 * Definition for singly-linked list.
 * public class ListNode {
 *     public int val;
 *     public ListNode next;
 *     public ListNode(int val=0, ListNode next=null) {
 *         this.val = val;
 *         this.next = next;
 *     }
 * }
 */

public class Solution {
    public ListNode MergeKLists(ListNode[] lists) {
        //Not solved..
        if (lists == null || lists.Length == 0) {
            return null;
        }
        return Divide(lists, 0, lists.Length - 1);
    }

    private ListNode Divide(ListNode[] lists, int l, int r) {
        if (l > r) {
            return null;
        }
        if (l == r) {
            return lists[l];
        }

        int mid = l + (r - l) / 2;
        ListNode left = Divide(lists, l, mid);
        ListNode right = Divide(lists, mid + 1, r);

        return Conquer(left, right);
    }

    private ListNode Conquer(ListNode l1, ListNode l2) {
        ListNode dummy = new ListNode(0);
        ListNode curr = dummy;

        while (l1 != null && l2 != null) {
            if (l1.val <= l2.val) {
                curr.next = l1;
                l1 = l1.next;
            } else {
                curr.next = l2;
                l2 = l2.next;
            }
            curr = curr.next;
        }

        if (l1 != null) {
            curr.next = l1;
        } else {
            curr.next = l2;
        }

        return dummy.next;
    }
}