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
//My solution
public class Solution {
    public ListNode RemoveNthFromEnd(ListNode head, int n) {

        ListNode slow = head;
        ListNode fast = head;
        ListNode dummy = head;

        for (int i = 1; i <= n; i++){
            fast = head.next;
            head = head.next;
        }
        if (fast == null){
            return dummy.next;
        }

        while(fast.next != null){
            slow = slow.next;
            fast = fast.next;
        }

        //slow is now n+1 th node from the end.
        slow.next = slow.next.next;
        return dummy;
    }
}
