// --------------------------------------------------------------------------
// -  suboptimal.cs         O(n + m) time / O(n + m) space
// -  Recursive linked list traversal with carry
// -  [linked-list-carry-recursion]
// -  ranks below optimal.cs (O(n + m) time / O(1) space)
// -
// -  Reference solution - not one you solved yourself
// -
// -  Recursion depth equals max list length; call stack accumulates
// -  O(max(m,n)) frames.
// --------------------------------------------------------------------------

public class Solution
{
    public ListNode Add(ListNode l1, ListNode l2, int carry)
    {
        if (l1 == null && l2 == null && carry == 0)
        {
            return null;
        }

        int v1 = 0;
        int v2 = 0;
        if (l1 != null)
        {
            v1 = l1.val;
        }
        if (l2 != null)
        {
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

    public ListNode AddTwoNumbers(ListNode l1, ListNode l2)
    {
        return Add(l1, l2, 0);
    }
}

/*
================================================================================
 PATTERN : Linked list digit addition - recursion carrying the carry
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Suboptimal
================================================================================
VARIABLES
  carry       carry coming in from the less significant digit already processed
  v1, v2      digit at this position, or 0 when that list has already ended
  sum         v1 + v2 + carry for this one position
  newCarry    sum / 10, the carry handed to the next recursive call
  nodeValue   sum % 10, the digit stored in the node built here
  nextNode    the already-built rest of the result list (higher digits)
WHY THIS PATTERN
  The digits are stored least significant first, so walking both lists forward
  is exactly the order you add by hand: position by position, right to left,
  with one carry passed along. That single piece of state, carry, is all that
  links one position to the next, so the problem folds naturally into a
  recursive call on l1.next and l2.next with newCarry. The stopping condition is
  the honest one: nothing is left in either list and carry is 0, so there is no
  digit to emit.
BETTER APPROACH
  The better version is the same algorithm written as a loop with a dummy head
  node and a tail pointer, advancing l1 and l2 while either is non-null or carry
  is non-zero. That uses only a few pointers of extra state, while this file
  puts one stack frame per digit, so the extra space grows with the longer list.
  On a very long list the recursion can overflow the call stack; the loop
  cannot. The digit work is identical, so this file loses on space and
  robustness, not on speed.
INVARIANT
  Every call to Add receives lists positioned at the same digit index and the
  carry produced by all lower positions. It returns the complete correctly
  formed list for that position and everything above it. By induction from the
  base case, where no digits and no carry give null, the list returned by the
  top call spells out the full sum, least significant digit first.
WATCH OUT
  Add returns null when both lists are null and carry is 0, so calling
  AddTwoNumbers(null, null) gives back null rather than a node holding 0; if the
  caller can pass two empty lists, that is a crash waiting to happen downstream.
  The recursion is not tail recursive: the new ListNode is built after nextNode
  comes back, so depth really is the length of the longer list. The null guards
  on l1 and l2 are repeated three times each (value read, and twice in the
  recursive call arguments) - easy to edit one and forget another. Leading zeros
  in the input are copied straight through, since nothing trims them.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Rewrite it without recursion.
     Use a dummy head, a tail pointer, and a while loop that runs while l1 !=
     null || l2 != null || carry != 0; append new ListNode(sum % 10) each turn
     and return dummy.next. Same time, constant extra space beyond the output.
  2. What if the digits were stored most significant first?
     You cannot add left to right with one carry. Either reverse both lists
     first, or push all digits onto two Stack<int> and pop them together,
     building the result by prepending each new node. Both cost extra space
     proportional to the lists.
  3. Can you avoid allocating a new list at all?
     Write the result into the longer input list in place, reusing its nodes,
     and only allocate when a final carry needs one more node. Saves allocations
     but destroys the input, which is often not allowed.
  4. What if each node held a digit in base 1000 instead of base 10?
     Only the two constants change: newCarry = sum / 1000 and nodeValue = sum %
     1000. The structure is untouched, which is a sign the carry logic is the
     real core here.
TRIGGER
  Two sequences to combine position by position where one small piece of state
  (a carry, a borrow) is the only thing passed forward.
C# NOTE
  LeetCode's ListNode has a constructor overload taking (val, next), so new
  ListNode(nodeValue, nextNode) replaces the object initializer new
  ListNode(nodeValue) { next = nextNode } and does not depend on next being
  publicly settable.
COMPLEXITY
  Time  : O(n + m)
  Space : O(n + m)
================================================================================
*/
