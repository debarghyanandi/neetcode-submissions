// --------------------------------------------------------------------------
// -  suboptimal.cs         O(n) time / O(n) space
// -  recursive digit-by-digit addition with carry
// -  [recursive-carry-addition]
// -  ranks below optimal.cs (O(n) time / O(1) space)
// -
// -  Reference solution - not one you solved yourself (from submission-0)
// -
// -  recurses one call per node pair, so call stack depth is O(n) auxiliary
// -  space beyond the output list
// --------------------------------------------------------------------------

public class Solution
{
    public ListNode Add(ListNode first, ListNode second, int carry)
    {
        if (first == null && second == null && carry == 0)
        {
            return null;
        }

        int firstValue = 0;
        int secondValue = 0;
        if (first != null)
        {
            firstValue = first.val;
        }
        if (second != null)
        {
            secondValue = second.val;
        }

        int sum = firstValue + secondValue + carry;
        int newCarry = sum / 10;
        int nodeValue = sum % 10;

        ListNode nextNode = Add(
            (first != null ? first.next : null),
            (second != null ? second.next : null),
            newCarry
        );

        return new ListNode(nodeValue) { next = nextNode };
    }

    public ListNode AddTwoNumbers(ListNode first, ListNode second)
    {
        return Add(first, second, 0);
    }
}

/*
================================================================================
 PATTERN : Recursion with carry passed down, nodes built on unwind
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Suboptimal
================================================================================
WHY THIS PATTERN
  Both lists store the least significant digit at the head, so column addition
  runs head-to-tail - the exact direction the recursion descends. The only state
  that has to travel forward is the carry, and it rides down as the third
  parameter of Add. Nothing has to be reversed, and no digit is looked at twice.
THE BASE CASE HAS THREE CLAUSES
  Add returns null only when first == null && second == null && carry == 0. The
  carry == 0 clause is the one people drop. With it gone, 5 -> null plus 5 ->
  null returns just 0 and silently loses the leading 1; with it in place, one
  extra frame runs with both inputs null, firstValue and secondValue default to
  0, and it emits the final 1 node. That same defaulting to 0 is what makes
  unequal lengths work - the short list just contributes zeros until the long
  one runs out.
INVARIANT
  Add(a, b, c) returns the complete digit list for the number a + b + c, where c
  is the carry out of every column already consumed by the callers above it. Two
  directions of flow meet in one function: carry flows down the stack (newCarry
  becomes the callee's carry), and nodes flow back up. The line new
  ListNode(nodeValue) { next = nextNode } executes only after the recursive call
  has returned, so the tail is fully built before the head node pointing at it
  exists - and because the list is LSB-first, that back-to-front construction
  still yields the digits in the right order.
WHY THIS LOSES TO THE ITERATIVE VERSION
  This burns one stack frame per output digit, and that stack is pure overhead -
  it holds firstValue, secondValue, sum and newCarry, none of which are needed
  after the recursive call returns. The recursion is tail-shaped in spirit but
  not in form, since the new node is constructed after the call, so nothing
  collapses it for you. The iterative version keeps a dummy head plus a tail
  pointer and loops while (first != null || second != null || carry != 0),
  running the identical sum / newCarry / nodeValue three lines in the body and
  allocating only the output nodes. Same work, constant auxiliary memory, no
  depth limit on long inputs. Recursion earns its keep when the call tree
  branches; here it never does.
WATCH OUT
  1. Advance each list independently: first != null ? first.next : null,
  evaluated per side. Writing Add(first.next, second.next, newCarry) throws a
  NullReferenceException the moment one list is shorter.
  2. newCarry = sum / 10 and nodeValue = sum % 10 are exact here because each
  val is 0..9 and carry is 0 or 1, so sum is at most 19 and newCarry is always 0
  or 1. No need to hand-roll a comparison; nothing to gain and a boundary to get
  wrong.
  3. The object initializer { next = nextNode } depends on next being a writable
  field on ListNode. If the node type only exposes a constructor taking (val,
  next), pass nextNode there instead.
TRIGGER
  Reach for this shape when a problem threads a small piece of forward state - a
  carry, a borrow, a running remainder - through a sequence traversed in the
  same direction the answer is built, and the output is itself a linked
  structure. The tell is that the accumulator must be passed as an argument
  while the result is assembled from the return value. Then ask whether the
  traversal ever branches; if it does not, write the loop instead.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
