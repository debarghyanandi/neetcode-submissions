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

        ListNode dummy = new ListNode(0, head);
        ListNode slow = dummy;
        ListNode fast = head;
        

        while (n > 0){
            fast = fast.next;
            n--;
        }
        
        while(fast != null){
            slow = slow.next;
            fast = fast.next;
        }

        //slow is now n+1 th node from the end.
        slow.next = slow.next.next;
        return dummy.next;
    }
}
