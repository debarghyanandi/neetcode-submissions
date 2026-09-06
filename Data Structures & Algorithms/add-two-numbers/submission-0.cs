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
    public ListNode Add(ListNode l1, ListNode l2, int carry) {
        if (l1 == null && l2 == null && carry == 0) {
            return null;
        }

        int v1 = 0;
        int v2 = 0;
        if (l1 != null) {
            v1 = l1.val;
        }
        if (l2 != null) {
            v2 = l2.val;
        }

        int sum = v1 + v2 + carry;
        int newCarry = sum / 10;
        int nodeValue = sum % 10;

        ListNode nextNode = Add(
            (l1 != null ? l1.next : null),
            (l2 != null ? l2.next : null),
            newCarry
        );

        return new ListNode(nodeValue) { next = nextNode };
    }

    public ListNode AddTwoNumbers(ListNode l1, ListNode l2) {
        return Add(l1, l2, 0);
    }
}