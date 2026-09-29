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
 PATTERN : Linked List Digit Math - recursive add with carry
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Suboptimal
================================================================================
VARIABLES
  carry      the carry coming in from the less significant digit (0 or 1)
  v1         digit of l1 at this position, or 0 if l1 has ended
  v2         digit of l2 at this position, or 0 if l2 has ended
  newCarry   sum / 10, the carry passed to the next position
  nodeValue  sum % 10, the digit stored in the node built now
  nextNode   head of the sum list for all later positions
WHY THIS PATTERN
  The digits are stored in reverse order, so the head of each list is the ones
  digit. This means you can walk both lists from the front and add digits the
  way you add numbers on paper, one column at a time. Each call to Add handles
  one column: it adds v1 + v2 + carry, keeps nodeValue, and passes newCarry to
  the call for the next column. The recursion builds the answer list in the same
  order as the input lists.
BETTER APPROACH
  The better approach is an iterative loop with a dummy head node and a tail
  pointer. It moves through both lists once, adds each new node at the tail, and
  uses only O(1) extra memory besides the output list. This file loses because
  every column adds one stack frame to Add. So the call stack grows to max(n, m)
  + 1 frames. That stack is where the extra space in the reported complexity
  comes from.
INVARIANT
  Add(l1, l2, carry) returns the digit list of (number left in l1) + (number
  left in l2) + carry. The base case is correct: when both lists are empty and
  carry is 0, the sum is 0, which is the empty list. In every other case, the
  call builds the correct lowest digit, nodeValue. The rest of the sum is
  exactly the rest of both lists plus newCarry, and the recursive call returns
  that. So the full result is correct by induction on the remaining length.
CARRY IN THE BASE CASE
  The stop check tests carry == 0 as well as both lists being null. Because of
  this, a final carry (for example 5 + 5 = 10) goes into one more call. That
  call makes the extra node with value 1. You do not need a separate "if carry >
  0, add a node" step after the loop.
WATCH OUT
  Each digit uses one recursive call, so a very long list can cause a
  StackOverflowException. In .NET you cannot catch that exception, and it ends
  the process. Also, the ternaries (l1 != null ? l1.next : null) are required:
  when one list is shorter, the code keeps recursing with that side as null, and
  reading .next on it directly would throw a NullReferenceException.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. What if the digits are stored most significant digit first (Add Two Numbers
  II)?
     Push both lists onto two stacks, or reverse both lists, then add from the
     ones digit. Put each new node at the front of the result. Stacks use O(n +
     m) extra memory. Reversing uses O(1) extra memory but changes the input
     lists.
  2. Can you avoid allocating a new list?
     Yes. Write each sum digit into l1's existing nodes. When l1 runs out, link
     to l2's remaining nodes, and allocate only the final carry node. This saves
     memory, but it destroys the caller's input, and the caller may not expect
     that.
  3. How would it change for a different base, such as base 16?
     Only the constant changes: newCarry = sum / base and nodeValue = sum %
     base. The carry can still only be 0 or 1, because the largest sum is
     (base-1) + (base-1) + 1.
TRIGGER
  Two numbers stored as digit lists with the ones digit first, and you must
  return their sum as a list: add column by column and pass a carry.
C# NOTE
  The expression new ListNode(nodeValue) { next = nextNode } is an object
  initializer. It runs the constructor first and then sets the public next
  field. This only works because next is a writable public member of ListNode.
COMPLEXITY
  Time  : O(n + m)
  Space : O(n + m)
================================================================================
*/
