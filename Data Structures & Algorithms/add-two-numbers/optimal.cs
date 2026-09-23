// ##########################################################################
// #  optimal.cs            O(n + m) time / O(1) space
// #  Iterative linked list traversal with carry
// #  [linked-list-carry-iteration]
// #  ranks above suboptimal.cs (O(n + m) time / O(n + m) space)
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  Single pass through both lists; carry propagates forward in constant
// #  space.
// ##########################################################################

public class Solution
{
    public ListNode AddTwoNumbers(ListNode l1, ListNode l2)
    {
        // my solution
        ListNode dummy = new ListNode(0);
        ListNode res = dummy;

        int carry = 0;
        while (l1 != null || l2 != null || carry != 0)
        {
            int x = l1 != null ? l1.val : 0;
            int y = l2 != null ? l2.val : 0;

            int sum = x + y + carry;
            carry = (sum) / 10;

            res.next = new ListNode(sum % 10);
            res = res.next;

            if (l1 != null)
                l1 = l1.next;

            if (l2 != null)
                l2 = l2.next;

        }

        return dummy.next;
    }
}

/*
================================================================================
 PATTERN : Linked List Traversal - digit-by-digit add with carry
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-1.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  dummy    a throwaway head node so we never special-case the first digit
  res      the moving tail of the result list, not the result itself
  carry    0 or 1 carried into the next digit position
  x        current digit of l1, or 0 once l1 is exhausted
  y        current digit of l2, or 0 once l2 is exhausted
WHY THIS PATTERN
  The digits are already stored least significant first, which is exactly the
  order school addition works in. So one left-to-right pass over both lists adds
  matching positions and pushes carry forward, with no reversal and no
  big-number conversion. The x and y ternaries let the two lists have different
  lengths by treating a missing node as digit 0, and carry != 0 in the loop
  condition keeps the loop alive for a final leading 1.
BRUTE FORCE
  The first instinct is to walk each list, build the number (string or long),
  add them, then split the sum back into digits and build a list. That is still
  O(n + m) work but it breaks as soon as the numbers are longer than a long can
  hold, and the string or BigInteger version drags in conversion and allocation
  for no gain. This code never forms the whole number, so length is unlimited.
INVARIANT
  Before each iteration, every digit position already visited has been written
  correctly into the result, and carry holds exactly the amount owed to the next
  position. Each step restores that by writing sum % 10 and setting carry = sum
  / 10, where sum = x + y + carry. The loop ends only when both lists are done
  and carry is 0, so nothing is left unwritten.
CARRY IN THE LOOP CONDITION
  Putting carry != 0 in the while condition is what handles 5 + 5 = 10 and 999 +
  1 = 1000. Without it you would need a separate tail block after the loop to
  append the leading 1. It also means the loop can run one extra time past the
  end of both lists, which is why x and y must default to 0 rather than
  dereferencing l1 or l2.
WATCH OUT
  The name res reads like "result", but it is the tail pointer; returning res
  instead of dummy.next would return only the last digit node. dummy.next is the
  real answer and dummy itself is discarded. The carry = sum / 10 line is
  correct only because each val is a single digit, so sum is at most 9 + 9 + 1 =
  19 and carry is always 0 or 1; if a node ever held a value above 9, the carry
  would exceed 1 and, while the division still works, any code that assumed
  carry is a flag would break. Note also that the O(1) space claim counts only
  the working variables, not the new nodes that form the output.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. What if the digits were stored most significant first?
     You cannot add left to right, because the carry moves the other way. Either
     reverse both lists first, or push all values onto two stacks and pop them
     together, or recurse to the end and add on the way back. Stacks and
     recursion cost O(n + m) extra space; reversing keeps it O(1) but mutates
     the input.
  2. Can you avoid allocating a new list at all?
     Yes, write the sum digits into the nodes of the longer input list and only
     allocate when the final carry needs one more node. That saves allocations
     but destroys the caller's input, which is usually not acceptable.
  3. How would you add k lists instead of two?
     Keep the same loop, but sum the current digit of every list that is still
     non-null plus carry. Now carry can be larger than 1, and carry = sum / 10
     already handles that correctly without any change.
TRIGGER
  When the data is a sequence and each position's result depends only on that
  position plus one small value handed forward from the previous position, do
  one pass carrying that value.
C# NOTE
  The x and y ternaries can be written as int x = l1?.val ?? 0; because l1?.val
  yields a nullable int that ?? collapses to 0 - same behaviour, less repetition
  of the null test.
COMPLEXITY
  Time  : O(n + m)
  Space : O(1)
================================================================================
*/
